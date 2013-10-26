using NUnit.Framework;
using System;
using System.IO;
using System.Reflection;

namespace Vinyl.Tests
{
    [TestFixture()]
    public class Test
    {
        [SetUp()]
        public void SetUp()
        {
            var a = Assembly.GetExecutingAssembly();
            var s = a.GetManifestResourceStream("Vinyl.Tests.Test.dll");
            using (var sw = File.Create("Test.dll"))
            {
                s.Seek(0, SeekOrigin.Begin);
                s.CopyTo(sw);
            }
        }

        [Test()]
        public void TestCase()
        {
        }

        [TearDown()]
        public void TearDown()
        {
            File.Delete("Test.dll");
        }
    }
}

