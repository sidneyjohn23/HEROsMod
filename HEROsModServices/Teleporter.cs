using Microsoft.Xna.Framework;
using ReLogic.Graphics;
using Terraria;

namespace HEROsMod.HEROsModServices
{
	internal class Teleporter : HEROsModService
	{
		// TODO, is this how I want to do this?
		public static Teleporter instance;

		public Teleporter()
		{
			instance = this;
		}

		public override void MyGroupUpdated()
		{
            HasPermissionToUse = HEROsModNetwork.LoginService.MyGroup.HasPermission("Teleport");
			//base.MyGroupUpdated();
		}

		public void PostDrawFullScreenMap()
		{
			if (HasPermissionToUse)
			{
				Main.spriteBatch.DrawString(Main.fontMouseText, "Right Click to teleport", new Vector2(15, Main.screenHeight - 80), Color.White);

				if (Main.mouseRight && Main.keyState.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.LeftControl))
				{
					int mapWidth = Main.maxTilesX * 16;
					int mapHeight = Main.maxTilesY * 16;
					Vector2 cursorPosition = new Vector2(Main.mouseX, Main.mouseY);

					cursorPosition.X -= Main.screenWidth / 2;
					cursorPosition.Y -= Main.screenHeight / 2;

					Vector2 mapPosition = Main.mapFullscreenPos;
					Vector2 cursorWorldPosition = mapPosition;

					cursorPosition /= 16;
					cursorPosition *= 16 / Main.mapFullscreenScale;
					cursorWorldPosition += cursorPosition;
					cursorWorldPosition *= 16;

					Player player = Main.player[Main.myPlayer];
					cursorWorldPosition.Y -= player.height;
					if (cursorWorldPosition.X < 0) cursorWorldPosition.X = 0;
					else if (cursorWorldPosition.X + player.width > mapWidth) cursorWorldPosition.X = mapWidth - player.width;
					if (cursorWorldPosition.Y < 0) cursorWorldPosition.Y = 0;
					else if (cursorWorldPosition.Y + player.height > mapHeight) cursorWorldPosition.Y = mapHeight - player.height;

					if (Main.netMode == 0) // single
					{
						player.Teleport(cursorWorldPosition, 1, 0);
						player.position = cursorWorldPosition;
						player.velocity = Vector2.Zero;
						player.fallStart = (int)(player.position.Y / 16f);
					}
					else // 1, client
					{
						HEROsModNetwork.GeneralMessages.RequestTeleport(cursorWorldPosition);
					}
				}
			}
		}
	}
}