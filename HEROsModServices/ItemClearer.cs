using HEROsMod.UIKit;
using System;
using Terraria;

namespace HEROsMod.HEROsModServices
{
	/// <summary>
	/// A Service that clears all items on the ground
	/// </summary>
	internal class ItemClearer : HEROsModService
	{
		public ItemClearer()
		{
            _name = "Item Clearer";
            _hotbarIcon = new UIImage(HEROsMod.instance.GetTexture("Images/canIcon"));
            _hotbarIcon.onLeftClick += _hotbarIcon_onLeftClick;
            HotbarIcon.Tooltip = "Clear Items on Ground";
		}

		private void _hotbarIcon_onLeftClick(object sender, EventArgs e)
		{
			//ClearItems
			if (ModUtils.NetworkMode == NetworkMode.None)
			{
				for (int i = 0; i < Main.item.Length; i++)
				{
					Main.item[i].active = false;
				}
				Main.NewText("Items on the ground were cleared");
			}
			else
			{
				HEROsModNetwork.GeneralMessages.RequestClearGroundItems();
			}
		}

		public override void MyGroupUpdated()
		{
            HasPermissionToUse = HEROsModNetwork.LoginService.MyGroup.HasPermission("ClearItems");
			//base.MyGroupUpdated();
		}
	}
}