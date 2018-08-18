using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using HEROsMod.UIKit.UIComponents;
using HEROsMod.UIKit;
using HEROsMod.HEROsModNetwork;

namespace HEROsMod.HEROsModServices {
    class HellevatorBuilder : HEROsModService {
        public static HellevatorBuilder instance;
        private HellevatorBuilderWindow window;
        public HellevatorBuilder(UIHotbar hotbar) {
            IsInHotbar = true;
            HotbarParent = hotbar;
            _name = "Hellevator Builder";
            instance = this;
            _hotbarIcon = new UIImage(HEROsMod.instance.GetTexture("Images/map"));
            HotbarIcon.Tooltip = "Hellevator Builder...";
            _hotbarIcon.OnLeftClick += _hotbarIcon_onLeftClick;
            window = new HellevatorBuilderWindow() {
                Visible = false
            };
            AddUIView(window);
        }

		public override void MyGroupUpdated() => HasPermissionToUse = true;

		private void _hotbarIcon_onLeftClick(object sender, EventArgs e) => window.Visible = !window.Visible;
	}

    class HellevatorBuilderWindow : UIWindow {
        public HellevatorBuilderWindow() {
            CenterToParent();
            Position = new Microsoft.Xna.Framework.Vector2(Width / 2, Height / 2);
            CanMove = true;
            //int buttonWidth = 100;
            Height = 150;
            Width = 250;

            UILabel lTitle = new UILabel("Hellevator Builder") {
                Scale = .6f,
                X = LargeSpacing,
                Y = LargeSpacing,
                OverridesMouse = false
            };
            AddChild(lTitle);

            UILabel lWidth = new UILabel("Width:") {
                X = LargeSpacing,
                Scale = .4f,
                Y = lTitle.Y + lTitle.Height + Spacing
            };
            AddChild(lWidth);
            UITextbox tbWidth = new UITextbox() {
                Numeric = true,
                HasDecimal = false,
                MaxCharacters = 2,
                X = lWidth.Width + 2 * LargeSpacing,
                Y = lWidth.Y,
                Scale = .4f ,
                Width = 20
            };
            AddChild(tbWidth);
            UIButton bOK = new UIButton("OK") {
                Anchor = AnchorPosition.BottomRight,
                X = Width - Spacing,
                Y = Height - Spacing
            };
            AddChild(bOK);
            bOK.OnLeftClick += BOK_onLeftClick;
            UIButton bCancel = new UIButton("Cancel") {
                Anchor = AnchorPosition.BottomRight,
                X = bOK.Position.X - bOK.Width - Spacing,
                Y = bOK.Position.Y
            };
            AddChild(bCancel);
            bCancel.OnLeftClick += BCancel_onLeftClick;
        }

		private void BCancel_onLeftClick(object sender, EventArgs e) => Visible = false;

		private void BOK_onLeftClick(object sender, EventArgs e) {
        }
    }
}
