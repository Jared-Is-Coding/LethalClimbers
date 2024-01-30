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
        private const string ModVersion = "1.0.6"; // This should be bumped up for every release

        // Logging
        public static ManualLogSource LogSource;

        // Harmony framework prep
        private readonly Harmony harmony = new Harmony(ModGUID);

        // Assets preparation
        public static AssetBundle ItemAssetBundle;

        // Audio clip lists
        public static List<AudioClip> MouthDogAIAudioClips = new List<AudioClip>();
        public static List<AudioClip> BoomBoxItemAudioClips = new List<AudioClip>();

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
            // Stamina patch
            // -------------------------------------------------------- //

            harmony.PatchAll(typeof(PlayerControllerBPatch));

            LogSource.LogInfo($"Ladder stamina patch complete");

            // -------------------------------------------------------- //
            // Start of round audio patch
            // -------------------------------------------------------- //

            // harmony.PatchAll(typeof(StartOfRoundPatch));

            // LogSource.LogInfo($"Round start audio patch complete");

            // -------------------------------------------------------- //
            // MouthDogAI audio patch
            // -------------------------------------------------------- //

            MouthDogAIAudioClips.Add(ItemAssetBundle.LoadAsset<AudioClip>("Assets/Sounds/Enemies/MouthDog/OndraYell1.wav"));
            harmony.PatchAll(typeof(MouthDogAIPatch));

            LogSource.LogInfo($"Eyeless Dog audio patch complete");

            // -------------------------------------------------------- //
            // BoomBoxItem audio patch
            // -------------------------------------------------------- //

            BoomBoxItemAudioClips.Add(ItemAssetBundle.LoadAsset<AudioClip>("Assets/Sounds/BoomBox/RappSnitch.wav"));
            BoomBoxItemAudioClips.Add(ItemAssetBundle.LoadAsset<AudioClip>("Assets/Sounds/BoomBox/MorgIce.wav"));
            harmony.PatchAll(typeof(BoomBoxItemPatch));

            LogSource.LogInfo($"Boombox audio patch complete");

            // -------------------------------------------------------- //
            // Plugin startup completed
            // -------------------------------------------------------- //

            LogSource.LogInfo($"Load complete");
        }
    }
}
