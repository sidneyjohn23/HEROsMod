using HEROsMod.UIKit;
using HEROsMod.UIKit.UIComponents;
using System;
using System.IO;
using System.Linq;
using Terraria;
using HEROsMod.HEROsModNetwork;

namespace HEROsMod.HEROsModServices
{
	internal class PlayerList : HEROsModService
	{
		public static CurrentPlayersWindow playersWindow;

		public PlayerList()
		{
			MultiplayerOnly = true;
			_name = "Player List";
			_hotbarIcon = new UIImage(HEROsMod.instance.GetTexture("Images/connectedPlayers"));
			_hotbarIcon.OnLeftClick += _hotbarIcon_onLeftClick;
			HotbarIcon.Tooltip = HEROsMod.HeroText("ViewConnectedPlayers");
		}

		private void _hotbarIcon_onLeftClick(object sender, EventArgs e)
		{
			if (playersWindow == null)
			{
				playersWindow = new CurrentPlayersWindow();
				playersWindow.Closed += PlayerWindowClosed;
                AddUIView(playersWindow);
			}
			else
			{
				playersWindow.Close();
			}
		}

		private void PlayerWindowClosed(object sender, EventArgs e) => playersWindow = null;

		public override void MyGroupUpdated()
		{
			// TODO! This prevents snoop, since IsAdmin might not be true.
			HasPermissionToUse = HEROsModNetwork.LoginService.MyGroup.IsAdmin;
			if (!HasPermissionToUse)
			{
				if (playersWindow != null)
				{
					playersWindow.Close();
				}
			}
		}

		public override void Destroy()
		{
			if (playersWindow != null)
			{
				playersWindow.Close();
			}
			base.Destroy();
		}
	}

	internal class CurrentPlayersWindow : UIWindow
	{
		private UIScrollView scrollView = new UIScrollView();
		public bool canOpenPlayerInfo = true;
		private static float spacing = 16f;
		private PlayerInfo playerInfo = null;

		public event EventHandler Closed;

		public CurrentPlayersWindow()
		{
			Width = 300;
			Height = 400;
			CanMove = true;
			Anchor = AnchorPosition.Center;
			CenterToParent();
			UILabel title = new UILabel(HEROsMod.HeroText("ConnectedPlayers"));
			UIImage bClose = new UIImage(closeTexture)
			{
				Anchor = AnchorPosition.TopRight,
				X = Width - spacing,
				Y = spacing
			};
			title.Scale = .6f;
			title.X = spacing;
			title.Y = spacing;
			title.OverridesMouse = false;
			scrollView.Width = Width - spacing * 2;
			scrollView.Height = Height - title.Height - spacing * 2;
			scrollView.X = spacing;
			scrollView.Y = title.Y + title.Height;

			AddChild(scrollView);
			AddChild(title);
			AddChild(bClose);
			bClose.OnLeftClick += BClose_onLeftClick;
			UpdateList();
		}

		private void BClose_onLeftClick(object sender, EventArgs e) => Close();

		public override void Update()
		{
			if (Main.gameMenu)
			{
				Visible = false;
			}

			base.Update();
		}

