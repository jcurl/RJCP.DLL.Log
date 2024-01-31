namespace RJCP.App.DltDump.Infrastructure
{
    using NUnit.Framework;

    [TestFixture]
    public class VersionTest
    {
        [Test]
        public void GetAssemblyVersion()
        {
            // Note, we're testing `Version.GetAssemblyVersion`, not the actual DltDump program version.
            string version = Version.GetAssemblyVersion(typeof(VersionTest));

            // This is the version in the .csproj file. There's no easy way to test the two use cases between
            // information version, and the assembly version.

            // When building, the version depends on the SDK that is being used (not the runtime):
            // * On .NET Core 7.x and earlier, the Informational version is "1.0.0".
            // * On .NET Core 8.0 and later, the Informational version is "1.0.0+githash".
            Assert.That(version, Is.EqualTo("1.0.0").Or.StartsWith("1.0.0+"));
        }

        [Test]
        public void GetAssemblyCopyright()
        {
            // Note, we're testing `Version.GetAssemblyVersion`, not the actual DltDump program version.
            string copyright = Version.GetAssemblyCopyright(typeof(VersionTest));

            Assert.That(copyright, Is.EqualTo("(C) 2022-2024, Jason Curl"));
        }
    }
}
