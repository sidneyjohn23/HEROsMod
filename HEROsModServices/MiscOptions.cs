using HEROsMod.UIKit;
using HEROsMod.UIKit.UIComponents;
using Microsoft.Xna.Framework;
using System;

namespace HEROsMod.HEROsModServices
{
	internal class MiscOptions : HEROsModService
	{
		private MiscOptionsWindow _miscOptionsHotbar;

		public MiscOptions()
		{
			IsHotbar = true;

			_hotbarIcon = new UIImage(HEROsMod.instance.GetTexture("Images/settings")/*Main.buffTexture[BuffID.Confused]*/);
			HotbarIcon.Tooltip = HEROsMod.HeroText("MiscOptions");
			HotbarIcon.OnLeftClick += HotbarIcon_onLeftClick;

            _miscOptionsHotbar = new MiscOptionsWindow()
            {
                HotBarParent = HEROsMod.ServiceHotbar
            };
            _miscOptionsHotbar.Hide();
            AddUIView(_miscOptionsHotbar);

			Hotbar = _miscOptionsHotbar;
		}

		private void HotbarIcon_onLeftClick(object sender, EventArgs e)
		{
			//Main.NewText("Toggle Hotbar");

			//_testHotbarWindow.Visible = !_testHotbarWindow.Visible;
			if (_miscOptionsHotbar.Selected)
			{
				_miscOptionsHotbar.Selected = false;
				_miscOptionsHotbar.Hide();
				//	Main.NewText("Hide Hotbar");

				//uIImage.ForegroundColor = buttonUnselectedColor;
			}
			else
			{
				//DisableAllWindows();
				_miscOptionsHotbar.Selected = true;
				_miscOptionsHotbar.Show();
				//Main.NewText("Show Hotbar");

				//uIImage.ForegroundColor = buttonSelectedColor;
			}
		}

		public override void MyGroupUpdated()
		{
            HasPermissionToUse = HEROsModNetwork.LoginService.MyGroup.HasPermission("ToggleBannedItems") ||
			HEROsModNetwork.LoginService.MyGroup.HasPermission("RevealMap") ||
			HEROsModNetwork.LoginService.MyGroup.HasPermission("LightHack") ||
			HEROsModNetwork.LoginService.MyGroup.IsAdmin ||
			HEROsModNetwork.LoginService.MyGroup.HasPermission("ToggleHardmodeEnemies") ||
			HEROsModNetwork.LoginService.MyGroup.HasPermission("ToggleGravestones");
			if (!HasPermissionToUse)
			{
				_miscOptionsHotbar.Hide();
			}

			//base.MyGroupUpdated();
		}
	}

	internal class MiscOptionsWindow : UIHotbar
	{
		//public UIImage bStampTiles;
		//public UIImage bEyeDropper;
		//public UIImage bFlipHorizontal;
		//public UIImage bFlipVertical;
		//public UIImage bToggleTransparentSelection;

		public MiscOptionsWindow()
		{
            buttonView = new UIView();
			Visible = false;
			//bStampTiles = new UIImage(Main.itemTexture[ItemID.Paintbrush]);
			//bEyeDropper = new UIImage(Main.itemTexture[ItemID.EmptyDropper]);
			//bFlipHorizontal = new UIImage(Main.itemTexture[ItemID.PadThai]);
			//bFlipVertical = new UIImage(Main.itemTexture[ItemID.Safe]);
			//bToggleTransparentSelection = new UIImage(Main.buffTexture[BuffID.Invisibility]);
			//bStampTiles.Tooltip = "    Paint Tiles";
			//bEyeDropper.Tooltip = "    Eye Dropper";
			//bFlipHorizontal.Tooltip = "    Flip Horizontal";
			//bFlipVertical.Tooltip = "    Flip Vertical";
			//bToggleTransparentSelection.Tooltip = "    Toggle Transparent Selection: On";
			//buttonView.AddChild(bStampTiles);
			//buttonView.AddChild(bEyeDropper);
			//buttonView.AddChild(bFlipHorizontal);
			//buttonView.AddChild(bFlipVertical);
			//buttonView.AddChild(bToggleTransparentSelection);

			//base.Width = 200f;
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
				//this.buttonView.Children[i].ForegroundColor = buttonUnselectedColor;
				num += buttonView.Children[i].Width + spacing;
			}
            Resize();
		}

		public override void Test()
		{
			//ModUtils.DebugText("TEST " + buttonView.ChildCount);
			//base.Width = 200f;
			//base.Height = 55f;
			//this.buttonView.Height = base.Height;
			//base.Anchor = AnchorPosition.Top;
			//this.AddChild(this.buttonView);
			//base.Position = new Vector2(Position.X, this.hiddenPosition);
			CenterXAxisToParentCenter();
			float num = spacing;
			for (int i = 0; i < buttonView.Children.Count; i++)
			{
                buttonView.Children[i].Anchor = AnchorPosition.Left;
                buttonView.Children[i].Position = new Vector2(num, 0f);
                buttonView.Children[i].CenterYAxisToParentCenter();
                buttonView.Children[i].Visible = true;
				//this.buttonView.Children[i].ForegroundColor = buttonUnselectedColor;
				num += buttonView.Children[i].Width + spacing;
			}
            Resize();
			//buttonView.BackgroundColor = Color.Pink;
			//buttonView.ForegroundColor = Color.Red;
			//base.Visible = false;
		}

		public override void Update()
		{
			DoSlideMovement();
			//base.CenterXAxisToParentCenter();
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

		//public void Hide()
		//{
		//	hidden = true;
		//	arrived = false;
		//}

		//public void Show()
		//{
		//	arrived = false;
		//	hidden = false;
		//	Visible = true;
		//}
	}
}