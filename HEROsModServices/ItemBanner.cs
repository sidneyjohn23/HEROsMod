using HEROsMod.UIKit;
using HEROsMod.UIKit.UIComponents;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HEROsMod.HEROsModServices
{
	internal class ItemBanner : HEROsModService
	{
		public static bool ItemsBanned { get; set; }

		public static int[] bannedProjectiles = new int[]
		{
			ProjectileID.RocketII,
			ProjectileID.RocketIV,
			ProjectileID.Bomb,
			ProjectileID.StickyBomb,
			ProjectileID.BombFish,
			ProjectileID.Dynamite,
			ProjectileID.StickyDynamite,
			ProjectileID.BouncyDynamite,
		};

		public ItemBanner(UIHotbar hotbar)
		{
			IsInHotbar = true;
			HotbarParent = hotbar;
			MultiplayerOnly = true;
			_name = "Item Banner";
			_hotbarIcon = new UIImage(HEROsMod.instance.GetTexture("Images/explosives")/*Main.itemTexture[888]*/)
			{
				Tooltip = HEROsMod.HeroText("BanDestructiveExplosives")
			};
			_hotbarIcon.OnLeftClick += _hotbarIcon_onLeftClick;
			HEROsModNetwork.GeneralMessages.ItemBannerToggleByServer += GeneralMessages_BannedItemsToggleByServer;
		}

		private void _hotbarIcon_onLeftClick(object sender, EventArgs e)
		{
			if (ModUtils.NetworkMode == NetworkMode.Client)
			{
				HEROsModNetwork.GeneralMessages.RequestToggleBannedItems();
			}
		}

		public override void MyGroupUpdated() => HasPermissionToUse = HEROsModNetwork.LoginService.MyGroup.HasPermission("ToggleBannedItems");//base.MyGroupUpdated();

		public void GeneralMessages_BannedItemsToggleByServer(bool itemsbanned)
		{
			if (itemsbanned)
			{
				_hotbarIcon.Opacity = .5f;
				HotbarIcon.Tooltip = HEROsMod.HeroText("UnbanDestructiveExplosives");
			}
			else
			{
				_hotbarIcon.Opacity = 1f;
				HotbarIcon.Tooltip = HEROsMod.HeroText("BanDestructiveExplosives");
			}
			ItemsBanned = itemsbanned;
		}
	}

	internal class ItemBannerGlobalProjectile : GlobalProjectile
	{
		public override bool Autoload(ref string name) => true;

		// Is only called on server??
		public override bool PreAI(Projectile projectile)
		{
			if (ItemBanner.ItemsBanned)
			{
				if (ItemBanner.bannedProjectiles.Contains(projectile.type))
				{
					if (!Main.dedServ)
					{
						//Projectile newProj = new Projectile();
						//newProj.SetDefaults(type);
						Main.NewText(string.Format(HEROsMod.HeroText("ProjectileIsBannerdOnTheServer"), projectile.Name), Color.Red.R, Color.Red.G, Color.Red.B);
					}
					//ErrorLogger.Log(Main.dedServ + " Item Banned");
					projectile.active = false;
					return false;
				}
			}
			return base.PreAI(projectile);
		}
	}
}