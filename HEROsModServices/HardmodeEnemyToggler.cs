using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HEROsMod;
using HEROsMod.HEROsModServices;
using HEROsMod.UIKit;
using HEROsMod.UIKit.UIComponents;
using Terraria;

namespace HEROsMod.HEROsModServices
{
    class HardmodeEnemyToggler : HEROsModService
    {
        public HardmodeEnemyToggler(UIHotbar hotbar)
        {
            IsInHotbar = true;
            HotbarParent = hotbar;
            _name = "Hardmode Enemy Toggler";
            _hotbarIcon = new UIImage(Main.itemTexture[1991])
            {
                Tooltip = "Toggle Hardmode Enemies"
            };
            _hotbarIcon.onLeftClick += _hotbarIcon_onLeftClick;
        }

        public override void MyGroupUpdated()
        {
            HasPermissionToUse = HEROsModNetwork.LoginService.MyGroup.HasPermission("ToggleHardmodeEnemies");
            base.MyGroupUpdated();
        }

        void _hotbarIcon_onLeftClick(object sender, EventArgs e)
        {
            if (ModUtils.NetworkMode == NetworkMode.None)
            {
                ToggleHardModeEnemies();
            }
            else
            {
                HEROsModNetwork.GeneralMessages.RequestToggleHardmodeEnemies();
            }
        }

        public static void ToggleHardModeEnemies()
        {
            Main.hardMode = !Main.hardMode;
            EnemyToggler.ClearNPCs();
            if (ModUtils.NetworkMode == NetworkMode.Server)
            {
                NetMessage.SendData(7);
            }
        }
    }
}
