namespace RJCP.App.DltDump.Infrastructure
{
    using RJCP.Core.Environment;

    /// <summary>
    /// A support class for generating Operating System dependent option strings.
    /// </summary>
    public static class OptionsGen
    {
        private static readonly string ShortOptionSymbol;
        private static readonly string LongOptionSymbol;
        private static readonly string AssignmentSymbol;

        static OptionsGen()
        {
            if (Platform.IsWinNT()) {
                ShortOptionSymbol = "/";
                LongOptionSymbol = "/";
                AssignmentSymbol = ":";
            } else {
                ShortOptionSymbol = "-";
                LongOptionSymbol = "--";
                AssignmentSymbol = "=";
            }
        }

        /// <summary>
        /// Provides the long option.
        /// </summary>
        /// <param name="option">The option.</param>
        /// <returns>The long option.</returns>
        public static string LongOpt(string option)
        {
            return $"{LongOptionSymbol}{option}";
        }

        /// <summary>
        /// Provides the long option with a value.
        /// </summary>
        /// <param name="option">The option.</param>
        /// <param name="value">The value.</param>
        /// <returns>The long option.</returns>
        public static string LongOpt(string option, string value)
        {
            return $"{LongOptionSymbol}{option}{AssignmentSymbol}{value}";
        }

        /// <summary>
        /// Returns an array of options
        /// </summary>
        /// <param name="args">
        /// The arguments to convert where <c>{0}</c> is the short option, <c>{1}</c> is the long option, <c>{2}</c> is
        /// the assignment symbol.
        /// </param>
        /// <returns>An array of options.</returns>
        public static string[] Format(string[] args)
        {
            string[] options = new string[args.Length];
            for (int index = 0; index < args.Length; index++) {
                options[index] = string.Format(args[index], ShortOptionSymbol, LongOptionSymbol, AssignmentSymbol);
            }
            return options;
        }

        /// <summary>
        /// Provides the short option.
        /// </summary>
        /// <param name="option">The option.</param>
        /// <returns>The short option.</returns>
        public static string ShortOpt(char option)
        {
            return $"{ShortOptionSymbol}{option}";
        }

        /// <summary>
        /// Provides the short option with a value.
        /// </summary>
        /// <param name="option">The option.</param>
        /// <param name="value">The value.</param>
        /// <returns>The short option.</returns>
        public static string ShortOpt(char option, string value)
        {
            return $"{ShortOptionSymbol}{option}{AssignmentSymbol}{value}";
        }
    }
}
