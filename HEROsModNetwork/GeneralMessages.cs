﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Localization;

namespace HEROsMod.HEROsModNetwork {
    internal class GeneralMessages {
        public delegate void TimePausedOrResumedEvent(bool timePaused);

        public static event TimePausedOrResumedEvent TimePausedOrResumedByServer;

        public delegate void EnemiesToggleEvent(bool enemiesCanSpawn);

        public static event EnemiesToggleEvent EnemiesToggledByServer;

        public delegate void GravestoneToggleEvent(bool gravestonesCanSpawn);

        public static event GravestoneToggleEvent GravestonesToggleByServer;

        public delegate void ItemBannerToggleEvent(bool itemsbanned);

        public static event ItemBannerToggleEvent ItemBannerToggleByServer;

        public delegate void PlayerEvent(HEROsModPlayer player);

        public static event PlayerEvent PlayerJoined;

        public static event PlayerEvent PlayerLeft;

        public static event EventHandler RegionsUpdated;

        private static BinaryWriter Writer {
            get { return Network.writer; }
        }

        public static void ProcessData(ref BinaryReader reader, int playerNumber) {
            MessageType msgType = (MessageType) reader.ReadByte();
            ModUtils.DebugText("GeneralMessage: " + msgType);
            switch (msgType) {
                case MessageType.UsingHEROsMod:
                    ProcessPlayerUsingHEROsMod(playerNumber);
                    break;

                case MessageType.RequestTimeChange:
                    ProcessTimeChangeRequest(ref reader, playerNumber);
                    break;

                case MessageType.TimePausedOrResumed:
                    ProcessTimePausedOrChanged(ref reader);
                    break;

                case MessageType.RequestClearGroundItems:
                    ProcessClearGroundItemsRequest(playerNumber);
                    break;

                case MessageType.RequestToggleEnemies:
                    ProcessToggleEnemiesRequest(playerNumber);
                    break;

                case MessageType.EnemiesToggled:
                    ProcessEnemiesToggled(ref reader);
                    break;

                case MessageType.RequestSpawnTownNPC:
                    ProcessSpawnTownNPCRequest(ref reader, playerNumber);
                    break;

                case MessageType.RequestStartRain:
                    ProcessStartRainRequest(playerNumber);
                    break;

                case MessageType.RequestStopRain:
                    ProcessStopRainRequest(playerNumber);
                    break;

                case MessageType.RequestForcedSundial:
                    ProcessForcedSundialRequest(playerNumber);
                    break;

                case MessageType.WaypointList:
                    ProcessWaypointList(ref reader);
                    break;

                case MessageType.RequestAddWaypoint:
                    ProcessAddWaypointRequest(ref reader, playerNumber);
                    break;

                case MessageType.RequestRemoveWaypoint:
                    ProcessRemoveWaypointReqeust(ref reader, playerNumber);
                    break;

                case MessageType.PlayerJoined:
                    ProcessPlayerJoined(ref reader);
                    break;

                case MessageType.PlayerLeft:
                    ProcessPlayerLeft(ref reader);
                    break;

                case MessageType.RegionList:
                    ProcessRegionList(ref reader);
                    break;

                case MessageType.CurrentToggles:
                    ProcessCurrentToggles(ref reader);
                    break;

                case MessageType.RequestCreateRegion:
                    ProcessCreateRegionRequest(ref reader, playerNumber);
                    break;

                case MessageType.RequestRemoveRegion:
                    ProcessRemoveRegionRequest(ref reader, playerNumber);
                    break;

                case MessageType.RequestRegisteredUsers:
                    ProcessRegisteredUsersRequest(playerNumber);
                    break;

                case MessageType.RegisteredUsers:
                    ProcessRegisteredUsersList(ref reader);
                    break;

                case MessageType.RequestAddPlayerToRegion:
                    ProcessAddPlayerToRegionRequest(ref reader, playerNumber);
                    break;

                case MessageType.RequestRemovePlayerFromRegion:
                    ProcessRemovePlayerFromRegionRequest(ref reader, playerNumber);
                    break;

                case MessageType.RequestAddGroupToRegion:
                    ProcessAddGroupToRegionRequest(ref reader, playerNumber);
                    break;

                case MessageType.RequestRemoveGroupFromRegion:
                    ProcessRemoveGroupFromRegionRequest(ref reader, playerNumber);
                    break;

                case MessageType.RequestRestoreTiles:
                    ProcessRestoreTilesRequest(ref reader, playerNumber);
                    break;

                case MessageType.RequestSetSpawnPoint:
                    ProcessSetSpawnPointRequest(playerNumber);
                    break;

                case MessageType.RequestToggleGravestones:
                    ProcessToggleGravestonesRequest(playerNumber);
                    break;

                case MessageType.GravestonesToggled:
                    ProcessGravestonesToggled(ref reader);
                    break;

                case MessageType.RequestChangeRegionColor:
                    ProcessChangeRegionColorRequest(ref reader, playerNumber);
                    break;

                case MessageType.RequestGodMode:
                    ProcessGodModeRequest(playerNumber);
                    break;

                case MessageType.AllowGodMode:
                    ProcessGodMode();
                    break;

                case MessageType.RequestTileModificationCheck:
                    ProcessTileModificationCheckRequest(ref reader, playerNumber);
                    break;

                case MessageType.RequestSpawnNPC:
                    ProcessSpawnNPCRequest(ref reader, playerNumber);
                    break;

                case MessageType.RequestStartEvent:
                    ProcessStartEventRequest(ref reader, playerNumber);
                    break;

                case MessageType.RequestStopEvents:
                    ProcessStopEventsRequest(playerNumber);
                    break;

                case MessageType.RequestToggleBannedItems:
                    ProcessToggleBannedItemsRequest(playerNumber);
                    break;

                case MessageType.BannedItemsToggled:
                    ProcessBannedItemsToggled(ref reader);
                    break;

                case MessageType.RequestBanPlayer:
                    ProcessRequestBanPlayer(ref reader, playerNumber);
                    break;

                case MessageType.RequestKickPlayer:
                    ProcessRequestKickPlayer(ref reader, playerNumber);
                    break;

                case MessageType.RequestTeleport:
                    ProcessRequestTeleport(ref reader, playerNumber);
                    break;

                case MessageType.ServerPlayerLeft:
                    ProcessHeLeft(playerNumber);
                    break;


                case MessageType.RequestToggleHardmodeEnemies:
                    ProcessToggleHardmodeEnemiesRequest(playerNumber);
                    break;

                case MessageType.PurifyWorld:
                    ProcessPurifyWorldRequest(playerNumber);
                    break;
            }
        }


