using LethalLib.Modules;

namespace LethalClimbers.Patches
{
    internal class ItemPatch
    {
        public class ItemData
        {
            private string AssetName;
            private int Rarity;
            private bool IsStoreItem;
            private Item ItemRef;

            public ItemData(string name, int rarity, bool isStoreItem)
            {
                AssetName = "Assets/Items/" + name;
                Rarity = rarity;
                IsStoreItem = isStoreItem;
            }

            public string GetItemPath() { return AssetName; }
            public int GetRarity() { return Rarity; }
            public bool GetIsStoreItem() { return IsStoreItem; }
            public void SetItem(Item ItemToSet) { ItemRef = ItemToSet; }
            public Item GetItem() { return ItemRef; }

        }

        public static ItemData[] ItemList = new ItemData[] {
            new ItemData("ChalkBagItem.asset", 1000, true),
            new ItemData("MatchBoxItem.asset", 1000, true),
            // new ItemData("TapeItem.asset", 1000, true)
        };

        public static void Start()
        {
            Item ThisScrapItemAsset;

            foreach (ItemData ThisScrapItem in ItemList)
            {
                // Load item
                ThisScrapItemAsset = BasePlugin.ItemAssetBundle.LoadAsset<Item>(ThisScrapItem.GetItemPath());

                // Register item with other clients
                NetworkPrefabs.RegisterNetworkPrefab(ThisScrapItemAsset.spawnPrefab);

                // Register item locally
                Items.RegisterScrap(ThisScrapItemAsset, ThisScrapItem.GetRarity(), Levels.LevelTypes.All);

                // Fix doubled sounds and other things
                Utilities.FixMixerGroups(ThisScrapItemAsset.spawnPrefab);

                // Set the item reference in the ItemData class
                ThisScrapItem.SetItem(ThisScrapItemAsset);

                // Prepare item for sale if it needs it
                if (ThisScrapItem.GetIsStoreItem())
                {
                    TerminalNode terminalNode = UnityEngine.ScriptableObject.CreateInstance<TerminalNode>();
                    terminalNode.clearPreviousText = true;
                    terminalNode.displayText = string.Format("This is info about a {0}\n\n", ThisScrapItemAsset.name);
                    Items.RegisterShopItem(ThisScrapItemAsset, null, null, terminalNode, 0);
                }

                BasePlugin.LogSource.LogInfo("Item Loaded: " + ThisScrapItemAsset.name);
            }
        }
    }
}
