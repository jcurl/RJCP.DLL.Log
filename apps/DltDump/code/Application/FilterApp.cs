namespace RJCP.App.DltDump.Application
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Domain;
    using Domain.Dlt;
    using Domain.InputStream;
    using Domain.OutputStream;
    using Resources;
    using RJCP.Diagnostics.Log;
    using RJCP.Diagnostics.Log.Constraints;
    using RJCP.Diagnostics.Log.Dlt;

    public class FilterApp
    {
        private enum InputResult
        {
            NotConnected,
            DecodeFailure,
            Connected
        }

        private readonly FilterConfig m_Config;

        public FilterApp(FilterConfig config)
        {
            if (config is null) throw new ArgumentNullException(nameof(config));

            m_Config = config;
        }

        public async Task<ExitCode> Run()
        {
            ICollection<IInputStream> inputs = ParseInputs();
            if (inputs is null || inputs.Count == 0)
                return ExitCode.InputError;

            Global.Instance.OutputStreamFactory.Force = m_Config.Force;
            Global.Instance.OutputStreamFactory.Split = m_Config.Split;
            Global.Instance.OutputStreamFactory.ConvertNonVerbose = m_Config.ConvertNonVerbose;
            Global.Instance.DltReaderFactory.FrameMap = m_Config.FrameMap;

            int processed = 0;
            bool partial = false;
            using (IOutputStream output = GetOutputStream()) {
                if (output is null) return ExitCode.OutputError;
                Global.Instance.DltReaderFactory.OutputStream = output;

                // Ensure our inputs are not overwritten.
                if (output is OutputBase outputBase) {
                    foreach (string uri in m_Config.Input) {
                        outputBase.AddProtectedFile(uri);
                    }
                }

                foreach (IInputStream input in inputs) {
                    try {
                        InputResult connected = await ProcessInput(input, output);
                        switch (connected) {
                        case InputResult.Connected:
                            processed++;
                            break;
                        case InputResult.DecodeFailure:
                            partial = true;
                            break;
                        }
                    } catch (OutputStreamException ex) {
                        Global.Instance.Terminal.StdOut.WrapLine(ex.Message);
                    } finally {
                        input.Dispose();
                    }
                }
            }

            if (processed == 0 && !partial) return ExitCode.NoFilesProcessed;
            if (processed != m_Config.Input.Count || partial) return ExitCode.PartialFilesProcessed;
            return ExitCode.Success;
        }

        private ICollection<IInputStream> ParseInputs()
        {
            int count = m_Config.Input.Count;
            if (count == 0) {
                Global.Instance.Terminal.StdOut.WrapLine(AppResources.FilterCheckError_NoStreams);
                return null;
            }

            List<IInputStream> inputs = new List<IInputStream>();
            foreach (string uri in m_Config.Input) {
                IInputStream input;
                try {
                    input = Global.Instance.InputStreamFactory.Create(uri);
                } catch (InputStreamException ex) {
                    Global.Instance.Terminal.StdOut.WrapLine(AppResources.FilterCheckError_Invalid, uri, ex.Message);
                    return null;
                }

                // In case of an error, and we return, the list and its contents are just garbage collected. They are
                // not disposed of as there is nothing to dispose (until it is opened).
                if (input is null) {
                    Global.Instance.Terminal.StdOut.WrapLine(AppResources.FilterCheckError_UnknownInput, uri);
                    return null;
                }
                if (input.IsLiveStream && count > 1) {
                    Global.Instance.Terminal.StdOut.WrapLine(AppResources.FilterCheckError_LiveStreams);
                    return null;
                }
                inputs.Add(input);
            }
            return inputs;
        }

        private async Task<InputResult> ProcessInput(IInputStream input, IOutputStream output)
        {
            SetInputStreamType(input);
            output.SetInput(input.InputFileName, Global.Instance.DltReaderFactory.InputFormat);

            bool retries;
            InputResult parsed = InputResult.NotConnected;
            do {
                bool connected = await ConnectInputStream(input, m_Config.ConnectRetries);
                if (!connected) return parsed;

                using (ITraceReader<DltTraceLineBase> reader = await GetReader(input)) {
                    if (reader is null) return parsed;

                    if (input.IsLiveStream && output is OutputBase outputBase) {
                        // 5 seconds.
                        outputBase.AutoFlushPeriod = 5000;
                    }

                    parsed = InputResult.Connected;
                    bool receivedLine = false;
                    try {
                        DltTraceLineBase line;
                        do {
                            line = await reader.GetLineAsync();
                            if (line is object) {
                                receivedLine = true;
                                output.Write(line);
                            }
                        } while (line is object);
                    } catch (OutputStreamException) {
                        // Propagate this exception upstream
                        throw;
                    } catch (Exception ex) {
                        // Any exception can occur while decoding. If so, it's aborted, and we try reading the next.
                        // Errors might be file format, or Operating System errors.
                        Log.App.TraceEvent(TraceEventType.Warning,
                            "Error while processing file (Exception {0}), see previous exceptions. {1}",
                            ex.GetType().Name, ex.Message);
                        Global.Instance.Terminal.StdOut.WrapLine(ex.Message);
                        return receivedLine ? InputResult.DecodeFailure : InputResult.NotConnected;
                    } finally {
                        input.Close();
                    }
                }

                retries = input.IsLiveStream && input.RequiresConnection && m_Config.ConnectRetries != 0;
            } while (retries);
            return parsed;
        }

        private void SetInputStreamType(IInputStream input)
        {
            switch (m_Config.InputFormat) {
            case InputFormat.Automatic:
                Global.Instance.DltReaderFactory.InputFormat = input.SuggestedFormat;
                Global.Instance.DltReaderFactory.OnlineMode = input.IsLiveStream;
                break;
            case InputFormat.File:
            case InputFormat.Pcap:
                Global.Instance.DltReaderFactory.InputFormat = m_Config.InputFormat;
                Global.Instance.DltReaderFactory.OnlineMode = false;
                break;
            case InputFormat.Network:
            case InputFormat.Serial:
                Global.Instance.DltReaderFactory.InputFormat = m_Config.InputFormat;

                // If we read from a file, then we never use the local time stamp.
                Global.Instance.DltReaderFactory.OnlineMode = !input.Scheme.Equals("file");
                break;
            }
        }

        /// <summary>
        /// Connects the input stream.
        /// </summary>
        /// <param name="input">The input stream to open and connect.</param>
        /// <param name="retries">The number of retries.</param>
        /// <returns>
        /// Is <see langword="true"/> when the connection is established within the number of retries, otherwise
        /// <see langword="false"/>.
        /// </returns>
        private static async Task<bool> ConnectInputStream(IInputStream input, int retries)
        {
            try {
                Log.App.TraceEvent(TraceEventType.Information,
                    "Input: {0}; LiveStream {1}; RequireConnection {2}; SuggestedFormat {3}; Connect Retries {4}",
                    input.Connection, input.IsLiveStream, input.RequiresConnection, input.SuggestedFormat, retries);

                input.Open();
                if (input.RequiresConnection) {
                    int connectAttempt = 0;
                    bool connected = false;
                    while (!connected && (retries < 0 || connectAttempt <= retries)) {
                        if (connectAttempt > 0) {
                            Global.Instance.Terminal.StdOut.WrapLine(AppResources.FilterOpenError_Retry, input.Connection, connectAttempt);
                        }
                        connected = await input.ConnectAsync();
                        connectAttempt++;
                    }

                    if (!connected) {
                        Global.Instance.Terminal.StdOut.WrapLine(AppResources.FilterOpenError_ConnectError, input.Connection);
                        input.Close();
                        return false;
                    }
                }

                return true;
            } catch (InputStreamException ex) {
                input.Close();
                Global.Instance.Terminal.StdOut.WrapLine(ex.Message);
                return false;
            } catch {
                input.Close();
                throw;
            }
        }

        private IOutputStream GetOutputStream()
        {
            IOutputStream output = null;
            try {
                output = Global.Instance.OutputStreamFactory.Create(m_Config.OutputFormat, m_Config.OutputFileName);
                if (output is null) {
                    Global.Instance.Terminal.StdOut.WrapLine(AppResources.FilterOutputError_UnknownOutput, m_Config.OutputFileName ?? "(none)");
                    return null;
                }

                // Configure the output streams depending on the options.
                if (output is ConsoleOutput consoleOutput) {
                    consoleOutput.ShowPosition = m_Config.ShowPosition;
                } else if (output is TextOutput textOutput) {
                    textOutput.ShowPosition = m_Config.ShowPosition;
                }

                Constraint filter = m_Config.GetFilter();
                if (filter is null) return output;

                if (m_Config.BeforeContext > 0 || m_Config.AfterContext > 0) {
                    return new ContextOutput(filter,
                        m_Config.BeforeContext, m_Config.AfterContext, output);
                } else {
                    return new FilterOutput(filter, output);
                }
            } catch (Exception ex) {
                if (output is object) output.Dispose();

                Global.Instance.Terminal.StdOut.WrapLine(AppResources.FilterOutputError,
                    m_Config.OutputFileName ?? "(none)",
                    ex.Message);
                throw;
            }
        }

        private static Task<ITraceReader<DltTraceLineBase>> GetReader(IInputStream input)
        {
            if (input.InputStream is object)
                return Global.Instance.DltReaderFactory.CreateAsync(input.InputStream);

            if (input.InputPacket is object)
                return Global.Instance.DltReaderFactory.CreateAsync(input.InputPacket);

            throw new InvalidOperationException("No reader for the input format");
        }
    }
}
