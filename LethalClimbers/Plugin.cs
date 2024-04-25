using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using LethalClimbers.Patches;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace LethalClimbers
{
	[BepInPlugin(ModGUID, ModName, ModVersion)]
	public class BasePlugin : BaseUnityPlugin
	{
		private static BasePlugin Instance;

		// Base mod configuration 
		private const string ModGUID = "jarediscoding.lethalclimbers";
		private const string ModName = "Lethal Climbers";
		private const string ModVersion = "1.2.0"; // This should be bumped up for every release

		// Logging
		public static ManualLogSource LogSource;

		// Harmony framework prep
		private readonly Harmony harmony = new Harmony(ModGUID);

		// Assets preparation
		public static AssetBundle ItemAssetBundle;

		// Audio clip lists
		public static List<AudioClip> MouthDogAIAudioClips = new List<AudioClip>();
		public static List<AudioClip> BoomBoxItemAudioClips = new List<AudioClip>();

		// Patch list
		private static readonly Type[] PatchList = new Type[]
		{
			typeof(PlayerControllerBPatch), // Ladder stamina patch
			// typeof(StartOfRoundPatch), // Start of round audio patch
			typeof(MouthDogAIPatch), // MouthDogAI audio patch
			typeof(BoomBoxItemPatch) // BoomBoxItem audio patch
		};

		void Awake()
		{
			// Safety catch
			if (Instance == null)
			{
				Instance = this;
			}

			// Prepare logger
			LogSource = BepInEx.Logging.Logger.CreateLogSource(ModGUID);

			// -------------------------------------------------------- //
			// Items patch
			// -------------------------------------------------------- //

			string ItemBundlePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "customitems");
			ItemAssetBundle = AssetBundle.LoadFromFile(ItemBundlePath);
			ItemPatch.Start();

			LogSource.LogInfo($"Custom items patch complete");

			// -------------------------------------------------------- //
			// Prepare audio clips
			// -------------------------------------------------------- //

			MouthDogAIAudioClips.Add(ItemAssetBundle.LoadAsset<AudioClip>("Assets/Sounds/Enemies/MouthDog/OndraYell1.wav"));
			BoomBoxItemAudioClips.Add(ItemAssetBundle.LoadAsset<AudioClip>("Assets/Sounds/BoomBox/RappSnitch.wav"));
			BoomBoxItemAudioClips.Add(ItemAssetBundle.LoadAsset<AudioClip>("Assets/Sounds/BoomBox/MorgIce.wav"));
			BoomBoxItemAudioClips.Add(ItemAssetBundle.LoadAsset<AudioClip>("Assets/Sounds/BoomBox/FeelGood.wav"));
			BoomBoxItemAudioClips.Add(ItemAssetBundle.LoadAsset<AudioClip>("Assets/Sounds/BoomBox/BoyLiarPt2.wav"));

			// -------------------------------------------------------- //
			// Initialize patches
			// -------------------------------------------------------- //
			
			foreach (Type thisType in PatchList)
			{
				harmony.PatchAll(thisType);

				LogSource.LogDebug($"{thisType} complete");
			}

			// -------------------------------------------------------- //
			// NetcodePatcher
			// See: https://github.com/EvaisaDev/UnityNetcodePatcher
			// -------------------------------------------------------- //

			Type[] types = Assembly.GetExecutingAssembly().GetTypes();

			foreach (var type in types)
			{
				MethodInfo[] methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

				foreach (MethodInfo method in methods)
				{
					object[] attributes = method.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false);

					if (attributes.Length > 0)
					{
						method.Invoke(null, null);
					}
				}
			}

			// -------------------------------------------------------- //
			// Plugin startup completed
			// -------------------------------------------------------- //

			LogSource.LogInfo($"Load complete");
		}
	}
}
