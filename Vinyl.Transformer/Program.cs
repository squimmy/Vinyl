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
            var a = Mono.Cecil.AssemblyDefinition.ReadAssembly(args[0]);

            foreach (var t in from module in a.Modules
                              from type in module.GetTypes()
                              where type.CustomAttributes.Any(x => x.AttributeType.FullName == typeof(Vinyl.RecordAttribute).FullName)
                              select type)
            {

                var dataContractConstructor = t.Module.Import(typeof(DataContractAttribute).GetConstructors().Single());
                var dataContract = new CustomAttribute(dataContractConstructor);

                var dataMemberConstructor = t.Module.Import(typeof(DataMemberAttribute).GetConstructors().Single());
                var dataMember = new CustomAttribute(dataMemberConstructor);

                MethodDefinition constructor = (from method in t.Methods
                                   where method.IsConstructor
                                   select method).Single();
                int argCount = 0;
                foreach (FieldDefinition field in t.Fields)
                {
                    field.IsInitOnly = true;

                    var paramName = StringUtils.FirstCharToLower(field.Name);
                    var param = new ParameterDefinition(paramName,
                                                        ParameterAttributes.None,
                                                        field.FieldType);
                    constructor.Parameters.Add(param);

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

                    var instructions = constructor.Body.Instructions;

                    instructions.InsertRange(instructions.Count - 1,
                        Instruction.Create(OpCodes.Ldarg_0),
                        loadArg,
                        Instruction.Create(OpCodes.Stfld, field));

                    field.CustomAttributes.Add(dataMember);
                }

                t.IsSealed = true;
                t.CustomAttributes.Add(dataContract);
            }
            a.Write(args[0]);
        }
    }
}
