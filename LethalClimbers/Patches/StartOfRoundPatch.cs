using HarmonyLib;
using UnityEngine;

namespace LethalClimbers.Patches
{
	[HarmonyPatch(typeof(StartOfRound))]
	internal class StartOfRoundPatch
	{
		[HarmonyPatch("Start")]
		[HarmonyPostfix]
		static void AudioOnStartPatch(StartOfRound __instance)
		{
			__instance.shipIntroSpeechSFX = BasePlugin.ItemAssetBundle.LoadAsset<AudioClip>("Assets/Sounds/vlad_likes_climbing.wav");
		}
	}
}
