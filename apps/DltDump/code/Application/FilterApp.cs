﻿namespace RJCP.App.DltDump.Application
{
    using System;
    using System.Threading.Tasks;
    using Domain;
    using Domain.InputStream;
    using Infrastructure.Dlt;
    using Resources;
    using RJCP.Diagnostics.Log;
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
            if (m_Config.Input.Count == 0)
                return ExitCode.OptionsError;

            int processed = 0;
            foreach (string uri in m_Config.Input) {
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

                            connected = true;
                            DltTraceLineBase line;
                            do {
                                line = await decoder.GetLineAsync();
                                if (line != null) {
                                    if (m_Config.ShowPosition) {
                                        Global.Instance.Terminal.StdOut.WriteLine("{0:x8}: {1}", line.Position, line.ToString());
                                    } else {
                                        Global.Instance.Terminal.StdOut.WriteLine(line.ToString());
                                    }
                                }
                            } while (line != null);
                        }
                    }
                } while (retries);
                if (connected) processed++;
            }

            if (processed == 0) return ExitCode.NoFilesProcessed;
            if (processed != m_Config.Input.Count) return ExitCode.PartialFilesProcessed;
            return ExitCode.Success;
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
