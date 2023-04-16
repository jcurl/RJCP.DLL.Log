namespace RJCP.Diagnostics.Log.Dlt.NonVerbose.Fibex
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// An exception occurred when loading a FIBEX file.
    /// </summary>
    [Serializable]
    public class FibexDuplicateKeyException : Exception
    {
        private readonly int m_MessageId;
        private readonly string m_EcuId;
        private readonly string m_AppId;
        private readonly string m_CtxId;

        /// <summary>
        /// Initializes a new instance of the <see cref="FibexDuplicateKeyException"/> class.
        /// </summary>
        public FibexDuplicateKeyException() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FibexDuplicateKeyException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public FibexDuplicateKeyException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FibexDuplicateKeyException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="inner">The inner exception.</param>
        public FibexDuplicateKeyException(string message, Exception inner) : base(message, inner) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FibexDuplicateKeyException"/> class.
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        /// <param name="message">The message that describes the error.</param>
        public FibexDuplicateKeyException(int messageId, string message)
            : this(messageId, null, null, null, message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FibexDuplicateKeyException"/> class.
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        /// <param name="appId">The application identifier.</param>
        /// <param name="ctxId">The context identifier.</param>
        /// <param name="message">The message that describes the error.</param>
        public FibexDuplicateKeyException(int messageId, string appId, string ctxId, string message)
            : this(messageId, appId, ctxId, null, message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FibexDuplicateKeyException"/> class.
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        /// <param name="appId">The application identifier.</param>
        /// <param name="ctxId">The context identifier.</param>
        /// <param name="ecuId">The ECU identifier.</param>
        /// <param name="message">The message that describes the error.</param>
        public FibexDuplicateKeyException(int messageId, string appId, string ctxId, string ecuId, string message)
            : base(message)
        {
            m_MessageId = messageId;
            m_AppId = appId;
            m_CtxId = ctxId;
            m_EcuId = ecuId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FibexDuplicateKeyException"/> class.
        /// </summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
        /// </param>
        protected FibexDuplicateKeyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            m_MessageId = info.GetInt32("messageid");
            m_EcuId = info.GetString("ecuid");
            m_AppId = info.GetString("appid");
            m_CtxId = info.GetString("ctxid");
        }

        /// <summary>
        /// When overridden in a derived class, sets the <see cref="SerializationInfo"/> with information about the
        /// exception.
        /// </summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
        /// </param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("messageid", m_MessageId);
            info.AddValue("ecuid", m_EcuId);
            info.AddValue("appid", m_AppId);
            info.AddValue("ctxid", m_CtxId);
            base.GetObjectData(info, context);
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            return $"{base.ToString()} (ECUID={m_EcuId ?? string.Empty} APPID={m_AppId ?? string.Empty} CTXID={m_CtxId ?? string.Empty}";
        }
    }
}
