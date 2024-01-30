using GameNetcodeStuff;
using HarmonyLib;

namespace LethalClimbers.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class PlayerControllerBPatch
    {
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void ladderSprintPatch(ref float ___sprintMeter, ref float ___sprintTime, ref bool ___isClimbingLadder)
        {
            // Quickly replenish stamina while the player is on a ladder
            if (___isClimbingLadder)
            {
                ___sprintMeter = UnityEngine.Mathf.Clamp(___sprintMeter + 0.002f + UnityEngine.Time.deltaTime / (___sprintTime), 0f, 1f); ;

                // Debug logging
                BasePlugin.LogSource.LogDebug($"Climber is resting. Sprint meter: {___sprintMeter}");
            }
        }
    }
}