		public void UpdateList()
		{
			scrollView.ClearContent();
			float yPos = spacing;
			for (int i = 0; i < Main.player.Length; i++)
			{
				Player player = Main.player[i];
				if (player.active)
				{
                    UIPlayerHead playerHead = new UIPlayerHead(player)
                    {
                        X = 8,
                        Y = yPos
                    };
                    yPos += playerHead.Height;
                    UILabel label = new UILabel(player.name)
                    {
                        Scale = .5f,
                        Anchor = AnchorPosition.Left,
                        X = playerHead.X + playerHead.Width + 8,
                        Y = playerHead.Y + playerHead.Width / 2 + 8
                    };
                    label.OnLeftClick += Label_onLeftClick;
					label.Tag = i;

					scrollView.AddChild(playerHead);
					scrollView.AddChild(label);
				}
			}

            if (HEROsModNetwork.LoginService.MyGroup.IsAdmin)
			{
				UILabel lOfflinePlayers = new UILabel(HEROsMod.HeroText("OfflineUsers"))
				{
					Scale = .6f,
					X = Spacing,
					Y = yPos + Spacing,
					ForegroundColor = Microsoft.Xna.Framework.Color.Yellow
				};
				yPos = lOfflinePlayers.Y + lOfflinePlayers.Height;
				scrollView.AddChild(lOfflinePlayers);
                ModUtils.DebugText("Length: " + HEROsModNetwork.Network.Players2.Count);
                ModUtils.DebugText("Registered count: " + HEROsModNetwork.Network.RegisteredUsers.Count);
                foreach (HEROsModNetwork.UserWithID user in HEROsModNetwork.Network.RegisteredUsers)
				{
                    ModUtils.DebugText("? " + user.Username);


                    foreach (HEROsModNetwork.HEROsModPlayer item in HEROsModNetwork.Network.Players2.Values)
                    {
                        ModUtils.DebugText("U2 " + item.Username + ": " + item.Index);
                    }

                    if (HEROsModNetwork.Network.Players2.Values.Any(x => x.Username == user.Username))
					{
						ModUtils.DebugText("Continue on " + user.Username);
						continue;
					}
                    UILabel lUser = new UILabel(user.Username)
                    {
                        Scale = .5f,
                        X = 40 + Spacing * 2,
                        Y = yPos,
                        ForegroundColor = new Microsoft.Xna.Framework.Color(200, 200, 200)
                    };
                    yPos += lUser.Height;
					lUser.OnLeftClick += LUser_onLeftClick;
					lUser.Tag = user.ID;
					scrollView.AddChild(lUser);
				}
			}

			scrollView.ContentHeight = yPos + spacing;
        }

		private void LUser_onLeftClick(object sender, EventArgs e)
		{
			UILabel label = (UILabel)sender;
			int userID = (int)label.Tag;

			OpenPlayerInfo(userID, true);
		}

		private void Label_onLeftClick(object sender, EventArgs e)
		{
			UILabel label = (UILabel)sender;
			int playerIndex = (int)label.Tag;
			if (HEROsModNetwork.LoginService.MyGroup.IsAdmin)
			{
				HEROsModNetwork.LoginService.RequestPlayerInfo(playerIndex);
			}
			else
			{
				OpenPlayerInfo(playerIndex, false);
			}
		}

		public void OpenPlayerInfo(int indexOfRequestedPlayer, bool offlinePlayer)
		{
			ClosePlayerInfo();
			playerInfo = new PlayerInfo(indexOfRequestedPlayer, offlinePlayer);
			AddChild(playerInfo);
		}

		public void ClosePlayerInfo()
		{
			if (playerInfo != null)
			{
				playerInfo.Parent.RemoveChild(playerInfo);
				playerInfo = null;
			}
		}

		public void Close()
		{
			ClosePlayerInfo();
			if (Parent != null)
			{
				Parent.RemoveChild(this);
			}

			Closed?.Invoke(this, EventArgs.Empty);
        }
	}

	internal class PlayerInfo : UIWindow
	{
		private static float spacing = 8f;

		//UIButton bBan;
		//UIButton bKick;
		private UIDropdown dropdown;

		private HEROsModNetwork.HEROsModPlayer player;
		private readonly int playerIndex;

