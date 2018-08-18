using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;

namespace HEROsMod.UIKit
{
	internal enum AnchorPosition
	{
		Left,
		Right,
		Top,
		Bottom,
		Center,
		TopLeft,
		TopRight,
		BottomLeft,
		BottomRight
	}

	internal class UIView
	{
		internal static Texture2D closeTexture;

		//statics
		public static UIView ExclusiveControl { get; set; } = null;

		public static bool GameMouseOverwritten { get; set; } = false;
		protected static int MouseX => Main.mouseX;
		protected static int MouseY => Main.mouseY;
		protected static bool MouseLeftButton = false;
		protected static bool MousePrevLeftButton = false;
		protected static bool MouseRightButton = false;
		protected static bool MousePrevRightButton = false;
		public static int ScrollAmount { get; set; } = 0;

		public static string HoverText { get; set; } = "";
		//public static Item HoverItem = new Item();
		protected static Item EmptyItem => new Item();
		public static bool HoverOverridden { get; set; } = false;

		public static float SmallSpacing => 4f;
		public static float Spacing => 8f;
		public static float LargeSpacing => 16f;

		/*
        protected static bool mouseLeftButton { get { return Main.mouseLeft; } }
        protected static bool mousePrevLeftButton { get { return !Main.mouseLeftRelease; } }
        protected static bool mouseRightButton { get { return Main.mouseRight; } }
        protected static bool mousePrevRightButton { get { return !Main.mouseRightRelease; } }
        */
		protected static Texture2D DummyTexture => ModUtils.DummyTexture;
		protected static GraphicsDevice Graphics => Main.graphics.GraphicsDevice;

		public Vector2 Position
		{
			get => new Vector2(X, Y);
			set
			{
				X = value.X;
				Y = value.Y;
			}
		}

		public float X
		{
			get;
			set;
		} = 0;

		public float Y
		{
			get;
			set;
		} = 0;

		public Vector2 DrawPosition => Position + Offset + (Parent != null ? Parent.DrawPosition - Parent.Origin : Vector2.Zero);

		public float Width { get; set; }
		public float Height { get; set; }

		//C
		protected bool mouseForChildrenHandled = false;

		public List<UIView> Children { get; } = new List<UIView>();
		private List<UIView> childrenToRemove = new List<UIView>();
		public UIView Parent { get; set; }

		//Mouse Events
		public delegate void ClickEventHandler(object sender, byte button);

		public event EventHandler OnHover;

		public event EventHandler OnLeftClick;

		public event EventHandler OnRightClick;

		public event EventHandler OnMouseEnter;

		public event EventHandler OnMouseLeave;

		public event ClickEventHandler OnMouseDown;

		public event ClickEventHandler OnMouseUp;

		private static bool mouseUpHandled = false;
		private static bool mouseDownHandled = false;

		protected bool leftButtonDown = false;
		protected bool rightButtonDown = false;

		//Mouse Variables
		private bool mousePreviouslyIn = false;

		public bool MouseInside => IsMouseInside();

		public int ChildCount => Children.Count;

		public Color ForegroundColor { get; set; } = Color.White;
		public Color BackgroundColor { get; set; } = Color.White;
		public AnchorPosition Anchor { get; set; } = AnchorPosition.TopLeft;
		public Vector2 Origin => GetOrigin();
		public Vector2 Offset { get; set; } = Vector2.Zero;
		public float Scale { get; set; } = 1f;
		public float Opacity
		{
			get => Opacity * (Parent != null ? Parent.Opacity : 1);
			set => Opacity = value;
		}

		public bool Visible { get; set; } = true;

		public bool OverridesMouse { get; set; } = true;

		public string Tooltip
		{
			get => Tooltip;
			set
			{
				if (value.Length > 0 && Tooltip.Length == 0)
				{
					//add event
					OnHover += DisplayTooltip;
				}
				else if (value.Length == 0 && Tooltip.Length > 0)
				{
					//remove event
					OnHover -= DisplayTooltip;
				}
				Tooltip = value;
			}
		}

		public bool UpdateWhenOutOfBounds { get; set; } = false;
		public object Tag { get; set; }

		public virtual void Update()
		{
			if (Parent == null)
			{
				mouseDownHandled = false;
				mouseUpHandled = false;
				GameMouseOverwritten = false;
			}
			mouseForChildrenHandled = false;
			if (Visible)
			{
				for (int i = 0; i < Children.Count; i++)
				{
					UIView child = Children[Children.Count - 1 - i];
					if (child.UpdateWhenOutOfBounds || child.InParent())
					{
						Children[Children.Count - 1 - i].Update();
					}
				}
				while (childrenToRemove.Count > 0)
				{
					Children.Remove(childrenToRemove[0]);
					childrenToRemove.RemoveAt(0);
				}

				if ((ExclusiveControl == null && Parent == null) || this == ExclusiveControl)
				{
					HandleMouseInput();
				}
			}
			/*
            if (Parent == null || Parent.IsMouseInside())
            {
                HandleMouseInput();
            }
             */
		}

		private void DisplayTooltip(object sender, EventArgs e) => HoverText = ((UIView)sender).Tooltip;

		public static void OverWriteGameMouseInput()
		{
			GameMouseOverwritten = true;
			Main.mouseLeft = false;
			Main.mouseLeftRelease = false;
			Main.mouseRight = false;
			Main.mouseLeft = false;
			//Main.oldMouseState = Main.mouseState; // TODO?
			HoverOverridden = true;
		}

