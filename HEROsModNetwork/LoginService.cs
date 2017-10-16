using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;

namespace HEROsMod.HEROsModNetwork {
    internal class LoginService {
        private static BinaryWriter Writer {
            get { return Network.writer; }
        }

        public static Group MyGroup {
            get {
                if (Network.Players2.ContainsKey(Main.myPlayer)) {
                    return Network.Players2[Main.myPlayer].Group;
                } else {
                    return null;
                }
            }
            set { Network.Players2[Main.myPlayer].Group = value; }
        }

        public static event EventHandler GroupChanged;

        public static event EventHandler MyGroupChanged;

        public static void ProcessData(ref BinaryReader reader, int playerNumber) {
            MessageType msgType = (MessageType) reader.ReadByte();
            ModUtils.DebugText("LoginMessage: " + msgType);
            switch (msgType) {
                case MessageType.RequestLogin:
                    ProcessLoginRequest(ref reader, playerNumber);
                    break;

                case MessageType.LoginSucess:
                    ProcessLoginSuccess(ref reader);
                    break;

                case MessageType.RequestLogout:
                    ProcessLogoutRequest(playerNumber);
                    break;

                case MessageType.LogoutSucess:
                    ProcessLogoutSuccess(ref reader);
                    break;

                case MessageType.RequestRegistration:
                    ReadRegistrationRequest(ref reader, playerNumber);
                    break;

                case MessageType.RequestAddGroup:
                    ProcessAddGroupReqest(ref reader, playerNumber);
                    break;

                case MessageType.RequestDeleteGroup:
                    ProcessDeleteGroupRequest(ref reader, playerNumber);
                    break;

                case MessageType.RequestGroupList:
                    SendGroupList(playerNumber);
                    break;

                case MessageType.GroupList:
                    ProcessGroupList(ref reader);
                    break;

                case MessageType.SetPlayerGroup:
                    ProcessGroupPermissions(ref reader);
                    break;

                case MessageType.RequestSetGroupPermissions:
                    ProcessSetGroupPermissionsRequest(ref reader, playerNumber);
                    break;

                case MessageType.RequestPlayerInfo:
                    ProcessPlayerInfoRequest(ref reader, playerNumber);
                    break;

                case MessageType.PlayerInfo:
                    ProcessPlayerInfo(ref reader);
                    break;

                case MessageType.RequestSetPlayerGroup:
                    ProcessSetPlayerGroupRequest(ref reader, playerNumber);
                    break;

                case MessageType.RequestSetOfflinePlayerGroup:
                    ProcessSetOfflinePlayerGroupRequest(ref reader, playerNumber);
                    break;

                case MessageType.ServerToClientHandshake:
                    Network.ServerUsingHEROsMod = true;
                    HEROsMod.ServiceHotbar.Visible = true;
                    GeneralMessages.TellServerImUsingHEROsMod();
                    break;

                case MessageType.PlayerList:
                    ProcessPlayerList(ref reader);
                    break;
            }
        }

        private static void WriteHeader(MessageType msgType) {
            Network.ResetWriter();
            Writer.Write((byte) Network.MessageType.LoginMessage);
            Writer.Write((byte) msgType);
        }

        public static void RequestLogin(string username, string password) {
            WriteHeader(MessageType.RequestLogin);
            Writer.Write(username);
            Writer.Write(password);
            Network.SendDataToServer();
        }

        public static void ProcessLoginRequest(ref BinaryReader reader, int playerNumber) {
            string username = reader.ReadString();
            string password = reader.ReadString();
            ProcessLoginRequest(username, password, playerNumber);
        }

