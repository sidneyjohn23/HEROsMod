using System;

using Terraria;

namespace HEROsMod.UIKit.UIComponents
{
	internal class HostPlayWindow : UIWindow
	{
		public HostPlayWindow()
		{
            Anchor = AnchorPosition.Center;
            UIWrappingLabel label = new UIWrappingLabel("Host & Play is not supported in Gamiki Mod.  Read our post about it by clicking the button below.", Width - 32f)
            {
                X = 16f,
                Y = 16f
            };
            UIButton bOk = new UIButton("Ok");
			UIButton bPost = new UIButton("View Post");

			bOk.Y = label.Y + label.Height + 16f;
			bOk.X = Width - bOk.Width - 16f;
			bPost.Y = bOk.Y;
			bPost.X = bOk.X - bPost.Width - 16f;
			Height = bOk.Y + bOk.Height + 16f;

			bOk.onLeftClick += bOk_onLeftClick;
			bPost.onLeftClick += bPost_onLeftClick;

            AddChild(label);
            AddChild(bOk);
            AddChild(bPost);
            CenterToParent();
		}

        private void bPost_onLeftClick(object sender, EventArgs e) => System.Diagnostics.Process.Start("http://HEROsModMod.com/Terraria_Forum/thread/Host__Play-2348");

        private void bOk_onLeftClick(object sender, EventArgs e)
		{
			Main.menuMode = 12;
            Parent.RemoveChild(this);
		}
	}
}