        private static void WriteHeader(MessageType msgType) {
            Network.ResetWriter();
            Writer.Write((byte) Network.MessageType.GeneralMessage);
            Writer.Write((byte) msgType);
        }

        #region Kick Player
        public static void RequestKickPlayer(string playername) {
            WriteHeader(MessageType.RequestKickPlayer);
            Writer.Write(playername);
            Network.SendDataToServer();
        }

        private static void ProcessRequestKickPlayer(ref BinaryReader reader, int playerNumber) {
            if (Network.Players2[playerNumber].Group.HasPermission("Kick")) {
                string playerToKick = reader.ReadString();
                ModUtils.DebugText("Server Kick request received: " + playerToKick);
                for (int j = 0; j < 255; j++) {
                    if (Main.player[j].active && Main.player[j].name.ToLower() == playerToKick) {
                        NetMessage.SendData(2, j, -1, NetworkText.FromKey("CLI.KickMessage", new object[0]), 0, 0f, 0f, 0f, 0, 0, 0);
                    }
                }
            }
        }
        #endregion

        #region Ban Player
        public static void RequestBanPlayer(string playername) {
            WriteHeader(MessageType.RequestBanPlayer);
            Writer.Write(playername);
            Network.SendDataToServer();
        }

        private static void ProcessRequestBanPlayer(ref BinaryReader reader, int playerNumber) {
            if (Network.Players2[playerNumber].Group.HasPermission("Ban")) {
                string playertoban = reader.ReadString();
                ModUtils.DebugText("Server Ban request received: " + playertoban);
                for (int k = 0; k < 255; k++) {
                    if (Main.player[k].active && Main.player[k].name.ToLower() == playertoban) {
                        Netplay.AddBan(k);
                        NetMessage.SendData(2, k, -1, NetworkText.FromKey("CLI.BanMessage", new object[0]), 0, 0f, 0f, 0f, 0, 0, 0);
                    }
                }
            }
        }
        #endregion

        #region Using HERO's Mod
        public static void TellServerImUsingHEROsMod() {
            WriteHeader(MessageType.UsingHEROsMod);
            Network.SendDataToServer();
        }

        private static void ProcessPlayerUsingHEROsMod(int playerNumber) {
            if (Network.NetworkMode == NetworkMode.Server) {
                ModUtils.DebugText("ProcessPlayerUsingHEROsMod: " + playerNumber);
                Network.Players2[playerNumber].UsingHEROsMod = true;
                Network.ProcessClientsUsingHEROsMod(playerNumber);
                LoginService.SendGroupList(playerNumber);
                LoginService.SendPlayerPermissions(playerNumber);
                SendWaypointListToPlayer(playerNumber);
                SendRegionListToPlayer(playerNumber);
                SendCurrentTogglesToPlayer(playerNumber);
                Network.SendTextToPlayer(@"Click the 'Login' button on the HEROsMod hotbar to Login or Create an Account", playerNumber, Color.Red);
            }
        }
        #endregion

        #region Time Change
        public static void RequestTimeChange(TimeChangeType tct) {
            WriteHeader(MessageType.RequestTimeChange);
            Writer.Write((byte) tct);
            Network.SendDataToServer();
        }

        public static void RequestTimeChange(TimeChangeType tct, double time, bool daytime) {
            WriteHeader(MessageType.RequestTimeChange);
            Writer.Write((byte) tct);
            Writer.Write(time);
            Writer.Write(daytime);
            Network.SendDataToServer();
        }

