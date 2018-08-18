﻿using System;

namespace HEROsMod.UIKit.UIComponents
{
	internal class SliderWithTextbox : UIView
	{
		public float Value
		{
			get => slider.Value;
			set => slider.Value = value;
		}

		public float Min
		{
			get => slider.MinValue;
			set => slider.MinValue = value;
		}

		public float Max
		{
			get => slider.MaxValue;
			set => slider.MaxValue = value;
		}

		public event EventHandler ValueChanged;

		private UITextbox textbox;
		private UISlider slider;

		public SliderWithTextbox(float startValue, float minValue, float maxValue)
		{
            textbox = new UITextbox()
            {
                Width = 125
            };
            textbox.KeyPressed += Textbox_KeyPressed;
			textbox.OnLostFocus += Textbox_OnLostFocus;
			textbox.Numeric = true;
			textbox.HasDecimal = true;
			slider = new UISlider();
			slider.ValueChanged += Slider_valueChanged;

			slider.X = textbox.X + textbox.Width + Spacing;
			AddChild(textbox);
			AddChild(slider);

			slider.MinValue = minValue;
			slider.MaxValue = maxValue;
			slider.Value = startValue;

			textbox.Text = slider.Value.ToString();

            Height = textbox.Height;
            Width = slider.X + slider.Width;
		}

		private void Textbox_OnLostFocus(object sender, EventArgs e) => textbox.Text = slider.Value.ToString();

		private void Textbox_KeyPressed(object sender, char key)
		{
			if (textbox.Text.Length == 0 || textbox.Text == "-")
			{
				slider.Value = slider.MinValue;
				return;
			}
			slider.Value = float.Parse(textbox.Text);
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

		private void Slider_valueChanged(object sender, float value)
		{
			if (!textbox.HadFocus)
			{
				textbox.Text = slider.Value.ToString();
			}
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }
	}
}