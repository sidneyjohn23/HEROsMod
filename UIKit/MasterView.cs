using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.GameInput;

namespace HEROsMod.UIKit
{
	internal class MasterView : UIView
	{
		private static MouseState mouseState = Mouse.GetState();
		private static MouseState previousMouseState = Mouse.GetState();

#pragma warning disable IDE1006 // Benennungsstile
		public static GameScreen gameScreen
#pragma warning restore IDE1006 // Benennungsstile
		{
			get
			{
				if (gameScreen == null)
				{
					gameScreen = new GameScreen();
					AddChildToMaster(gameScreen);
				}
				return gameScreen;
			}
			set => gameScreen = value;
		}

		private static MenuScreen _menuScreen = null;

#pragma warning disable IDE1006 // Benennungsstile
		public static MenuScreen menuScreen
#pragma warning restore IDE1006 // Benennungsstile
		{
			get
			{
				if (menuScreen == null)
				{
					menuScreen = new MenuScreen();
					AddChildToMaster(menuScreen);
				}
				return menuScreen;
			}
			set => menuScreen = value;
		}

		private static MapScreen _mapScreen = null;

#pragma warning disable IDE1006 // Benennungsstile
		public static MapScreen mapScreen
#pragma warning restore IDE1006 // Benennungsstile
		{
			get
			{
				if (mapScreen == null)
				{
					mapScreen = new MapScreen();
					AddChildToMaster(mapScreen);
				}
				return mapScreen;
			}
			set => mapScreen = value;
		}

		private static MasterView masterView = new MasterView();

		public MasterView()
		{
			Width = Main.screenWidth;
			Height = Main.screenHeight;
		}

		public static void ClearMasterView()
		{
			masterView.Children.Clear();
			_mapScreen = null;
			_menuScreen = null;
			gameScreen = null;
		}

		public static void UpdateMaster()
		{
			mouseState = Mouse.GetState();
			MouseLeftButton = mouseState.LeftButton == ButtonState.Pressed;
			MousePrevLeftButton = previousMouseState.LeftButton == ButtonState.Pressed;
			MouseRightButton = mouseState.RightButton == ButtonState.Pressed;
			MousePrevRightButton = previousMouseState.RightButton == ButtonState.Pressed;
			ScrollAmount = PlayerInput.ScrollWheelDeltaForUI;
			// UIView.ScrollAmount = (mouseState.ScrollWheelValue - previousMouseState.ScrollWheelValue) / 2;
			previousMouseState = mouseState;
			//HoverItem = EmptyItem;
			HoverText = "";
			GameMouseOverwritten = false;
			masterView.Update();
		}

		public static void DrawMaster(SpriteBatch spriteBatch)
		{
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.None, null, null, Main.UIScaleMatrix);
			masterView.Draw(spriteBatch);
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.None, null, null, Main.UIScaleMatrix);
			//Console.WriteLine("update: " + UpdateCalls + "draws: " + DrawCalls);
		}

		public static void AddChildToMaster(UIView view) => masterView.AddChild(view);

		public static void RemoveChildFromMaster(UIView view) => masterView.RemoveChild(view);

		public class GameScreen : UIView
		{
			public GameScreen() => OverridesMouse = false;

			public override void Update()
			{
				if (!Main.gameMenu && !Main.mapFullscreen)
				{
					Visible = true;
				}
				else
				{
					Visible = false;
				}

				base.Update();
			}

			protected new float Width => Parent.Width;

			protected new float Height => Parent.Height;
		}

		public class MenuScreen : UIView
		{
			public MenuScreen() => OverridesMouse = false;

			public override void Update()
			{
                Visible = Main.gameMenu;
				base.Update();
			}

			protected new float Width => Parent.Width;

			protected new float Height => Parent.Height;
		}

		public class MapScreen : UIView
		{
			public MapScreen() => OverridesMouse = false;

			public override void Update()
			{
                Visible = !Main.gameMenu && Main.mapFullscreen;
				base.Update();
			}

			protected new float Width => Parent.Width;

			protected new float Height => Parent.Height;
		}
	}
}