using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace HEROsMod.UIKit
{
	internal class UIButton : UIView
	{
		public static Texture2D buttonBackground;
		private static Texture2D buttonFill;

		public static Texture2D ButtonFill
		{
			get
			{
				if (buttonFill == null)
				{
					Color[] edgeColors = new Color[buttonBackground.Width * buttonBackground.Height];
					buttonBackground.GetData(edgeColors);
					Color[] fillColors = new Color[buttonBackground.Height];
					for (int y = 0; y < fillColors.Length; y++)
					{
						fillColors[y] = edgeColors[buttonBackground.Width - 1 + y * buttonBackground.Width];
					}
					buttonFill = new Texture2D(UIView.Graphics, 1, fillColors.Length);
					buttonFill.SetData(fillColors);
				}
				return buttonFill;
			}
		}

		private Color hoverColor = new Color(38, 42, 120);
		private Color drawColor;

		private UILabel label = new UILabel("");

		public string Text
		{
			get => label.Text;
			set
			{
				label.Text = value;
				label.Anchor = AnchorPosition.Center;
				ScaleText();
				label.CenterToParent();
				label.Position = new Vector2(label.Position.X, label.Position.Y + 2);
			}
		}

		public bool AutoSize { get; set; }

		public UIButton(string text)
		{
			AutoSize = true;
            AddChild(label);
            Text = text;
            BackgroundColor = new Color(28, 32, 119);
			drawColor = BackgroundColor;
            OnMouseEnter += new EventHandler(UIButton_onMouseEnter);
            OnMouseLeave += new EventHandler(UIButton_onMouseLeave);
		}

		public UIButton(string text, Color backgroundColor, Color hoverColor)
		{
			AutoSize = true;
            AddChild(label);
            Text = text;
            BackgroundColor = backgroundColor;
			drawColor = BackgroundColor;
			this.hoverColor = hoverColor;
            OnMouseEnter += new EventHandler(UIButton_onMouseEnter);
            OnMouseLeave += new EventHandler(UIButton_onMouseLeave);
		}

		public void SetTextColor(Color color) => label.ForegroundColor = color;

		private void UIButton_onMouseLeave(object sender, EventArgs e) => drawColor = BackgroundColor;

		private void UIButton_onMouseEnter(object sender, EventArgs e) => drawColor = hoverColor;

		protected new float Width
		{
			get
			{
				if (AutoSize)
				{
					return label.Width + buttonBackground.Width * 2 + 30;
				}
				else
				{
					return Width;
				}
			}
			set
			{
				Width = value;

				ScaleText();
				label.CenterToParent();
				label.Position = new Vector2(label.Position.X, label.Position.Y + 2);
			}
		}

		protected new float Height => buttonBackground.Height;

		private void ScaleText()
		{
			if (!AutoSize)
			{
				Vector2 size = label.font.MeasureString(label.Text);
				if (size.X > Width - (buttonBackground.Width * 2 + 10))
				{
					label.Scale = (Width - (buttonBackground.Width * 2 + 10)) / size.X;
					if (size.Y * label.Scale > Height)
					{
						label.Scale = Height / size.Y;
					}
				}
				else
				{
					label.Scale = Height / size.Y;
				}
			}
			else
			{
				label.Scale = Height / label.font.MeasureString(label.Text).Y;
			}
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(buttonBackground, DrawPosition, null, drawColor * Opacity, 0f, Origin, 1f, SpriteEffects.None, 0f);
			int fillWidth = (int)Width - 2 * buttonBackground.Width;
			Vector2 pos = DrawPosition;
			pos.X += buttonBackground.Width;
			spriteBatch.Draw(ButtonFill, pos - Origin, null, drawColor * Opacity, 0f, Vector2.Zero, new Vector2(fillWidth, 1f), SpriteEffects.None, 0f);
			pos.X += fillWidth;
			spriteBatch.Draw(buttonBackground, pos, null, drawColor * Opacity, 0f, Origin, 1f, SpriteEffects.FlipHorizontally, 0f);
			base.Draw(spriteBatch);
		}
	}
}