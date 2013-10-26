using System;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace Vinyl.Tests
{
    [TestFixture()]
    public class Test
    {
        private Assembly testAssembly;

        [SetUp()]
        public void SetUp()
        {
            var outputDirectory = "Test.dll";
            var a = Assembly.GetExecutingAssembly();
            var s = a.GetManifestResourceStream("Vinyl.Tests.Test.dll");
            using (var sw = File.Create(outputDirectory))
            {
                s.Seek(0, SeekOrigin.Begin);
                s.CopyTo(sw);
            }
            Vinyl.Transformer.MainClass.Main(new [] {outputDirectory});
            testAssembly = Assembly.LoadFile(outputDirectory);
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

