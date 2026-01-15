using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

[HarmonyPatch(typeof(ToolItemStatesLiquid), "TakeLiquid")]
public class LiquidTranspiler
{
    public static IEnumerable<CodeInstruction> Transpiler(
        IEnumerable<CodeInstruction> instructions,
        ILGenerator generator
    )
    {
        var continueLabel = generator.DefineLabel();

        foreach (var instruction in instructions)
        {
            if (instruction.opcode == OpCodes.Ldloca_S)
            {
                yield return CodeInstruction.Call(typeof(CheatPack4), "IsShardCheatEnabled");
                yield return new CodeInstruction(OpCodes.Brfalse, continueLabel);
                yield return new CodeInstruction(OpCodes.Ret);
                instruction.labels.Add(continueLabel);
            }
            yield return instruction;
        }
    }
}
