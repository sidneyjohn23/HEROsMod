using Microsoft.Xna.Framework.Graphics;

using Terraria;

namespace HEROsMod.UIKit
{
	internal class UIPlayerHead : UIView
	{
		public bool lookRight = true;

		public Player DrawPlayer { get; set; }

		public UIPlayerHead(Player player)
		{
			DrawPlayer = player;
			Width = 40;
			Height = 40;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			int pd = DrawPlayer.direction;
			DrawPlayer.direction = lookRight ? 1 : -1;
			ModUtils.DrawPlayerHead(DrawPlayer, DrawPosition.X + Width / 2, DrawPosition.Y + Height / 2);
			DrawPlayer.direction = pd;
			base.Draw(spriteBatch);
		}
	}
}