using Microsoft.Xna.Framework;
using ReLogic.Graphics;
using System.Collections.Generic;

namespace HEROsMod.UIKit
{
	internal class UIWrappingLabel : UIView
	{
		private DynamicSpriteFont font = UILabel.DefaultFont;
		private List<UILabel> labels = new List<UILabel>();

		public string Text
		{
			get => Text;
			set
			{
				Text = value;
				SetLabels();
			}
		}

		public UIWrappingLabel()
		{
			Scale = .5f;
			Width = 200;
		}

		public UIWrappingLabel(string text, float width)
		{
            Width = width;
			Scale = .5f;
            Text = text;
		}

		private void SetLabels()
		{
			for (int i = 0; i < labels.Count; i++)
			{
				RemoveChild(labels[i]);
			}
			labels.Clear();
			if (Text.Length > 0)
			{
				string[] words = Text.Split(' ');
				UILabel currentLabel = null;
				for (int i = 0; i < words.Length; i++)
				{
					Vector2 wordSize = font.MeasureString(words[i] + " ") * Scale;
					if (currentLabel == null || currentLabel.Width + wordSize.X > Width)
					{
                        currentLabel = new UILabel()
                        {
                            Scale = Scale,
                            font = font
                        };
                        currentLabel.Position = new Vector2(0, labels.Count * currentLabel.Height);
						labels.Add(currentLabel);
						AddChild(currentLabel);
					}
					currentLabel.Text += words[i];
					if (i != words.Length - 1)
					{
						currentLabel.Text += " ";
					}
				}
			}
		}

		protected new float Height
		{
			get
			{
				float result = 0;
				for (int i = 0; i < labels.Count; i++)
				{
					result += labels[i].Height;
				}
				return result;
			}
		}
	}
}