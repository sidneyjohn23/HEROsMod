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
            HotbarIcon.onLeftClick += HotbarIcon_onLeftClick;

            _testHotbarWindow = new TestHotbarWindow
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
			if (_testHotbarWindow.selected)
			{
				_testHotbarWindow.selected = false;
				_testHotbarWindow.Hide();
				//Main.NewText("Hide Hotbar");

				//uIImage.ForegroundColor = buttonUnselectedColor;
			}
			else
			{
				//DisableAllWindows();
				_testHotbarWindow.selected = true;
				_testHotbarWindow.Show();
				//Main.NewText("Show Hotbar");

				//uIImage.ForegroundColor = buttonSelectedColor;
			}
		}
	}

	internal class TestHotbarWindow : UIHotbar
	{
		public new UIView buttonView;
		public UIImage bStampTiles;
		public UIImage bEyeDropper;
		public UIImage bFlipHorizontal;
		public UIImage bFlipVertical;
		public UIImage bToggleTransparentSelection;

		public TestHotbarWindow()
		{
            buttonView = new UIView();
			base.Visible = false;
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
			base.Width = 200f;
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
                buttonView.children[i].ForegroundColor = buttonUnselectedColor;
				num += buttonView.children[i].Width + spacing;
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

		public new void Hide()
		{
			hidden = true;
			arrived = false;
		}

		public new void Show()
		{
			arrived = false;
			hidden = false;
			Visible = true;
		}
	}
}