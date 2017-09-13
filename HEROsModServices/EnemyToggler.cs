﻿using HEROsMod.UIKit;
using System;
using Terraria;
using Terraria.ModLoader;

namespace HEROsMod.HEROsModServices
{
	/// <summary>
	/// A Service that let's you toggle the enemies on the map
	/// </summary>
	internal class EnemyToggler : HEROsModService
	{
		public static bool EnemiesAllowed = true;

		public EnemyToggler()
		{
            _name = "Enemy Toggler";
            _hotbarIcon = new UIImage(HEROsMod.instance.GetTexture("Images/npcIcon"));
            _hotbarIcon.onLeftClick += _hotbarIcon_onLeftClick;
            HotbarIcon.Tooltip = "Disable Enemy Spawns";
            _hotbarIcon.Opacity = 1f;
			HEROsModNetwork.GeneralMessages.EnemiesToggledByServer += GeneralMessages_EnemiesToggledByServer;
		}

		private void GeneralMessages_EnemiesToggledByServer(bool enemiesCanSpawn)
		{
			if (enemiesCanSpawn)
			{
                _hotbarIcon.Opacity = 1f;
                HotbarIcon.Tooltip = "Disable Enemy Spawns";
			}
			else
			{
                _hotbarIcon.Opacity = .5f;
                HotbarIcon.Tooltip = "Enable Enemy Spawns";
			}
		}

		public static void ToggleNPCs()
		{
			if (EnemiesAllowed)
			{
				ClearNPCs();
			}
			EnemiesAllowed = !EnemiesAllowed;
		}

		public static void ClearNPCs()
		{
			for (int i = 0; i < Main.npc.Length; i++)
			{
				if (Main.npc[i] != null && !Main.npc[i].townNPC)
				{
					Main.npc[i].life = 0;
					if (Main.netMode == 2) NetMessage.SendData(23, -1, -1, null, i, 0f, 0f, 0f, 0);
				}
			}
		}

		private void _hotbarIcon_onLeftClick(object sender, EventArgs e)
		{
			if (ModUtils.NetworkMode != NetworkMode.None)
			{
				HEROsModNetwork.GeneralMessages.RequestToggleEnemies();
			}
			else
			{
				ToggleNPCs();
				if (EnemiesAllowed)
				{
                    _hotbarIcon.Opacity = 1f;
                    HotbarIcon.Tooltip = "Disable Enemy Spawns";
				}
				else
				{
                    _hotbarIcon.Opacity = .5f;
                    HotbarIcon.Tooltip = "Enable Enemy Spawns";
				}
				Main.NewText("Enemy spawns " + (EnemiesAllowed ? "enabled" : "disabled"));
			}
		}

		public override void MyGroupUpdated()
		{
            HasPermissionToUse = HEROsModNetwork.LoginService.MyGroup.HasPermission("ToggleEnemies");
			//base.MyGroupUpdated();
		}

		public override void Destroy()
		{
			HEROsModNetwork.GeneralMessages.EnemiesToggledByServer -= GeneralMessages_EnemiesToggledByServer;
			EnemiesAllowed = true;
			base.Destroy();
		}
	}

	public class EnemyTogglerGlobalNPC : GlobalNPC
	{
		public override bool Autoload(ref string name) => true;

		public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
		{
			if (!EnemyToggler.EnemiesAllowed)
			{
				spawnRate = 0;
				maxSpawns = 0;
			}
		}
	}
}