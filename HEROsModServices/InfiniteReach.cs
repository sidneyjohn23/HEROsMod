﻿using HEROsMod.UIKit;
using System;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HEROsMod.HEROsModServices
{
	internal class InfiniteReach : HEROsModService
	{
		/* =========Hooks============
         * This feature requires Multiple hooks in Player.cs
         * Hook 1:
         * At the end of the Reset Effects Method and after
         * Player.tileRangeX = 5;
		 * Player.tileRangeY = 4;
         * Add
         * if (HEROsMod.ModUtils.InfiniteReach)
            {
                Player.tileRangeX = int.MaxValue / 32 - 20;
                Player.tileRangeY = int.MaxValue / 32 - 20;
            }
         * Hook 2:
         *
         */
		public static bool Enabled { get; set; }

		public InfiniteReach()
		{
			Enabled = false;
            _name = "Infinite Reach";
            _hotbarIcon = new UIImage(HEROsMod.instance.GetTexture("Images/infiniteReach")/*Main.itemTexture[407]*/);
            _hotbarIcon.onLeftClick += _hotbarIcon_onLeftClick;
            HotbarIcon.Tooltip = HEROsMod.HeroText("EnableInfiniteReach");
			Disable();
		}

		private void Enable()
		{
            _hotbarIcon.Opacity = 1f;
			Enabled = true;
            HotbarIcon.Tooltip = HEROsMod.HeroText("DisableInfiniteReach");
		}

		private void Disable()
		{
            _hotbarIcon.Opacity = .5f;
			Enabled = false;
            HotbarIcon.Tooltip = HEROsMod.HeroText("EnableInfiniteReach");
		}

		private void _hotbarIcon_onLeftClick(object sender, EventArgs e)
		{
			if (Enabled)
			{
				Disable();
			}
			else
			{
				Enable();
			}
		}

		//public static void TileRangeHook()
		//{
		//    if (Enabled)
		//    {
		//        Player.tileRangeX = Main.maxTilesX;
		//        Player.tileRangeY = Main.maxTilesX;
		//    }
		//}

		public override void Destroy()
		{
			Disable();
			base.Destroy();
		}

		public override void MyGroupUpdated()
		{
            HasPermissionToUse = HEROsModNetwork.LoginService.MyGroup.HasPermission("InfiniteReach");
			if (!HasPermissionToUse)
			{
				Disable();
			}
			//base.MyGroupUpdated();
		}

		public override void Update()
		{
			base.Update();

			Player player = Main.player[Main.myPlayer];
			if (Enabled)
			{
				//if (Main.SmartCursorEnabled)
				//{
				//	Main.SmartCursorEnabled = false;
				//	Main.NewText("Smart Cursor automatically disabled in infinte reach mode.");
				//}

				// Works with: Place tiles, walls. Axe, Hammer, Pick.
				Item selected = player.inventory[player.selectedItem];
				if (selected.createTile >= 0 || selected.createWall >= 0 || selected.pick > 0 || selected.axe > 0 || selected.hammer > 0)
				{
					// TODO, hammering tile destorys walls too??
					player.itemTime = 0;
				}
			}
		}
	}

	public class InfiniteReachModPlayer : ModPlayer
	{
        public override bool Autoload(ref string name) => true;

        public override void ResetEffects()
		{
			if (player.whoAmI == Main.myPlayer)
			{
				if (InfiniteReach.Enabled)
				{
					Player.tileRangeX = int.MaxValue / 32 - 20;
					Player.tileRangeY = int.MaxValue / 32 - 20;

					if (Main.SmartCursorEnabled)
					{
						Main.SmartCursorEnabled = false;
						Main.NewText(HEROsMod.HeroText("SmartCursorAutomaticallyDisabledInfinteReachMod"));
					}
				}
			}
		}

		//public override void PostUpdate()
		//{
		//	if (player.whoAmI == Main.myPlayer)
		//	{
		//		if (InfiniteReach.Enabled)
		//		{
		//			if (player.inventory[player.selectedItem].createTile >= 0 || player.inventory[player.selectedItem].createWall >= 0)
		//			{
		//				player.itemTime = 0;
		//			}
		//		}
		//	}
		//}
	}
}