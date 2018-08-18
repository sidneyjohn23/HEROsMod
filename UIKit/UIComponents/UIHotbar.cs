﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace HEROsMod.UIKit.UIComponents
{
	internal class UIHotbar : UIWindow
	{
		internal RasterizerState _rasterizerState = new RasterizerState
		{
			ScissorTestEnable = true
		};

		internal static float slideMoveSpeed = 8f;
		internal float lerpAmount;

		//internal UIHotbar parentHotbar;
		internal UIView HotBarParent;

		internal float ShownPosition => (float)Main.screenHeight - base.Height * 2 - 12f + 6;

		internal float HiddenPosition
		{
			get
			{
				if (HotBarParent != null)
				{
					return (float)Main.screenHeight - base.Height - 12f;
				}
				//else if (mod.hotbar != null && !mod.hotbar.hidden && hidden)
				//{
				//	return (float)Main.screenHeight - base.Height - 12f;
				//}
				else
				{
					return (float)Main.screenHeight;
				}
			}
		}

		public void Hide()
		{
			hidden = true;
			arrived = false;
		}

		public void Show()
		{
			arrived = false;
			hidden = false;
			Visible = true;
		}

		public virtual void Test()
		{
		}

		internal float spacing = 8f;

		public bool hidden;
		internal bool arrived;

		internal bool Selected
		{
			get => Selected;
			set
			{
				if (value == false)
				{
					hidden = true;
				}
				else
				{
					hidden = false;
					Visible = true;
					//HotBarParent
					if (HEROsMod.ServiceHotbar.HotBarChild != null && HEROsMod.ServiceHotbar.HotBarChild != this)
					{
						HEROsMod.ServiceHotbar.HotBarChild.Selected = false;
					}
					HEROsMod.ServiceHotbar.HotBarChild = this;
				}
				arrived = false;
				Selected = value;
			}
		}

		public UIView buttonView;
		internal static Color buttonUnselectedColor = Color.LightSkyBlue;
		internal static Color buttonSelectedColor = Color.White;
		internal static Color buttonSelectedHiddenColor = Color.Blue;

		internal void DoSlideMovement()
		{
			if (!arrived)
			{
				//Main.NewText("Not Arrived");

				if (hidden)
				{
                    lerpAmount -= .01f * slideMoveSpeed;
					if (lerpAmount < 0f)
					{
                        lerpAmount = 0f;
						arrived = true;
                        //	Main.NewText("Arrived, Not Visible");
                        Visible = false;
					}
					float y = MathHelper.SmoothStep(HiddenPosition, ShownPosition, lerpAmount);
					Position = new Vector2(Position.X, y);
				}
				else
				{
                    lerpAmount += .01f * slideMoveSpeed;
					if (lerpAmount > 1f)
					{
                        lerpAmount = 1f;
						arrived = true;
						//	Main.NewText("Arrived, Visible");
					}
					float y2 = MathHelper.SmoothStep(HiddenPosition, ShownPosition, lerpAmount);
					Position = new Vector2(Position.X, y2);
				}
			}
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (Visible)
			{
				spriteBatch.End();
				//spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, this._rasterizerState);
				spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, null, null, _rasterizerState, null, Main.UIScaleMatrix);
				//	Rectangle scissorRectangle = new Rectangle((int)base.X- (int)base.Width, (int)base.Y, (int)base.Width, (int)base.Height);
				//Parent.Position.Y
				//		Main.NewText((int)Parent.Position.Y + " " + (int)shownPosition);
				//	Rectangle scissorRectangle = new Rectangle((int)(base.X - base.Width / 2), (int)(shownPosition), (int)base.Width, (int)base.Height);
				Rectangle scissorRectangle = new Rectangle((int)(base.X - Width / 2), (int)(ShownPosition), (int)Width, (int)(HotBarParent.Position.Y - ShownPosition));
				/*if (scissorRectangle.X < 0)
				{
					scissorRectangle.Width += scissorRectangle.X;
					scissorRectangle.X = 0;
				}
				if (scissorRectangle.Y < 0)
				{
					scissorRectangle.Height += scissorRectangle.Y;
					scissorRectangle.Y = 0;
				}
				if ((float)scissorRectangle.X + base.Width > (float)Main.screenWidth)
				{
					scissorRectangle.Width = Main.screenWidth - scissorRectangle.X;
				}
				if ((float)scissorRectangle.Y + base.Height > (float)Main.screenHeight)
				{
					scissorRectangle.Height = Main.screenHeight - scissorRectangle.Y;
				}*/
				scissorRectangle = ModUtils.GetClippingRectangle(spriteBatch, scissorRectangle);
				Rectangle scissorRectangle2 = spriteBatch.GraphicsDevice.ScissorRectangle;
				spriteBatch.GraphicsDevice.ScissorRectangle = scissorRectangle;

				base.Draw(spriteBatch);

				spriteBatch.GraphicsDevice.ScissorRectangle = scissorRectangle2;
				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Main.UIScaleMatrix);
			}
			//	base.Draw(spriteBatch);
		}
	}
}