        private static void ProcessTimeChangeRequest(ref BinaryReader reader, int playerNumber) {
            if (Network.Players2[playerNumber].Group.HasPermission("ChangeTimeWeather")) {
                TimeChangeType tct = (TimeChangeType) reader.ReadByte();
                switch (tct) {
                    case TimeChangeType.SetToNoon:
                        Main.dayTime = true;
                        Main.time = 27000.0;
                        Network.SendTextToAllPlayers("The time has been changed to noon by " + Main.player[playerNumber].name);
                        break;

                    case TimeChangeType.SetToMidnight:
                        Main.dayTime = false;
                        Main.time = 27000.0;
                        Network.SendTextToAllPlayers("The time has been changed to midnight by " + Main.player[playerNumber].name);
                        break;

                    case TimeChangeType.SetToNight:
                        Main.dayTime = false;
                        Main.time = 0;
                        Network.SendTextToAllPlayers("The time has been changed to night by " + Main.player[playerNumber].name);
                        break;

                    case TimeChangeType.Pause:
                        HEROsModServices.TimeWeatherChanger.TimePaused = !HEROsModServices.TimeWeatherChanger.TimePaused;
                        if (HEROsModServices.TimeWeatherChanger.TimePaused) {
                            HEROsModServices.TimeWeatherChanger.PausedTime = Main.time;
                            Network.SendTextToAllPlayers("Time has been paused by " + Main.player[playerNumber].name);
                        } else {
                            Network.SendTextToAllPlayers("Time has been resumed by " + Main.player[playerNumber].name);
                        }
                        TimePausedOrResumed();
                        break;

                    case TimeChangeType.SpecificTime:
                        double time = reader.ReadDouble();
                        bool daytime = reader.ReadBoolean();
                        Main.dayTime = daytime;
                        Main.time = time;
                        break;
                }
                NetMessage.SendData(7, -1, -1, null, 0, 0f, 0f, 0f, 0);
            }
        }
        #endregion

        #region Time Pause
        private static void TimePausedOrResumed() {
            WriteHeader(MessageType.TimePausedOrResumed);
            Writer.Write(HEROsModServices.TimeWeatherChanger.TimePaused);
            Network.SendDataToAllHEROsModUsers();
        }

        private static void ProcessTimePausedOrChanged(ref BinaryReader reader) {
            bool timePaused = reader.ReadBoolean();
            TimePausedOrResumedByServer?.Invoke(timePaused);
        }
        #endregion

        #region Clear Ground Items
        public static void RequestClearGroundItems() {
            WriteHeader(MessageType.RequestClearGroundItems);
            Network.SendDataToServer();
        }

        private static void ProcessClearGroundItemsRequest(int playerNumber) {
            if (Network.Players2[playerNumber].Group.HasPermission("ClearItems")) {
                Network.ClearGroundItems();
                Network.SendTextToAllPlayers("Items on the ground were cleared by " + Netplay.Clients/*.serverSock*/[playerNumber].Name/*name*/);
            }
        }
        #endregion

        #region Toggle Enemies
        public static void RequestToggleEnemies() {
            WriteHeader(MessageType.RequestToggleEnemies);
            Network.SendDataToServer();
        }

        private static void ProcessToggleEnemiesRequest(int playerNumber) {
            if (Network.Players2[playerNumber].Group.HasPermission("ToggleEnemies")) {
                HEROsModServices.EnemyToggler.ToggleNPCs();
                if (HEROsModServices.EnemyToggler.EnemiesAllowed) {
                    Network.SendTextToAllPlayers("Enemy spawns were enabled by " + Main.player[playerNumber].name);
                } else {
                    Network.SendTextToAllPlayers("Enemy spawns were disabled by " + Main.player[playerNumber].name);
                }
                EnemiesToggled();
            }
        }

        private static void EnemiesToggled() {
            WriteHeader(MessageType.EnemiesToggled);
            Writer.Write(HEROsModServices.EnemyToggler.EnemiesAllowed);
            Network.SendDataToAllHEROsModUsers();
        }

        private static void ProcessEnemiesToggled(ref BinaryReader reader) {
            if (Network.NetworkMode == NetworkMode.Server) return;
            bool enemiesCanSpawn = reader.ReadBoolean();
            EnemiesToggledByServer?.Invoke(enemiesCanSpawn);
        }
        #endregion

        #region Spawn Town NPC
        public static void RequestSpawnTownNPC(int npcID) {
            WriteHeader(MessageType.RequestSpawnTownNPC);
            Writer.Write(npcID);
            Network.SendDataToServer();
        }

        private static void ProcessSpawnTownNPCRequest(ref BinaryReader reader, int playerNumber) {
            if (Network.Players2[playerNumber].Group.HasPermission("SpawnNPCs")) {
                int npcId = reader.ReadInt32();
                Network.SpawnNPC(npcId, Main.player[playerNumber].position);
            }
        }
        #endregion

        #region Start/Stop Rain
        public static void RequestStartRain() {
            WriteHeader(MessageType.RequestStartRain);
            Network.SendDataToServer();
        }

        private static void ProcessStartRainRequest(int playerNumber) {
            if (Network.Players2[playerNumber].Group.HasPermission("ChangeTimeWeather")) {
                ModUtils.StartRain();
                Network.SendTextToAllPlayers("Rain has been turned on by " + Main.player[playerNumber].name);
            }
        }

        public static void RequestStopRain() {
            WriteHeader(MessageType.RequestStopRain);
            Network.SendDataToServer();
        }

        private static void ProcessStopRainRequest(int playerNumber) {
            if (Network.Players2[playerNumber].Group.HasPermission("ChangeTimeWeather")) {
                ModUtils.StopRain();
                Network.SendTextToAllPlayers("Rain has been turned off by " + Main.player[playerNumber].name);
            }
        }
        #endregion

        #region Start/Stop Sandstorm
        public static void RequestStartSandstorm() {
            WriteHeader(MessageType.RequestStartSandstorm);
            Network.SendDataToServer();
        }

