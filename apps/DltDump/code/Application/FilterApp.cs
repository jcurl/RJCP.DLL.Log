namespace RJCP.App.DltDump.Application
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
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
            if (!CheckInput())
                return ExitCode.OptionsError;

            int processed = 0;
            foreach (string file in m_Config.Input) {
                ITraceReader<DltTraceLineBase> decoder = await GetDecoder(file);
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
                processed++;
            }

            if (processed == 0) return ExitCode.NoFilesProcessed;
            if (processed != m_Config.Input.Count) return ExitCode.PartialFilesProcessed;
            return ExitCode.Success;
        }

        private bool CheckInput()
        {
            if (m_Config.Input.Count == 0) return false;

            foreach (string file in m_Config.Input) {
                if (!File.Exists(file)) {
                    Terminal.WriteLine(AppResources.FilterApp_InputFileNotFound, file);
                    return false;
                }
            }

            return true;
        }

        private static async Task<ITraceReader<DltTraceLineBase>> GetDecoder(string uri)
        {
            try {
                return await Global.Instance.DltReaderFactory.CreateAsync(uri);
            } catch (FileNotFoundException) {
                Terminal.WriteLine(AppResources.FilterOpenError_FileNotFound, uri);
                return null;
            } catch (DirectoryNotFoundException) {
                Terminal.WriteLine(AppResources.FilterOpenError_DirectoryNotFound, uri);
                return null;
            } catch (PathTooLongException) {
                Terminal.WriteLine(AppResources.FilterOpenError_PathTooLong, uri);
                return null;
            } catch (IOException ex) {
                Terminal.WriteLine(AppResources.FilterOpenError_IOException, uri, ex.Message);
                return null;
            } catch (ArgumentException ex) {
                Terminal.WriteLine(AppResources.FilterOpenError_InvalidFile, uri, ex.Message);
                return null;
            } catch (NotSupportedException ex) {
                Terminal.WriteLine(AppResources.FilterOpenError_InvalidFile, uri, ex.Message);
                return null;
            } catch (System.Security.SecurityException ex) {
                Terminal.WriteLine(AppResources.FilterOpenError_Security, uri, ex.Message);
                return null;
            } catch (UnauthorizedAccessException) {
                Terminal.WriteLine(AppResources.FilterOpenError_Unauthorized, uri);
                return null;
            }
        }
    }
}
