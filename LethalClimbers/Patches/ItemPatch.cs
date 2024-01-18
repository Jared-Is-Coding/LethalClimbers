using LethalLib.Modules;

namespace LethalClimbers.Patches
{
    internal class ItemPatch
    {
        public class ItemData
        {
            private string Path;
            private string Name;
            private string Description;
            private int Rarity;
            private bool IsStoreItem;
            private int StoreValue;
            private Item ItemRef;

            private void WriteItemData(string path, int rarity, bool isStoreItem, string description, int storeValue)
            {
                Path = "Assets/Items/" + path;
                Name = path.Split(".")[0];
                Rarity = rarity;
                IsStoreItem = isStoreItem;
                Description = description;
                StoreValue = storeValue;
            }

            public ItemData(string path, int rarity)
            {
                WriteItemData(path, rarity, false, 0);
            }

            public ItemData(string path, int rarity, bool isStoreItem)
            {
                WriteItemData(path, rarity, isStoreItem, "None", 0);
            }

            public ItemData(string path, int rarity, bool isStoreItem, string description)
            {
                WriteItemData(path, rarity, isStoreItem, "None", 0);
            }

            public ItemData(string path, int rarity, bool isStoreItem, string description, int storeValue)
            {
                WriteItemData(path, rarity, isStoreItem, "None", storeValue);
            }

            public string GetItemPath() { return Path; }
            public string GetItemName() { return Name; }
            public string GetItemDescription() { return Description; }
            public int GetRarity() { return Rarity; }
            public bool GetIsStoreItem() { return IsStoreItem; }
            public int GetStoreValue() { return StoreValue; }
            public void SetItem(Item ItemToSet) { ItemRef = ItemToSet; }
            public Item GetItem() { return ItemRef; }

        }

        public static ItemData[] ItemList = new ItemData[] {
            new ItemData("Grigri.asset", 1000)
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
                    terminalNode.displayText = string.Format("This is info about a {0}\n\n", ThisScrapItem.GetItemName());
                    Items.RegisterShopItem(ThisScrapItemAsset, null, null, terminalNode, ThisScrapItem.GetStoreValue());
                }

                BasePlugin.LogSource.LogInfo("Item Loaded: " + ThisScrapItemAsset.name);
            }
        }
    }
}
