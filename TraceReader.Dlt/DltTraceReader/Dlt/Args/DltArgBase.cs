namespace RJCP.Diagnostics.Log.Dlt.Args
{
    using System.Text;

    /// <summary>
    /// Base class for defining immutable argument classes of a given data type.
    /// </summary>
    /// <typeparam name="T">The type of the data.</typeparam>
    /// <remarks>Should be extended by classes that define DLT arguments in verbose mode.</remarks>
    public abstract class DltArgBase<T> : IDltArg
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DltArgBase{T}"/> class.
        /// </summary>
        protected DltArgBase() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DltArgBase{T}"/> class with the given data.
        /// </summary>
        /// <param name="data">The value of the data.</param>
        protected DltArgBase(T data)
        {
            Data = data;
        }

        /// <summary>
        /// Gets the value of the data.
        /// </summary>
        /// <value>The data value.</value>
        public T Data { get; }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            return Data.ToString();
        }

        /// <summary>
        /// Appends the argument to the end of the <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="strBuilder">The string builder to append this argument to.</param>
        /// <returns>The reference to the original string builder.</returns>
        public abstract StringBuilder Append(StringBuilder strBuilder);
    }
}
