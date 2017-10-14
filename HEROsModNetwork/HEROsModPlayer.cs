using System;

using Terraria;

namespace HEROsMod.HEROsModNetwork
{
    public class HEROsModPlayer
    {
        private int _playerIndex = -1;

        public int Index
        {
            get { return _playerIndex; }
        }

        public RemoteClient ServerInstance
        {
            get
            {
                return Netplay.Clients[_playerIndex];
            }
        }

        public Player GameInstance
        {
            get
            {
                return Main.player[_playerIndex];
            }
        }

        public int ID { get; set; }
        public Group Group { get; set; }
        public bool UsingHEROsMod { get; set; }
        public string Username { get; set; }
        public bool BackupHostility { get; set; }
        public int BackupTeam { get; set; }
        
        public HEROsModPlayer(int playerIndex)
        {
            Reset();
            _playerIndex = playerIndex;
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