		private bool InParent()
		{
			float h = Parent.Height;

			return !((Position.Y + Offset.Y < 0 && Position.Y + Offset.Y + Height < 0) ||
				   (Position.Y + Offset.Y > h && Position.Y + Offset.Y + Height > h));
		}

		private void HandleMouseInput()
		{
			for (int i = 0; i < Children.Count; i++)
			{
				UIView child = Children[Children.Count - 1 - i];
				if (child.Visible)
				{
					if (child.Parent == null || child.UpdateWhenOutOfBounds || child.mousePreviouslyIn || (child.InParent() && (child.Parent.MouseInside || child.Parent.UpdateWhenOutOfBounds) && !child.Parent.mouseForChildrenHandled))
					{
						child.HandleMouseInput();
					}
				}
			}

			if (OverridesMouse && MouseInside)
			{
				if (OnMouseLeave != null)
				{
				}
				if (Parent != null)
				{
					Parent.mouseForChildrenHandled = true;
					if (OverridesMouse)
					{
						OverWriteGameMouseInput();
					}
				}
				OnHover?.Invoke(this, new EventArgs());
				if (!mousePreviouslyIn)
				{
					OnMouseEnter?.Invoke(this, new EventArgs());
				}
				if (!MousePrevLeftButton && MouseLeftButton)
				{
					leftButtonDown = true;
					if (OnMouseDown != null && !mouseDownHandled)
					{
						OnMouseDown(this, 0);
					}
				}
				if (MousePrevLeftButton && !MouseLeftButton)
				{
					if (OnMouseUp != null && !mouseUpHandled)
					{
						OnMouseUp(this, 0);
					}
					if (leftButtonDown && OnLeftClick != null)
					{
						OnLeftClick(this, EventArgs.Empty);
					}
				}
				if (!MousePrevRightButton && MouseRightButton)
				{
					rightButtonDown = true;
					OnMouseDown?.Invoke(this, 1);
				}
				if (MousePrevRightButton && !MouseRightButton)
				{
					OnMouseUp?.Invoke(this, 1);
					if (rightButtonDown && OnRightClick != null)
					{
						OnRightClick(this, EventArgs.Empty);
					}
				}
				mousePreviouslyIn = true;
			}
			else
			{
				if (OnMouseLeave != null)
				{
				}
				if (mousePreviouslyIn)
				{
					OnMouseLeave?.Invoke(this, new EventArgs());
				}
				mousePreviouslyIn = false;
			}

			if (!MouseLeftButton)
			{
				leftButtonDown = false;
			}
			if (!MouseRightButton)
			{
				rightButtonDown = false;
			}
		}

		protected virtual Vector2 GetOrigin()
		{
			float centerX = Width / 2;
			float centerY = Height / 2;
			switch (Anchor)
			{
				case AnchorPosition.TopLeft:
					return Vector2.Zero;
				case AnchorPosition.Left:
					return new Vector2(0, centerY);
				case AnchorPosition.Right:
					return new Vector2(Width, centerY);
				case AnchorPosition.Top:
					return new Vector2(centerX, 0);
				case AnchorPosition.Bottom:
					return new Vector2(centerX, Height);
				case AnchorPosition.Center:
					return new Vector2(centerX, centerY);
				case AnchorPosition.TopRight:
					return new Vector2(Width, 0);
				case AnchorPosition.BottomLeft:
					return new Vector2(0, Height);
				case AnchorPosition.BottomRight:
					return new Vector2(Width, Height);
			}
			return Vector2.Zero;
		}

		protected virtual bool IsMouseInside()
		{
			/*
            if (inScrollView != null)
            {
                if (!inScrollView.MouseInside)
                    return false;
            }
             */
			Vector2 pos = DrawPosition - Origin;
			if (MouseX >= pos.X && MouseX <= pos.X + Width &&
				MouseY >= pos.Y && MouseY <= pos.Y + Height)
			{
				return true;
			}
			return false;
		}

		protected virtual Vector2 GetParentCenter()
		{
			float w = Main.screenWidth;
			float h = Main.screenHeight;
			if (Parent != null)
			{
				w = Parent.Width;
				h = Parent.Height;
			}
			return new Vector2(w / 2, h / 2);
		}

		public void CenterToParent() => Position = GetParentCenter();

		public void CenterXAxisToParentCenter() => Position = new Vector2(GetParentCenter().X, Position.Y);

		public void CenterYAxisToParentCenter() => Position = new Vector2(Position.X, GetParentCenter().Y);

		public virtual void Draw(SpriteBatch spriteBatch)
		{
			if (Visible)
			{
				int prevChildCount = ChildCount;
				for (int i = 0; i < ChildCount; i++)
				{
					if (ChildCount != prevChildCount)
					{
						//Main.NewText("We broke");
						break;
					}
					UIView child = Children[i];
					if (child.UpdateWhenOutOfBounds || child.InParent())
					{
						if (child.Visible)
						{
							child.Draw(spriteBatch);
						}
					}
				}
			}
		}

		public virtual void AddChild(UIView view)
		{
			view.Parent = this;
			view.OnMouseDown += new ClickEventHandler(View_onMouseDown);
			view.OnMouseUp += new ClickEventHandler(View_onMouseUp);
			Children.Add(view);
		}

		public void RemoveAllChildren() => Children.Clear();

		private void View_onMouseUp(object sender, byte button) => mouseUpHandled = true;

		private void View_onMouseDown(object sender, byte button) => mouseDownHandled = true;

		public void RemoveChild(UIView view) => childrenToRemove.Add(view);

		public void MoveToFront()
		{
			Parent.Children.Remove(this);
			Parent.Children.Add(this);
		}
	}
}