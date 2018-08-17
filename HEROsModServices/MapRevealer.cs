﻿using HEROsMod.UIKit;
using HEROsMod.UIKit.UIComponents;
using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace HEROsMod.HEROsModServices
{
	/// <summary>
	/// A service that reveals the map for the player
	/// </summary>
	internal class MapRevealer : HEROsModService
	{
		public static MapRevealer instance;

		public MapRevealer(UIHotbar hotbar)
		{
			IsInHotbar = true;
			HotbarParent = hotbar;
			this._name = "Map Revealer";
			this._hotbarIcon = new UIImage(HEROsMod.instance.GetTexture("Images/map")/*ModUtils.RevealMapTexture*/);
			this._hotbarIcon.onLeftClick += _hotbarIcon_onLeftClick;
			this.HotbarIcon.Tooltip = HEROsMod.HeroText("RevealMap");
			instance = this;
		}

		public override void MyGroupUpdated()
		{
            HasPermissionToUse = HEROsModNetwork.LoginService.MyGroup.HasPermission("RevealMap");
			//base.MyGroupUpdated();
		}

		private void _hotbarIcon_onLeftClick(object sender, EventArgs e)
		{
			if (Main.netMode != 1)
			{
				RevealWholeMap();
			}
			else
			{
				Point center = Main.player[Main.myPlayer].Center.ToTileCoordinates();
				RevealAroundPoint(center.X, center.Y);
			}
		}

		public static int MapRevealSize = 300;

		public static void RevealAroundPoint(int x, int y)
		{
			for (int i = x - MapRevealSize / 2; i < x + MapRevealSize / 2; i++)
			{
				for (int j = y - MapRevealSize / 2; j < y + MapRevealSize / 2; j++)
				{
					if (WorldGen.InWorld(i, j))
						Main.Map.Update(i, j, 255);
				}
			}
			Main.refreshMap = true;
		}

		public static void RevealWholeMap()
		{
			for (int i = 0; i < Main.maxTilesX; i++)
			{
				for (int j = 0; j < Main.maxTilesY; j++)
				{
					if (WorldGen.InWorld(i, j))
						Main.Map.Update(i, j, 255);
				}
			}
			Main.refreshMap = true;
		}

		public void PostDrawFullScreenMap()
		{
			if (Main.mapFullscreen && HasPermissionToUse)
			{
				if (Main.mouseRight && Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl))
				{
					int mapWidth = Main.maxTilesX * 16;
					int mapHeight = Main.maxTilesY * 16;
					Vector2 cursorPosition = new Vector2(Main.mouseX, Main.mouseY);

					cursorPosition.X -= Main.screenWidth / 2;
					cursorPosition.Y -= Main.screenHeight / 2;

					Vector2 mapPosition = Main.mapFullscreenPos;
					Vector2 cursorWorldPosition = mapPosition;

					cursorPosition /= 16;
					cursorPosition *= 16 / Main.mapFullscreenScale;
					cursorWorldPosition += cursorPosition;
					//cursorWorldPosition *= 16;

					RevealAroundPoint((int)cursorWorldPosition.X, (int)cursorWorldPosition.Y);
				}
			}
		}
	}
}