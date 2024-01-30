using HarmonyLib;
using UnityEngine;

namespace LethalClimbers.Patches
{
    [HarmonyPatch(typeof(BoomboxItem))]
    internal class BoomBoxItemPatch
    {
        [HarmonyPatch(nameof(BoomboxItem.Start))]
        [HarmonyPostfix]
        public static void Start_Patch(BoomboxItem __instance)
        {
            int newClipCount = BasePlugin.BoomBoxItemAudioClips.Count;

            // Get the original instance audio
            AudioClip[] originalMusic = __instance.musicAudios;

            // Extend the array by one
            __instance.musicAudios = new AudioClip[originalMusic.Length + newClipCount];

            // Fill the new array with the old instance audio clips
            int musicArrayIndex = 0;
            foreach (AudioClip thisAudioClip in originalMusic)
            {
                __instance.musicAudios[musicArrayIndex] = thisAudioClip;
                musicArrayIndex++;
            }

            // Add our new music
            foreach (AudioClip thisAudioClip in BasePlugin.BoomBoxItemAudioClips)
            {
                __instance.musicAudios[musicArrayIndex] = thisAudioClip;
                musicArrayIndex++;

                // Debug logging
                BasePlugin.LogSource.LogDebug($"{__instance} - Added new music track: {thisAudioClip}");
            }

        }
    }
}
