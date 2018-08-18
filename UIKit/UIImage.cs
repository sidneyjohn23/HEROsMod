using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HEROsMod.UIKit
{
	internal class UIImage : UIView
	{

		public Texture2D Texture { get; set; }

#pragma warning disable IDE1006 // Benennungsstile
		private float width => Texture.Width;
		private float height => Texture.Height;
#pragma warning restore IDE1006 // Benennungsstile

		public SpriteEffects SpriteEffect { get; set; } = SpriteEffects.None;

		private Rectangle? sourceRectangle = null;

		public Rectangle SourceRectangle
		{
			get
			{
				if (sourceRectangle == null)
				{
					sourceRectangle = new Rectangle();
				}

				return (Rectangle)sourceRectangle;
			}
			set => sourceRectangle = value;
		}

		public int SR_X
		{
			get => SourceRectangle.X;
			set => SourceRectangle = new Rectangle(value, SourceRectangle.Y, SourceRectangle.Width, SourceRectangle.Height);
		}

		public int SR_Y
		{
			get => SourceRectangle.X;
			set => SourceRectangle = new Rectangle(SourceRectangle.X, value, SourceRectangle.Width, SourceRectangle.Height);
		}

		public int SR_Width
		{
			get => SourceRectangle.X;
			set => SourceRectangle = new Rectangle(SourceRectangle.X, SourceRectangle.Y, value, SourceRectangle.Height);
		}

		public int SR_Height
		{
			get => SourceRectangle.X;
			set => SourceRectangle = new Rectangle(SourceRectangle.X, SourceRectangle.Y, SourceRectangle.Width, value);
		}

		public UIImage(Texture2D texture) => Texture = texture;

		public UIImage()
		{
		}

		protected new float Width
		{
			get
			{
				if (sourceRectangle != null)
				{
					return ((Rectangle)sourceRectangle).Width * Scale;
				}

				return width * Scale;
			}
		}

		protected new float Height
		{
			get
			{
				if (sourceRectangle != null)
				{
					return ((Rectangle)sourceRectangle).Height * Scale;
				}

				return height * Scale;
			}
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (Visible)
			{
				spriteBatch.Draw(Texture, DrawPosition, sourceRectangle, ForegroundColor * Opacity, 0f, Origin / Scale, Scale, SpriteEffect, 0f);
			}

			base.Draw(spriteBatch);
		}
	}
}