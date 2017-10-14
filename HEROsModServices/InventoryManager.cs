using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.UI;

namespace HEROsMod.HEROsModServices
{
	internal class InventoryManager : HEROsModService
	{
		private static float scale = .5f;

		private static Player player
		{
			get { return Main.player[Main.myPlayer]; }
		}

		private static bool[] lockedSlots = new bool[40];

		public static string categoryName = "Inventory Manager";

		// public static KeyBinding kQuickMoveItem;
		private static KeyBinding kQuickStack;

		private static KeyBinding kSortInventory;
		private static KeyBinding kSwapHotbar;

		private static int[] _itemSortArray;
		private static bool Loaded;

		public InventoryManager()
		{
            _name = "Inventory Manager";
		}

		public override void Unload()
		{
			Loaded = false;
		}

		public static void SetKeyBindings()
		{
			kQuickStack = KeybindController.AddKeyBinding("Quick Stack", "Q");
			kSortInventory = KeybindController.AddKeyBinding("Sort Inventory", "C");
			kSwapHotbar = KeybindController.AddKeyBinding("Swap Hotbar", "V");
		}

		public override void Update()
		{
			if (!Main.gameMenu && !Main.mapFullscreen)
			{
				if (Main.playerInventory)
				{
					if (kSortInventory.KeyPressed)
					{
						ModUtils.Sort();
						Recipe.FindRecipes();
					}
				}
                
				
					if (kQuickStack.KeyPressed)
					{
						Player player = Main.player[Main.myPlayer];
						if (player.chest != -1)
						{
							ChestUI.QuickStack();
						}
						else
						{
							player.QuickStackAllChests();
							Recipe.FindRecipes();
						}
					}
				

				if (kSwapHotbar.KeyPressed)
				{
					SwapHotbar();
				}
			}
		}

		private static void SwapHotbar()
		{
			Item[] tempItems = new Item[10];
			for (int i = 0; i < 10; i++)
			{
				tempItems[i] = (Item)player.inventory[i].Clone();
				player.inventory[i] = (Item)player.inventory[40 + i].Clone();
			}

			for (int i = 0; i < 10; i++)
			{
				player.inventory[40 + i] = (Item)tempItems[i].Clone();
			}
			Main.PlaySound(7, -1, -1, 1);
		}

		private static void DrawLocks(SpriteBatch spriteBatch)
		{
			if (Main.playerInventory)
			{
				float inventoryScale = .85f;
				for (int x = 0; x < 10; x++)
				{
					for (int y = 1; y < 5; y++)
					{
						int itemPosX = (int)(20f + (float)(x * 56) * inventoryScale);
						int itemPosY = (int)(20f + (float)(y * 56) * inventoryScale);
						int itemNum = x + y * 10;

						bool locked = lockedSlots[itemNum - 10];
						if (locked)
						{
							float scale = .75f;
							Vector2 pos = new Vector2(itemPosX + 25, itemPosY + 2);
							spriteBatch.Draw(Main.HBLockTexture[0], pos, null, Color.DarkGray * .8f, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
						}
					}
				}
			}
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			DrawLocks(spriteBatch);
		}
	}
}