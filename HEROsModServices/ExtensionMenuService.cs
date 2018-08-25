using HEROsMod.UIKit;
using HEROsMod.UIKit.UIComponents;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;

namespace HEROsMod.HEROsModServices
{
	internal class ExtensionMenuService : HEROsModService
	{
		private ExtensionMenuWindow _extensionMenuHotbar;

		private List<GenericExtensionService> genericServices;

		public ExtensionMenuService()
		{
			genericServices = new List<GenericExtensionService>();

			IsHotbar = true;

            _hotbarIcon = new UIImage(HEROsMod.instance.GetTexture("Images/extensions"));
            HotbarIcon.Tooltip = HEROsMod.HeroText("ExtensionTools");
            HotbarIcon.onLeftClick += HotbarIcon_onLeftClick;

            _extensionMenuHotbar = new ExtensionMenuWindow
            {
                HotBarParent = HEROsMod.ServiceHotbar
            };
            _extensionMenuHotbar.Hide();
            AddUIView(_extensionMenuHotbar);

			Hotbar = _extensionMenuHotbar;
		}

		private void HotbarIcon_onLeftClick(object sender, EventArgs e)
		{
			bool childAvailable = false;
			foreach (GenericExtensionService item in genericServices)
			{
				childAvailable |= item.HasPermissionToUse;
			}
			if (childAvailable)
			{
				if (_extensionMenuHotbar.selected)
				{
					_extensionMenuHotbar.selected = false;
					_extensionMenuHotbar.Hide();
				}
				else
				{
					_extensionMenuHotbar.selected = true;
					_extensionMenuHotbar.Show();
				}
			}
			else
			{
				Main.NewText(HEROsMod.HeroText("NoExtensionsLoadedNote"));
			}
		}

		public override void MyGroupUpdated()
		{
			bool childAvailable = false;
			foreach (GenericExtensionService item in genericServices)
			{
				item.MyGroupUpdated();
				childAvailable |= item.HasPermissionToUse;
			}
			HasPermissionToUse = childAvailable;
			if (!HasPermissionToUse)
			{
				_extensionMenuHotbar.Hide();
			}
		}

        internal void AddGeneric(GenericExtensionService genericService) => genericServices.Add(genericService);
    }

	internal class ExtensionMenuWindow : UIHotbar
	{
		public ExtensionMenuWindow()
		{
            buttonView = new UIView();
			base.Visible = false;

			base.Height = 55f;
            buttonView.Height = base.Height;
			base.Anchor = AnchorPosition.Top;
            AddChild(buttonView);
			base.Position = new Vector2(Position.X, hiddenPosition);
			base.CenterXAxisToParentCenter();
			float num = spacing;
			for (int i = 0; i < buttonView.children.Count; i++)
			{
                buttonView.children[i].Anchor = AnchorPosition.Left;
                buttonView.children[i].Position = new Vector2(num, 0f);
                buttonView.children[i].CenterYAxisToParentCenter();
                buttonView.children[i].Visible = true;
				num += buttonView.children[i].Width + spacing;
			}
            Resize();
		}

		public override void test()
		{
			base.CenterXAxisToParentCenter();
			float num = spacing;
			for (int i = 0; i < buttonView.children.Count; i++)
			{
                buttonView.children[i].Anchor = AnchorPosition.Left;
                buttonView.children[i].Position = new Vector2(num, 0f);
                buttonView.children[i].CenterYAxisToParentCenter();
                buttonView.children[i].Visible = true;
				num += buttonView.children[i].Width + spacing;
			}
            Resize();
		}

		public override void Update()
		{
			DoSlideMovement();
			base.Update();
		}

		public void Resize()
		{
			float num = spacing;
			for (int i = 0; i < buttonView.children.Count; i++)
			{
				if (buttonView.children[i].Visible)
				{
                    buttonView.children[i].X = num;
					num += buttonView.children[i].Width + spacing;
				}
			}
			base.Width = num;
            buttonView.Width = base.Width;
		}
	}
}