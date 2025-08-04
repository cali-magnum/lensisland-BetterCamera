using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace BetterCamera.Patches
{
    /// <summary>
    /// Transpiles CameraController.RotateCamera to remove
    /// the two zero-assignments that snap your pitch back.
    /// </summary>
    [HarmonyPatch(typeof(CameraController), nameof(CameraController.RotateCamera))]
    static class PitchLockPatch
    {
        // References to the two private fields we want to never clear
        static readonly FieldInfo fTargetXRotOffset =
            AccessTools.Field(typeof(CameraController), "targetXRotOffset");
        static readonly FieldInfo fTargetXRotOffsetReal =
            AccessTools.Field(typeof(CameraController), "targetXRotOffsetReal");

        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count - 2; i++)
            {
                // pattern: ldarg.0; ldc.r4 0f; stfld targetXRotOffsetReal (or targetXRotOffset)
                if (codes[i].opcode == OpCodes.Ldarg_0
                    && codes[i + 1].opcode == OpCodes.Ldc_R4
                    && codes[i + 1].operand is float val && val == 0f
                    && codes[i + 2].opcode == OpCodes.Stfld
                    && codes[i + 2].operand is FieldInfo fi
                    && (fi == fTargetXRotOffsetReal || fi == fTargetXRotOffset))
                {
                    // NOP out all three
                    codes[i].opcode = OpCodes.Nop; codes[i].operand = null;
                    codes[i + 1].opcode = OpCodes.Nop; codes[i + 1].operand = null;
                    codes[i + 2].opcode = OpCodes.Nop; codes[i + 2].operand = null;
                }
            }
            return codes.AsEnumerable();
        }
    }
}