        private static void ProcessStartSandstormRequest(int playerNumber) {
            if (Network.Players2[playerNumber].Group.HasPermission("ChangeTimeWeather")) {
                ModUtils.StartSandstorm();
                Network.SendTextToAllPlayers("Sandstorm has been turned on by " + Main.player[playerNumber].name);
            }
        }

        public static void RequestStopSandstorm() {
            WriteHeader(MessageType.RequestStopSandstorm);
            Network.SendDataToServer();
        }

        private static void ProcessStopSandstormRequest(int playerNumber) {
            if (Network.Players2[playerNumber].Group.HasPermission("ChangeTimeWeather")) {
                ModUtils.StopSandstorm();
                Network.SendTextToAllPlayers("Sandstorm has been turned off by " + Main.player[playerNumber].name);
            }
        }
        #endregion

        #region Forced Sundial
        public static void RequestForcedSundial() {
            WriteHeader(MessageType.RequestForcedSundial);
            Network.SendDataToServer();
        }

        private static void ProcessForcedSundialRequest(int playerNumber) {
            if (Network.Players2[playerNumber].Group.HasPermission("ChangeTimeWeather")) {
                Main.fastForwardTime = true;
                Main.sundialCooldown = 0;
                NetMessage.SendData(7, -1, -1, null, 0, 0f, 0f, 0f, 0, 0, 0);
                Network.SendTextToAllPlayers("Forced Enchanted Sundial initiated by " + Main.player[playerNumber].name);
            }
        }
        #endregion

        #region Waypoint List; Add/Remove Waypoint
        public static void SendWaypointListToPlayer(int playerNumber) {
            WriteHeader(MessageType.WaypointList);

            List<HEROsModServices.Waypoint> points = HEROsModServices.Waypoints.points;
            Writer.Write(points.Count);
            for (int i = 0; i < points.Count; i++) {
                Writer.Write(points[i].name);
                Writer.Write(points[i].position.X);
                Writer.Write(points[i].position.Y);
            }
            Network.SendDataToPlayer(playerNumber);
        }

        private static void ProcessWaypointList(ref BinaryReader reader) {
            if (Network.NetworkMode == NetworkMode.Client) {
                HEROsModServices.Waypoints.ClearPoints();
                int numOfPoints = reader.ReadInt32();
                for (int i = 0; i < numOfPoints; i++) {
                    string name = reader.ReadString();
                    Vector2 position = new Vector2(reader.ReadSingle(), reader.ReadSingle());
                    HEROsModServices.Waypoints.AddWaypoint(name, position);
                }
            }
        }

        public static void RequestAddWaypoint(string name, Vector2 position) {
            WriteHeader(MessageType.RequestAddWaypoint);
            Writer.Write(name);
            Writer.WriteVector2(position);
            Network.SendDataToServer();
        }

        private static void ProcessAddWaypointRequest(ref BinaryReader reader, int playerNumber) {
            if (Network.Players2[playerNumber].Group.HasPermission("EditWaypoints")) {
                string name = reader.ReadString();
                Vector2 position = reader.ReadVector2();
                if (HEROsModServices.Waypoints.AddWaypoint(name, position)) {
                    SendWaypointListToPlayer(-2);
                } else {
                    Network.SendTextToPlayer("A waypoint with this name already exists, please enter another name.", playerNumber);
                }
            }
        }

        public static void RequestRemoveWaypoint(int waypointIndex) {
            WriteHeader(MessageType.RequestRemoveWaypoint);
            Writer.Write(waypointIndex);
            Network.SendDataToServer();
        }

        private static void ProcessRemoveWaypointReqeust(ref BinaryReader reader, int playerNumber) {
            if (Network.Players2[playerNumber].Group.HasPermission("EditWaypoints")) {
                ModUtils.DebugText("ProcessRemoveWaypointReqeust: " + playerNumber);
                List<HEROsModServices.Waypoint> points = HEROsModServices.Waypoints.points;
                int waypointIndex = reader.ReadInt32();
                if (waypointIndex >= 0 && waypointIndex < points.Count) {
                    HEROsModServices.Waypoints.RemoveWaypoint(waypointIndex);
                    SendWaypointListToPlayer(-2);
                }
            }
        }
        #endregion

        #region Player Joined/Left
        public static void TellClientsPlayerJoined(int playerIndex) {
            WriteHeader(MessageType.PlayerJoined);
            Writer.Write(playerIndex);
            Network.SendDataToAllHEROsModUsers();
        }

        private static void ProcessPlayerJoined(ref BinaryReader reader) {
            if (Network.NetworkMode == NetworkMode.Server) return;
            int playerIndex = reader.ReadInt32();
            Network.Players2.Add(playerIndex, new HEROsModPlayer(playerIndex));
        }

        public static void TellServerILeft() {
            WriteHeader(MessageType.ServerPlayerLeft);
            Network.SendDataToServer();
        }

        private static void ProcessHeLeft(int playerIndex) {
            if (Network.NetworkMode == NetworkMode.Server) {
                Network.Players2.Remove(playerIndex);
            }
        }

        public static void TellClientsPlayerLeft(int playerIndex) {
            foreach (var item in Network.Players2.Values) {
                if (item.Index != playerIndex) {
                    WriteHeader(MessageType.PlayerLeft);
                    Writer.Write(playerIndex);
                    Network.SendDataToPlayer(item.Index);
                }
            }
        }

        private static void ProcessPlayerLeft(ref BinaryReader reader) {
            if (Network.NetworkMode == NetworkMode.Server) return;
            int playerIndex = reader.ReadInt32();
            Network.Players2.Remove(playerIndex);
        }
        #endregion

