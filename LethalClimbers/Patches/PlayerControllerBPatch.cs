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
            if (___isClimbingLadder)
            {
                ___sprintMeter = UnityEngine.Mathf.Clamp(___sprintMeter + UnityEngine.Time.deltaTime / (___sprintTime + 27f), 0f, 1f); ;

                BasePlugin.LogSource.LogInfo("Climber is resting. Sprint meter: " + ___sprintMeter.ToString());
            }
        }
    }
}
