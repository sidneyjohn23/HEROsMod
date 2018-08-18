using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace HEROsMod.UIKit
{
	internal class UIListView : UIView
	{
		private List<UILabel> labels = new List<UILabel>();
		private List<string> items = new List<string>();
		public bool SelectableItems = true;
		public int SelectedIndex { get; private set; } = -1;

		public string[] Items => items.ToArray();

		public UIListView() => Width = 200;

		protected new float Height
		{
			get
			{
				float height = 0;
				if (labels.Count > 0)
				{
					height = labels[labels.Count - 1].Position.Y + labels[labels.Count - 1].Height;
				}
				return height;
			}
		}

		public void AddItem(string text)
		{
            UILabel label = new UILabel(text)
            {
                Tag = labels.Count
            };
            label.OnLeftClick += Label_onLeftClick;
			label.Scale = .5f;
			label.Position = new Vector2(0, Height);
			items.Add(text);
			labels.Add(label);
            AddChild(label);
		}

		public void ClearItems()
		{
			RemoveAllChildren();
			labels.Clear();
			items.Clear();
		}

		private void Label_onLeftClick(object sender, EventArgs e)
		{
			UILabel label = (UILabel)sender;
			SelectedIndex = (int)label.Tag;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (SelectableItems)
			{
				if (SelectedIndex > -1)
				{
					UILabel label = labels[SelectedIndex];
					Vector2 pos = label.DrawPosition;
					spriteBatch.Draw(ModUtils.DummyTexture, new Rectangle((int)pos.X, (int)pos.Y, (int)Width, (int)label.Height), Color.Pink);
				}
			}
			base.Draw(spriteBatch);
		}
	}
}