namespace RJCP.App.DltDump.Application
{
    using System;
    using System.IO;
    using Resources;
    using Services;

    public class FilterApp
    {
        private readonly FilterConfig m_Config;

        public FilterApp(FilterConfig config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            m_Config = config;
        }

        public ExitCode Run()
        {
            if (!CheckInput())
                return ExitCode.OptionsError;

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
    }
}
