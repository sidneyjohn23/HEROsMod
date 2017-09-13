﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Graphics;
using System;
using Terraria;

namespace HEROsMod.UIKit.UIComponents
{
	internal class Slot : UIView
	{
		public Item item = new Item();
		public int index = -1;
		public static Texture2D backgroundTexture = Main.inventoryBack9Texture;
		public bool functionalSlot = false;
		public bool IsTrachCan { get; set; }

		public event EventHandler ItemChanged;

		private bool rightClicking;

		public Slot(Vector2 position, int itemNum, bool noclick = false)
		{
            Position = position;
			Init(itemNum, noclick);
		}

		public Slot(int itemNum, bool noclick = false)
		{
			Init(itemNum, noclick);
		}

		private void Init(int netID, bool noclick)
		{
            Scale = .85f;
			item.netDefaults(netID);
			if (!noclick)
			{
                onLeftClick += Slot2_onLeftClick;
                onRightClick += Slot2_onRightClick;
                onMouseDown += Slot2_onMouseDown;
			}
            onHover += Slot2_onHover;
		}

		protected override float GetWidth()
		{
			return backgroundTexture.Width * Scale;
		}

		protected override float GetHeight()
		{
			return backgroundTexture.Height * Scale;
		}

		private void Slot2_onHover(object sender, EventArgs e)
		{
			HoverText = item.Name;
			HoverItem = item.Clone();
		}

		private void Slot2_onLeftClick(object sender, EventArgs e)
		{
			if (IsTrachCan)
			{
				Player player = Main.player[Main.myPlayer];
				//if (Main.mouseItem.type != 0)
				//{
				//	player.trashItem.SetDefaults(0, false);
				//}
				//Utils.Swap<Item>(ref player.trashItem, ref Main.mouseItem);
				//if (Main.mouseItem.type > 0)
				//{
				//	player.trashItem = Main.mouseItem.Clone();
				//	Main.mouseItem.SetDefaults(0);
				//}
				//else if (player.trashItem.type > 0)
				//{
				//	Main.mouseItem = player.trashItem.Clone();
				//	player.trashItem.SetDefaults(0);
				//}
				//if (Main.mouseItem.type != 0)
				//{
				//	Main.NewText("trashItem1 " + player.trashItem );
				//	player.trashItem.SetDefaults(0, false);
				//	Main.NewText("trashItem2 " + player.trashItem );
				//}
				//Utils.Swap<Item>(ref player.trashItem, ref Main.mouseItem);
				//if (Main.mouseItem.type > 0)
				//{
				//	Main.NewText("Mouseitem1 " + Main.mouseItem.name);
				//	Main.NewText("trashItem1 " + player.trashItem );
				//	player.trashItem = Main.mouseItem.Clone();
				//	//Main.mouseItem.SetDefaults(0);
				//	Main.NewText("trashItem2 " + player.trashItem );
				//}
				//else if (player.trashItem.type > 0)
				//{
				//	Main.mouseItem = player.trashItem.Clone();
				//	player.trashItem.SetDefaults(0);
				//}
			}
			else if (functionalSlot)
			{
				Item oldMouseItem = Main.mouseItem.Clone();
				Main.mouseItem = item.Clone();
				item = oldMouseItem.Clone();
				if (Main.mouseItem.IsNotTheSameAs(item) && ItemChanged != null)
				{
					ItemChanged(this, EventArgs.Empty);
				}
			}
			else if (Main.mouseItem.type == 0)
			{
				if (Main.keyState.IsKeyDown(Keys.LeftShift))
				{
					Main.player[Main.myPlayer].QuickSpawnItem(item.type, item.maxStack);
					return;
				}
				Main.mouseItem = item.Clone();
				Main.mouseItem.stack = Main.mouseItem.maxStack;
			}
		}

