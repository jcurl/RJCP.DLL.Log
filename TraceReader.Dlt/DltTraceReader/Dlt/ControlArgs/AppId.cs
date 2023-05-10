namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Application Id info type.
    /// </summary>
    /// <remarks>
    /// This is intended to be a data holding class that should be used by the classes implementing support for the Get
    /// Log Info DLT control message payload. It provides a structure support similar to the one being described in
    /// section 7.7.7.1.5.2 AppIDsType of the Diagnostic Log and Trace AUTOSAR Release 4.2.2 specification.
    /// <para>
    /// Besides exposing the name and the optional description of the application ID, it also holds information about
    /// the associated context IDs.
    /// </para>
    /// </remarks>
    public sealed class AppId
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppId"/> class.
        /// </summary>
        /// <param name="name">The name of the application ID.</param>
        public AppId(string name) : this(name, string.Empty) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppId"/> class.
        /// </summary>
        /// <param name="name">The name of the application ID.</param>
        /// <param name="description">The description of the application ID.</param>
        /// <exception cref="ArgumentNullException"><paramref name="description"/> is <see langword="null"/>.</exception>
        public AppId(string name, string description)
        {
            Name = name ?? string.Empty;
            ContextIds = new List<ContextId>();
            Description = description ?? string.Empty;
        }

        /// <summary>
        /// Gets the name of the application ID.
        /// </summary>
        /// <value>The name of the application ID.</value>
        /// <remarks>
        /// According to the Diagnostic Log and Trace AUTOSAR Release 4.2.2 specification, the name of an application ID
        /// is expected to have a maximum of 4 characters.
        /// </remarks>
        public string Name { get; }

        /// <summary>
        /// Gets the context ids associated with this application ID.
        /// </summary>
        /// <value>The context ids associated with this application ID.</value>
        /// <remarks>Can be populated with <see cref="ContextId"/> instances.</remarks>
        public IList<ContextId> ContextIds { get; }

        /// <summary>
        /// Gets the description of the application ID.
        /// </summary>
        /// <value>The description of the application ID.</value>
        /// <exception cref="ArgumentNullException">Set value is <see langword="null"/>.</exception>
        public string Description { get; }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        /// <remarks>The result shall not include the <see cref="Description"/> value.</remarks>
        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            StringAppend(result);
            return result.ToString();
        }

        /// <summary>
        /// Appends the argument to the end of the <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="strBuilder">The string builder to append this argument to.</param>
        public void StringAppend(StringBuilder strBuilder)
        {
            if (strBuilder is null) throw new ArgumentNullException(nameof(strBuilder));

            if (!string.IsNullOrEmpty(Name)) {
                strBuilder.Append(Name).Append(" (");
            } else {
                strBuilder.Append('(');
            }

            bool isFirst = true;
            foreach (ContextId context in ContextIds) {
                if (!isFirst) strBuilder.Append(", ");
                context.StringAppend(strBuilder);
                isFirst = false;
            }

            strBuilder.Append(')');
        }
    }
}