        #region Toggles
        public static void SendCurrentTogglesToPlayer(int playerNumber) {
            WriteHeader(MessageType.CurrentToggles);

            Writer.Write(HEROsModServices.EnemyToggler.EnemiesAllowed);
            Writer.Write(HEROsModServices.TimeWeatherChanger.TimePaused);
            Writer.Write(Network.GravestonesAllowed);
            Writer.Write(HEROsModServices.ItemBanner.ItemsBanned);

            Network.SendDataToPlayer(playerNumber);
        }

        // TODO, singleplayer
        private static void ProcessCurrentToggles(ref BinaryReader reader) {
            if (Network.NetworkMode == NetworkMode.Server) return;
            bool enemiesAllowed = reader.ReadBoolean();
            EnemiesToggledByServer?.Invoke(enemiesAllowed);

            bool timePaused = reader.ReadBoolean();
            TimePausedOrResumedByServer?.Invoke(timePaused);

            bool gravestonesAllowed = reader.ReadBoolean();
            GravestonesToggleByServer?.Invoke(gravestonesAllowed);

            bool itemsBanned = reader.ReadBoolean();
            ItemBannerToggleByServer?.Invoke(itemsBanned);
        }

        internal static void ProcessCurrentTogglesSP(bool enemiesAllowed, bool gravestonesAllowed, bool itemsBanned, bool timePaused) {
            EnemiesToggledByServer?.Invoke(enemiesAllowed);
            GravestonesToggleByServer?.Invoke(gravestonesAllowed);
            ItemBannerToggleByServer?.Invoke(itemsBanned);
            TimePausedOrResumedByServer?.Invoke(timePaused);
        }
        #endregion

        #region Region List/Create/Delete Region
        public static void SendRegionListToAllPlayers() {
            SendRegionListToPlayer(-2);
        }

        public static void SendRegionListToPlayer(int playerNumber) {
            WriteHeader(MessageType.RegionList);
            Writer.Write(Network.Regions.Count);
            for (int i = 0; i < Network.Regions.Count; i++) {
                Writer.Write(Network.Regions[i].Export());
            }
            Network.SendDataToPlayer(playerNumber);
        }

        private static void ProcessRegionList(ref BinaryReader reader) {
            if (Network.NetworkMode == NetworkMode.Server) return;
            Network.Regions.Clear();

            int numberOfRegions = reader.ReadInt32();
            for (int i = 0; i < numberOfRegions; i++) {
                Network.Regions.Add(Region.GetRegionFromBinaryReader(ref reader));
            }
            RegionsUpdated?.Invoke(null, EventArgs.Empty);
        }

        public static void RequestCreateRegion(Region region) {
            WriteHeader(MessageType.RequestCreateRegion);
            Writer.Write(region.Export());
            Network.SendDataToServer();
        }

        private static void ProcessCreateRegionRequest(ref BinaryReader reader, int playerNumber) {
            if (Network.Players2[playerNumber].Group.IsAdmin) {
                Region region = Region.GetRegionFromBinaryReader(ref reader);
                DatabaseController.AddRegion(ref region);
                Network.Regions.Add(region);
                SendRegionListToAllPlayers();
                Network.SendTextToAllPlayers(@"'" + region.Name + "' region created by " + Network.Players2[playerNumber].ServerInstance.Name);
            }
        }

        public static void RequestRemoveRegion(Region region) {
            WriteHeader(MessageType.RequestRemoveRegion);
            Writer.Write(region.ID);
            Network.SendDataToServer();
        }

        private static void ProcessRemoveRegionRequest(ref BinaryReader reader, int playerNumber) {
            if (Network.Players2[playerNumber].Group.IsAdmin) {
                int regionID = reader.ReadInt32();
                Region region = Network.GetRegionByID(regionID);
                if (region != null) {
                    DatabaseController.RemoveRegion(region);
                    Network.Regions.Remove(region);
                    SendRegionListToAllPlayers();
                }
            }
        }
        #endregion

        #region Registered Users List
        public static void RequestRegisteredUsers() {
            WriteHeader(MessageType.RequestRegisteredUsers);
            Network.SendDataToServer();
        }

        private static void ProcessRegisteredUsersRequest(int playerNumber) {
            if (Network.Players2[playerNumber].Group.IsAdmin) {
                SendRegisteredUsersToPlayer(playerNumber);
            }
        }

        public static void SendRegisteredUsersToPlayer(int playerNumber) {
            WriteHeader(MessageType.RegisteredUsers);
            UserWithID[] players = DatabaseController.GetRegisteredUsers();
            if (players.Length == 0) return;
            Writer.Write(players.Length);

            for (int i = 0; i < players.Length; i++) {
                Writer.Write(players[i].Username);
                Writer.Write(players[i].ID);
                Writer.Write(players[i].groupID);
            }

            Network.SendDataToPlayer(playerNumber);
        }

        private static void ProcessRegisteredUsersList(ref BinaryReader reader) {
            if (Network.NetworkMode == NetworkMode.Server) return;
            Network.RegisteredUsers.Clear();
            int numberOfUsers = reader.ReadInt32();

            for (int i = 0; i < numberOfUsers; i++) {
                UserWithID user = new UserWithID() {
                    Username = reader.ReadString(),
                    ID = reader.ReadInt32(),
                    groupID = reader.ReadInt32()
                };
                Network.RegisteredUsers.Add(user);
            }
        }
        #endregion

