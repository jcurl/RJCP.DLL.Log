﻿namespace RJCP.App.DltDump.Application
{
    using System;
    using System.Threading.Tasks;
    using Domain;
    using Domain.InputStream;
    using Domain.OutputStream;
    using Infrastructure.Dlt;
    using Resources;
    using RJCP.Diagnostics.Log;
    using RJCP.Diagnostics.Log.Constraints;
    using RJCP.Diagnostics.Log.Dlt;
    using Services;

    public class FilterApp
    {
        private readonly FilterConfig m_Config;

        public FilterApp(FilterConfig config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            m_Config = config;
        }

        public async Task<ExitCode> Run()
        {
            if (!CheckInputs())
                return ExitCode.InputError;

            Global.Instance.OutputStreamFactory.Force = m_Config.Force;
            Global.Instance.OutputStreamFactory.Split = m_Config.Split;

            int processed = 0;
            using (IOutputStream output = GetOutputStream()) {
                if (output == null) return ExitCode.OutputError;

                foreach (string uri in m_Config.Input) {
                    try {
                        bool connected = await ProcessInput(uri, output);
                        if (connected) processed++;
                    } catch (OutputStreamException ex) {
                        Terminal.WriteLine(ex.Message);
                    }
                }
            }

            if (processed == 0) return ExitCode.NoFilesProcessed;
            if (processed != m_Config.Input.Count) return ExitCode.PartialFilesProcessed;
            return ExitCode.Success;
        }

        private async Task<bool> ProcessInput(string uri, IOutputStream output)
        {
            bool retries;
            bool connected = false;
            do {
                using (IInputStream inputStream = await GetInputStream(uri, m_Config.ConnectRetries)) {
                    if (inputStream == null) {
                        retries = false;
                        continue;
                    }

                    retries = inputStream.IsLiveStream && m_Config.ConnectRetries != 0;
                    using (ITraceReader<DltTraceLineBase> decoder = await GetDecoder(inputStream)) {
                        if (decoder == null) {
                            retries = false;
                            continue;
                        }

                        output.SetInput(inputStream.Connection, Global.Instance.DltReaderFactory.InputFormat);

                        connected = true;
                        DltTraceLineBase line;
                        do {
                            line = await decoder.GetLineAsync();
                            if (line != null) output.Write(line);
                        } while (line != null);
                    }
                }
            } while (retries);
            return connected;
        }

        private bool CheckInputs()
        {
            int count = m_Config.Input.Count;
            if (count == 0) {
                Terminal.WriteLine(AppResources.FilterCheckError_NoStreams);
                return false;
            }

            foreach (string uri in m_Config.Input) {
                IInputStream inputStream = Global.Instance.InputStreamFactory.Create(uri);
                if (inputStream == null) {
                    Terminal.WriteLine(AppResources.FilterCheckError_UnknownInput, uri);
                    return false;
                }
                if (inputStream.IsLiveStream && count > 1) {
                    Terminal.WriteLine(AppResources.FilterCheckError_LiveStreams);
                    return false;
                }
            }
            return true;
        }

        private static async Task<IInputStream> GetInputStream(string uri, int retries)
        {
            IInputStream inputStream = null;
            try {
                inputStream = Global.Instance.InputStreamFactory.Create(uri);
                if (inputStream == null) {
                    Terminal.WriteLine(AppResources.FilterOpenError_CreateError, uri);
                    return null;
                }

                inputStream.Open();
                if (inputStream.RequiresConnection) {
                    int connectAttempt = 0;
                    bool connected = false;
                    while (!connected && (retries < 0 || connectAttempt <= retries)) {
                        if (connectAttempt > 0) {
                            Terminal.WriteLine(AppResources.FilterOpenError_Retry, uri, connectAttempt);
                        }
                        connected = await inputStream.ConnectAsync();
                        connectAttempt++;
                    }

                    if (!connected) {
                        Terminal.WriteLine(AppResources.FilterOpenError_ConnectError, uri);
                        inputStream.Dispose();
                        return null;
                    }
                }

                return inputStream;
            } catch (InputStreamException ex) {
                if (inputStream != null) {
                    inputStream.Dispose();
                }
                Terminal.WriteLine(ex.Message);
                return null;
            } catch {
                if (inputStream != null) {
                    inputStream.Dispose();
                }
                throw;
            }
        }

        private IOutputStream GetOutputStream()
        {
            IOutputStream output = null;
            try {
                output = Global.Instance.OutputStreamFactory.Create(m_Config.OutputFormat, m_Config.OutputFileName);
                if (output == null) {
                    Terminal.WriteLine(AppResources.FilterOutputError_UnknownOutput, m_Config.OutputFileName ?? "(none)");
                    return null;
                }

                // Configure the output streams depending on the options.
                if (output is ConsoleOutput consoleOutput) {
                    consoleOutput.ShowPosition = m_Config.ShowPosition;
                } else if (output is TextOutput textOutput) {
                    textOutput.ShowPosition = m_Config.ShowPosition;
                }

                Constraint filter = m_Config.GetFilter();
                if (filter == null) return output;

                if (m_Config.BeforeContext > 0 || m_Config.AfterContext > 0) {
                    return new ContextOutput(filter,
                        m_Config.BeforeContext, m_Config.AfterContext, output);
                } else {
                    return new FilterOutput(filter, output);
                }
            } catch (Exception ex) {
                if (output != null) output.Dispose();

                Terminal.WriteLine(AppResources.FilterOutputError,
                    m_Config.OutputFileName ?? "(none)",
                    ex.Message);
                throw;
            }
        }

        private async Task<ITraceReader<DltTraceLineBase>> GetDecoder(IInputStream inputStream)
        {
            switch (m_Config.InputFormat) {
            case InputFormat.Automatic:
                Global.Instance.DltReaderFactory.InputFormat = inputStream.SuggestedFormat;
                Global.Instance.DltReaderFactory.OnlineMode = inputStream.IsLiveStream;
                break;
            case InputFormat.File:
                Global.Instance.DltReaderFactory.InputFormat = m_Config.InputFormat;
                Global.Instance.DltReaderFactory.OnlineMode = false;
                break;
            case InputFormat.Network:
            case InputFormat.Serial:
                Global.Instance.DltReaderFactory.InputFormat = m_Config.InputFormat;

                // If we read from a file, then we never use the local time stamp.
                Global.Instance.DltReaderFactory.OnlineMode = !inputStream.Scheme.Equals("file");
                break;
            }

            return await Global.Instance.DltReaderFactory.CreateAsync(inputStream.InputStream);
        }
    }
}
