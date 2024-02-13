using LethalLib.Modules;

namespace LethalClimbers.Patches
{
	internal class ItemPatch
	{
		// Moon types
		private class MoonTypes
		{
			public static readonly Levels.LevelTypes IndustrialMoons = Levels.LevelTypes.ExperimentationLevel;
			public static readonly Levels.LevelTypes ForestMoonws = Levels.LevelTypes.VowLevel & Levels.LevelTypes.MarchLevel;
			public static readonly Levels.LevelTypes DesertMoons = Levels.LevelTypes.AssuranceLevel & Levels.LevelTypes.OffenseLevel;
			public static readonly Levels.LevelTypes IceMoons = Levels.LevelTypes.DineLevel & Levels.LevelTypes.RendLevel & Levels.LevelTypes.TitanLevel;
		}

		// List of custom items
		private static readonly ItemData[] ItemList = new ItemData[]
		{
			new ItemData("Chalk Brush", 40),
			new ItemData("Chalk Bucket", 40),
			new ItemData("Climbing Hold", 40),
			new ItemData("Grigri", 40),
			new ItemData("Helmet", 40),
			new ItemData("Ice Axe", 30, MoonTypes.IceMoons),
			new ItemData("Quickdraw", 40),
			new ItemData("Rope", 30)
		};

		private class ItemData
		{
			private Item ItemRef;
			private string Path;
			private string Name;
			private Levels.LevelTypes ValidMoons;
			private int Rarity;
			private bool IsStoreItem;
			private string StoreDescription;
			private int StoreValue;

			private void WriteItemData(string path, int rarity, Levels.LevelTypes validMoons, bool isStoreItem, string storeDescription, int storeValue)
			{
				Path = $"Assets/Items/{path}/{path}.asset";
				Name = path.Split(".")[0];
				Rarity = rarity;
				ValidMoons = validMoons;
				IsStoreItem = isStoreItem;
				StoreDescription = storeDescription;
				StoreValue = storeValue;
			}

			public ItemData(string path, int rarity)
			{
				WriteItemData(path, rarity, Levels.LevelTypes.All, false, string.Empty, 0);
			}

			public ItemData(string path, int rarity, Levels.LevelTypes validMoons)
			{
				WriteItemData(path, rarity, validMoons, false, string.Empty, 0);
			}

			public ItemData(string path, int rarity, Levels.LevelTypes validMoons, bool isStoreItem, string storeDescription, int storeValue)
			{
				WriteItemData(path, rarity, validMoons, isStoreItem, storeDescription, storeValue);
			}

			public string GetPath() { return Path; }
			public string GetName() { return Name; }
			public int GetRarity() { return Rarity; }
			public Levels.LevelTypes GetValidMoons() { return ValidMoons; }
			public bool GetIsStoreItem() { return IsStoreItem; }
			public string GetStoreDescription() { return StoreDescription; }
			public int GetStoreValue() { return StoreValue; }
			public void SetItem(Item ItemToSet) { ItemRef = ItemToSet; }
			public Item GetItem() { return ItemRef; }
		}

		public static void Start()
		{
			Item ThisScrapItemAsset;

			foreach (ItemData ThisScrapItem in ItemList)
			{
				// Load item
				ThisScrapItemAsset = BasePlugin.ItemAssetBundle.LoadAsset<Item>(ThisScrapItem.GetPath());

				// Register item with other clients
				NetworkPrefabs.RegisterNetworkPrefab(ThisScrapItemAsset.spawnPrefab);

				// Register item locally
				Items.RegisterScrap(ThisScrapItemAsset, ThisScrapItem.GetRarity(), ThisScrapItem.GetValidMoons());

				// Fix doubled sounds and other things
				Utilities.FixMixerGroups(ThisScrapItemAsset.spawnPrefab);

				// Set item reference in ItemData class
				ThisScrapItem.SetItem(ThisScrapItemAsset);

				// Prepare item for sale needed
				if (ThisScrapItem.GetIsStoreItem())
				{
					TerminalNode terminalNode = UnityEngine.ScriptableObject.CreateInstance<TerminalNode>();
					terminalNode.clearPreviousText = true;
					terminalNode.displayText = $"Here's some info about a {ThisScrapItem.GetName()}:\n\n{ThisScrapItem.GetStoreDescription()}";
					Items.RegisterShopItem(ThisScrapItemAsset, null, null, terminalNode, ThisScrapItem.GetStoreValue());
				}

				// Debug logging
				BasePlugin.LogSource.LogDebug($"Climbing item loaded: {ThisScrapItemAsset.name}");
			}
		}
	}
}
