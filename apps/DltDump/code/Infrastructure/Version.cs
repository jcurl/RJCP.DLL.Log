namespace RJCP.App.DltDump.Infrastructure
{
    using System;
    using System.Reflection;

    public static class Version
    {
        public static string GetAssemblyVersion(Type type)
        {
            Assembly assembly = type.Assembly;
            if (Attribute.GetCustomAttribute(assembly,
                typeof(AssemblyInformationalVersionAttribute)) is AssemblyInformationalVersionAttribute infoVersion) {
                return infoVersion.InformationalVersion;
            }
            return assembly.GetName().Version.ToString();
        }

        public static string GetAssemblyCopyright(Type type)
        {
            Assembly assembly = type.Assembly;
            if (Attribute.GetCustomAttribute(assembly,
                typeof(AssemblyCopyrightAttribute)) is AssemblyCopyrightAttribute copyright) {
                return copyright.Copyright;
            }
            return string.Empty;
        }
    }
}
