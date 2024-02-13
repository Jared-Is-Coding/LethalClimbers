using HarmonyLib;
using UnityEngine;

namespace LethalClimbers.Patches
{
	[HarmonyPatch(typeof(BoomboxItem))]
	internal class BoomBoxItemPatch
	{
		[HarmonyPatch(nameof(BoomboxItem.Start))]
		[HarmonyPostfix]
		public static void StartPatch(BoomboxItem __instance)
		{
			int newClipCount = BasePlugin.BoomBoxItemAudioClips.Count;

			// Get original instance audio
			AudioClip[] originalMusic = __instance.musicAudios;

			// Extend the array by one
			__instance.musicAudios = new AudioClip[originalMusic.Length + newClipCount];

			// Fill new array with old instance audio clips
			int musicArrayIndex = 0;
			foreach (AudioClip thisAudioClip in originalMusic)
			{
				__instance.musicAudios[musicArrayIndex] = thisAudioClip;
				musicArrayIndex++;
			}

			// Add new music
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
