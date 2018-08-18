using HEROsMod.HEROsModServices;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Terraria;

// TODO, save position of ui windows, save separate client json

namespace HEROsMod.HEROsModNetwork
{
    // Database should only be on the server or single. Data should be accessed from their respective member variables.
    public class HEROsModDatabase
    {
        // Global to the Server config
        public List<DatabasePlayer> players;

        public List<DatabaseGroup> groups;

        // Specific to world
        public List<DatabaseWorld> worlds;
    }

    public class DatabasePlayer
    {
        public int ID;
        public string username;
        public string password;
        public int group;
    }

    public class DatabaseGroup
    {
        public int ID;
        public string name;
        public string[] permissions;
    }

    public class DatabaseWorld
    {
        public int worldID;
        public string name;
        public bool BanDestructiveExplosives;
        public bool TimePaused;
        public double TimePausedTime;

        //public bool TimePausedIsDay;
        public bool GraveStonesDisabled;

        public bool NPCSpawnsDiabled;
        public List<DatabaseRegion> regions = new List<DatabaseRegion>();
        public List<DatabaseWaypoint> waypoints = new List<DatabaseWaypoint>();
    }

    public class DatabaseRegion
    {
        public int ID;
        public string name;
        public int x;
        public int y;
        public int width;
        public int height;
        public int owner;
        public Color color;
        public int[] permissionsGroups;
        public int[] permissionsPlayers;
    }

    public class DatabaseWaypoint
    {
        public string name;
        public float x;
        public float y;
    }

    internal class DatabaseController
    {
        private static readonly string jsonDatabaseFilename = "HEROsModDatabase";

        private static HEROsModDatabase database;
        private static DatabaseWorld currentDatabaseWorld;
        private static SHA512 md5hash = SHA512.Create();

        public static void LoadSetting(string settingName)
        {
            ModUtils.DebugText("LoadSetting");
            Directory.CreateDirectory(Main.SavePath);
            string path = string.Concat(new object[]
                {
                    Main.SavePath,
                    Path.DirectorySeparatorChar,
                    settingName,
                    ".json"
                });
            if (File.Exists(path))
            {
                ModUtils.DebugText("LoadSetting File Exists");
                using (StreamReader r = new StreamReader(path))
                {
                    string json = r.ReadToEnd();
                    database = JsonConvert.DeserializeObject<HEROsModDatabase>(json);
                }
            }
            else
            {
                ModUtils.DebugText("LoadSetting File Doesn't Exist");
            }
            if (database == null)
            {
                ModUtils.DebugText("Warning: Database null in LoadSetting");
                database = new HEROsModDatabase();
            }
            if (database.players == null)
            {
                ModUtils.DebugText("Warning: Database players null in LoadSetting");
                database.players = new List<DatabasePlayer>();
            }
            if (database.worlds == null)
            {
                ModUtils.DebugText("Warning: Database worlds null in LoadSetting");
                database.worlds = new List<DatabaseWorld>();
            }
            if (database.groups == null)
            {
                ModUtils.DebugText("Warning: Database groups null in LoadSetting");
                database.groups = new List<DatabaseGroup>();
            }
        }

		internal static void SaveSetting() => SaveSetting(jsonDatabaseFilename);

		public static void SaveSetting(string settingName)
        {
            if (!Main.dedServ && Main.netMode == 2)
            {
                ModUtils.DebugText("WARNING: non ded client saving");
            }
            ModUtils.DebugText("SaveSetting");
            if (currentDatabaseWorld != null)
            {
                currentDatabaseWorld.waypoints.Clear();
                foreach (Waypoint waypoint in Waypoints.points)
                {
                    currentDatabaseWorld.waypoints.Add(new DatabaseWaypoint() { name = waypoint.name, x = waypoint.position.X, y = waypoint.position.Y });
                }
                currentDatabaseWorld.GraveStonesDisabled = !Network.GravestonesAllowed;
                currentDatabaseWorld.BanDestructiveExplosives = ItemBanner.ItemsBanned;
                currentDatabaseWorld.NPCSpawnsDiabled = !EnemyToggler.EnemiesAllowed;
                currentDatabaseWorld.TimePaused = TimeWeatherChanger.TimePaused;
                if (currentDatabaseWorld.TimePaused)
                {
                    currentDatabaseWorld.TimePausedTime = TimeWeatherChanger.PausedTime;
                    //	currentDatabaseWorld.TimePausedIsDay = TimeWeatherChanger.PausedTimeDayTime;
                }
            }

            Directory.CreateDirectory(Main.SavePath);
            string path = string.Concat(new object[]
                {
                    Main.SavePath,
                    Path.DirectorySeparatorChar,
                    settingName,
					".json"
                });
            string json = JsonConvert.SerializeObject(database, Formatting.Indented);
            File.WriteAllText(path, json);
        }

