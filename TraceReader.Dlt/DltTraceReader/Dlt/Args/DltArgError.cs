namespace RJCP.Diagnostics.Log.Dlt.Args
{
    using System.Runtime.CompilerServices;
    using System.Text;

    /// <summary>
    /// This class represents a decoder error, with the message in the argument string.
    /// </summary>
    public class DltArgError : IDltArg
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DltArgError"/> class.
        /// </summary>
        /// <param name="message">The message describing the error.</param>
        public DltArgError(string message)
        {
            SetMessage(message);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DltArgError"/> class.
        /// </summary>
        /// <param name="format">The format message describing the error.</param>
        /// <param name="args">The arguments for the format.</param>
        public DltArgError(string format, params object[] args)
        {
            SetMessage(format, args);
        }

        /// <summary>
        /// Gets or sets the message associated with this decoding error.
        /// </summary>
        /// <value>The message associated with this decoding error.</value>
        public string Message { get; private set; }

        /// <summary>
        /// Sets the message.
        /// </summary>
        /// <param name="message">The message describing the error.</param>
        public void SetMessage(string message)
        {
            Message = message ?? string.Empty;
        }

        /// <summary>
        /// Sets the message.
        /// </summary>
        /// <param name="format">The format message describing the error.</param>
        /// <param name="args">The arguments for the format.</param>
        public void SetMessage(string format, params object[] args)
        {
            Message = string.Format(format, args);
        }

        /// <summary>
        /// Appends the argument to the end of the <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="strBuilder">The string builder to append this argument to.</param>
        /// <returns>StringBuilder.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public StringBuilder Append(StringBuilder strBuilder)
        {
            return strBuilder.Append(Message);
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            return Message;
        }
    }
}
