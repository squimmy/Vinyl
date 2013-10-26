using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Vinyl.Transformer
{
    public class MainClass
    {
        public static void Main(string[] args)
        {
            var a = Mono.Cecil.AssemblyDefinition.ReadAssembly(args[0]);

            foreach (var t in from module in a.Modules
                              from type in module.GetTypes()
                              where type.CustomAttributes.Any(x => x.AttributeType.FullName == typeof(Vinyl.RecordAttribute).FullName)
                              select type)
            {
                MethodDefinition constructor = (from method in t.Methods
                                   where method.IsConstructor
                                   select method).Single();
                int argCount = 0;
                foreach (var param in constructor.Parameters)
                {
                    var fieldName = StringUtils.FirstCharToUpper(param.Name);
                    var field = new FieldDefinition(fieldName,
                                                    FieldAttributes.Public |
                                                    FieldAttributes.InitOnly,
                                                    param.ParameterType);
                    ++argCount;
                    Instruction loadArg;
                    if (argCount == 1)
                        loadArg = Instruction.Create(OpCodes.Ldarg_1);
                    else if (argCount == 2)
                        loadArg = Instruction.Create(OpCodes.Ldarg_2);
                    else if (argCount == 3)
                        loadArg = Instruction.Create(OpCodes.Ldarg_3);
                    else
                        loadArg = Instruction.Create(OpCodes.Ldarg_S, argCount);

                    t.Fields.Add(field);
                    var instructions = constructor.Body.Instructions;

                    instructions.InsertRange(instructions.Count - 1,
                        Instruction.Create(OpCodes.Ldarg_0),
                        loadArg,
                        Instruction.Create(OpCodes.Stfld, field));
                }

                a.Write(args[0]);
            }
        }
    }
}
