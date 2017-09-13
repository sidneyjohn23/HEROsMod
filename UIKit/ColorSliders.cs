﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;

namespace HEROsMod.UIKit
{
	internal class HueSlider : UISlider
	{
		public HueSlider()
		{
            Width = ModUtils.HueTexture.Width;
		}

		public override void DrawBackground(SpriteBatch spriteBatch)
		{
			Vector2 pos = DrawPosition;
			pos.Y += (sliderTexture.Height - ModUtils.HueTexture.Height) / 2;
			Main.spriteBatch.Draw(ModUtils.HueTexture, pos, Color.White);
		}
	}

	internal class SaturationSlider : UISlider
	{
		public float Hue { get; set; }
		public float Luminosity { get; set; }

		public SaturationSlider()
		{
            Width = ModUtils.HueTexture.Width;
		}

		public override void DrawBackground(SpriteBatch spriteBatch)
		{
			Vector2 pos = DrawPosition;
			pos.Y += (sliderTexture.Height - ModUtils.HueTexture.Height) / 2;
			Main.spriteBatch.Draw(Main.colorBarTexture, pos, Color.White);
			int fillWidth = 167;
			for (int k = 0; k <= fillWidth; k++)
			{
				float saturation = (float)k / (float)fillWidth;
				Color color4 = Main.hslToRgb(Hue, saturation, Luminosity);
				Main.spriteBatch.Draw(Main.colorBlipTexture, new Vector2((float)(pos.X + k + 5), (float)(pos.Y + 4)), color4);
			}
		}
	}

	internal class LuminositySlider : UISlider
	{
		public float Hue { get; set; }
		public float Saturation { get; set; }

		public LuminositySlider()
		{
            Width = ModUtils.HueTexture.Width;
		}

		public override void DrawBackground(SpriteBatch spriteBatch)
		{
			Vector2 pos = DrawPosition;
			pos.Y += (sliderTexture.Height - ModUtils.HueTexture.Height) / 2;
			Main.spriteBatch.Draw(Main.colorBarTexture, pos, Color.White);

			int fillWidth = 167;
			for (int l = 0; l <= fillWidth; l++)
			{
				float luminosity = (float)l / (float)fillWidth;
				Color color5 = Main.hslToRgb(Hue, Saturation, luminosity);
				Main.spriteBatch.Draw(Main.colorBlipTexture, new Vector2((float)(pos.X + l + 5), (float)(pos.Y + 4)), color5);
			}
		}
	}
}