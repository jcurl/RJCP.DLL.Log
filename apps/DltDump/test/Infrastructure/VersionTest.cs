namespace RJCP.App.DltDump.Infrastructure
{
    using NUnit.Framework;

    [TestFixture]
    public class VersionTest
    {
        [Test]
        public void GetAssemblyVersion()
        {
            string version = Version.GetAssemblyVersion(typeof(VersionTest));

            // This is the version in the .csproj file. There's no easy way to test the two use cases between
            // information version, and the assembly version.
            Assert.That(version, Is.EqualTo("1.0.0"));
        }

        [Test]
        public void GetAssemblyCopyright()
        {
            string copyright = Version.GetAssemblyCopyright(typeof(VersionTest));

            Assert.That(copyright, Is.EqualTo("(C) 2022, Jason Curl"));
        }
    }
}
