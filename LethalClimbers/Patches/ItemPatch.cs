using LethalLib.Modules;

namespace LethalClimbers.Patches
{
    internal class ItemPatch
    {
        public class ItemData
        {
            private Item ItemRef;
            private string Path;
            private string Name;
            private int Rarity;
            private bool IsStoreItem;
            private string Description;
            private int StoreValue;

            private void WriteItemData(string path, int rarity, bool isStoreItem, string description, int storeValue)
            {
                Path = "Assets/Items/" + path + "/" + path + ".asset";
                Name = path.Split(".")[0];
                Rarity = rarity;
                IsStoreItem = isStoreItem;
                Description = description;
                StoreValue = storeValue;
            }

            public ItemData(string path, int rarity)
            {
                WriteItemData(path, rarity, false, "None", 0);
            }

            public ItemData(string path, int rarity, bool isStoreItem)
            {
                WriteItemData(path, rarity, isStoreItem, "None", 0);
            }

            public ItemData(string path, int rarity, bool isStoreItem, string description)
            {
                WriteItemData(path, rarity, isStoreItem, description, 0);
            }

            public ItemData(string path, int rarity, bool isStoreItem, string description, int storeValue)
            {
                WriteItemData(path, rarity, isStoreItem, description, storeValue);
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
            new ItemData("Chalk Bucket", 50, false, "Chalk (aka magnesium carbonate, MgCO3) isn’t just for gymnasts: Climbers everywhere sport pasty white hands to combat moisture and improve grip.\n\nThe good news is that this is one climbing accessory where personal preference rules, and over-analysis isn’t needed. It’s OK to simply pick chalk by feel and chalk bags by color.\n\nChalk is either pure MgCO3 or an added drying agent is mixed in. Each formulation has its fans. Some welcome the extra drying, while others prefer the feel of pure chalk and would rather not breathe in the added agents.\n\nChalk bags hold a reservoir of loose chalk that you purchase separately. Plunge your hands in to get a thorough coating.\n\nLeaving behind chalk smears or visual aids like tick marks is considered very bad form. And if you’re generating clouds of dust each time you chalk up, you’re overdoing it. Use chalk judiciously, carry a brush and clean up after yourself. And always ask about the local ethic: Some climbing areas forbid the use of chalk altogether."),
            new ItemData("Climbing Hold", 50, false, "A climbing hold is a shaped grip that is usually attached to a climbing wall so climbers can grab or step on it.\n\nOn most walls, climbing holds are arranged in paths, called routes, by specially trained route setters.\n\nClimbing holds come in a large array of sizes and shapes to provide different levels of challenge to a climber.\n\nClimbing holds are either bolted to a wall via hex-head bolts and existing t-nuts or they are screwed on with several small screws.\n\nIn extreme cases, concrete anchors may be used (if putting holds on the underside of a bridge, for example)."),
            new ItemData("Grigri", 50, false, "The GriGri is an assisted braking belay device manufactured by Petzl. It is primarily used while rock climbing to maintain a safe and effective climbing system.\n\nLike all belay devices, the Grigri is used with climbing harnesses and climbing ropes to create a safe and efficient climbing system. Of course, when lead climbing or top-roping, the belayer must property load the climbing rope through the GriGri before anyone leaves the ground.\n\nOnce the rope is loaded into the GriGri, the device is attached to the belayer’s harness with a locking climbing carabiner. This system allows the belayer to serve as an anchor for the climber. Should the climber fall, the belayer’s weight creates tension in the rope and limits the fall distance in a controlled manner.\n\nThe GriGri allows the belayer to manage the system while the climber makes upward progress. The rope can move in both directions through a GriGri. A good belayer uses the device to maintain an appropriate amount of slack in the system."),
            new ItemData("Helmet", 50, false, "Climbing helmets are designed to protect you against several common climbing scenarios; for example, when:\n\t- rocks or hardware get kicked loose above you\n\t- you peel off and whip into a wall\n\t- you hit your head on an overhang\n\nAll helmets must meet industry standards for impact protection, with the standard for overhead protection being greater than the side-protection standard."),
            new ItemData("Quickdraw", 50, false, "A quickdraw (also known as an extender) is a piece of climbing equipment used by rock and ice climbers to allow the climbing rope to run freely through protection such as bolt anchors or other traditional gear while leading.\n\nA quickdraw consists of two carabiners connected by a semi-rigid material (sometimes called the \"dogbone\"). One carabiner has a straight gate and connects to an anchoring device. The other carabiner is for the climbing rope, and uses a bent gate.\n\nQuickdraws are manufactured with either a solid carabiner gate or a wire carabiner gate for its lighter weight."),
            new ItemData("Rope", 40, false, "A climbing rope is a rope that is used in climbing. It is a critical part of an extensive chain of protective equipment (which also includes climbing harnesses, anchors, belay devices, and carabiners) used by climbers to help prevent potentially fatal fall-related accidents.\n\nClimbing ropes must meet very strict requirements so that they do not break in the event of an accidental fall. However, they also need to be light, flexible for knotting, and resistant to chafing over sharp and rough rocks; all that in all possible weather conditions.\n\nAlthough ropes made of natural fibres such as hemp and flax were used in the early days of alpinism, modern climbing uses kernmantle ropes made of a core of nylon or other synthetic material and intertwined in a special way, surrounded by a separate sheath woven over it. The main strength of the rope is in the core, and the sheath of the rope represents only a small fraction of the overall strength of the rope.\n\nClimbing ropes can be classified into three categories according to their elasticity: static, semi-static, and dynamic ropes.")
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
                    terminalNode.displayText = string.Format("Here's some info about a {0}:\n\n{1}", ThisScrapItem.GetItemName(), ThisScrapItem.GetItemDescription());
                    Items.RegisterShopItem(ThisScrapItemAsset, null, null, terminalNode, ThisScrapItem.GetStoreValue());
                }

                BasePlugin.LogSource.LogInfo("Climbing item loaded: " + ThisScrapItemAsset.name);
            }
        }
    }
}
