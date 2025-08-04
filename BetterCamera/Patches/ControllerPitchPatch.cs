using HarmonyLib;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem; // for Gamepad

namespace BetterCamera.Patches
{
    /// <summary>
    /// Feeds the gamepad right stick Y-axis into cameraPitchDelta,
    /// but only when you actually move the stick—so KBM still works
    /// if you haven’t touched the controller.
    /// </summary>
    [HarmonyPatch(typeof(CameraController), nameof(CameraController.RotateCamera))]
    static class ControllerPitchPatch
    {
        // private field in CameraController
        static readonly FieldInfo fCameraPitchDelta =
            AccessTools.Field(typeof(CameraController), "cameraPitchDelta");

        // small threshold so float jitter doesn’t count as “input”
        const float DEADZONE = 0.1f;

        [HarmonyPrefix]
        static void Prefix(CameraController __instance, ref bool orbit)
        {
            if (!BetterCamera.Instance.IsEnabled.Value)
                return;

            var gp = Gamepad.current;
            if (gp == null)
                return;

            float y = gp.rightStick.ReadValue().y;
            if (Mathf.Abs(y) <= DEADZONE)
                return;

            // apply only actual stick movement
            fCameraPitchDelta.SetValue(__instance, y);
            // force orbit so our global “no snap-back” still applies
            orbit = true;
        }
    }
}
