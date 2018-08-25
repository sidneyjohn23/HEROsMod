using HEROsMod.UIKit;
using HEROsMod.UIKit.UIComponents;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace HEROsMod.HEROsModServices
{
	internal class GenericExtensionService : HEROsModService
	{
		private readonly Texture2D texture;
		private readonly Action buttonClickedAction;
		private readonly Action<bool> groupUpdated;
		private readonly Func<string> tooltip;
		private readonly string permissionName;

		//public GenericExtensionService(UIHotbar hotbar)
		//{
		//	_name = "Undefined";
		//	IsInHotbar = true;
		//	HotbarParent = hotbar;
		//	_hotbarIcon = new UIImage(HEROsMod.instance.GetTexture("Images/spawn")/*Main.itemTexture[69]*/);
		//	_hotbarIcon.Tooltip = "Set Spawn Point";
		//	HotbarIcon.onLeftClick += new EventHandler(button_onLeftClick);
		//	HotbarIcon.onHover += new EventHandler(button_onHover);
		//}

		public GenericExtensionService(ExtensionMenuService extensionMenuService, Texture2D texture, string permissionName, Action buttonClickedAction, Action<bool> groupUpdated, Func<string> tooltip)
		{
			UIHotbar hotbar = extensionMenuService.Hotbar;

			this.texture = texture;
			this.buttonClickedAction = buttonClickedAction;
			this.groupUpdated = groupUpdated;
			this.tooltip = tooltip;
			this.permissionName = permissionName;

			_name = "Undefined";
			IsInHotbar = true;
			HotbarParent = hotbar;
            _hotbarIcon = new UIImage(this.texture)
            {
                Tooltip = "Set Spawn Point"
            };
            HotbarIcon.onLeftClick += new EventHandler(button_onLeftClick);
			HotbarIcon.onHover += new EventHandler(button_onHover);
		}

		public override void MyGroupUpdated()
		{
			HasPermissionToUse = HEROsModNetwork.LoginService.MyGroup.HasPermission(permissionName);
			groupUpdated(HasPermissionToUse);
		}

        private void button_onLeftClick(object sender, EventArgs e) => buttonClickedAction();

        private void button_onHover(object sender, EventArgs e) => HotbarIcon.Tooltip = tooltip();
    }
}