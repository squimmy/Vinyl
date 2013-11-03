using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Runtime.Serialization;

namespace Vinyl.Transformer
{
    public class Transformer
    {
        public static void TransformAssembly(AssemblyDefinition assembly)
        {
            foreach (var module in assembly.Modules)
                transformModule(module);
        }

        private static void transformModule(ModuleDefinition module)
        {
            var dataContractConstructor =
                module.Import(typeof(DataContractAttribute).GetConstructors().Single());
            var dataContract = new CustomAttribute(dataContractConstructor);

            var dataMemberConstructor =
                module.Import(typeof(DataMemberAttribute).GetConstructors().Single());
            var dataMember = new CustomAttribute(dataMemberConstructor);

            foreach (var type in module.Types.Where(hasRecordAttribute))
                transformType(type, dataContract, dataMember);
        }

        private static void transformType(TypeDefinition type,
                                          CustomAttribute dataContract,
                                          CustomAttribute dataMember)
        {
            foreach (var field in type.Fields)
                transformField(field, dataMember);

            var constructor = type.Methods.Where(m => m.IsConstructor).Single();

            transformConstructor(constructor, type.Fields);

            type.IsSealed = true;
            type.CustomAttributes.Add(dataContract);
        }

        private static void transformField(FieldDefinition field,
                                           CustomAttribute dataMember)
        {
            field.IsInitOnly = true;
            field.CustomAttributes.Add(dataMember);
        }

        private static void transformConstructor(MethodDefinition constructor,
                                                IEnumerable<FieldDefinition> fields)
        {
            var parameters = fields.Select(f =>
               new ParameterDefinition(StringUtils.FirstCharToLower(f.Name),
                        　             ParameterAttributes.None,
                        　             f.FieldType));
            foreach (var parameter in parameters)
                constructor.Parameters.Add(parameter);

            var instructions = constructor.Body.Instructions;
            var assignmentInstructions = fields.Select((f, i) =>
                new Instruction[] {
                    ILHelper.GetLoadArg(0),
                    ILHelper.GetLoadArg(i + 1),
                    Instruction.Create(OpCodes.Stfld, f)
            });

            instructions.InsertRange<Instruction>(instructions.Count - 1,
                                                  from instructionSet in assignmentInstructions
                                                  from instruction in instructionSet
                                                  select instruction);
        }

        private static Boolean hasRecordAttribute(TypeDefinition type)
        {
            return type.CustomAttributes.Any(x =>
                x.AttributeType.FullName == typeof(Vinyl.RecordAttribute).FullName);
        }
    }
}