        #region Regions Add/Remove Users/Groups
        public static void RequestAddPlayerToRegion(Region region, int playerID) {
            WriteHeader(MessageType.RequestAddPlayerToRegion);
            Writer.Write(region.ID);
            Writer.Write(playerID);
            Network.SendDataToServer();
        }

        private static void ProcessAddPlayerToRegionRequest(ref BinaryReader reader, int playerNumber) {
            if (Network.Players2[playerNumber].Group.IsAdmin) {
                Region region = Network.GetRegionByID(reader.ReadInt32());
                if (region == null) return;
                int playerID = reader.ReadInt32();
                if (region.AddPlayer(playerID)) {
                    DatabaseController.WriteRegionPermissions(region);
                    SendRegionListToAllPlayers();
                }
            }
        }

        public static void RequestRemovePlayerFromRegion(Region region, int playerID) {
            WriteHeader(MessageType.RequestRemovePlayerFromRegion);
            Writer.Write(region.ID);
            Writer.Write(playerID);
            Network.SendDataToServer();
        }

        private static void ProcessRemovePlayerFromRegionRequest(ref BinaryReader reader, int playerNumber) {
            if (Network.Players2[playerNumber].Group.IsAdmin) {
                Region region = Network.GetRegionByID(reader.ReadInt32());
                if (region == null) return;
                int playerID = reader.ReadInt32();
                if (region.RemovePlayer(playerID)) {
                    DatabaseController.WriteRegionPermissions(region);
                    SendRegionListToAllPlayers();
                }
            }
        }

        public static void RequestAddGroupToRegion(Region region, int groupID) {
            WriteHeader(MessageType.RequestAddGroupToRegion);
            Writer.Write(region.ID);
            Writer.Write(groupID);
            Network.SendDataToServer();
        }

        private static void ProcessAddGroupToRegionRequest(ref BinaryReader reader, int playerNumber) {
            if (Network.Players2[playerNumber].Group.IsAdmin) {
                Region region = Network.GetRegionByID(reader.ReadInt32());
                if (region == null) return;
                int groupID = reader.ReadInt32();
                if (region.AddGroup(groupID)) {
                    DatabaseController.WriteRegionPermissions(region);
                    SendRegionListToAllPlayers();
                }
            }
        }

        public static void RequestRemoveGroupFromRegion(Region region, int groupID) {
            WriteHeader(MessageType.RequestRemoveGroupFromRegion);
            Writer.Write(region.ID);
            Writer.Write(groupID);
            Network.SendDataToServer();
        }

        private static void ProcessRemoveGroupFromRegionRequest(ref BinaryReader reader, int playerNumber) {
            if (Network.Players2[playerNumber].Group.IsAdmin) {
                Region region = Network.GetRegionByID(reader.ReadInt32());
                if (region == null) return;
                int groupID = reader.ReadInt32();
                if (region.RemoveGroup(groupID)) {
                    DatabaseController.WriteRegionPermissions(region);
                    SendRegionListToAllPlayers();
                }
            }
        }

        public static void RequestToChangeColorOfRegion(Region region, Color color) {
            WriteHeader(MessageType.RequestChangeRegionColor);
            Writer.Write(region.ID);
            Writer.WriteRGB(color);
            Network.SendDataToServer();
        }

        private static void ProcessChangeRegionColorRequest(ref BinaryReader reader, int playerNumber) {
            if (Network.Players2[playerNumber].Group.IsAdmin) {
                Region region = Network.GetRegionByID(reader.ReadInt32());
                if (region == null) return;
                Color color = reader.ReadRGB();

                region.Color = color;
                DatabaseController.WriteRegionColor(region);
                SendRegionListToAllPlayers();
            }
        }
        #endregion

        #region Restore Tiles
        public static void RequestRestoreTiles(int playerID, bool onlinePlayer) {
            WriteHeader(MessageType.RequestRestoreTiles);

            Writer.Write(playerID);
            Writer.Write(onlinePlayer);
            Network.SendDataToServer();
        }

        private static void ProcessRestoreTilesRequest(ref BinaryReader reader, int playerNumber) {
            if (Network.Players2[playerNumber].Group.IsAdmin) {
                int playerID = reader.ReadInt32();
                bool onlinePlayer = reader.ReadBoolean();
                if (onlinePlayer) {
                    playerID = Network.Players2[playerID].ID;
                }
                TileChangeController.RestoreTileChangesMadeByPlayer(playerID);
            }
        }
        #endregion

        #region Set Spawnpoint
        public static void RequestSetSpawnPoint() {
            WriteHeader(MessageType.RequestSetSpawnPoint);
            Network.SendDataToServer();
        }

        private static void ProcessSetSpawnPointRequest(int playerNumber) {
            if (Network.Players2[playerNumber].Group.IsAdmin) {
                Player player = Main.player[playerNumber];

                Main.spawnTileX = (int) (player.position.X - 8 + player.width / 2) / 16;
                Main.spawnTileY = (int) (player.position.Y + player.height) / 16;

                Network.SendTextToAllPlayers(string.Format("Spawn Point set to X:{0} Y:{1} by {2}", Main.spawnTileX, Main.spawnTileY, player.name));
            }
        }
        #endregion

        #region Toggle Gravestones
        public static void RequestToggleGravestones() {
            WriteHeader(MessageType.RequestToggleGravestones);
            Network.SendDataToServer();
        }

