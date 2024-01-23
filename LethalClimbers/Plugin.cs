using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using LethalClimbers.Patches;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace LethalClimbers
{
    [BepInPlugin(ModGUID, ModName, ModVersion)]
    public class BasePlugin : BaseUnityPlugin
    {
        private const string ModGUID = "jarediscoding.lethalclimbers";
        private const string ModName = "Lethal Climbers";
        private const string ModVersion = "0.0.1";

        private readonly Harmony harmony = new Harmony(ModGUID);
        private static BasePlugin Instance;
        internal ManualLogSource LogSource;

        public static AssetBundle ItemAssetBundle;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            LogSource = BepInEx.Logging.Logger.CreateLogSource(ModGUID);

            // Prepare item assets bundle
            string ItemBundlePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "customitems");
            ItemAssetBundle = AssetBundle.LoadFromFile(ItemBundlePath);

            // Start items patch
            // ItemPatch.Start();

            // Patch
            // Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), ModGUID);

            // Infinite sprinting patch
            harmony.PatchAll(typeof(PlayerControllerBPatch));

            // Plugin startup notice
            LogSource.LogInfo($"{ModGUID} is loaded.");
        }
    }
}
