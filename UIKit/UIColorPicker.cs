using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace HEROsMod.UIKit
{
	internal class UIColorPicker : UIView
	{
		public event EventHandler ColorChanged;

		public Color Color
		{
			get => Main.hslToRgb(Hue, Saturation, Luminosity);
			set
			{
				Vector3 hsl = Main.rgbToHsl(value);
				Hue = hsl.X;
				Saturation = hsl.Y;
				Luminosity = hsl.Z;
			}
		}

		public float Hue
		{
			get => hueSlider.Value;
			set
			{
				hueSlider.Value = value;
				saturationSlider.Hue = value;
				luminositySlider.Hue = value;
			}
		}

		public float Saturation
		{
			get => saturationSlider.Value;
			set
			{
				saturationSlider.Value = value;
				luminositySlider.Saturation = value;
			}
		}

		public float Luminosity
		{
			get => luminositySlider.Value;
			set
			{
				luminositySlider.Value = value;
				saturationSlider.Luminosity = value;
			}
		}

		private HueSlider hueSlider;
		private SaturationSlider saturationSlider;
		private LuminositySlider luminositySlider;

		public UIColorPicker()
		{
			hueSlider = new HueSlider();
			saturationSlider = new SaturationSlider();
			luminositySlider = new LuminositySlider();

			saturationSlider.Y = hueSlider.Height;
			luminositySlider.Y = saturationSlider.Y + saturationSlider.Height;
            Width = hueSlider.Width;
            Height = luminositySlider.Y + luminositySlider.Height;

			hueSlider.ValueChanged += HueSlider_valueChanged;
			saturationSlider.ValueChanged += SaturationSlider_valueChanged;
			luminositySlider.ValueChanged += LuminositySlider_valueChanged;

            Color = Color.White;

			AddChild(hueSlider);
			AddChild(saturationSlider);
			AddChild(luminositySlider);
		}

		private void TriggerColorChangedEvent() => ColorChanged?.Invoke(this, EventArgs.Empty);

		private void LuminositySlider_valueChanged(object sender, float value)
		{
			Luminosity = luminositySlider.Value;
			TriggerColorChangedEvent();
		}

		private void SaturationSlider_valueChanged(object sender, float value)
		{
			Saturation = saturationSlider.Value;
			TriggerColorChangedEvent();
		}

		private void HueSlider_valueChanged(object sender, float value)
		{
			Hue = hueSlider.Value;
			TriggerColorChangedEvent();
		}
	}
}