        public static void ProcessLoginRequest(string username, string password, int playerNumber) {
            int groupID = 0;
            int playerID = 0;

            if (Network.Players2.Values.Any((x) => x.Username == username)) {
                Network.SendTextToPlayer("This account is already logged on in this server.", playerNumber);
                return;
            }
            if (DatabaseController.Login(ref username, password, ref playerID, ref groupID)) {

                Network.Players2[playerNumber].Username = username;
                Network.Players2[playerNumber].ID = playerID;
                if (groupID == 0) {
                    groupID = Network.DefaultGroup.ID;
                    DatabaseController.SetPlayerGroup(playerID, groupID);
                }
                Network.Players2[playerNumber].Group = Network.GetGroupByID(groupID);
                if (Network.Players2[playerNumber].UsingHEROsMod)
                    LoginSuccess(playerNumber);
                Network.SendTextToPlayer("You have successfully logged in.  You are in the " + Network.Players2[playerNumber].Group.Name + " Group.", playerNumber, Color.Green);

            } else {
                Network.SendTextToPlayer("Invalid Username or Password", playerNumber, Color.Red);
            }
        }

        private static void LoginSuccess(int playerNumber) {
            if (Network.NetworkMode == NetworkMode.Server) {
                if (Network.Players2[playerNumber].Group == null) {
                    Network.Players2[playerNumber].Group = Network.DefaultGroup;
                }
                WriteHeader(MessageType.LoginSucess);
                Writer.Write(Network.Players2[playerNumber].Group.ID);
                Network.SendDataToPlayer(playerNumber);
                SendPlayerPermissions(playerNumber);
                SendPlayerList(playerNumber);
            }
        }

        private static void ProcessLoginSuccess(ref BinaryReader reader) {
            if (Network.NetworkMode != NetworkMode.Server) {
                int id = reader.ReadInt32();
                Network.Players2[Main.myPlayer].Group = Network.GetGroupByID(id);
                HEROsModServices.Login.LoggedIn = true;
            }
        }

        public static void RequestLogout() {
            if (Network.NetworkMode != NetworkMode.Server) {
                WriteHeader(MessageType.RequestLogout);
                Network.SendDataToServer();
            }
        }

        public static void ProcessLogoutRequest(int playerNumber) {
            if (Network.NetworkMode == NetworkMode.Server) {
                HEROsModPlayer player = Network.Players2[playerNumber];
                player.Group = Network.DefaultGroup;
                player.Username = String.Empty;
                if (player.UsingHEROsMod) {
                    WriteHeader(MessageType.LogoutSucess);
                    Writer.Write(player.Group.ID);
                    Network.SendDataToPlayer(playerNumber);
                    SendPlayerPermissions(playerNumber);
                }
                if (Network.WillFreezeNonLoggedIn) {
                    Network.SendPlayerToPosition(player, new Vector2(Main.spawnTileX * 16, Main.spawnTileY * 16));
                }
            }
        }

        private static void ProcessLogoutSuccess(ref BinaryReader reader) {
            if (Network.NetworkMode != NetworkMode.Server) {
                int id = reader.ReadInt32();
                Network.Players2[Main.myPlayer].Group = Network.GetGroupByID(id);
                HEROsModServices.Login.LoggedIn = false;
            }
        }

        public static void RequestRegistration(string username, string password) {
            WriteHeader(MessageType.RequestRegistration);
            Writer.Write(username);
            Writer.Write(password);
            Network.SendDataToServer();
        }

        private static void ReadRegistrationRequest(ref BinaryReader reader, int playernNumber) {
            string username = reader.ReadString();
            string password = reader.ReadString();
            ProcessRegistrationRequest(username, password, playernNumber);
        }

        public static void ProcessRegistrationRequest(string username, string password, int playerNumber) {
            DatabaseController.RegistrationResult regResult = DatabaseController.Register(username, password);
            switch (regResult) {
                case DatabaseController.RegistrationResult.Sucess:
                    Network.SendTextToPlayer("You have successfully registered.  Please login.", playerNumber);
                    foreach (var player in Network.Players2) {
                        if (player.Value.ServerInstance.IsActive && player.Value.Group.IsAdmin) {
                            GeneralMessages.SendRegisteredUsersToPlayer(player.Key);
                        }
                    }
                    break;

                case DatabaseController.RegistrationResult.UsernameTaken:
                    Network.SendTextToPlayer("This username has already been taken.", playerNumber);
                    break;

                case DatabaseController.RegistrationResult.Error:
                    Network.SendTextToPlayer("An error occured when trying to register.", playerNumber);
                    break;
            }
        }

