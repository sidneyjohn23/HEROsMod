using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;

namespace HEROsMod.UIKit
{
	internal class UISlider : UIView
	{
		protected static int padding = 8;
		protected static Texture2D sliderTexture = Main.colorSliderTexture;
		internal static Texture2D barTexture;
		private static Texture2D barFill;

		private static Texture2D BarFill
		{
			get
			{
				if (barFill == null)
				{
					Color[] edgeColors = new Color[barTexture.Width * barTexture.Height];
					barTexture.GetData(edgeColors);
					Color[] fillColors = new Color[barTexture.Height];
					for (int y = 0; y < fillColors.Length; y++)
					{
						fillColors[y] = edgeColors[barTexture.Width - 1 + y * barTexture.Width];
					}
					barFill = new Texture2D(UIView.Graphics, 1, fillColors.Length);
					barFill.SetData(fillColors);
				}
				return barFill;
			}
		}

		public delegate void SliderEventHandler(object sender, float value);

		public event SliderEventHandler ValueChanged;
		private float value = 0f;

		public float Value
		{
			get => value;
			set
			{
				if (value < MinValue)
				{
					value = MinValue;
				}
				if (value > MaxValue)
				{
					value = MaxValue;
				}
				this.value = value;
			}
		}

		public float MinValue
		{
			get;
			set;
		} = 0f;

		public float MaxValue
		{
			get;
			set;
		} = 1f;

		protected new float Width { get; set; } = 100f;

		protected new float Height => sliderTexture.Height;

		public override void Update()
		{
			base.Update();
			if (ModUtils.MouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
			{
				leftButtonDown = false;
			}
			if (leftButtonDown)
			{
				float sliderPos = UIView.MouseX - DrawPosition.X + Origin.X;
				if (sliderPos < padding)
				{
					sliderPos = padding;
				}
				else if (sliderPos > Width - padding)
				{
					sliderPos = Width - padding;
				}

				sliderPos -= padding;
				sliderPos /= Width - padding * 2;
                Value = (MaxValue - MinValue) * sliderPos + MinValue;
                ValueChanged?.Invoke(this, Value);
            }
		}

		public virtual void DrawBackground(SpriteBatch spriteBatch)
		{
			Vector2 pos = DrawPosition;
			pos.Y += (sliderTexture.Height - barTexture.Height) / 2;
			spriteBatch.Draw(barTexture, pos, null, BackgroundColor, 0f, Origin, 1f, SpriteEffects.None, 0f);
			int fillWidth = (int)Width - 2 * barTexture.Width;
			pos.X += barTexture.Width;
			spriteBatch.Draw(BarFill, pos - Origin, null, BackgroundColor, 0f, Vector2.Zero, new Vector2(fillWidth, 1f), SpriteEffects.None, 0f);
			pos.X += fillWidth;
			spriteBatch.Draw(barTexture, pos, null, BackgroundColor, 0f, Origin, 1f, SpriteEffects.FlipHorizontally, 0f);
			Vector2 sliderPos = DrawPosition;
			sliderPos.X += padding - sliderTexture.Width / 2;
			sliderPos.X += (Width - padding * 2) * ((value - MinValue) / (MaxValue - MinValue));
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			DrawBackground(spriteBatch);
			Vector2 sliderPos = DrawPosition;
			sliderPos.X += padding - sliderTexture.Width / 2;
			sliderPos.X += (Width - padding * 2) * ((value - MinValue) / (MaxValue - MinValue));
			spriteBatch.Draw(sliderTexture, sliderPos, null, BackgroundColor, 0f, Origin, 1f, SpriteEffects.None, 0f);

			base.Draw(spriteBatch);
		}
	}
}