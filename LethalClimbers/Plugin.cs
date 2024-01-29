using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using LethalClimbers.Patches;
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
        private const string ModVersion = "1.0.4"; // This should be bumped up for every release

        // Logging
        public static ManualLogSource LogSource;

        // Harmony framework prep
        private readonly Harmony harmony = new Harmony(ModGUID);

        // Assets preparation
        public static AssetBundle ItemAssetBundle;

        // Audio clip lists
        public static List<AudioClip> MouthDogAIAudioClips = new List<AudioClip>();

        void Awake()
        {
            // Safety catch
            if (Instance == null)
            {
                Instance = this;
            }

            // Prepare logger
            LogSource = BepInEx.Logging.Logger.CreateLogSource(ModGUID);

            // Prepare item assets bundle
            string ItemBundlePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "customitems");
            ItemAssetBundle = AssetBundle.LoadFromFile(ItemBundlePath);

            // Items patch
            ItemPatch.Start();

            // Stamina patch
            harmony.PatchAll(typeof(PlayerControllerBPatch));

            // Start of round audio patch
            // harmony.PatchAll(typeof(StartOfRoundPatch));

            // MouthDogAI audio patch
            MouthDogAIAudioClips.Add(ItemAssetBundle.LoadAsset<AudioClip>("Assets/Sounds/Enemies/MouthDog/OndraYell1.wav"));
            harmony.PatchAll(typeof(MouthDogAIPatch));

            // Plugin startup notice
            LogSource.LogInfo($"{ModGUID} is loaded.");
        }
    }
}