        private static void ProcessToggleGravestonesRequest(int playerNumber) {
            if (Network.Players2[playerNumber].Group.HasPermission("ToggleGravestones")) {
                Network.GravestonesAllowed = !Network.GravestonesAllowed;
                if (Network.GravestonesAllowed) {
                    Network.SendTextToAllPlayers("Gravestones were enabled by " + Main.player[playerNumber].name);
                } else {
                    Network.SendTextToAllPlayers("Gravestones were disabled by " + Main.player[playerNumber].name);
                }
                GravestonesToggled();
            }
        }

        private static void GravestonesToggled() {
            WriteHeader(MessageType.GravestonesToggled);
            Writer.Write(Network.GravestonesAllowed);
            Network.SendDataToAllHEROsModUsers();
        }

        private static void ProcessGravestonesToggled(ref BinaryReader reader) {
            if (Network.NetworkMode == NetworkMode.Server) return;
            bool gravestonesAllowed = reader.ReadBoolean();
            GravestonesToggleByServer?.Invoke(gravestonesAllowed);
        }
        #endregion

        #region God Mode
        public static void RequestGodMode() {
            WriteHeader(MessageType.RequestGodMode);
            Network.SendDataToServer();
        }

        private static void ProcessGodModeRequest(int playerNumber) {
            if (Network.Players2[playerNumber].Group.HasPermission("GodMode")) {
                AllowGodMode(playerNumber);
            }
        }

        private static void AllowGodMode(int playerNumber) {
            WriteHeader(MessageType.AllowGodMode);
            Network.SendDataToPlayer(playerNumber);
        }

        private static void ProcessGodMode() {
            if (Network.NetworkMode == NetworkMode.Server) return;
            if (!HEROsModServices.GodModeService.Enabled) {
                HEROsModServices.GodModeService.Enabled = true;
            }
        }
        #endregion

        #region Tile Modification Check
        public static void RequestTileModificationCheck(Vector2 tileCoords) {
            WriteHeader(MessageType.RequestTileModificationCheck);
            Writer.Write((int) tileCoords.X);
            Writer.Write((int) tileCoords.Y);
            Network.SendDataToServer();
        }

        private static void ProcessTileModificationCheckRequest(ref BinaryReader reader, int playerNumber) {
            if (Network.Players2[playerNumber].Group.HasPermission("CheckTiles")) {
                int x = reader.ReadInt32();
                int y = reader.ReadInt32();

                if (x >= 0 && y >= 0 && x < Main.maxTilesX && y < Main.maxTilesY) {
                    int playerID = Network.TileLastChangedBy[x, y];
                    if (playerID >= 0) {
                        UserWithID user = null;
                        UserWithID[] users = DatabaseController.GetRegisteredUsers();
                        for (int i = 0; i < users.Length; i++) {
                            if (users[i].ID == playerID) {
                                user = users[i];
                            }
                        }
                        if (user != null) {
                            Network.SendTextToPlayer("Tile last modified by " + user.Username, playerNumber);
                        }
                    } else {
                        Network.SendTextToPlayer("This tile has not been modified since last server boot up.", playerNumber);
                    }
                }
            } else {
                Network.SendTextToPlayer("You don't have permission to do that.", playerNumber);
            }
        }
        #endregion

        #region Spawn NPC
        public static void RequestSpawnNPC(int npcType) {
            WriteHeader(MessageType.RequestSpawnNPC);
            Writer.Write(npcType);
            Network.SendDataToServer();
        }

        private static void ProcessSpawnNPCRequest(ref BinaryReader reader, int playerNumber) {
            HEROsModPlayer player = Network.Players2[playerNumber];
            if (player.Group.HasPermission("SpawnNPCs")) {
                int npcType = reader.ReadInt32();
                NPC newNPC = new NPC();
                newNPC.SetDefaults(npcType);

                if (newNPC.townNPC || HEROsModServices.NPCStats.boundNPC.Contains(npcType)) {
                    for (int i = 0; i < Main.npc.Length; i++) {
                        NPC npc = Main.npc[i];
                        if (npc.type == newNPC.type) {
                            npc.Teleport(player.GameInstance.position, 1);
                            return;
                        }
                    }
                }
                NPC.NewNPC((int) player.GameInstance.position.X, (int) player.GameInstance.position.Y, npcType);
            }
        }
        #endregion

        #region Invasion Events
        public static void RequestStartEvent(HEROsModServices.Events e) {
            WriteHeader(MessageType.RequestStartEvent);
            Writer.Write((byte) e);
            Network.SendDataToServer();
        }

        private static void ProcessStartEventRequest(ref BinaryReader reader, int playerNumber) {
            if (Network.Players2[playerNumber].Group.HasPermission("StartEvents")) {
                HEROsModServices.Events e = (HEROsModServices.Events) reader.ReadByte();
                HEROsModServices.InvasionService.StartEvent(e);
            }
        }

        public static void RequestStopEvents() {
            WriteHeader(MessageType.RequestStopEvents);
            Network.SendDataToServer();
        }

        private static void ProcessStopEventsRequest(int playerNumber) {
            if (Network.Players2[playerNumber].Group.HasPermission("StartEvents")) {
                HEROsModServices.InvasionService.StopAllEvents();
                Network.SendTextToAllPlayers("All events have been stopped by " + Network.Players2[playerNumber].GameInstance.name);
            }
        }
        #endregion

