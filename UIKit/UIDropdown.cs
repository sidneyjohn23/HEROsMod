using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace HEROsMod.UIKit
{
	internal class UIDropdown : UIView
	{
		internal static Texture2D capUp;
		internal static Texture2D capDown;
		private bool itemsShown = false;
		private UIWindow itemsWindow = new UIWindow();
		private int selectedItem = 0;

		public int SelectedItem
		{
			get => selectedItem;
			set
			{
				selectedItem = value;
				selectedLabel.Text = items[value];
			}
		}

		private List<string> items = new List<string>();
		private UILabel selectedLabel = new UILabel("");
		public int ItemCount => items.Count;
		public string Text => selectedLabel.Text;

		public event EventHandler SelectedChanged;

		public bool ItemsVisible => itemsWindow.Visible;

		public UIDropdown()
		{
            UpdateWhenOutOfBounds = true;
			itemsWindow.UpdateWhenOutOfBounds = true;
			selectedLabel.ForegroundColor = Color.Black;
			selectedLabel.Scale = .5f;
			selectedLabel.X = 6;
			selectedLabel.TextOutline = false;
			itemsWindow.UpdateWhenOutOfBounds = true;
			itemsWindow.BackgroundColor = new Color(81, 91, 184);
            OnLeftClick += UIDropdown_onLeftClick;
			AddChild(selectedLabel);
			AddChild(itemsWindow);
			itemsWindow.Visible = false;
			SelectedChanged += UIDropdown_selectedChanged;
		}

		private void UIDropdown_selectedChanged(object sender, EventArgs e) => MouseLeftButton = false;

		private void UIDropdown_onLeftClick(object sender, EventArgs e)
		{
            MoveToFront();
			ToggleShowingItems();
		}

		private void ToggleShowingItems()
		{
			if (itemsShown)
			{
				HideItems();
			}
			else
			{
				ShowItems();
			}
		}

		private void ShowItems()
		{
			itemsWindow.Visible = true;
			itemsShown = true;
		}

		private void HideItems()
		{
			itemsWindow.Visible = false;
			itemsShown = false;
		}

		public void AddItem(string item)
		{
			items.Add(item);
			if (itemsWindow.ChildCount == 0)
			{
				selectedLabel.Text = item;
			}

			UILabel label = new UILabel(item);
			UIRect bg = new UIRect();
			label.Scale = .4f;
			label.X = 8;
			bg.X = 3;
			bg.Y = Height + ((label.Height) * itemsWindow.ChildCount);
			label.Tag = itemsWindow.ChildCount;
			bg.Tag = label.Tag;
			label.OnLeftClick += Label_onLeftClick;
			bg.OnLeftClick += Label_onLeftClick;
			bg.Width = itemsWindow.Width - 6;
			bg.Height = label.Height;
			bg.ForegroundColor = Color.White * 0f;
			bg.OnMouseEnter += Bg_onMouseEnter;
			bg.OnMouseLeave += Bg_onMouseLeave;

			itemsWindow.Height = bg.Y + bg.Height + 8;
			bg.AddChild(label);
			itemsWindow.AddChild(bg);
		}

		private void Bg_onMouseLeave(object sender, EventArgs e)
		{
			UIRect rect = (UIRect)sender;
			rect.ForegroundColor = Color.White * 0f;
		}

		private void Bg_onMouseEnter(object sender, EventArgs e)
		{
			UIRect rect = (UIRect)sender;
			rect.ForegroundColor = Color.Black * .1f;
		}

		public string GetItem(int index) => items[index];

		public void ClearItems()
		{
			itemsWindow.RemoveAllChildren();
			selectedItem = 0;
			selectedLabel.Text = "";
			items.Clear();
		}

		private void Label_onLeftClick(object sender, EventArgs e)
		{
            MoveToFront();
			UIView label = (UIView)sender;
			int tag = (int)label.Tag;
			selectedLabel.Text = items[tag];
			if (tag != selectedItem)
			{
				selectedItem = tag;
                SelectedChanged?.Invoke(this, new EventArgs());
            }
			HideItems();
			MouseLeftButton = false;
		}

		protected new float Width
		{
			get => Width;
			set
			{
				Width = value;
				itemsWindow.Width = value;
			}
		}

		protected new float Height
		{
			get
			{
				if (itemsWindow.Visible)
				{
					return itemsWindow.Height;
				}
				else
				{
					return UIButton.buttonBackground.Height;
				}
			}
		}

		public override void Update()
		{
			if (itemsShown && !itemsWindow.MouseInside && MouseLeftButton)
			{
				HideItems();
			}
			base.Update();
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			itemsWindow.Draw(spriteBatch);
			spriteBatch.Draw(UIButton.buttonBackground, DrawPosition, null, BackgroundColor, 0f, Origin, 1f, SpriteEffects.None, 0f);
			int fillWidth = (int)Width - 2 * UIButton.buttonBackground.Width;
			Vector2 pos = DrawPosition;
			pos.X += UIButton.buttonBackground.Width;
			spriteBatch.Draw(UIButton.ButtonFill, pos - Origin, null, BackgroundColor, 0f, Vector2.Zero, new Vector2(fillWidth, 1f), SpriteEffects.None, 0f);
			pos.X += fillWidth;
			spriteBatch.Draw(UIButton.buttonBackground, pos, null, BackgroundColor, 0f, Origin, 1f, SpriteEffects.FlipHorizontally, 0f);
			if (itemsWindow.Visible)
			{
				spriteBatch.Draw(capUp, new Vector2(DrawPosition.X + Width - capUp.Width, DrawPosition.Y), Color.White);
			}
			else
			{
				spriteBatch.Draw(capDown, new Vector2(DrawPosition.X + Width - capUp.Width, DrawPosition.Y), Color.White);
			}

			selectedLabel.Draw(spriteBatch);
		}
	}
}