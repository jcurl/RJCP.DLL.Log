namespace RJCP.Diagnostics.Log.Dlt.Args
{
    using System.Text;

    /// <summary>
    /// IDltArg represents a single argument found in a DLT verbose message's payload.
    /// </summary>
    public interface IDltArg
    {
        /// <summary>
        /// Appends the argument to the end of the <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="strBuilder">The string builder to append this argument to.</param>
        /// <returns>The original <paramref name="strBuilder"/> for chaining.</returns>
        StringBuilder Append(StringBuilder strBuilder);
    }
}
