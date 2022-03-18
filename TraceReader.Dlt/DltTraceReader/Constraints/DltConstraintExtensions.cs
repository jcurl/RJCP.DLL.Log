namespace RJCP.Diagnostics.Log.Constraints
{
    using System;
    using Dlt;

    /// <summary>
    /// Defines extension methods on top of <see cref="Constraint"/> for building constraint expressions.
    /// </summary>
    public static class DltConstraintExtensions
    {
        /// <summary>
        /// A check constraint against a DLT ECU Id <see cref="DltTraceLineBase.EcuId"/>.
        /// </summary>
        /// <param name="constraint">The <see cref="Constraint"/> to apply the expression for.</param>
        /// <param name="id">The ECU identifier that should match.</param>
        /// <returns>The <see cref="Constraint"/> object for chaining.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="id"/> is <see langword="null"/>.</exception>
        public static Constraint DltEcuId(this Constraint constraint, string id)
        {
            return constraint.Expr(new DltEcuId(id));
        }

        /// <summary>
        /// A check constraint against a DLT Context Id <see cref="DltTraceLineBase.ContextId"/>.
        /// </summary>
        /// <param name="constraint">The <see cref="Constraint"/> to apply the expression for.</param>
        /// <param name="id">The Context identifier that should match.</param>
        /// <returns>The <see cref="Constraint"/> object for chaining.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="id"/> is <see langword="null"/>.</exception>
        public static Constraint DltCtxId(this Constraint constraint, string id)
        {
            return constraint.Expr(new DltCtxId(id));
        }

        /// <summary>
        /// A check constraint against a DLT Application Identifier <see cref="DltTraceLineBase.ApplicationId"/>.
        /// </summary>
        /// <param name="constraint">The <see cref="Constraint"/> to apply the expression for.</param>
        /// <param name="appId">The Application identifier that should match.</param>
        /// <returns>The <see cref="Constraint"/> object for chaining.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="appId"/> is <see langword="null"/>.</exception>
        public static Constraint DltAppId(this Constraint constraint, string appId)
        {
            return constraint.Expr(new DltAppId(appId));
        }

        /// <summary>
        /// A check constraint against the DLT type and subtype <see cref="DltTraceLineBase.Type"/>.
        /// </summary>
        /// <param name="constraint">The <see cref="Constraint"/> to apply the expression for.</param>
        /// <param name="type">The DLT Type that should be identical.</param>
        /// <returns>The <see cref="Constraint"/> object for chaining.</returns>
        /// <remarks>
        /// For more details about the DLT type and subtype possible values, see <see cref="DltType"/>.
        /// </remarks>
        public static Constraint DltType(this Constraint constraint, DltType type)
        {
            return constraint.Expr(new DltMessageType(type));
        }

        /// <summary>
        /// A check constraint against the DLT <see cref="DltLineFeatures.IsVerbose"/> flag.
        /// </summary>
        /// <param name="constraint">The <see cref="Constraint"/> to apply the expression for.</param>
        /// <param name="isVerbose">
        /// If <see langword="true"/>, the <see cref="DltTraceLineBase"/> is expected to be
        /// <see cref="DltLineFeatures.IsVerbose"/>.
        /// </param>
        /// <returns>The <see cref="Constraint"/> object for chaining.</returns>
        public static Constraint DltIsVerbose(this Constraint constraint, bool isVerbose)
        {
            return constraint.Expr(new DltIsVerbose(isVerbose));
        }

        /// <summary>
        /// A check constraint against the DLT <see cref="DltTraceLineBase.SessionId"/> flag.
        /// </summary>
        /// <param name="constraint">The <see cref="Constraint"/> to apply the expression for.</param>
        /// <param name="sessionId">The session identifier that should match.</param>
        /// <returns>The <see cref="Constraint"/> object for chaining.</returns>
        public static Constraint DltSessionId(this Constraint constraint, int sessionId)
        {
            return constraint.Expr(new DltSessionId(sessionId));
        }

        /// <summary>
        /// Checks that <see cref="DltTraceLineBase.DeviceTimeStamp"/> is more than a specified value.
        /// </summary>
        /// <param name="constraint">The <see cref="Constraint"/> to apply the expression for.</param>
        /// <param name="milliseconds">The device time in milliseconds, that should have been reached.</param>
        /// <returns>The <see cref="Constraint"/> object for chaining.</returns>
        /// <exception cref="InvalidOperationException">Constraints expression is read only.</exception>
        public static Constraint Awake(this Constraint constraint, int milliseconds)
        {
            return constraint.Expr(new Awake(milliseconds));
        }

        /// <summary>
        /// Checks that <see cref="DltTraceLineBase.DeviceTimeStamp"/> is more than a specified value.
        /// </summary>
        /// <param name="constraint">The <see cref="Constraint"/> to apply the expression for.</param>
        /// <param name="deviceTime">The device time that should have been reached.</param>
        /// <returns>The <see cref="Constraint"/> object for chaining.</returns>
        /// <exception cref="InvalidOperationException">Constraints expression is read only.</exception>
        public static Constraint Awake(this Constraint constraint, TimeSpan deviceTime)
        {
            return constraint.Expr(new Awake(deviceTime));
        }
    }
}
