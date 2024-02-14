namespace RJCP.Diagnostics.Log.Decoder
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Decodes a line written by TeraTerm, which has a time stamp at the front.
    /// </summary>
    public class TeraTermDecoder : TextDecoderBase<LogTraceLine>
    {
        private int m_Line;
        private DateTime m_CurrentTimeStamp = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);

        /// <summary>
        /// Gets the line that was decoded.
        /// </summary>
        /// <param name="line">The line as a memory slice.</param>
        /// <param name="position">The position in the stream where the memory slice starts.</param>
        /// <returns>A new <see cref="LogTraceLine"/> derived object.</returns>
        /// <remarks>
        /// Use the <paramref name="line"/> as a span to convert to a string, or to further process. The
        /// <see cref="Span{T}"/> reduces the number of string copy operations when scanning the string, which can
        /// result in faster decoding.
        /// </remarks>
        protected override LogTraceLine GetLine(Span<char> line, long position)
        {
            string text;
            if (line.Length >= 30 && line[0] == '[' && line[29] == ']' && GetTimeStamp(line)) {
                if (line.Length >= 31) {
                    text = line[31..].ToString();
                } else {
                    text = string.Empty;
                }
            } else {
                text = new string(line);
            }

            LogTraceLine traceLine = new(text, m_Line, position) {
                TimeStamp = m_CurrentTimeStamp
            };
            m_Line++;
            return traceLine;
        }

        private bool GetTimeStamp(ReadOnlySpan<char> line)
        {
            // Format is "[Fri Aug 19 16:25:30.861 2011] "
            bool success = true;

            int month = -1;
            ReadOnlySpan<char> sMonth = line[5..8];
            if (sMonth.Equals("Jan", StringComparison.OrdinalIgnoreCase)) {
                month = 1;
            } else if (sMonth.Equals("Feb", StringComparison.OrdinalIgnoreCase)) {
                month = 2;
            } else if (sMonth.Equals("Mar", StringComparison.OrdinalIgnoreCase)) {
                month = 3;
            } else if (sMonth.Equals("Apr", StringComparison.OrdinalIgnoreCase)) {
                month = 4;
            } else if (sMonth.Equals("May", StringComparison.OrdinalIgnoreCase)) {
                month = 5;
            } else if (sMonth.Equals("Jun", StringComparison.OrdinalIgnoreCase)) {
                month = 6;
            } else if (sMonth.Equals("Jul", StringComparison.OrdinalIgnoreCase)) {
                month = 7;
            } else if (sMonth.Equals("Aug", StringComparison.OrdinalIgnoreCase)) {
                month = 8;
            } else if (sMonth.Equals("Sep", StringComparison.OrdinalIgnoreCase)) {
                month = 9;
            } else if (sMonth.Equals("Oct", StringComparison.OrdinalIgnoreCase)) {
                month = 10;
            } else if (sMonth.Equals("Nov", StringComparison.OrdinalIgnoreCase)) {
                month = 11;
            } else if (sMonth.Equals("Dec", StringComparison.OrdinalIgnoreCase)) {
                month = 12;
            } else {
                success = false;
            }

            // Compiler is too dumb to see that at the end of the block, if success is true, then all parameters
            // are defined.
            int day = 0, hour = 0, min = 0, sec = 0, millisec = 0, year = 0;

            success = success && int.TryParse(line[9..11], NumberStyles.None, CultureInfo.InvariantCulture, out day);
            success = success && int.TryParse(line[12..14], NumberStyles.None, CultureInfo.InvariantCulture, out hour);
            success = success && int.TryParse(line[15..17], NumberStyles.None, CultureInfo.InvariantCulture, out min);
            success = success && int.TryParse(line[18..20], NumberStyles.None, CultureInfo.InvariantCulture, out sec);
            success = success && int.TryParse(line[21..24], NumberStyles.None, CultureInfo.InvariantCulture, out millisec);
            success = success && int.TryParse(line[25..29], NumberStyles.None, CultureInfo.InvariantCulture, out year);

            if (success) {
                try {
                    m_CurrentTimeStamp = new DateTime(year, month, day, hour, min, sec, millisec, DateTimeKind.Local);
                } catch (ArgumentOutOfRangeException) {
                    success = false;
                }
            }
            return success;
        }
    }
}
