using System;
using System.Linq;

namespace Vinyl.Transformer
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var a = Mono.Cecil.AssemblyDefinition.ReadAssembly(args[0]);

            foreach (var t in from module in a.Modules
                              from type in module.GetTypes()
                              where type.CustomAttributes.Any(x => x.AttributeType.FullName == typeof(Vinyl.RecordAttribute).FullName)
                              select type)
            {
                var constructor = (from method in t.Methods
                                   where method.IsConstructor
                                   select method).Single();
                foreach (var param in constructor.Parameters)
                    Console.WriteLine(param.Name);
            }
        }
    }
}
