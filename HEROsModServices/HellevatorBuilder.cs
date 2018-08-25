using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using HEROsMod.UIKit.UIComponents;
using HEROsMod.UIKit;
using HEROsMod.HEROsModNetwork;
using Microsoft.Xna.Framework;

namespace HEROsMod.HEROsModServices
{
	class HellevatorBuilder : HEROsModService
	{
		public static HellevatorBuilder instance;
		private HellevatorBuilderWindow window;
		public HellevatorBuilder(UIHotbar hotbar)
		{
			IsInHotbar = true;
			HotbarParent = hotbar;
			_name = "Hellevator Builder";
			instance = this;
			_hotbarIcon = new UIImage(HEROsMod.instance.GetTexture("Images/map"));
			HotbarIcon.Tooltip = "Hellevator Builder...";
			_hotbarIcon.onLeftClick += _hotbarIcon_onLeftClick;
			window = new HellevatorBuilderWindow()
			{
				Visible = false
			};
			AddUIView(window);
			window.Closed += WindowClosed;
		}

		public override void MyGroupUpdated() => HasPermissionToUse = true;

		private void _hotbarIcon_onLeftClick(object sender, EventArgs e)
		{
			window.PressedOK = false;
			window.Visible = !window.Visible;
		}

		private void WindowClosed(object sender, EventArgs e)
		{
			if (window.PressedOK)
			{
				Main.NewText("Building Hellevator...", Color.Crimson);
				BuildHellevator();
				Main.NewText("Hellevator successfully built!", Color.Crimson);
			}
		}
		private void BuildHellevator()
		{
			Vector2 position = Main.player[Main.myPlayer].position;
			int hell = Main.maxTilesY - 190;
			int positionX = (int)position.X / 16;
			int positionY = (int)position.Y / 16;
			int i;
			for (i = positionY; i <= hell; i++)
			{
				foreach (Tile t in new Tile[] { Main.tile[positionX, i], Main.tile[positionX + 1, i] })
				{
					t.ClearTile();
				}
			}
			while (Main.tile[positionX, i + 1].type != 0 && Main.tile[positionX + 1, i + 1].type != 0 && i < Main.maxTilesY)
			{
				i++;
				foreach (Tile t in new Tile[] { Main.tile[positionX, i], Main.tile[positionX + 1, i] })
				{
					t.ClearTile();
				}
			}
		}
	}

	class HellevatorBuilderWindow : UIWindow
	{
		internal bool PressedOK = false;
		internal event EventHandler Closed;

		protected virtual void OnClosed(EventArgs e) => Closed?.Invoke(this, e);
		public HellevatorBuilderWindow()
		{
			CenterToParent();
			Position = new Microsoft.Xna.Framework.Vector2(Width / 2, Height / 2);
			CanMove = true;
			//int buttonWidth = 100;
			Height = 150;
			Width = 250;

			UILabel lTitle = new UILabel("Hellevator Builder")
			{
				Scale = .6f,
				X = LargeSpacing,
				Y = LargeSpacing,
				OverridesMouse = false
			};
			AddChild(lTitle);

			UILabel lWidth = new UILabel("Width:")
			{
				X = LargeSpacing,
				Scale = .4f,
				Y = lTitle.Y + lTitle.Height + Spacing
			};
			AddChild(lWidth);
			UITextbox tbWidth = new UITextbox()
			{
				Numeric = true,
				HasDecimal = false,
				MaxCharacters = 2,
				X = lWidth.Width + 2 * LargeSpacing,
				Y = lWidth.Y,
				Scale = .4f,
				Width = 20
			};
			AddChild(tbWidth);
			UIButton bOK = new UIButton("OK")
			{
				Anchor = AnchorPosition.BottomRight,
				X = Width - Spacing,
				Y = Height - Spacing
			};
			AddChild(bOK);
			bOK.onLeftClick += BOK_onLeftClick;
			UIButton bCancel = new UIButton("Cancel")
			{
				Anchor = AnchorPosition.BottomRight,
				X = bOK.Position.X - bOK.Width - Spacing,
				Y = bOK.Position.Y
			};
			AddChild(bCancel);
			bCancel.onLeftClick += BCancel_onLeftClick;
		}

		private void BCancel_onLeftClick(object sender, EventArgs e)
		{
			PressedOK = false;
			Visible = false;
		}

		private void BOK_onLeftClick(object sender, EventArgs e)
		{
			PressedOK = true;
			Visible = false;
			OnClosed(new EventArgs());
		}
	}
}
