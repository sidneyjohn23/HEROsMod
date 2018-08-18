using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace HEROsMod.HEROsModNetwork {
    public class Network {
        public static int NetworkVersion = 2;
        public static bool ServerUsingHEROsMod { get; set; }
		public static NetworkMode NetworkMode => ModUtils.NetworkMode;
		public static bool LoggedIn { get; set; }

        public static List<Group> Groups { get; set; }
        public static Dictionary<int, HEROsModPlayer> Players2 = new Dictionary<int, HEROsModPlayer>();
        public static List<Region> Regions { get; set; }
        public static List<UserWithID> RegisteredUsers { get; set; }
        public static bool GravestonesAllowed { get; set; }
        public static bool WillFreezeNonLoggedIn { get; set; }
        public static int[,] TileLastChangedBy { get; set; }
        public static HEROsModPlayer LastTileKilledBy { get; set; }
        public static MemoryStream memoryStream;
        public static BinaryWriter writer;
        public static Group DefaultGroup;
        public static Group AdminGroup;

        public static int AuthCode;

        private static Color[] ChatColor => new Color[]{
            Color.LightBlue,
            Color.LightCoral,
            Color.LightCyan,
            Color.LightGoldenrodYellow,
            Color.LightGray,
            Color.LightPink,
            Color.LightSkyBlue,
            Color.LightYellow
        };

        private static readonly int chatColorIndex = 0;

        private static readonly float authMessageTimer = 0f;
        private static float freezeTimer = 0f;
        private static float sendTimeTimer = 1f;

        public const string HEROsModCheckMessage = "-Install HEROs Mod For Advanced Features.  Type /login to login.  Type /register to register an account.";

        public static void InitializeWorld() {
            if (NetworkMode == NetworkMode.Server) {
                TileLastChangedBy = new int[Main.maxTilesX, Main.maxTilesY];
                for (int x = 0; x < TileLastChangedBy.GetLength(0); x++) {
                    for (int y = 0; y < TileLastChangedBy.GetLength(1); y++) {
                        TileLastChangedBy[x, y] = -1;
                    }
                }

                TileChangeController.Init();
                Groups = DatabaseController.GetGroups();
                Regions = DatabaseController.GetRegions();
            }
        }

        // On Load Mod
        public static void Init() {
            // Reset Values to defaults.
            Group.PermissionList.Clear();
            foreach (PermissionInfo item in Group.DefaultPermissions) {
                Group.PermissionList.Add(item);
            }

            ServerUsingHEROsMod = false;
            GravestonesAllowed = true;
            WillFreezeNonLoggedIn = true;
            Groups = new List<Group>();
            RegisteredUsers = new List<UserWithID>();
            Regions = new List<Region>();
            ResetWriter();
            LoggedIn = false;

            AdminGroup = new Group("Admin");
            AdminGroup.MakeAdmin();

            DatabaseController.Init();

            if (NetworkMode == NetworkMode.Server) {
                Groups = DatabaseController.GetGroups();

                foreach (Group group in Groups) {
                    if (group.Name == "Default") {
                        DefaultGroup = group;
                        break;
                    }
                }
                LoginService.GroupChanged += LoginService_GroupChanged;

                AuthCode = Main.rand.Next(100000, 999999);
                Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine(string.Format(HEROsMod.HeroText("DedicatedServerAutoMessage"), AuthCode));
				Console.ResetColor();

                RegisteredUsers.AddRange(DatabaseController.GetRegisteredUsers());
            }
        }

        public static void Update() {
            if (NetworkMode == NetworkMode.Server) {
                ModUtils.SetDeltaTime();
                //ErrorLogger.Log("Network.Update");
                //Console.WriteLine("Network.Update");
                freezeTimer -= ModUtils.DeltaTime;
                if (freezeTimer <= 0) {
                    freezeTimer = 1f;

                    FreezeNonLoggedInPlayers();
                }

                if (HEROsModServices.TimeWeatherChanger.TimePaused) {
                    Main.time = HEROsModServices.TimeWeatherChanger.PausedTime;
                    if (sendTimeTimer > 0) {
                        sendTimeTimer -= ModUtils.DeltaTime;
                        if (sendTimeTimer <= 0) {
                            sendTimeTimer = 1f;
                            NetMessage.SendData(7, -1, -1, null, 0, 0f, 0f, 0f, 0);
                        }
                    }
                }
            }
        }

		private static void LoginService_GroupChanged(object sender, EventArgs e) =>
			//Send group list to all HEROsMod users
			LoginService.SendGroupList(-2);

		public static void ResetWriter() {
            if (memoryStream != null) {
                memoryStream.Close();
            }
            memoryStream = new MemoryStream();
            writer = new BinaryWriter(memoryStream);
        }

        public static bool PlayerHasPermissionToBuildAtBlock(HEROsModPlayer player, int x, int y) {
            bool canBuild = false;

            if (player.Group.IsAdmin) {
                canBuild = true;
            }

            if (!canBuild && player.Group.HasPermission("ModifyTerrain")) {
                canBuild = true;
                for (int i = 0; i < Regions.Count; i++) {
                    //if region contains tile
                    if (Regions[i].ContainsTile(x, y)) {
                        bool canBuildInRegion = false;
                        for (int j = 0; j < Regions[i].AllowedGroupsIDs.Count; j++) {
                            if (player.Group.ID == Regions[i].AllowedGroupsIDs[j]) {
                                //can build in region
                                canBuildInRegion = true;
                                break;
                            }
                        }
                        // if can't build in region chack if player can build in the region
                        if (!canBuildInRegion) {
                            for (int j = 0; j < Regions[i].AllowedPlayersIDs.Count; j++) {
                                if (player.ID == Regions[i].AllowedPlayersIDs[j]) {
                                    canBuildInRegion = true;
                                    break;
                                }
                            }
                        }
                        canBuild = canBuildInRegion;
                        if (!canBuild)
						{
							break;
						}
					}
                }
            }
            return canBuild;
        }

        // TODO -- How will any of these work....?
        public static bool CheckIncomingDataForHEROsModMessage(ref byte msgType, ref BinaryReader binaryReader, int playerNumber) {
            long readerPos = binaryReader.BaseStream.Position;

            switch (msgType) {
                case 12:
                    if (NetworkMode == NetworkMode.Server) {
                        ModUtils.DebugText("State: " + Netplay.Clients[playerNumber].State);
                        if (Netplay.Clients[playerNumber].State == 3) {
                            PlayerJoined(playerNumber);
                        }
                    }
                    break;
                case 17: //Terrain Modified
                    if (NetworkMode == NetworkMode.Server) {
                        bool canBuild = false;
                        TileModifyType tileModifyType = (TileModifyType) binaryReader.ReadByte();
                        int x = binaryReader.ReadInt16();
                        int y = binaryReader.ReadInt16();
                        short placeType = binaryReader.ReadInt16();
                        int style = binaryReader.ReadByte();
                        bool fail = placeType == 1;
                        HEROsModPlayer player = Players2[playerNumber];

                        Tile tile;
                        if (x >= 0 && y >= 0 && x < Main.maxTilesX && y < Main.maxTilesY) {
                            tile = Main.tile[x, y];
                        } else {
                            binaryReader.BaseStream.Position = readerPos;
                            return false;
                        }
                        if (!canBuild) {
                            canBuild = PlayerHasPermissionToBuildAtBlock(player, x, y);
                        }

                        if (tileModifyType == TileModifyType.PlaceTile && placeType == TileID.LandMine) {
                            SendTextToPlayer("Landmines are disabled on this server", playerNumber, Color.Red);
                        } else if (canBuild) {
                            TileLastChangedBy[x, y] = player.ID;
                            binaryReader.BaseStream.Position = readerPos;
                            if (tileModifyType == TileModifyType.KillTile) {
                                LastTileKilledBy = player;
                                WorldGen.KillTile(x, y, fail, false, false);
                                NetMessage.SendData(17, -1, playerNumber, null, (int) tileModifyType, x, y, placeType, style);
                                LastTileKilledBy = null;
                                return true;
                            } else {
                                TileChangeController.RecordChanges(player, x, y);
                            }
                            return false;
                        } else {
							SendTextToPlayer(HEROsMod.HeroText("YouDoNotHavePermissionToBuildHere"), playerNumber, Color.Red);
						}

						switch (tileModifyType) {
                            case TileModifyType.KillTile:
                                NetMessage.SendData(17, playerNumber, -1, null, (int) TileModifyType.PlaceTile, x, y, tile.type, tile.slope());
                                break;

                            case TileModifyType.PlaceTile:
                                NetMessage.SendData(17, playerNumber, -1, null, (int) TileModifyType.KillTile, x, y, placeType, style);
                                break;

                            case TileModifyType.KillWall:
                                NetMessage.SendData(17, playerNumber, -1, null, (int) TileModifyType.PlaceWall, x, y, tile.wall, style);
                                break;

                            case TileModifyType.PlaceWall:
                                NetMessage.SendData(17, playerNumber, -1, null, (int) TileModifyType.KillWall, x, y, placeType, style);
                                break;

                            case TileModifyType.KillTileNoItem:
                                NetMessage.SendData(17, playerNumber, -1, null, (int) TileModifyType.PlaceTile, x, y, tile.type, tile.slope());
                                break;

                            case TileModifyType.PlaceWire:
                                NetMessage.SendData(17, playerNumber, -1, null, (int) TileModifyType.KillWire, x, y, placeType, style);
                                break;

                            case TileModifyType.PlaceWire2:
                                NetMessage.SendData(17, playerNumber, -1, null, (int) TileModifyType.KillWire2, x, y, placeType, style);
                                break;

                            case TileModifyType.PlaceWire3:
                                NetMessage.SendData(17, playerNumber, -1, null, (int) TileModifyType.KillWire3, x, y, placeType, style);
                                break;

                            case TileModifyType.KillWire:
                                NetMessage.SendData(17, playerNumber, -1, null, (int) TileModifyType.PlaceWire, x, y, placeType, style);
                                break;

                            case TileModifyType.KillWire2:
                                NetMessage.SendData(17, playerNumber, -1, null, (int) TileModifyType.PlaceWire2, x, y, placeType, style);
                                break;

                            case TileModifyType.KillWire3:
                                NetMessage.SendData(17, playerNumber, -1, null, (int) TileModifyType.PlaceWire3, x, y, placeType, style);
                                break;

                            case TileModifyType.KillActuator:
                                NetMessage.SendData(17, playerNumber, -1, null, (int) TileModifyType.PlaceActuator, x, y, placeType, style);
                                break;

                            case TileModifyType.PlaceActuator:
                                NetMessage.SendData(17, playerNumber, -1, null, (int) TileModifyType.KillActuator, x, y, placeType, style);
                                break;

                            case TileModifyType.PoundTile:
                                NetMessage.SendData(17, playerNumber, -1, null, (int) TileModifyType.PoundTile, x, y, placeType, tile.slope());
                                break;

                            case TileModifyType.SlopeTile:
                                NetMessage.SendData(17, playerNumber, -1, null, (int) TileModifyType.SlopeTile, x, y, placeType, tile.slope());
                                break;
                        }
                        return true;
                    }
                    break;

                case 63: //block painted
                    if (NetworkMode == NetworkMode.Server) {
                        int x = binaryReader.ReadInt16();
                        int y = binaryReader.ReadInt16();
                        byte paintColor = binaryReader.ReadByte();
                        HEROsModPlayer player = Players2[playerNumber];

						if (PlayerHasPermissionToBuildAtBlock(player, x, y))
						{
							TileLastChangedBy[x, y] = player.ID;
							binaryReader.BaseStream.Position = readerPos;
							return false;
						}
						else
						{
							NetMessage.SendData(63, playerNumber, -1, null, x, (float)y, (float)Main.tile[x, y].color());
							SendTextToPlayer(HEROsMod.HeroText("YouDoNotHavePermissionToBuildHere"), playerNumber, Color.Red);
							return true;
						}
					}
					break;

                case 64: //wall painted
                    if (NetworkMode == NetworkMode.Server) {
                        int x = binaryReader.ReadInt16();
                        int y = binaryReader.ReadInt16();
                        byte paintColor = binaryReader.ReadByte();
                        HEROsModPlayer player = Players2[playerNumber];

						if (PlayerHasPermissionToBuildAtBlock(player, x, y))
						{
							TileLastChangedBy[x, y] = player.ID;
							binaryReader.BaseStream.Position = readerPos;
							return false;
						}
						else
						{
							NetMessage.SendData(64, playerNumber, -1, null, x, (float)y, (float)Main.tile[x, y].wallColor());
							SendTextToPlayer(HEROsMod.HeroText("YouDoNotHavePermissionToBuildHere"), playerNumber, Color.Red);
							return true;
						}
					}
					break;
			}

            //we need to set the stream position back to where it was before we got it
            binaryReader.BaseStream.Position = readerPos;
            return false;
        }

        public static void HEROsModMessaged(BinaryReader binaryReader, int playerNumber) {
            //We found a HEROsMod only message
            MessageType subMsgType = (MessageType) binaryReader.ReadByte();
            switch (subMsgType) {
                case MessageType.GeneralMessage:
                    GeneralMessages.ProcessData(ref binaryReader, playerNumber);
                    break;
                case MessageType.LoginMessage:
                    LoginService.ProcessData(ref binaryReader, playerNumber);
                    break;
            }
        }

        private static void PlayerJoined(int playerNumber) {
            ModUtils.DebugText("New player");
            ModUtils.DebugText("Players2 Count: " + Players2.Count);
            HEROsModPlayer user = new HEROsModPlayer(playerNumber);
            ModUtils.DebugText("Add player");
            Players2.Add(playerNumber, user);
            // chat message hack: SendTextToPlayer(HEROsModCheckMessage, playerNumber, Color.Red);
            ModUtils.DebugText("Get Packet");
			Terraria.ModLoader.ModPacket packet = HEROsMod.instance.GetPacket();
            ModUtils.DebugText("Write data");
            packet.Write((byte) MessageType.LoginMessage);
            packet.Write((byte) LoginService.MessageType.ServerToClientHandshake);
            ModUtils.DebugText("Send Server to Client Handshake");
            packet.Send(playerNumber);
        }

		public static void ProcessClientsUsingHEROsMod(int playerNumber) => GeneralMessages.TellClientsPlayerJoined(playerNumber);


		public static void OnDisconnect() {
            StreamWriter file = new StreamWriter("G:/terraria-chat.txt");
			Terraria.UI.Chat.ChatLine[] chatLines = Main.chatLine;
            for (int i = 0; i < Main.numChatLines; i++) {
                if (chatLines[i] != null) {
                    file.WriteLine(chatLines[i].parsedText[0].Text);
                }
            }
            file.Flush();
            file.Close();
        }


        public static void PlayerLeft(int playerIndex) {
            Players2.Remove(playerIndex);
            GeneralMessages.TellClientsPlayerLeft(playerIndex);
        }

        private static void FreezeNonLoggedInPlayers() {
            foreach (HEROsModPlayer player in Players2.Values) {
                if (player.ServerInstance.IsActive) {
                    if (player.Username == string.Empty) {
                        //player.GameInstance.AddBuff(47, 7200);
                        //	Console.WriteLine("Freeze " + i);
                        NetMessage.SendData(55, player.Index, -1, null, player.Index, 47, 120, 0f, 0);
                    }
                }
            }
        }

        public static void SendPlayerToPosition(HEROsModPlayer player, Vector2 position) {
            position /= 16;
            int prevSpawnX = player.GameInstance.SpawnX;
            int prevSpawnY = player.GameInstance.SpawnY;
            player.GameInstance.SpawnX = (int) position.X;
            player.GameInstance.SpawnY = (int) position.Y;
            NetMessage.SendData(12, -1, -1, null, player.Index, 0f, 0f, 0f, 0);
            player.GameInstance.SpawnX = prevSpawnX;
            player.GameInstance.SpawnY = prevSpawnY;
        }

        public static void SendTextToPlayer(string msg, int playerIndex, Color? color = null) {
            Color c = color.GetValueOrDefault(Color.White);
            //NetMessage.SendData(25, playerIndex, -1, msg, 255, c.R, c.G, c.B, 0);
            NetMessage.SendChatMessageToClient(NetworkText.FromLiteral(msg), c, playerIndex);
        }

        public static void SendTextToAllPlayers(string msg, Color? color = null) {
            Color c = color.GetValueOrDefault(Color.White);
            //NetMessage.SendData(25, -1, -1, msg, 255, c.R, c.G, c.B, 0);
            NetMessage.BroadcastChatMessage(NetworkText.FromLiteral(msg), c, -1);
        }

        public static void SendDataToServer() {
            ModUtils.DebugText("SendDataToServer " + memoryStream.ToArray());
			Terraria.ModLoader.ModPacket a = HEROsMod.instance.GetPacket();
            a.Write(memoryStream.ToArray());
            a.Send();
            ResetWriter();
            //NetMessage.SendData(HEROsModNetworkMessageType);
        }

        public static void SendDataToPlayer(int playerNumber) {
            if (playerNumber == -2) {
                SendDataToAllHEROsModUsers();
            } else {
				Terraria.ModLoader.ModPacket a = HEROsMod.instance.GetPacket();
                a.Write(memoryStream.ToArray());
                a.Send(playerNumber);
                ResetWriter();

                //NetMessage.SendData(HEROsModNetworkMessageType, playerNumber)
            }
        }

        public static void SendDataToAllHEROsModUsers() {
            foreach (KeyValuePair<int, HEROsModPlayer> user in Players2) {
                byte[] bytes = memoryStream.ToArray();
                if (user.Value != null && user.Value.UsingHEROsMod) {
					Terraria.ModLoader.ModPacket a = HEROsMod.instance.GetPacket();
                    a.Write(memoryStream.ToArray());
                    a.Send(user.Key);
                    ResetWriter();
                    //NetMessage.SendData(HEROsModNetworkMessageType, i);
                    writer.Write(bytes);
                }
            }
            ResetWriter();
        }

        public static Group GetGroupByID(int id) {
            if (id == -1)
			{
				return AdminGroup;
			}

			for (int i = 0; i < Groups.Count; i++) {
                if (Groups[i].ID == id)
				{
					return Groups[i];
				}
			}
            return null;
        }

        public static Group GetGroupByName(string name) {
            if (name == AdminGroup.Name)
			{
				return AdminGroup;
			}

			for (int i = 0; i < Groups.Count; i++) {
                if (Groups[i].Name == name)
				{
					return Groups[i];
				}
			}
            return null;
        }

        public static Region GetRegionByID(int id) {
            for (int i = 0; i < Regions.Count; i++) {
                if (Regions[i].ID == id)
				{
					return Regions[i];
				}
			}
            return null;
        }

        public static void ClearGroundItems() {
            for (int i = 0; i < Main.item.Length; i++) {
                if (Main.item[i].active) {
                    Main.item[i].SetDefaults(0);
                    NetMessage.SendData(21, -1, -1, null, i, 0f, 0f, 0f, 0);
                }
            }
        }

        public static void SpawnNPC(int type, Vector2 position) {
            bool npcFound = false;
            for (int i = 0; i < Main.npc.Length; i++) {
                NPC n = Main.npc[i];
                if (n.type == type) {
                    n.position = position;
                    npcFound = true;
                    if (Main.netMode == 2)
					{
						NetMessage.SendData(23, -1, -1, null, i, 0f, 0f, 0f, 0);
					}

					break;
                }
            }
            if (!npcFound)
			{
				NPC.NewNPC((int) position.X, (int) position.Y, type);
			}
		}

		public static void ResetAllPlayers() => Players2.Clear();

		public static void ResendPlayerTileData(HEROsModPlayer player) {
            int sectionX = Netplay.GetSectionX((int) (player.GameInstance.position.X / 16f));
            int sectionY = Netplay.GetSectionY((int) (player.GameInstance.position.Y / 16f));

            int num = 0;
            for (int i = sectionX - 1; i < sectionX + 2; i++) {
                for (int j = sectionY - 1; j < sectionY + 2; j++) {
                    if (i >= 0 && i < Main.maxSectionsX && j >= 0 && j < Main.maxSectionsY) {
                        num++;
                    }
                }
            }
            int num2 = num;
            NetMessage.SendData(9, player.Index, -1, Language.GetText("LegacyInterface.44").ToNetworkText(), num2, 0f, 0f, 0f, 0);
            Netplay.Clients[player.Index].StatusText2 = "is receiving tile data";
            Netplay.Clients[player.Index].StatusMax += num2;
            for (int k = sectionX - 1; k < sectionX + 2; k++) {
                for (int l = sectionY - 1; l < sectionY + 2; l++) {
                    if (k >= 0 && k < Main.maxSectionsX && l >= 0 && l < Main.maxSectionsY) {
                        NetMessage.SendSection(player.Index, k, l, false);
                        NetMessage.SendData(11, player.Index, -1, null, k, l, k, l, 0);
                    }
                }
            }
        }

        public static void ResendPlayerAllTileData(HEROsModPlayer player) {
            SendTextToPlayer("Getting the complete tile data, please wait...", player.Index, Color.Red);
            NetMessage.SendData(9, player.Index, -1, Language.GetText("LegacyInterface.44").ToNetworkText(), 0, 0f, 0f, 0f, 0);
            Netplay.Clients[player.Index].StatusText2 = "is receiving tile data";
            for (int k = 0; k < Main.maxSectionsX; k++) {
                for (int l = 0; l < Main.maxSectionsY; l++) {
                    if (k >= 0 && k < Main.maxSectionsX && l >= 0 && l < Main.maxSectionsY) {
                        NetMessage.SendSection(player.Index, k, l, false);
                        NetMessage.SendData(11, player.Index, -1, null, k, l, k, l, 0);
                    }
                }
            }
            SendTextToPlayer("Receiving tile data successful!", player.Index, Color.Red);
        }

		public enum MessageType
		{
			GeneralMessage,
			LoginMessage,
			SnoopMessage,
			CTFMessage
		}

        public enum TileModifyType : byte {
            KillTile,
            PlaceTile,
            KillWall,
            PlaceWall,
            KillTileNoItem,
            PlaceWire,
            KillWire,
            PoundTile,
            PlaceActuator,
            KillActuator,
            PlaceWire2,
            KillWire2,
            PlaceWire3,
            KillWire3,
            SlopeTile,
            FrameTrack
        }
    }
}