        public static void RequestAddGroup(string groupName) {
            WriteHeader(MessageType.RequestAddGroup);
            Writer.Write(groupName);
            Network.SendDataToServer();
        }

        private static void ProcessAddGroupReqest(ref BinaryReader reader, int playerNumber) {
            if (Network.NetworkMode == NetworkMode.Server) {
                if (!Network.Players2[playerNumber].Group.IsAdmin) return;
                string newGroupName = reader.ReadString();
                for (int i = 0; i < Network.Groups.Count; i++) {
                    //Check to make sure that group does not already exist
                    if (Network.Groups[i].Name.ToLower() == newGroupName.ToLower()) {
                        Network.SendTextToPlayer("A group with this name already exist", playerNumber);
                        return;
                    }
                }
                Group newGroup = new Group(newGroupName);
                DatabaseController.AddGroup(ref newGroup);
                Network.Groups.Add(newGroup);
                GroupChanged?.Invoke(null, EventArgs.Empty);
            }
        }

        public static void RequestDeleteGroup(int groupID) {
            WriteHeader(MessageType.RequestDeleteGroup);
            Writer.Write(groupID);
            Network.SendDataToServer();
        }

        private static void ProcessDeleteGroupRequest(ref BinaryReader reader, int playerNumber) {
            if (Network.NetworkMode == NetworkMode.Server) {
                if (!Network.Players2[playerNumber].Group.IsAdmin) return;
                Group groupToDelete = Network.GetGroupByID(reader.ReadInt32());
                if (groupToDelete != null) {
                    if (groupToDelete.Name.ToLower() != "default") {
                        foreach (var user in Network.Players2) {
                            if (user.Value.Group == groupToDelete) {
                                user.Value.Group = Network.DefaultGroup;
                                SendPlayerPermissions(user.Key);
                            }
                        }
                        DatabaseController.DeleteGroup(groupToDelete);
                        Network.Groups.Remove(groupToDelete);
                        GroupChanged?.Invoke(null, EventArgs.Empty);
                    } else {
                        Network.SendTextToPlayer("You can not delete the default group.", playerNumber);
                    }
                } else {
                    Network.SendTextToPlayer("Group could not be found", playerNumber);
                }
            }
        }

        public static void RequestGroupList() {
            WriteHeader(MessageType.RequestGroupList);
            Network.SendDataToServer();
        }

        public static void SendPlayerList(int playerNumber) {
            if (Network.NetworkMode == NetworkMode.Server) {
                WriteHeader(MessageType.PlayerList);
                Writer.Write(Network.Players2.Count);
                foreach (var item in Network.Players2) {
                    Writer.Write(item.Key);
                    Writer.Write(item.Value.Username);
                    Writer.Write(item.Value.ID);
                    Writer.Write(item.Value.Group.ID);

                    ModUtils.DebugText("Key: " + item.Key + "; Username: " + item.Value.Username + "; ID: " + item.Value.ID + "; GroupID: " + item.Value.Group.ID + "; Index: " + item.Value.Index);
                }
                Network.SendDataToPlayer(playerNumber);
            }
        }

        public static void ProcessPlayerList(ref BinaryReader reader) {
            if (Network.NetworkMode != NetworkMode.Server) {
                Dictionary<int, HEROsModPlayer> players = new Dictionary<int, HEROsModPlayer>();
                int numPlayers = reader.ReadInt32();
                for (int i = 1; i <= numPlayers; i++) {
                    int index = reader.ReadInt32();
                    string username = reader.ReadString();
                    int id = reader.ReadInt32();
                    int groupID = reader.ReadInt32();
                    HEROsModPlayer player = new HEROsModPlayer(index) {
                        Username = username,
                        ID = id,
                        UsingHEROsMod = true,
                        Group = Network.GetGroupByID(groupID)
                    };
                    players.Add(index, player);
                    var item = players[index];
                    ModUtils.DebugText("GroupName: " + item.Group.Name);
                    ModUtils.DebugText("Key: " + index + "; Username: " + item.Username + "; ID: " + item.ID + "; Index: " + item.Index);

                }
                Network.Players2 = players;
            }
        }


