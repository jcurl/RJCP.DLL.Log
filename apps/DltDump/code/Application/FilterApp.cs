namespace RJCP.App.DltDump.Application
{
    using System;
    using System.Threading.Tasks;
    using Domain;
    using Domain.InputStream;
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
                using (IInputStream inputStream = await GetInputStream(uri)) {
                    if (inputStream == null) continue;

                    using (ITraceReader<DltTraceLineBase> decoder = await GetDecoder(inputStream)) {
                        if (decoder == null) continue;

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
                processed++;
            }

            if (processed == 0) return ExitCode.NoFilesProcessed;
            if (processed != m_Config.Input.Count) return ExitCode.PartialFilesProcessed;
            return ExitCode.Success;
        }

        private static async Task<IInputStream> GetInputStream(string uri)
        {
            IInputStream inputStream = null;
            try {
                inputStream = Global.Instance.InputStreamFactory.Create(uri);
                if (inputStream == null) {
                    Terminal.WriteLine(AppResources.FilterOpenError_CreateError, uri);
                    return null;
                }

                bool connected = await inputStream.ConnectAsync();
                if (!connected) {
                    Terminal.WriteLine(AppResources.FilterOpenError_ConnectError, uri);
                    inputStream.Dispose();
                    return null;
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

        private static async Task<ITraceReader<DltTraceLineBase>> GetDecoder(IInputStream inputStream)
        {
            return await Global.Instance.DltReaderFactory.CreateAsync(inputStream.InputStream);
        }
    }
}
