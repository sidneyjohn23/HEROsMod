using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;

namespace HEROsMod.UIKit
{
	internal class UILabel : UIView
	{
		public static DynamicSpriteFont DefaultFont => Main.fontDeathText;
		public DynamicSpriteFont font;
		private string text = "";

		public string Text
		{
			get => text;
			set
			{
				text = value;
				SetWidthHeight();
			}
		}

		public bool TextOutline { get; set; } = true;

		private float width = 0;
		private float height = 0;

		public UILabel(string text)
		{
			font = DefaultFont;
            Text = text;
		}

		public UILabel()
		{
			font = DefaultFont;
            Text = "";
		}

		protected override Vector2 GetOrigin() => base.GetOrigin();

		private void SetWidthHeight()
		{
			if (Text != null)
			{
				Vector2 size = font.MeasureString(Text);
				width = size.X;
				height = size.Y;
			}
			else
			{
				width = 0;
				height = 0;
			}
		}

		protected new float Width => width * Scale;

		protected new float Height
		{
			get
			{
				if (height == 0)
				{
					return font.MeasureString("H").Y * Scale;
				}
				else
				{
					return height * Scale;
				}
			}
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (Text != null)
			{
				if (TextOutline)
				{
					Utils.DrawBorderStringFourWay(spriteBatch, font, Text, DrawPosition.X, DrawPosition.Y, ForegroundColor, Color.Black * Opacity, Origin / Scale, Scale);
				}
				else
				{
					spriteBatch.DrawString(font, Text, DrawPosition, ForegroundColor * Opacity, 0f, Origin / Scale, Scale, SpriteEffects.None, 0f);
				}
			}
			base.Draw(spriteBatch);
		}
	}
}