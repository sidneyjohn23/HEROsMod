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
			HotbarIcon.OnLeftClick += HotbarIcon_onLeftClick;

            _extensionMenuHotbar = new ExtensionMenuWindow()
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
				if (_extensionMenuHotbar.Selected)
				{
					_extensionMenuHotbar.Selected = false;
					_extensionMenuHotbar.Hide();
				}
				else
				{
					_extensionMenuHotbar.Selected = true;
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
			Visible = false;

			base.Height = 55f;
            buttonView.Height = base.Height;
			Anchor = AnchorPosition.Top;
            AddChild(buttonView);
			Position = new Vector2(Position.X, HiddenPosition);
			CenterXAxisToParentCenter();
			float num = spacing;
			for (int i = 0; i < buttonView.Children.Count; i++)
			{
                buttonView.Children[i].Anchor = AnchorPosition.Left;
                buttonView.Children[i].Position = new Vector2(num, 0f);
                buttonView.Children[i].CenterYAxisToParentCenter();
                buttonView.Children[i].Visible = true;
				num += buttonView.Children[i].Width + spacing;
			}
            Resize();
		}

		public override void Test()
		{
			CenterXAxisToParentCenter();
			float num = spacing;
			for (int i = 0; i < buttonView.Children.Count; i++)
			{
                buttonView.Children[i].Anchor = AnchorPosition.Left;
                buttonView.Children[i].Position = new Vector2(num, 0f);
                buttonView.Children[i].CenterYAxisToParentCenter();
                buttonView.Children[i].Visible = true;
				num += buttonView.Children[i].Width + spacing;
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
			for (int i = 0; i < buttonView.Children.Count; i++)
			{
				if (buttonView.Children[i].Visible)
				{
                    buttonView.Children[i].X = num;
					num += buttonView.Children[i].Width + spacing;
				}
			}
			Width = num;
            buttonView.Width = Width;
		}
	}
}