        public static void InitializeWorld()
        {
            foreach (DatabaseWorld world in database.worlds)
            {
                if (world.worldID == Main.worldID)
                {
                    currentDatabaseWorld = world;
                }
            }
            if (currentDatabaseWorld == null)
            {
                currentDatabaseWorld = new DatabaseWorld() { worldID = Main.worldID, name = Main.worldName };
                database.worlds.Add(currentDatabaseWorld);
                SaveSetting();
            }
            Waypoints.ClearPoints();
            foreach (DatabaseWaypoint waypoint in currentDatabaseWorld.waypoints)
            {
                Waypoints.AddWaypoint(waypoint.name, new Vector2(waypoint.x, waypoint.y));
            }
            Network.GravestonesAllowed = !currentDatabaseWorld.GraveStonesDisabled;
            ItemBanner.ItemsBanned = currentDatabaseWorld.BanDestructiveExplosives;
            EnemyToggler.EnemiesAllowed = !currentDatabaseWorld.NPCSpawnsDiabled;
            TimeWeatherChanger.TimePaused = currentDatabaseWorld.TimePaused;
            if (TimeWeatherChanger.TimePaused)
            {
                TimeWeatherChanger.PausedTime = currentDatabaseWorld.TimePausedTime;
                //	TimeWeatherChanger.PausedTimeDayTime = currentDatabaseWorld.TimePausedIsDay;
            }
            if (Main.netMode == 0)
            {
                GeneralMessages.ProcessCurrentTogglesSP(EnemyToggler.EnemiesAllowed, Network.GravestonesAllowed, ItemBanner.ItemsBanned, TimeWeatherChanger.TimePaused);
            }
        }

        public static void Init()
        {
            ResetDatabase();
            LoadSetting(jsonDatabaseFilename);
            if (!HasDefaultGroup())
            {
                Console.WriteLine("No Default group");
                Group defaultGroup = new Group("Default");
                AddGroup(ref defaultGroup);
            }
        }

        private static void ResetDatabase()
        {
            database = new HEROsModDatabase()
            {
                players = new List<DatabasePlayer>(),
                groups = new List<DatabaseGroup>(),
                worlds = new List<DatabaseWorld>()
            };
            currentDatabaseWorld = null;
        }

        public static bool Login(ref string username, string password, ref int playerID, ref int groupID)
        {
            foreach (DatabasePlayer user in database.players)
            {

                if (user.username.ToLower() == username.ToLower() && user.password == BitConverter.ToString(md5hash.ComputeHash(password.ToByteArray())))
                {
                    user.password = password;
                    username = user.username;
                    playerID = user.ID;
                    groupID = user.group;
                    return true;
                } else if (user.username.ToLower() == username.ToLower() && user.password == password) {
                    //user.password = BitConverter.ToString(md5hash.ComputeHash(password.ToByteArray()));
                    username = user.username;
                    playerID = user.ID;
                    groupID = user.group;
                    return true;
                }
                
            }
            return false;
        }

        public static RegistrationResult Register(string username, string password)
        {
            if (database.players.Any(x => x.username == username.ToLower()))
            {
                return RegistrationResult.UsernameTaken;
            }
            if (database.players.Count == 0)
            {
                database.players.Add(
                    new DatabasePlayer() { username = username, password = password /*BitConverter.ToString(md5hash.ComputeHash(password.ToByteArray()))*/, ID = GetAvailablePlayerID(), group = -1 }
                );
            }
            else
            {
                database.players.Add(
                    new DatabasePlayer() { username = username, password = password /*BitConverter.ToString(md5hash.ComputeHash(password.ToByteArray()))*/, ID = GetAvailablePlayerID() }
                );
            }
            SaveSetting(jsonDatabaseFilename);
            Network.RegisteredUsers.AddRange(GetRegisteredUsers());
            return RegistrationResult.Sucess;
        }

        private static int GetAvailablePlayerID()
        {
            int next = 0;
            foreach (DatabasePlayer item in database.players)
            {
                if (item.ID >= next)
                {
                    next = item.ID + 1;
                }
            }
            return next;
        }

        private static int GetAvailableGroupID()
        {
            int next = 0;
            foreach (DatabaseGroup item in database.groups)
            {
                if (item.ID >= next)
                {
                    next = item.ID + 1;
                }
            }
            return next;
        }

        private static int GetAvailableRegionID()
        {
            int next = 0;
            foreach (DatabaseRegion item in currentDatabaseWorld.regions)
            {
                if (item.ID >= next)
                {
                    next = item.ID + 1;
                }
            }
            return next;
        }