		public PlayerInfo(int playerIndex, bool offlineUser)
		{
			if (!offlineUser)
			{
				player = HEROsModNetwork.Network.Players2[playerIndex];
			}

			this.playerIndex = playerIndex;
            UpdateWhenOutOfBounds = true;
			Width = 350;
			UIImage bClose = new UIImage(closeTexture);
			UILabel lGroup = new UILabel("Group:");
			UIButton bBan = new UIButton("Ban");
			UIButton bKick = new UIButton("Kick");
			UILabel label = new UILabel();
			SnoopWindow snoopWindow = new SnoopWindow();
			snoopWindow.SetPlayer(Main.player[0], 0);
			dropdown = new UIDropdown();
			UIButton bTeleport = new UIButton("Teleport To");
			UIButton bRestore = new UIButton("Restore Changes Made by this Player");
			bTeleport.AutoSize = false;
			bTeleport.Width = 150;
			bRestore.AutoSize = false;

			// if logged in
			if (player != null && player.Username.Length > 0)
			{
				dropdown.AddItem(HEROsModNetwork.Network.AdminGroup.Name);
				for (int i = 0; i < HEROsModNetwork.Network.Groups.Count; i++)
				{
					dropdown.AddItem(HEROsModNetwork.Network.Groups[i].Name);
					if (player.Group.Name == HEROsModNetwork.Network.Groups[i].Name)
					{
						dropdown.SelectedItem = i + 1;
					}
				}
			}
			else if (player == null)
			{
				HEROsModNetwork.UserWithID user = HEROsModNetwork.Network.RegisteredUsers[playerIndex];
				dropdown.AddItem(HEROsModNetwork.Network.AdminGroup.Name);
				for (int i = 0; i < HEROsModNetwork.Network.Groups.Count; i++)
				{
					dropdown.AddItem(HEROsModNetwork.Network.Groups[i].Name);

					if (user.groupID == HEROsModNetwork.Network.Groups[i].ID)
					{
						dropdown.SelectedItem = i + 1;
					}
				}
			}
			dropdown.SelectedChanged += Dropdown_selectedChanged;

			bClose.Y = spacing;
			lGroup.Scale = .5f;
			lGroup.X = spacing;
			lGroup.Y = spacing;
			dropdown.X = lGroup.X + lGroup.Width + 4;
			dropdown.Y = lGroup.Y;
			dropdown.Width = 200;
			dropdown.UpdateWhenOutOfBounds = true;
			if (player != null && player.Username.Length > 0)
			{
				label.Text = "Logged in as " + player.Username;
			}
			else
			{
				label.Text = "Not Logged In";
			}

			label.X = spacing;
			label.Y = dropdown.Y + dropdown.Height + spacing;
			label.Scale = .35f;
			bBan.X = label.X;
			bBan.Y = label.Y + label.Height + spacing;
			bKick.X = bBan.X + bBan.Width + spacing;
			bKick.Y = bBan.Y;

			bTeleport.X = Width - bTeleport.Width - spacing;
			bTeleport.Y = bBan.Y;

			bRestore.X = Spacing;
			bRestore.Y = bTeleport.Y + bTeleport.Height + spacing;

			snoopWindow.X = bRestore.X;
			snoopWindow.Y = bRestore.Y + bRestore.Height + spacing;

			Width = bTeleport.X + bTeleport.Width + spacing;
			Height = bRestore.Y + bRestore.Height + spacing;

			bRestore.Width = Width - spacing * 2;

			AddChild(bClose);
			AddChild(label);
			HEROsModNetwork.Group myGroup = HEROsModNetwork.LoginService.MyGroup;
			if (!offlineUser)
			{
				if (myGroup.HasPermission("TeleportToPlayers"))
				{
					AddChild(bTeleport);
				}

				if (myGroup.HasPermission("Ban"))
				{
					AddChild(bBan);
				}

				if (myGroup.HasPermission("Kick"))
				{
					AddChild(bKick);
				}

				if (myGroup.HasPermission("Snoop"))
				{
					snoopWindow.SetPlayer(Main.player[playerIndex], playerIndex);
					AddChild(snoopWindow);
					Width = snoopWindow.X + snoopWindow.Width + spacing * 2;
					Height = snoopWindow.Y + snoopWindow.Height + spacing * 2;
				}
			}
			if (myGroup.IsAdmin)
			{
				AddChild(lGroup);
				AddChild(dropdown);
				if (offlineUser || player != null && player.Username.Length > 0)
				{
					//AddChild(bRestore);
				}
			}
			bBan.Tag = Main.player[playerIndex].name;
			bKick.Tag = Main.player[playerIndex].name;

			bClose.X = Width - bClose.Width - spacing;
			bKick.OnLeftClick += BKick_onLeftClick;
			bBan.OnLeftClick += BBan_onLeftClick;
			bClose.OnLeftClick += BClose_onLeftClick;
			bTeleport.OnLeftClick += BTeleport_onLeftClick;
			bRestore.OnLeftClick += BRestore_onLeftClick;
		}

