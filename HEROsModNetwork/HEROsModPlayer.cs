using System;

using Terraria;

namespace HEROsMod.HEROsModNetwork
{
    public class HEROsModPlayer
    {

		public int Index { get; } = -1;

		public RemoteClient ServerInstance => Netplay.Clients[Index];

		public Player GameInstance => Main.player[Index];

		public int ID { get; set; }
        public Group Group { get; set; }
        public bool UsingHEROsMod { get; set; }
        public string Username { get; set; }
        public bool BackupHostility { get; set; }
        public int BackupTeam { get; set; }
        
        public HEROsModPlayer(int playerIndex)
        {
            Reset();
            Index = playerIndex;
        }

        public void Reset()
        {
            Username = string.Empty;
            UsingHEROsMod = false;
            Group = Network.DefaultGroup;
        }
    }

    public class UserWithID
    {
        public string Username = string.Empty;
        public int ID = -1;
        public int groupID = -2;
    }
}