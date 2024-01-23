using GameNetcodeStuff;
using HarmonyLib;

namespace LethalClimbers.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class PlayerControllerBPatch
    {
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void infiniteSprintPatch(ref float ___sprintMeter)
        {
            ___sprintMeter = 1f;
        }
    }
}