		public static UserWithID[] GetRegisteredUsers() => database.players.Select((x) => new UserWithID() { Username = x.username, ID = x.ID, groupID = x.group }).ToArray();

		public static void SetPlayerGroup(int playerID, int groupID)
        {
            DatabasePlayer p = database.players.Where(x => x.ID == playerID).FirstOrDefault();
            if (p != null)
            {
                p.group = groupID;
            }
            SaveSetting(jsonDatabaseFilename);
        }

		private static bool HasDefaultGroup() => database.groups.Any(x => x.name == "Default");

		public static void AddGroup(ref Group group)
        {
            int newid = GetAvailableGroupID();
            DatabaseGroup newGroup = new DatabaseGroup() { name = group.Name, ID = newid };
            database.groups.Add(newGroup);

            group.ID = newid;
            SetGroupPermissions(group);
            SaveSetting(jsonDatabaseFilename);
        }

        public static void DeleteGroup(Group group)
        {
            DatabaseGroup databaseGroup = database.groups.Where(x => x.ID == group.ID).FirstOrDefault();
            if (databaseGroup != null)
            {
                foreach (DatabasePlayer player in database.players)
                {
                    if (player.group == databaseGroup.ID)
                    {
                        player.group = Network.DefaultGroup.ID;
                    }
                }
                database.groups.Remove(databaseGroup);
            }
            SaveSetting(jsonDatabaseFilename);
        }

        public static void SetGroupPermissions(Group group)
        {
            DatabaseGroup g = database.groups.Where(x => group.Name == x.name).FirstOrDefault();
            if (g != null)
            {
                g.permissions = group.Permissions.Where(x => x.Value).Select(x => x.Key).ToArray();//group.ExportPermissions();
            }
            SaveSetting(jsonDatabaseFilename);
        }

        public static List<Group> GetGroups()
        {
            List<Group> result = new List<Group>();
            foreach (DatabaseGroup dbGroup in database.groups)
            {
                Group group = new Group(dbGroup.name)
                {
                    ID = dbGroup.ID
                };
                group.ImportPermissions(dbGroup.permissions);
                result.Add(group);
            }
            return result;
        }

        public static List<Region> GetRegions()
        {
            List<Region> result = new List<Region>();
            foreach (DatabaseRegion dbRegion in currentDatabaseWorld.regions)
            {
                Region region = new Region(dbRegion.name, dbRegion.x, dbRegion.y, dbRegion.width, dbRegion.height, dbRegion.owner);
                region.ImportPermissions(dbRegion.permissionsGroups, dbRegion.permissionsPlayers);
                region.ID = dbRegion.ID;

                region.Color = dbRegion.color;
                result.Add(region);
            }
            return result;
        }

        public static void AddRegion(ref Region region, ref int owner)
        {
            int newid = GetAvailableRegionID();

            DatabaseRegion dbRegion = new DatabaseRegion() {
                ID = newid,
                name = region.Name,
                x = region.X,
                y = region.Y,
                width = region.Width,
                height = region.Height,
                color = region.Color,
                permissionsPlayers = region.AllowedPlayersIDs.ToArray(),
                permissionsGroups = region.AllowedGroupsIDs.ToArray(),
                owner = owner
            };
            region.ID = dbRegion.ID;
            currentDatabaseWorld.regions.Add(dbRegion);
            SaveSetting(jsonDatabaseFilename);
        }

        public static void WriteRegionPermissions(Region region)
        {
            DatabaseRegion r = currentDatabaseWorld.regions.Where(x => region.ID == x.ID).FirstOrDefault();
            if (r != null)
            {
                r.permissionsPlayers = region.AllowedPlayersIDs.ToArray();
                r.permissionsGroups = region.AllowedGroupsIDs.ToArray();
            }
            SaveSetting(jsonDatabaseFilename);
        }

        public static void WriteRegionColor(Region region)
        {
            DatabaseRegion r = currentDatabaseWorld.regions.Where(x => region.ID == x.ID).FirstOrDefault();
            if (r != null)
            {
                r.color = region.Color;
            }
            SaveSetting(jsonDatabaseFilename);
        }

        public static void RemoveRegion(Region region)
        {
            DatabaseRegion databaseRegion = currentDatabaseWorld.regions.Where(x => x.ID == region.ID).FirstOrDefault();
            if (databaseRegion != null)
            {
                currentDatabaseWorld.regions.Remove(databaseRegion);
            }
            SaveSetting(jsonDatabaseFilename);
        }

        public enum RegistrationResult
        {
            Error,
            UsernameTaken,
            Sucess
        }
    }
}