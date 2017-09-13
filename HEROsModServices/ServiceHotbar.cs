﻿using HEROsMod.UIKit;
using HEROsMod.UIKit.UIComponents;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;

namespace HEROsMod.HEROsModServices
{
	internal class ServiceHotbar : UIWindow
	{
		/// <summary>
		/// Container View for service icons
		/// </summary>
		private UIView _iconView;

		private List<UIView> _view = new List<UIView>();
		private bool _collapsed = false;
		private float _lerpAmount = 0f;
		private UIImage collapseArrow;
		private UIImage collapseButton;

		internal UIHotbar HotBarChild;

		/// <summary>
		/// Target Y Position for the hotbar when not hidden.
		/// </summary>
		private float _shownYPosition
		{
			get
			{
				return Main.screenHeight - Height - 12;
			}
		}

		/// <summary>
		/// Target Y Position for the hotbar when hidden.
		/// </summary>
		private float _hiddenYPosition
		{
			get
			{
				return Main.screenHeight;
			}
		}

		/// <summary>
		/// Returns if the hotbar is collapsed or not
		/// </summary>
		public bool Collapsed
		{
			get { return _collapsed; }
		}

		public Vector2 ChatOffsetPosition
		{
			get
			{
				if (Visible)
					return new Vector2(0, Position.Y - Main.screenHeight - collapseArrow.Height);
				else return Vector2.Zero;
			}
		}

		public ServiceHotbar()
		{
			HEROsMod.ServiceController.ServiceAdded += ServiceAddedOrRemoved;
			HEROsMod.ServiceController.ServiceRemoved += ServiceAddedOrRemoved;
			InitUI();
		}

		// Recalculate buttons.
		private void ServiceAddedOrRemoved(HEROsModService modifiedService)
		{
			// Clear existing icons in the Hotbar
			_iconView.RemoveAllChildren();
			// For each service, add its icon to the hotbar
			float xPos = Spacing;
			for (int i = 0; i < HEROsMod.ServiceController.Services.Count; i++)
			{
				HEROsModService service = HEROsMod.ServiceController.Services[i];
				if (service.HotbarIcon == null || !service.HasPermissionToUse) continue;
				if (service.IsHotbar)
				{
					service.Hotbar.buttonView.RemoveAllChildren();
					service.Hotbar.test();
				}
				if (service.IsInHotbar/* && service.HotbarParent.buttonView != null*/)
				{
					//ErrorLogger.Log("adding " + service.Name);
					//ErrorLogger.Log("adding 1" + service.HotbarParent.ChildCount);
					//ErrorLogger.Log("adding 3" + service.HotbarParent.buttonView.ChildCount);

					UIImage icon = HEROsMod.ServiceController.Services[i].HotbarIcon;
					//icon.Anchor = AnchorPosition.Left;
					//icon.X = xPos;
					//icon.Y = 0;
					//xPos += icon.Width + Spacing;
					service.HotbarParent.buttonView.AddChild(icon);
					//_iconView.AddChild(icon);
					//icon.CenterYAxisToParentCenter();

					service.HotbarParent.test();

					//ModUtils.DebugText("added " + service.Name);
				}
				else
				{
					UIImage icon = HEROsMod.ServiceController.Services[i].HotbarIcon;
					icon.Anchor = AnchorPosition.Left;
					icon.X = xPos;
					icon.Y = 0;
					xPos += icon.Width + Spacing;
					_iconView.AddChild(icon);
					icon.CenterYAxisToParentCenter();
				}
			}
			if (_iconView.ChildCount > 0)
			{
                Width = _iconView.GetLastChild().X + _iconView.GetLastChild().Width + Spacing;
				_iconView.Width = Width;
			}
			collapseButton.CenterXAxisToParentCenter();
			collapseArrow.Position = collapseButton.Position;
		}

		private void InitUI()
		{
            Height = 54;///55; // 38 + 8 + 8 = 54
			Width = 0;
            Anchor = AnchorPosition.Top;
            UpdateWhenOutOfBounds = true;
			MasterView.gameScreen.AddChild(this);
            _iconView = new UIView()
            {
                Width = Width,
                Height = Height
            };
            AddChild(_iconView);

            collapseButton = new UIImage(HEROsMod.instance.GetTexture("Images/CollapseBar/CollapseButtonHorizontal"))
            {
                UpdateWhenOutOfBounds = true
            };
            collapseArrow = new UIImage(HEROsMod.instance.GetTexture("Images/CollapseBar/CollapseArrowHorizontal"))
            {
                UpdateWhenOutOfBounds = true
            };
            collapseButton.Anchor = AnchorPosition.Top;
			collapseArrow.Anchor = AnchorPosition.Top;
			collapseArrow.SpriteEffect = SpriteEffects.FlipVertically;
			AddChild(collapseButton);
			AddChild(collapseArrow);
			collapseButton.Position = new Vector2(0, -collapseButton.Height);
			collapseButton.CenterXAxisToParentCenter();
			collapseArrow.Position = collapseButton.Position;
			collapseArrow.onLeftClick += collapseArrow_onLeftClick;
		}

		private void collapseArrow_onLeftClick(object sender, EventArgs e)
		{
			if (HotBarChild != null && HotBarChild.selected)
			{
				HotBarChild.selected = false;
				//HotBarChild = null;
				return;
			}
			_collapsed = !Collapsed;
			if (Collapsed)
			{
				//if(HotBarChild != null)
				//{
				//	HotBarChild.Hide();
				//}
				collapseArrow.SpriteEffect = SpriteEffects.None;
			}
			else
			{
				//HotBarChild?.Show();
				collapseArrow.SpriteEffect = SpriteEffects.FlipVertically;
			}
		}

		public override void Update()
		{
			if (HotBarChild != null && HotBarChild.Visible)
			{
				collapseButton.Position = new Vector2(0, -collapseButton.Height - (Y - HotBarChild.Y));
				collapseButton.CenterXAxisToParentCenter();
				collapseArrow.Position = collapseButton.Position;
			}

			float moveSpeed = 10f;
			if (Collapsed)
			{
				_lerpAmount -= ModUtils.DeltaTime * moveSpeed;
				if (_lerpAmount < 0f) _lerpAmount = 0f;
			}
			else
			{
				_lerpAmount += ModUtils.DeltaTime * moveSpeed;
				if (_lerpAmount > 1f) _lerpAmount = 1f;
			}
			float yPos = MathHelper.SmoothStep(_hiddenYPosition, _shownYPosition, _lerpAmount);
            Position = new Vector2(X, yPos);
            CenterXAxisToParentCenter();

			base.Update();
		}

		protected override bool IsMouseInside()
		{
			return base.IsMouseInside() || collapseArrow.MouseInside;
		}
	}
}