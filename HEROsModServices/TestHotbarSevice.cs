using HEROsMod.UIKit;
using HEROsMod.UIKit.UIComponents;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;

namespace HEROsMod.HEROsModServices
{
	internal class TestHotbarService : HEROsModService
	{
		private TestHotbarWindow _testHotbarWindow;

		public TestHotbarService()
		{
            _hotbarIcon = new UIImage(Main.buffTexture[3]);
            HotbarIcon.Tooltip = "Test Hotbar";
            HotbarIcon.OnLeftClick += HotbarIcon_onLeftClick;

            _testHotbarWindow = new TestHotbarWindow()
            {
                HotBarParent = HEROsMod.ServiceHotbar
            };
            _testHotbarWindow.Hide();
            AddUIView(_testHotbarWindow);
		}

		private void HotbarIcon_onLeftClick(object sender, EventArgs e)
		{
			//Main.NewText("Toggle Hotbar");

			//_testHotbarWindow.Visible = !_testHotbarWindow.Visible;
			if (_testHotbarWindow.Selected)
			{
				_testHotbarWindow.Selected = false;
				_testHotbarWindow.Hide();
				//Main.NewText("Hide Hotbar");

				//uIImage.ForegroundColor = buttonUnselectedColor;
			}
			else
			{
				//DisableAllWindows();
				_testHotbarWindow.Selected = true;
				_testHotbarWindow.Show();
				//Main.NewText("Show Hotbar");

				//uIImage.ForegroundColor = buttonSelectedColor;
			}
		}
	}

	internal class TestHotbarWindow : UIHotbar
	{
		new public UIView buttonView;
		public UIImage bStampTiles;
		public UIImage bEyeDropper;
		public UIImage bFlipHorizontal;
		public UIImage bFlipVertical;
		public UIImage bToggleTransparentSelection;

		public TestHotbarWindow()
		{
            buttonView = new UIView();
			Visible = false;
			bStampTiles = new UIImage(Main.itemTexture[ItemID.Paintbrush]);
			bEyeDropper = new UIImage(Main.itemTexture[ItemID.EmptyDropper]);
			bFlipHorizontal = new UIImage(Main.itemTexture[ItemID.PadThai]);
			bFlipVertical = new UIImage(Main.itemTexture[ItemID.Safe]);
			bToggleTransparentSelection = new UIImage(Main.buffTexture[BuffID.Invisibility]);
			bStampTiles.Tooltip = "    Paint Tiles";
			bEyeDropper.Tooltip = "    Eye Dropper";
			bFlipHorizontal.Tooltip = "    Flip Horizontal";
			bFlipVertical.Tooltip = "    Flip Vertical";
			bToggleTransparentSelection.Tooltip = "    Toggle Transparent Selection: On";
			buttonView.AddChild(bStampTiles);
			buttonView.AddChild(bEyeDropper);
			buttonView.AddChild(bFlipHorizontal);
			buttonView.AddChild(bFlipVertical);
			buttonView.AddChild(bToggleTransparentSelection);
			Width = 200f;
			Height = 55f;
            buttonView.Height = Height;
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
                buttonView.Children[i].ForegroundColor = buttonUnselectedColor;
				num += buttonView.Children[i].Width + spacing;
			}
            Resize();
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

		new public void Hide()
		{
			hidden = true;
			arrived = false;
		}

		new public void Show()
		{
			arrived = false;
			hidden = false;
			Visible = true;
		}
	}
}