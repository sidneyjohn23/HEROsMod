//using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;

namespace HEROsMod.UIKit
{
	internal class UIWindow : UIView
	{
		public bool ClickAndDrag { get; set; } = true;

		private bool dragging = false;
		private Vector2 dragAnchor = Vector2.Zero;
		private readonly bool _constrainInsideParent = true;
		public bool CanMove = false;

		public UIWindow()
		{
			Width = 500;
			Height = 300;
			BackgroundColor = new Color(53, 35, 111, 255) * 0.685f;
			OnMouseDown += new ClickEventHandler(UIWindow_onMouseDown);
			OnMouseUp += new ClickEventHandler(UIWindow_onMouseUp);
		}

		private void UIWindow_onMouseUp(object sender, byte button)
		{
			if (dragging)
			{
				dragging = false;
			}
		}

		private void UIWindow_onMouseDown(object sender, byte button)
		{
			MoveToFront();
			if (CanMove)
			{
				if (button == 0)
				{
					dragging = true;
					dragAnchor = new Vector2(MouseX, MouseY) - DrawPosition;
				}
			}
		}

		protected new float Height { get; set; }

		protected new float Width { get; set; }

		public override void Update()
		{
			base.Update();
			if (dragging)
			{
				Position = new Vector2(MouseX, MouseY) - dragAnchor;
				if (_constrainInsideParent)
				{
					if (Position.X - Origin.X < 0)
					{
						X = Origin.X;
					}
					else if (Position.X + Width - Origin.X > Parent.Width)
					{
						X = Parent.Width - Width + Origin.X;
					}

					if (Y - Origin.Y < 0)
					{
						Y = Origin.Y;
					}
					else if (Y + Height - Origin.Y > Parent.Height)
					{
						Y = Parent.Height - Height + Origin.Y;
					}
				}
			}

			if (Visible && (IsMouseInside()/* || button.MouseInside*/))
			{
				Main.player[Main.myPlayer].mouseInterface = true;
				Main.player[Main.myPlayer].showItemIcon = false;
			}
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (Visible)
			{
				Utils.DrawInvBG(spriteBatch, DrawPosition.X - Origin.X, DrawPosition.Y - Origin.Y, Width, Height, BackgroundColor);
			}
			//spriteBatch.Draw(dummyTexture, new Rectangle((int)DrawPosition.X, (int)DrawPosition.Y, (int)Width, (int)Height), Color.Blue);
			base.Draw(spriteBatch);
		}
	}
}