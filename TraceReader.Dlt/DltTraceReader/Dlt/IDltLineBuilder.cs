namespace RJCP.Diagnostics.Log.Dlt
{
    using System;
    using System.Collections.Generic;
    using Args;
    using ControlArgs;

    /// <summary>
    /// DLT trace line builder interface.
    /// </summary>
    /// <remarks>
    /// This interface has been designed following the Diagnostic Log and Trace AutoSAR Release 4.2.2 specification.
    /// </remarks>
    public interface IDltLineBuilder
    {
        /// <summary>
        /// Prepare the builder for the construction of a new DLT trace line.
        /// </summary>
        /// <returns>The current instance of the <see cref="IDltLineBuilder"/>.</returns>
        /// <remarks>
        /// Calling this method can result in losing the previously created DLT trace line, unless the DLT trace line is
        /// retrieved by calling the <see cref="GetResult()"/> method.
        /// </remarks>
        IDltLineBuilder Reset();

        /// <summary>
        /// Creates and returns the DLT trace line instance.
        /// </summary>
        /// <returns>The DLT trace line instance.</returns>
        /// <remarks>
        /// This method should be used after all DLT trace line fields have been populated, otherwise the resulting
        /// trace line instance shall be incomplete.
        /// </remarks>
        DltTraceLineBase GetResult();

        /// <summary>
        /// Indicates that bytes were skipped. Take a snapshot of the time stamps.
        /// </summary>
        /// <param name="bytes">The number of bytes that were skipped.</param>
        /// <param name="reason">The reason why bytes were skipped.</param>
        void AddSkippedBytes(int bytes, string reason);

        /// <summary>
        /// Gets the current number of skipped bytes.
        /// </summary>
        /// <value>The number of skipped bytes.</value>
        long SkippedBytes { get; }

        /// <summary>
        /// Creates and returns the DLT trace line instance expressing skipped data.
        /// </summary>
        /// <returns>The DLT trace line for skipped bytes.</returns>
        DltTraceLineBase GetSkippedResult();

        /// <summary>
        /// Gets the features which are set for this line.
        /// </summary>
        /// <value>The features set for this line.</value>
        DltLineFeatures Features { get; set; }

        /// <summary>
        /// Gets or sets the ECU identifier.
        /// </summary>
        /// <value>The ECU identifier.</value>
        string EcuId { get; }

        /// <summary>
        /// Sets the ECU identifier.
        /// </summary>
        /// <param name="id">The ECU identifier.</param>
        /// <returns>The current instance of the <see cref="IDltLineBuilder"/>.</returns>
        /// <remarks>
        /// Following the Diagnostic Log and Trace AutoSAR Release 4.2.2 specification, the ECU ID is expected to be no
        /// longer than 4 ASCII characters. Using this method will not set the WEID bit and update the ECU ID if the
        /// WEID bit is not already set with a call to <see cref="SetEcuId(string)"/>.
        /// </remarks>
        IDltLineBuilder SetStorageHeaderEcuId(string id);

        /// <summary>
        /// Sets the ECU identifier.
        /// </summary>
        /// <param name="id">The ECU identifier.</param>
        /// <returns>The current instance of the <see cref="IDltLineBuilder"/>.</returns>
        /// <remarks>
        /// Following the Diagnostic Log and Trace AutoSAR Release 4.2.2 specification, the ECU ID is expected to be no
        /// longer than 4 ASCII characters.
        /// </remarks>
        IDltLineBuilder SetEcuId(string id);

        /// <summary>
        /// Gets or sets the application identifier.
        /// </summary>
        /// <value>The application identifier.</value>
        string ApplicationId { get; }

        /// <summary>
        /// Sets the application identifier.
        /// </summary>
        /// <param name="id">The application identifier.</param>
        /// <returns>The current instance of the <see cref="IDltLineBuilder"/>.</returns>
        /// <remarks>
        /// Following the Diagnostic Log and Trace AutoSAR Release 4.2.2 specification, the Application ID is expected
        /// to be no longer than 4 ASCII characters.
        /// </remarks>
        IDltLineBuilder SetApplicationId(string id);

        /// <summary>
        /// Gets or sets the context identifier.
        /// </summary>
        /// <value>The context identifier.</value>
        string ContextId { get; }

        /// <summary>
        /// Sets the context identifier.
        /// </summary>
        /// <param name="id">The context identifier.</param>
        /// <returns>The current instance of the <see cref="IDltLineBuilder"/>.</returns>
        /// <remarks>
        /// Following the Diagnostic Log and Trace AutoSAR Release 4.2.2 specification, the Context ID is expected to be
        /// no longer than 4 ASCII characters.
        /// </remarks>
        IDltLineBuilder SetContextId(string id);

        /// <summary>
        /// Gets or sets the message counter value.
        /// </summary>
        /// <value>The message counter value.</value>
        int Count { get; }

        /// <summary>
        /// Sets the message counter value.
        /// </summary>
        /// <param name="value">The message counter value.</param>
        /// <returns>The current instance of the <see cref="IDltLineBuilder"/>.</returns>
        IDltLineBuilder SetCount(byte value);

        /// <summary>
        /// Gets or sets the device time stamp.
        /// </summary>
        /// <value>The device time stamp.</value>
        TimeSpan DeviceTimeStamp { get; }

        /// <summary>
        /// Sets the device time stamp.
        /// </summary>
        /// <param name="ticks">The number of ticks.</param>
        /// <returns>The current instance of the <see cref="IDltLineBuilder"/>.</returns>
        /// <remarks>
        /// The measurement unit being used is the tick, which is equal to 100 nanoseconds or one ten-millionth of a
        /// second. There are 10,000 ticks in a millisecond.
        /// </remarks>
        IDltLineBuilder SetDeviceTimeStamp(long ticks);

        /// <summary>
        /// Gets or sets the type of the DLT message.
        /// </summary>
        /// <value>The type of the DLT message.</value>
        DltType DltType { get; }

        /// <summary>
        /// Sets the type and subtype of the DLT trace line.
        /// </summary>
        /// <param name="type">The type and subtype of the DLT trace line.</param>
        /// <returns>The current instance of the <see cref="IDltLineBuilder"/>.</returns>
        IDltLineBuilder SetDltType(DltType type);

        /// <summary>
        /// Gets or sets a value indicating whether this instance is verbose message.
        /// </summary>
        /// <value>
        /// Returns <see langword="true"/> if this instance is verbose; otherwise, <see langword="false"/>.
        /// </value>
        bool IsVerbose { get; }

        /// <summary>
        /// Set the trace line to verbose or non-verbose mode.
        /// </summary>
        /// <param name="value">Boolean value indicating if the DLT trace line is in verbose or non-verbose mode.</param>
        /// <returns>The current instance of the <see cref="IDltLineBuilder"/>.</returns>
        IDltLineBuilder SetIsVerbose(bool value);

        /// <summary>
        /// Gets or sets the session identifier.
        /// </summary>
        /// <value>The session identifier.</value>
        int SessionId { get; }

        /// <summary>
        /// Sets the session identifier.
        /// </summary>
        /// <param name="id">The session identifier.</param>
        /// <returns>The current instance of the <see cref="IDltLineBuilder"/>.</returns>
        /// <remarks>
        /// Following the Diagnostic Log and Trace AutoSAR Release 4.2.2 specification, the Session ID field shall be a
        /// 32-bit unsigned integer.
        /// </remarks>
        IDltLineBuilder SetSessionId(int id);

        /// <summary>
        /// Gets or sets the time stamp.
        /// </summary>
        /// <value>The time stamp of the device.</value>
        DateTime TimeStamp { get; }

        /// <summary>
        /// Sets the time stamp.
        /// </summary>
        /// <param name="seconds">The number of seconds since 01.01.1970 (UNIX time).</param>
        /// <param name="microseconds">The number of microseconds of the second (between 0 and 1.000.000).</param>
        /// <returns>The current instance of the <see cref="IDltLineBuilder"/>.</returns>
        IDltLineBuilder SetTimeStamp(int seconds, int microseconds);

        /// <summary>
        /// Sets the time stamp.
        /// </summary>
        /// <param name="dateTime">The logging time stamp.</param>
        /// <returns>The current instance of the <see cref="IDltLineBuilder"/>.</returns>
        IDltLineBuilder SetTimeStamp(DateTime dateTime);

        /// <summary>
        /// Gets or sets the position in the stream.
        /// </summary>
        /// <value>The position in the stream.</value>
        long Position { get; }

        /// <summary>
        /// Sets the position of the stream where the line is decoded.
        /// </summary>
        /// <param name="position">The position in the stream where the line is decoded.</param>
        /// <returns>The current instance of the <see cref="IDltLineBuilder"/>.</returns>
        IDltLineBuilder SetPosition(long position);

        /// <summary>
        /// Gets a value indicating whether arguments are encoded as big endian.
        /// </summary>
        /// <value>
        /// Is <see langword="true"/> if arguments are encoded big endian; otherwise, <see langword="false"/>.
        /// </value>
        bool BigEndian { get; }

        /// <summary>
        /// Sets the if the arguments of the trace line are encoded using big endian.
        /// </summary>
        /// <param name="bigEndian">
        /// if set to <see langword="true"/> then arguments are encoded as big endian, else if <see langword="false"/>,
        /// they're encoded as little endian..
        /// </param>
        /// <returns>The current instance of the <see cref="IDltLineBuilder"/>.</returns>
        IDltLineBuilder SetBigEndian(bool bigEndian);

        /// <summary>
        /// Gets the number of arguments found in the payload of a verbose message.
        /// </summary>
        /// <value>The number of arguments found in the payload of a verbose message.</value>
        /// <remarks>
        /// The value being returned is expected to be the same as it is found in the extended header of a DLT packet.
        /// </remarks>
        int NumberOfArgs { get; }

        /// <summary>
        /// Sets the number of arguments found in the payload of a verbose message.
        /// </summary>
        /// <param name="value">The argument count.</param>
        /// <returns>The current instance of the <see cref="IDltLineBuilder"/>.</returns>
        /// <remarks>
        /// The value being set is expected to be the same as it is found in the extended header of a DLT packet.
        /// </remarks>
        IDltLineBuilder SetNumberOfArgs(byte value);

        /// <summary>
        /// Gets the arguments as a non-modifiable list.
        /// </summary>
        /// <value>The arguments.</value>
        IReadOnlyList<IDltArg> Arguments { get; }

        /// <summary>
        /// Adds the given argument to the payload.
        /// </summary>
        /// <param name="argument">The argument to be added to the payload.</param>
        /// <returns>The current instance of the <see cref="IDltLineBuilder"/>.</returns>
        IDltLineBuilder AddArgument(IDltArg argument);

        /// <summary>
        /// Adds the given arguments to the payload.
        /// </summary>
        /// <param name="arguments">The argument to be added to the payload.</param>
        /// <returns>The current instance of the <see cref="IDltLineBuilder"/>.</returns>
        IDltLineBuilder AddArguments(IEnumerable<IDltArg> arguments);

        /// <summary>
        /// Gets the control payload.
        /// </summary>
        /// <value>The control payload.</value>
        IControlArg ControlPayload { get; }

        /// <summary>
        /// Sets the control payload.
        /// </summary>
        /// <param name="service">The control payload.</param>
        /// <returns>The current instance of the <see cref="IDltLineBuilder"/>.</returns>
        IDltLineBuilder SetControlPayload(IControlArg service);
    }
}
