using System;
using Mono.Cecil.Cil;

namespace Vinyl.Transformer
{
    public class ILHelper
    {
        public static Instruction GetLoadArg(int index)
        {
            if (index == 1)
                return Instruction.Create(OpCodes.Ldarg_1);
            else if (index == 2)
                return Instruction.Create(OpCodes.Ldarg_2);
            else if (index == 3)
                return Instruction.Create(OpCodes.Ldarg_3);
            else
                return Instruction.Create(OpCodes.Ldarg_S, index);
        }
    }
}