        #region Toggle Banned Items
        public static void RequestToggleBannedItems() {
            WriteHeader(MessageType.RequestToggleBannedItems);
            Network.SendDataToServer();
        }

        private static void ProcessToggleBannedItemsRequest(int playerNumber) {
            if (Network.Players2[playerNumber].Group.HasPermission("ToggleBannedItems")) {
                HEROsModServices.ItemBanner.ItemsBanned = !HEROsModServices.ItemBanner.ItemsBanned;
                if (HEROsModServices.ItemBanner.ItemsBanned) {
                    Network.SendTextToAllPlayers("Destructive Explosives banned by " + Network.Players2[playerNumber].ServerInstance.Name);
                } else {
                    Network.SendTextToAllPlayers("Destructive Explosives unbanned by " + Network.Players2[playerNumber].ServerInstance.Name);
                }
                BannedItemsToggled();
            }
        }

        private static void BannedItemsToggled() {
            WriteHeader(MessageType.BannedItemsToggled);
            Writer.Write(HEROsModServices.ItemBanner.ItemsBanned);
            Network.SendDataToAllHEROsModUsers();
        }

        private static void ProcessBannedItemsToggled(ref BinaryReader reader) {
            if (Network.NetworkMode == NetworkMode.Server) return;
            bool gravestonesAllowed = reader.ReadBoolean();
            ItemBannerToggleByServer?.Invoke(gravestonesAllowed);
        }
        #endregion

        #region Teleport
        public static void RequestTeleport(Vector2 destination) {
            WriteHeader(MessageType.RequestTeleport);
            Writer.WriteVector2(destination);
            Network.SendDataToServer();
        }
        private static void ProcessRequestTeleport(ref BinaryReader reader, int playerNumber) {
            if (Network.Players2[playerNumber].Group.HasPermission("Teleport")) {
                Vector2 destination = reader.ReadVector2();
                Main.player[playerNumber].Teleport(destination, 1, 0);
                RemoteClient.CheckSection(playerNumber, destination, 1);
                NetMessage.SendData(65, -1, -1, null, 0, playerNumber, destination.X, destination.Y, 1, 0, 0);
            }
        }
        #endregion

        #region Toggle Hardmode Enemies

        public static void RequestToggleHardmodeEnemies() {
            WriteHeader(MessageType.RequestToggleHardmodeEnemies);
            Network.SendDataToServer();
        }


        private static void ProcessToggleHardmodeEnemiesRequest(int playerNumber) {
            if (Network.Players2[playerNumber].Group.HasPermission("ToggleHardmodeEnemies")) {
                HEROsModServices.HardmodeEnemyToggler.ToggleHardModeEnemies();
                if (Main.hardMode) {
                    Network.SendTextToAllPlayers("Hardmode enemies enabled by " + Network.Players2[playerNumber].GameInstance.name);
                } else {
                    Network.SendTextToAllPlayers("Hardmode enemies disabled by " + Network.Players2[playerNumber].GameInstance.name);
                }
            }
        }

        #endregion

        #region Purify World

        public static void RequestPurifyWorld() {
            WriteHeader(MessageType.PurifyWorld);
            Network.SendDataToServer();
        }


        private static void ProcessPurifyWorldRequest(int playerNumber) {
            if (Network.Players2[playerNumber].Group.HasPermission("PurifyWorld")) {
                Network.SendTextToAllPlayers("Please wait, the world is going to be purified, this may take a few moments to minutes...", Color.Crimson);
                HEROsModServices.WorldPurifier.PurifyWorld();
                for (int i = 0; i < Main.player.Length; i++) {
                    if (Main.player[i].active) {
                        Network.ResendPlayerAllTileData(Network.Players2[i]);
                    }
                }
                Network.SendTextToAllPlayers("Purifying the world complete!", Color.Crimson);
            }
        }

        #endregion

        private enum MessageType : byte {
            UsingHEROsMod,
            RequestTimeChange,
            TimePausedOrResumed,
            RequestClearGroundItems,
            RequestToggleEnemies,
            EnemiesToggled,
            RequestSpawnTownNPC,
            RequestStartRain,
            RequestStopRain,
            RequestStartSandstorm,
            RequestStopSandstorm,
            WaypointList,
            RequestAddWaypoint,
            RequestRemoveWaypoint,
            PlayerJoined,
            PlayerLeft,
            RegionList,
            RequestCreateRegion,
            RequestRemoveRegion,
            RequestRegisteredUsers,
            RegisteredUsers,
            RequestAddPlayerToRegion,
            RequestRemovePlayerFromRegion,
            RequestAddGroupToRegion,
            RequestChangeRegionColor,
            RequestRemoveGroupFromRegion,
            RequestRestoreTiles,
            RequestSetSpawnPoint,
            RequestToggleGravestones,
            GravestonesToggled,
            RequestToggleHardmodeEnemies,
            RequestGodMode,
            AllowGodMode,
            RequestTileModificationCheck,
            RequestSpawnNPC,
            RequestStartEvent,
            RequestStopEvents,
            RequestToggleBannedItems,
            BannedItemsToggled,
            RequestKickPlayer,
            RequestBanPlayer,
            RequestTeleport,
            RequestForcedSundial,
            CurrentToggles,
            ServerPlayerJoined,
            ServerPlayerLeft,
            PurifyWorld
        }

        public enum TimeChangeType {
            SetToNoon,
            SetToNight,
            SetToMidnight,
            Pause,
            SpecificTime
        }
    }
}