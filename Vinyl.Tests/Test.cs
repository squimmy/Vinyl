using System;
using System.CodeDom.Compiler;
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
            var thisAssembly = Assembly.GetExecutingAssembly();
            var outputDirectory = "Test.dll";

            var codeProvider = new Microsoft.CSharp.CSharpCodeProvider();
            var options = new CompilerParameters();
            options.OutputAssembly = outputDirectory;
            options.ReferencedAssemblies.Add("Vinyl");

            using (var source = thisAssembly.GetManifestResourceStream("Vinyl.Tests.SampleCode.cs"))
            using (var sr = new StreamReader(source))
            {
                var compileResult = codeProvider.CompileAssemblyFromSource(options, sr.ReadToEnd());
                if (compileResult.Errors.HasErrors)
                    throw new Exception(
                        "Compilation failed: \n" +
                        string.Join("\n", from error in compileResult.Errors.Cast<CompilerError>()
                                          select "\t" + error.ToString()));
            }

            Vinyl.Transformer.MainClass.Main(new [] {outputDirectory});

            testAssembly = Assembly.LoadFile(outputDirectory);
        }

        [Test()]
        public void Fields_Are_Generated_For_Each_Constructor_Arg()
        {
            var person = testAssembly.GetType("Test.Person");

            var fields = from field in person.GetFields()
                         select Tuple.Create(field.FieldType, field.Name.ToLower());

            var constructorArgs = from param in person.GetConstructors().Single().GetParameters()
                                  select Tuple.Create(param.ParameterType, param.Name.ToLower());
            CollectionAssert.AreEqual(constructorArgs, fields);
        }

        [Test()]
        public void Construtor_Args_Are_Assigned_To_Fields()
        {
            var person = testAssembly.GetType("Test.Person");
            var instance = Activator.CreateInstance(person, new object[] {42, "Joe"});
            var age = person.GetField("Age").GetValue(instance);
            var name = person.GetField("Name").GetValue(instance);
            Assert.AreEqual(42, age);
            Assert.AreEqual("Joe", name);
        }

        [Test()]
        public void Fields_Are_Readonly()
        {
            var person = testAssembly.GetType("Test.Person");
            foreach (var field in person.GetFields())
                Assert.IsTrue(field.IsInitOnly);
        }

        [Test()]
        public void Classes_Are_Sealed()
        {
            var person = testAssembly.GetType("Test.Person");
            Assert.IsTrue(person.IsSealed);
        }

        [TearDown()]
        public void TearDown()
        {
            File.Delete("Test.dll");
        }
    }
}

