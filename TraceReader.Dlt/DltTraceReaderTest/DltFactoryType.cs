namespace RJCP.Diagnostics.Log
{
    /// <summary>
    /// The type of factory to use when testing.
    /// </summary>
    /// <remarks>
    /// Used as part of NUnit test cases to easily test the very closely related types of DLT packets.
    /// </remarks>
    public enum DltFactoryType
    {
        Standard,
        File,
        Serial
    }
}