        public static void SendGroupList(int playerNumber) {
            if (Network.NetworkMode == NetworkMode.Server) {
                WriteHeader(MessageType.GroupList);
                int numOfGroups = Network.Groups.Count;
                Writer.Write(numOfGroups);
                for (int i = 0; i < Network.Groups.Count; i++) {
                    Writer.Write(Network.Groups[i].Name);
                    Writer.Write(Network.Groups[i].ID);
                    byte[] permissions = Network.Groups[i].ExportPermissions();
                    Writer.Write(permissions.Length);
                    Writer.Write(permissions);
                }
                Network.SendDataToPlayer(playerNumber);
            }
        }

        private static void ProcessGroupList(ref BinaryReader reader) {
            if (Network.NetworkMode != NetworkMode.Server) {
                Network.Groups.Clear();
                int numOfGroups = reader.ReadInt32();
                for (int i = 0; i < numOfGroups; i++) {
                    string groupName = reader.ReadString();
                    Group group = new Group(groupName) {
                        ID = reader.ReadInt32()
                    };
                    int permissionsLength = reader.ReadInt32();
                    group.ImportPermissions(reader.ReadBytes(permissionsLength));
                    Network.Groups.Add(group);
                }
                GroupChanged?.Invoke(null, EventArgs.Empty);
            }
        }

        public static void SendAllPlayersPermissions() {
            foreach (var player in Network.Players2) {
                if (player.Value.ServerInstance.IsActive) {
                    SendPlayerPermissions(player.Key);
                }
            }
        }

        public static void SendPlayerPermissions(int playerNumber) {
            if (Network.NetworkMode == NetworkMode.Server) {
                WriteHeader(MessageType.SetPlayerGroup);
                HEROsModPlayer player = Network.Players2[playerNumber];
                Group group = player.Group;
                Writer.Write(group.Name);
                Writer.Write(group.ID);
                Writer.Write(group.IsAdmin);
                byte[] permissions = group.ExportPermissions();
                //if(CTF.CaptureTheFlag.GameInProgress)
                //{
                //    permissions = Network.CTFGroup.ExportPermissions();
                //}
                Writer.Write(permissions.Length);
                Writer.Write(permissions);
                Network.SendDataToPlayer(playerNumber);

                if (group.IsAdmin) GeneralMessages.SendRegisteredUsersToPlayer(playerNumber);
            }
        }

        private static void ProcessGroupPermissions(ref BinaryReader reader) {
            string groupName = reader.ReadString();
            Group group = new Group(groupName) {
                ID = reader.ReadInt32()
            };
            bool isAdmin = reader.ReadBoolean();
            if (isAdmin) {
                group.ID = -1;
                group.IsAdmin = true;
                //group.MakeAdmin();
            }
            int permissionsLength = reader.ReadInt32();
            group.ImportPermissions(reader.ReadBytes(permissionsLength));

            MyGroup = group;
            MyGroupChanged?.Invoke(null, EventArgs.Empty);
        }

        public static void RequestSetGroupPermissions(Group group) {
            WriteHeader(MessageType.RequestSetGroupPermissions);
            Writer.Write(group.ID);
            byte[] permissions = group.ExportPermissions();
            Writer.Write(permissions.Length);
            Writer.Write(permissions);
            Network.SendDataToServer();
        }

        private static void ProcessSetGroupPermissionsRequest(ref BinaryReader reader, int playerNumber) {
            if (Network.NetworkMode == NetworkMode.Server) {
                if (!Network.Players2[playerNumber].Group.IsAdmin) return;
                int id = reader.ReadInt32();
                Group group = Network.GetGroupByID(id);
                int permissionsLength = reader.ReadInt32();
                group.ImportPermissions(reader.ReadBytes(permissionsLength));
                DatabaseController.SetGroupPermissions(group);

                foreach (var user in Network.Players2) {
                    if (user.Value.Group == group) {
                        SendPlayerPermissions(user.Key);
                    }
                }
                GroupChanged?.Invoke(null, EventArgs.Empty);
            }
        }

