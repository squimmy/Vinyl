using Mono.Cecil;

namespace Vinyl.Transformer
{
    public class MainClass
    {
        public static void Main(string[] args)
        {
            var assembly = AssemblyDefinition.ReadAssembly(args[0]);
            Transformer.TransformAssembly(assembly);
            assembly.Write(args[0]);
        }
    }
}