		private void BRestore_onLeftClick(object sender, EventArgs e)
		{
			if (player != null)
			{
				GeneralMessages.RequestRestoreTiles(playerIndex, true);
			}
			else
			{
				GeneralMessages.RequestRestoreTiles(playerIndex, false);
			}
		}

		private void BBan_onLeftClick(object sender, EventArgs e)
		{
			//ServerTools.SendTextToServer(Messages.banPlayer + name);
			UIButton button = (UIButton)sender;
			string tag = (string)button.Tag;
			ModUtils.DebugText("Ban tag " + tag);

			GeneralMessages.RequestBanPlayer(tag);
		}

		private void BTeleport_onLeftClick(object sender, EventArgs e)
		{
			if (HEROsModNetwork.LoginService.MyGroup.HasPermission("TeleportToPlayers"))
			{
				Main.player[Main.myPlayer].Teleport(Main.player[playerIndex].position);
			}
		}

		private void BClose_onLeftClick(object sender, EventArgs e) => Parent.RemoveChild(this);

		private void Dropdown_selectedChanged(object sender, EventArgs e)
		{
			if (player == null)
			{
				HEROsModNetwork.Group playersNewGroup = HEROsModNetwork.Network.GetGroupByName(dropdown.GetItem(dropdown.SelectedItem));
				HEROsModNetwork.LoginService.RequestSetOfflinePlayerGroup(playerIndex, playersNewGroup);
			}
			else
			{
				//send new group to server
				HEROsModNetwork.Group playersNewGroup = HEROsModNetwork.Network.GetGroupByName(dropdown.GetItem(dropdown.SelectedItem));
				HEROsModNetwork.LoginService.RequestSetPlayerGroup(playerIndex, playersNewGroup);
			}
		}

		private void BKick_onLeftClick(object sender, EventArgs e)
		{
			UIButton button = (UIButton)sender;
			string tag = (string)button.Tag;
			ModUtils.DebugText("Kick tag " + tag);
			GeneralMessages.RequestKickPlayer(tag);
			//ServerTools.SendTextToServer(Messages.kickPlayer + tag);
			//TODO
		}

		public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
		{
            X = Parent.Width;
			base.Draw(spriteBatch);
		}
	}

	internal class SnoopWindow : UIView
	{
		//static float spacing = 16f;
		private Player player;
        private int playerIndex;

		private UIWindow itemsView;

        public SnoopWindow()
		{
			itemsView = new UIWindow();
			//itemsView.X = spacing;
			//itemsView.Y = spacing;
			for (int i = 0; i < 50; i++)
			{
				Slot slot = new Slot(0, false)
				{
					functionalSlot = true
				};
				int index = i;
				slot.ItemChanged += (a, b) => ItemSlot_ItemChanged(slot, index);
				slot.X = 8 + i % 10 * slot.Width;
				slot.Y = 8 + i / 10 * slot.Height;

				itemsView.AddChild(slot);
			}
			Width = itemsView.Children.Last().X + itemsView.Children.Last().Width + 8;
			float yPos = itemsView.Children.Last().Y + itemsView.Children.Last().Height + 10;
			for (int i = 50; i < 58; i++)
			{
				int index = i - 50;
				Slot slot = new Slot(0, false)
				{
					functionalSlot = true
				};
				int idx = i;
				slot.ItemChanged += (a, b) => ItemSlot_ItemChanged(slot, idx);
				slot.Scale = .6f;
				slot.X = 8 + index % 2 * slot.Width;
				slot.Y = yPos + index / 2 * slot.Height;
				itemsView.AddChild(slot);
			}
			Slot mouseSlot = new Slot(0, false)
			{
				functionalSlot = true
			};
			mouseSlot.ItemChanged += (a, b) => ItemSlot_ItemChanged(mouseSlot, 58);
			mouseSlot.X = itemsView.Children.Last().X + itemsView.Children.Last().Width + 4;
			mouseSlot.Y = itemsView.Children.Last().Y + itemsView.Children.Last().Height - mouseSlot.Height;
			itemsView.AddChild(mouseSlot);
			float xPos = mouseSlot.X + mouseSlot.Width + 4;
			for (int i = 0; i < 16; i++)
			{
				Slot slot = new Slot(0, false)
				{
					functionalSlot = true
				};
				int index = i;
				slot.ItemChanged += (a, b) => ItemSlot_ItemChanged(slot, index, true);
				slot.Scale = .7f;
				slot.X = xPos + i % 8 * slot.Width;
				slot.Y = yPos + i / 8 * slot.Height;
				itemsView.AddChild(slot);
			}
			AddChild(itemsView);

			Height = mouseSlot.Y + mouseSlot.Height + 8;

			itemsView.Width = Width;
			itemsView.Height = Height;

            UILabel label = new UILabel("Mouse Item")
            {
                Scale = .5f,
                Anchor = AnchorPosition.Left,
                Y = mouseSlot.Y + mouseSlot.Height / 2 + 4,
                X = mouseSlot.X + mouseSlot.Width + 4
            };
            itemsView.AddChild(label);

            itemsView.Children[18].OnLeftClick += A;
		}