        public static void RequestPlayerInfo(int indexOfRequestedPlayer) {
            WriteHeader(MessageType.RequestPlayerInfo);
            Writer.Write(indexOfRequestedPlayer);
            Network.SendDataToServer();
        }

        private static void ProcessPlayerInfoRequest(ref BinaryReader reader, int playerNumber) {
            Group playerGroup = Network.Players2[playerNumber].Group;
            if (playerGroup.IsAdmin) {
                int indexOfRequestedPlayer = reader.ReadInt32();
                SendPlayerInfo(indexOfRequestedPlayer, playerNumber);
            }
        }

        private static void SendPlayerInfo(int indexOfRequestedPlayer, int playerNumber) {
            WriteHeader(MessageType.PlayerInfo);
            HEROsModPlayer player = Network.Players2[indexOfRequestedPlayer];
            Writer.Write(player.Username);
            Writer.Write(player.Group.ID);
            Writer.Write(indexOfRequestedPlayer);
            Network.SendDataToPlayer(playerNumber);
        }

        private static void ProcessPlayerInfo(ref BinaryReader reader) {
            string username = reader.ReadString();
            int groupID = reader.ReadInt32();
            int indexOfRequestedPlayer = reader.ReadInt32();
            Network.Players2[indexOfRequestedPlayer].Username = username;
            Network.Players2[indexOfRequestedPlayer].Group = Network.GetGroupByID(groupID);
            if (HEROsModServices.PlayerList.playersWindow != null) {
                HEROsModServices.PlayerList.playersWindow.OpenPlayerInfo(indexOfRequestedPlayer, false);
            }
        }

        public static void RequestSetPlayerGroup(int playerIndex, Group group) {
            WriteHeader(MessageType.RequestSetPlayerGroup);
            Writer.Write(playerIndex);
            Writer.Write(group.ID);
            Network.SendDataToServer();
        }

        private static void ProcessSetPlayerGroupRequest(ref BinaryReader reader, int playerNumber) {
            if (Network.Players2[playerNumber].Group.IsAdmin) {
                int playerIndex = reader.ReadInt32();
                int groupID = reader.ReadInt32();

                Network.Players2[playerIndex].Group = Network.GetGroupByID(groupID);
                SendPlayerPermissions(playerIndex);
                DatabaseController.SetPlayerGroup(Network.Players2[playerIndex].ID, groupID);
            }
        }

        public static void RequestSetOfflinePlayerGroup(int playerIndex, Group group) {
            WriteHeader(MessageType.RequestSetOfflinePlayerGroup);
            Writer.Write(playerIndex);
            Writer.Write(group.ID);
            Network.SendDataToServer();
        }

        private static void ProcessSetOfflinePlayerGroupRequest(ref BinaryReader reader, int playerNumber) {
            if (Network.Players2[playerNumber].Group.IsAdmin) {
                int id = reader.ReadInt32();
                int groupID = reader.ReadInt32();

                //Network.Players[id].Group = Network.GetGroupByID(groupID);
                //SendPlayerPermissions(id);
                DatabaseController.SetPlayerGroup(id, groupID);
                foreach (var player in Network.Players2) {
                    if (player.Value.ServerInstance.IsActive && player.Value.Group.IsAdmin) {
                        GeneralMessages.SendRegisteredUsersToPlayer(player.Key);
                    }
                }
                //GeneralMessages.SendRegisteredUsersToPlayer(playerNumber);
            }
        }

        public enum MessageType {
            RequestLogin,
            LoginSucess,
            RequestLogout,
            LogoutSucess,
            RequestRegistration,
            RequestAddGroup,
            RequestDeleteGroup,
            RequestGroupList,
            GroupList,
            SetPlayerGroup,
            RequestSetGroupPermissions,
            RequestPlayerInfo,
            PlayerInfo,
            RequestSetPlayerGroup,
            RequestSetOfflinePlayerGroup,
            ServerToClientHandshake,
            RequestPlayerList,
            PlayerList,

        }
    }
}