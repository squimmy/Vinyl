using System;
using System.Linq;
using Mono.Cecil;

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
                {
                    var fieldName = StringUtils.FirstCharToUpper(param.Name);
                    var field = new FieldDefinition(fieldName,
                                                    FieldAttributes.Public |
                                                    FieldAttributes.InitOnly,
                                                    param.ParameterType);
                    t.Fields.Add(field);
                    Console.WriteLine(param.Name);
                }

                a.Write(args[0]);
            }
        }
    }
}
