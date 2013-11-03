using System;
using System.Linq;
using System.Runtime.Serialization;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Vinyl.Transformer
{
    public class MainClass
    {
        public static void Main(string[] args)
        {
            var assembly = Mono.Cecil.AssemblyDefinition.ReadAssembly(args[0]);
            Transformer.TransformAssembly(assembly);
            assembly.Write(args[0]);
        }
    }
}
