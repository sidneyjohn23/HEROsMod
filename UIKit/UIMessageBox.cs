using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace HEROsMod.UIKit
{
	internal enum UIMessageBoxType
	{
		Ok,
		YesNo
	}

	internal class UIMessageBox : UIWindow
	{
		private readonly UIMessageBoxType MessageType = UIMessageBoxType.Ok;
		private UIWrappingLabel label = new UIWrappingLabel();
		private UIButton okButton = null;
		private UIButton yesButton = null;
		private UIButton noButton = null;

		public event EventHandler YesClicked;

		public event EventHandler NoClicked;

		public string Text
		{
			get => label.Text;
			set
			{
				label.Text = value;
				Height = label.Height + 70;
				PositionButtons();
			}
		}

		public UIMessageBox()
		{
			AddButtons();
            Text = "";
		}

		public UIMessageBox(string text, bool ExclusiveControl = false)
		{
			AddButtons();
            Text = text;
			if (ExclusiveControl)
			{
				UIView.ExclusiveControl = this;
			}
		}

		public UIMessageBox(string text, UIMessageBoxType messageBoxType, bool ExclusiveControl = false)
		{
            MessageType = messageBoxType;
			AddButtons();
            Text = text;

			if (ExclusiveControl)
			{
				UIView.ExclusiveControl = this;
			}
		}

		private void AddButtons()
		{
            Anchor = AnchorPosition.Center;
			label.Anchor = AnchorPosition.Top;
			label.Width = Width - 30;
			label.Position = new Vector2(Width / 2, 10);
			AddChild(label);
			if (MessageType == UIMessageBoxType.Ok)
			{
                okButton = new UIButton("Ok")
                {
                    Anchor = AnchorPosition.BottomRight
                };
                AddChild(okButton);
				okButton.OnLeftClick += new EventHandler(OkButton_onLeftClick);
			}
			else if (MessageType == UIMessageBoxType.YesNo)
			{
				noButton = new UIButton("No");
				yesButton = new UIButton("Yes");
				noButton.Anchor = AnchorPosition.BottomRight;
				yesButton.Anchor = AnchorPosition.BottomRight;
				AddChild(noButton);
				AddChild(yesButton);
				noButton.OnLeftClick += NoButton_onLeftClick;
				yesButton.OnLeftClick += YesButton_onLeftClick;
			}
		}

		private void YesButton_onLeftClick(object sender, EventArgs e)
		{
            YesClicked?.Invoke(this, EventArgs.Empty);
            if (Parent != null)
			{
				if (ExclusiveControl == this)
				{
					ExclusiveControl = null;
				}

				Parent.RemoveChild(this);
			}
		}

		private void NoButton_onLeftClick(object sender, EventArgs e)
		{
            NoClicked?.Invoke(this, EventArgs.Empty);
            if (Parent != null)
			{
				if (ExclusiveControl == this)
				{
					ExclusiveControl = null;
				}

				Parent.RemoveChild(this);
			}
		}

		private void OkButton_onLeftClick(object sender, EventArgs e)
		{
			if (Parent != null)
			{
				if (ExclusiveControl == this)
				{
					ExclusiveControl = null;
				}

				Parent.RemoveChild(this);
			}
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (Parent != null)
			{
				CenterToParent();
			}

			base.Draw(spriteBatch);
		}

		private void PositionButtons()
		{
			if (MessageType == UIMessageBoxType.Ok)
			{
				okButton.Position = new Vector2(Width - 8, Height - 8);
			}
			else if (MessageType == UIMessageBoxType.YesNo)
			{
				noButton.Position = new Vector2(Width - 8, Height - 8);
				yesButton.Position = new Vector2(noButton.Position.X - noButton.Width - 8, noButton.Position.Y);
			}
		}
	}
}