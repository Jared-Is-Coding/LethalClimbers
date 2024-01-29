using HarmonyLib;
using System;
using System.Linq;

namespace LethalClimbers.Patches
{
    [HarmonyPatch(typeof(MouthDogAI))]
    internal class MouthDogAIPatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        static void audioStart(MouthDogAI __instance)
        {
            // Substitute a new scream SFX every time the MouthDogAI is updated
            __instance.screamSFX = BasePlugin.MouthDogAIAudioClips.OrderBy(x => Guid.NewGuid()).FirstOrDefault();
        }

        // This is so wildly buggy lol
        /*[HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void audioUpdate(MouthDogAI __instance, ref bool ___inLunge, ref float ___lungeCooldown)
        {
            // Play scream SFX every time the MouthDogAI inLunge is true

            // TODO | Need to get this to properly cool down... lol

            if (___inLunge && ___lungeCooldown < 0.01f)
            {
                __instance.creatureVoice.PlayOneShot(BasePlugin.MouthDogAIAudioClips.OrderBy(x => Guid.NewGuid()).FirstOrDefault());
            }

        }*/
    }
}