        private void A(object sender, EventArgs e) {
            player.inventory[18] = player.inventory[19].Clone();
            int plr = playerIndex;
            //for (int i = 0; i < 59; i++) {
                NetMessage.SendData(5, -1, -1, null, playerIndex, 18);
                //NetMessage.SendData(5, playerIndex, Main.myPlayer, null, plr, i, Main.player[plr].inventory[i].prefix, 0f, 0, 0, 0);
            //}


        }

		private void ItemSlot_ItemChanged(Slot slot, int index, bool armor = false)
		{
			Main.playerInventory = true;
			RequestSyncItemNonOwner(player, slot.item, index + (armor ? 59 : 0));
			//ErrorLogger.Log("Slot " + slot.item.type);
		}

		public override void Update()
		{
			if (player != null)
			{
				for (int i = 0; i < 59; i++)
				{
					Slot slot = (Slot)itemsView.Children[i];
					slot.item = player.inventory[i].Clone();
				}
				for (int i = 0; i < 16; i++)
				{
					Slot slot = (Slot)itemsView.Children[i + 59];
					slot.item = player.armor[i].Clone();
				}
			}
			base.Update();
		}

		public void SetPlayer(Player player, int playerIndex)
		{
			this.player = player;
            this.playerIndex = playerIndex;
		}

		internal static void RequestSyncItemNonOwner(Player player, Item item, int index)
		{
			Terraria.ModLoader.ModPacket packet = HEROsMod.instance.GetPacket();
			packet.Write((byte)Network.MessageType.GeneralMessage);
			packet.Write((byte)GeneralMessages.MessageType.SyncItemNonOwner);
			packet.Write((byte)player.whoAmI);
			packet.Write((byte)index);
			Terraria.ModLoader.IO.ItemIO.Send(item, packet, true);
			packet.Send();
		}

		internal static void ProcessSyncItemNonOwner(ref BinaryReader reader, int playerNumber)
		{
			if (Main.netMode == Terraria.ID.NetmodeID.Server)
			{
				if (Network.Players2[playerNumber].Group.HasPermission("Snoop"))
				{
					byte player = reader.ReadByte();
					int inventoryindex = reader.ReadByte();
					Item item = Terraria.ModLoader.IO.ItemIO.Receive(reader, true);

					Terraria.ModLoader.ModPacket packet = HEROsMod.instance.GetPacket();
					packet.Write((byte)Network.MessageType.GeneralMessage);
					packet.Write((byte)GeneralMessages.MessageType.SyncItemNonOwner);
					packet.Write((byte)player);
					packet.Write((byte)inventoryindex);
					Terraria.ModLoader.IO.ItemIO.Send(item, packet, true);
					packet.Send(player);
				}
			}
			else
			{
				byte player = reader.ReadByte();
				int inventoryindex = reader.ReadByte();
				Item item = Terraria.ModLoader.IO.ItemIO.Receive(reader, true);

				if (player == Main.myPlayer)
				{
					if (inventoryindex < 59)
					{
						Main.LocalPlayer.inventory[inventoryindex] = item;
					}
					else
					{
						Main.LocalPlayer.armor[inventoryindex - 59] = item;
					}
					// send 5 or just let clientClone take care of it?
				}
				else
				{
					// Bug
				}
			}
		}
	}
}