		private void Slot2_onRightClick(object sender, EventArgs e)
		{
			//if (IsTrachCan)
			//{
			//}
			//else if (functionalSlot)
			//{
			//}
			//else if (Main.mouseItem.type == 0)
			//{
			//	if (Main.stackSplit <= 1 && item.type > 0 && (Main.mouseItem.IsTheSameAs(item) || Main.mouseItem.type == 0))
			//	{
			//		int num2 = Main.superFastStack + 1;
			//		for (int j = 0; j < num2; j++)
			//		{
			//			if ((Main.mouseItem.stack < Main.mouseItem.maxStack || Main.mouseItem.type == 0) && item.stack > 0)
			//			{
			//				if (j == 0)
			//				{
			//					Main.PlaySound(18, -1, -1, 1);
			//				}
			//				if (Main.mouseItem.type == 0)
			//				{
			//					Main.mouseItem.netDefaults(item.netID);
			//					if (item.prefix != 0)
			//					{
			//						Main.mouseItem.Prefix((int)item.prefix);
			//					}
			//					Main.mouseItem.stack = 0;
			//				}
			//				Main.mouseItem.stack++;
			//				if (Main.stackSplit == 0)
			//				{
			//					Main.stackSplit = 15;
			//				}
			//				else
			//				{
			//					Main.stackSplit = Main.stackDelay;
			//				}
			//			}
			//		}
			//	}
			//}
		}

		private void Slot2_onMouseDown(object sender, byte button)
		{
			if (button == 0)
			{
				return;
			}
			rightClicking = true;
		}

		public override void Update()
		{
			if (!UIView.MouseRightButton)
			{
                rightClicking = false;
			}
			if (rightClicking)
			{
				Main.playerInventory = true;

				if (Main.stackSplit <= 1 && item.type > 0 && (Main.mouseItem.IsTheSameAs(item) || Main.mouseItem.type == 0))
				{
					int num2 = Main.superFastStack + 1;
					for (int j = 0; j < num2; j++)
					{
						if ((Main.mouseItem.stack < Main.mouseItem.maxStack || Main.mouseItem.type == 0) && item.stack > 0)
						{
							if (j == 0)
							{
								Main.PlaySound(18, -1, -1, 1);
							}
							if (Main.mouseItem.type == 0)
							{
								Main.mouseItem.netDefaults(item.netID);
								if (item.prefix != 0)
								{
									Main.mouseItem.Prefix((int)item.prefix);
								}
								Main.mouseItem.stack = 0;
							}
							Main.mouseItem.stack++;
							if (Main.stackSplit == 0)
							{
								Main.stackSplit = 15;
							}
							else
							{
								Main.stackSplit = Main.stackDelay;
							}
						}
					}
				}
			}
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(IsTrachCan ? Main.inventoryBack7Texture : backgroundTexture, DrawPosition, null, Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);

			if (IsTrachCan)
			{
                item = Main.player[Main.myPlayer].trashItem;
			}
			Texture2D texture = Main.itemTexture[item.type];
			if (IsTrachCan && item.type == 0)
			{
				texture = Main.trashTexture;
			}
			Rectangle rectangle2;
			if (Main.itemAnimations[item.type] != null)
			{
				rectangle2 = Main.itemAnimations[item.type].GetFrame(texture);
			}
			else
			{
				rectangle2 = texture.Frame(1, 1, 0, 0);
			}

			float itemScale = 1f;
			float pixelWidth = backgroundTexture.Width * Scale * .6f;
			if (rectangle2.Width > pixelWidth || rectangle2.Height > pixelWidth)
			{
				if (rectangle2.Width > texture.Height)
				{
					itemScale = pixelWidth / (float)rectangle2.Width;
				}
				else
				{
					itemScale = pixelWidth / (float)rectangle2.Height;
				}
			}
			//itemScale *= Scale;
			Vector2 pos = DrawPosition;
			pos.X += backgroundTexture.Width * Scale / 2 - (rectangle2.Width * itemScale / 2);
			pos.Y += backgroundTexture.Height * Scale / 2 - (rectangle2.Height * itemScale / 2);

			if (IsTrachCan && item.type == 0)
			{
				spriteBatch.Draw(texture, pos, null, new Color(100, 100, 100, 100), 0f, Vector2.Zero, itemScale, SpriteEffects.None, 0f);
			}
			else
			{
				spriteBatch.Draw(texture, pos, new Rectangle?(rectangle2), item.GetAlpha(Color.White), 0f, Vector2.Zero, itemScale, SpriteEffects.None, 0f);
				if (item.color != default(Color))
				{
					spriteBatch.Draw(texture, pos, new Rectangle?(rectangle2), item.GetColor(Color.White), 0f, Vector2.Zero, itemScale, SpriteEffects.None, 0f);
				}
				if (item.stack > 1)
				{
					spriteBatch.DrawString(Main.fontItemStack, item.stack.ToString(), new Vector2(DrawPosition.X + 10f * Scale, DrawPosition.Y + 26f * Scale), Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
				}
			}

			base.Draw(spriteBatch);
		}
	}
}