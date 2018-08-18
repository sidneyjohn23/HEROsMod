using Terraria;

namespace HEROsMod.UIKit
{
	internal class UIScreen : UIView
	{
		public UIScreen() => OverridesMouse = false;

		protected new float Width => Main.screenWidth;

		protected new float Height => Main.screenHeight;
	}
}