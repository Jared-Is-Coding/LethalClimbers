using LethalLib.Modules;

namespace LethalClimbers.Patches
{
    internal class ItemPatch
    {
        public static void Start()
        {
            // Create item
            Item battery = BasePlugin.ItemAssetBundle.LoadAsset<Item>("Items/Battery");

            // Register item with other clients
            NetworkPrefabs.RegisterNetworkPrefab(battery.spawnPrefab);

            // Fix doubled sounds and other things
            Utilities.FixMixerGroups(battery.spawnPrefab);

            // Register item locally
            Items.RegisterScrap(battery, 1000, Levels.LevelTypes.All);

            // Prepare item for sale
            TerminalNode terminalNode = UnityEngine.ScriptableObject.CreateInstance<TerminalNode>();
            terminalNode.clearPreviousText = true;
            terminalNode.displayText = "This is info about a battery";
            Items.RegisterShopItem(battery, null, null, terminalNode, 1000);
        }
    }
}
