using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HEROsMod.UIKit
{
	internal class UIRect : UIView
	{
		public UIRect()
		{
            Width = 10;
            Height = 10;
		}

		public UIRect(Vector2 position, float width, float height)
		{
            Position = position;
            Width = width;
            Height = height;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			Texture2D texture = ModUtils.DummyTexture;
			spriteBatch.Draw(texture, new Rectangle((int)(DrawPosition.X - Origin.X), (int)(DrawPosition.Y - Origin.Y), (int)Width, (int)Height), ForegroundColor);
			base.Draw(spriteBatch);
		}
	}
}