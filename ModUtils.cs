using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ModLoader;

namespace HEROsMod
{
    internal static class ModUtils
    {
        private static MethodInfo _drawPlayerHeadMethod;
        private static MethodInfo _loadPlayersMethod;
        private static MethodInfo _startRainMethod;
        private static MethodInfo _stopRainMethod;
        private static MethodInfo _startSandstormMethod;
        private static MethodInfo _stopSandstormMethod;
        
        private static MethodInfo _mouseTextMethod;

        private static MethodInfo _invasionWarningMethod;
        private static MethodInfo _itemSortingSortMethod;
        private static FieldInfo _npcDefaultSpawnRate;
        private static FieldInfo _npcDefaultMaxSpawns;
        
        private static FieldInfo _hueTexture;

        private static Texture2D _dummyTexture;
        private static float _deltaTime;

        private static Item[] previousInventoryItems;

        public static event EventHandler InventoryChanged;

        public static bool InterfaceVisible { get; set; }

        /// <summary>
        /// A 1x1 pixel white texture.
        /// </summary>
        public static Texture2D DummyTexture
        {
            get
            {
                if (_dummyTexture == null)
                {
                    _dummyTexture = new Texture2D(Main.instance.GraphicsDevice, 1, 1);
                    _dummyTexture.SetData(new Color[] { Color.White });
                }
                return _dummyTexture;
            }
        }

        public static KeyboardState PreviousKeyboardState { get; set; }
        public static MouseState MouseState { get; set; }
        public static MouseState PreviousMouseState { get; set; }

        /// <summary>
        /// Time in seconds that has passed since the last update call.
        /// </summary>
        public static float DeltaTime
        {
            get { return _deltaTime; }
        }

        public static int NPCDefaultSpawnRate
        {
            get { return (int)_npcDefaultSpawnRate.GetValue(null); }
            set { _npcDefaultSpawnRate.SetValue(null, value); }
        }

        public static int NPCDefaultMaxSpawns
        {
            get { return (int)_npcDefaultMaxSpawns.GetValue(null); }
            set { _npcDefaultMaxSpawns.SetValue(null, value); }
        }

        public static Texture2D HueTexture
        {
            get
            {
                return (Texture2D)_hueTexture.GetValue(Main.instance);
            }
        }

        public static Item HoverItem
        {
            get { return Main.HoverItem; }
            set { Main.HoverItem = value; }
        }

        /// <summary>
        /// Gets or Sets if the game camera is free to move from the players position
        /// </summary>
        public static bool FreeCamera { get; set; }

        public static NetworkMode NetworkMode
        {
            get
            {
                return (NetworkMode)Main.netMode;
            }
        }

        public static void Init()
        {
            InitReflection();
            InterfaceVisible = true;

            if (NetworkMode != NetworkMode.Server)
            {
                FreeCamera = false;

                previousInventoryItems = new Item[Main.player[Main.myPlayer].inventory.Length];
                SetPreviousInventory();
                Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -10000, 10000);
            }
        }

        private static void InitReflection()
        {
            try
            {
                //	Main.DrawPlayerHead
                _drawPlayerHeadMethod = Main.instance.GetType().GetMethod("DrawPlayerHead", BindingFlags.NonPublic | BindingFlags.Instance);
                _loadPlayersMethod = typeof(Main).GetMethod("LoadPlayers", BindingFlags.NonPublic | BindingFlags.Static);
                _startRainMethod = typeof(Main).GetMethod("StartRain", BindingFlags.NonPublic | BindingFlags.Static);
                _stopRainMethod = typeof(Main).GetMethod("StopRain", BindingFlags.NonPublic | BindingFlags.Static);
                _startSandstormMethod = typeof(Sandstorm).GetMethod("StartSandstorm", BindingFlags.NonPublic | BindingFlags.Static);
                _stopSandstormMethod = typeof(Sandstorm).GetMethod("StopSandstorm", BindingFlags.NonPublic | BindingFlags.Static);
                _mouseTextMethod = typeof(Main).GetMethod("MouseText", BindingFlags.NonPublic | BindingFlags.Instance);
                _invasionWarningMethod = typeof(Main).GetMethod("InvasionWarning", BindingFlags.NonPublic | BindingFlags.Static);
                _npcDefaultSpawnRate = typeof(NPC).GetField("defaultSpawnRate", BindingFlags.NonPublic | BindingFlags.Static);
                _npcDefaultMaxSpawns = typeof(NPC).GetField("defaultMaxSpawns", BindingFlags.NonPublic | BindingFlags.Static);
                
                _hueTexture = Main.instance.GetType().GetField("hueTexture", BindingFlags.NonPublic | BindingFlags.Instance); // private
                 Assembly terraria = Assembly.GetAssembly(typeof(Main));
                _itemSortingSortMethod = terraria.GetType("Terraria.UI.ItemSorting").GetMethod("Sort", BindingFlags.Public | BindingFlags.Static);
            }
            catch (Exception e)
            {
                DebugText(e.Message + " " + e.StackTrace);
            }
        }

        public static void Update()
        {
            if (!Main.gameMenu)
            {
                if (ItemChanged())
                {
                    InventoryChanged?.Invoke(null, EventArgs.Empty);
                    SetPreviousInventory();
                }
            }
        }

        private static bool ItemChanged()
        {
            Player player = Main.player[Main.myPlayer];
            for (int i = 0; i < player.inventory.Length - 1; i++)
            {
                if (player.inventory[i].IsNotTheSameAs(previousInventoryItems[i]))
                {
                    return true;
                }
            }
            return false;
        }

        private static void SetPreviousInventory()
        {
            Player player = Main.player[Main.myPlayer];
            for (int i = 0; i < player.inventory.Length; i++)
            {
                previousInventoryItems[i] = player.inventory[i].Clone();
            }
        }

        /// <summary>
        /// Draw the head of a player on screen
        /// </summary>
        /// <param name="player">Player who's head is to be drawn</param>
        /// <param name="x">X Draw Pos</param>
        /// <param name="y">Y Draw Pos</param>
        /// <param name="alpha">Draw Alpha</param>
        /// <param name="scale">Draw Scale</param>
        public static void DrawPlayerHead(Player player, float x, float y, float alpha = 1f, float scale = 1f)
        {
            _drawPlayerHeadMethod.Invoke(Main.instance, new object[] { player, x, y, alpha, scale });
        }

        public static void LoadPlayers()
        {
            _loadPlayersMethod.Invoke(null, null);
        }

        public static void StartRain()
        {
            _startRainMethod.Invoke(null, null);
        }

        public static void StopRain()
        {
            _stopRainMethod.Invoke(null, null);
        }

        public static void StartSandstorm()
        {
            _startSandstormMethod.Invoke(null, null);
        }

        public static void StopSandstorm()
        {
            _stopSandstormMethod.Invoke(null, null);
        }

        public static void LoadNPC(int i)
        {
            Main.instance.LoadNPC(i);
        }

        public static void LoadProjectile(int i)
        {
            Main.instance.LoadProjectile(i);
        }

        public static void LoadTiles(int i)
        {
            Main.instance.LoadTiles(i);
        }

        public static void MouseText(string cursorText, int rare = 0, byte diff = 0)
        {
            _mouseTextMethod.Invoke(Main.instance, new object[] { cursorText, rare, diff });
        }

        public static void InvasionWarning()
        {
            _invasionWarningMethod.Invoke(null, null);
        }

        public static void Sort()
        {
            _itemSortingSortMethod.Invoke(null, null);
        }

        public static void MoveToPosition(Vector2 newPos)
        {
            Player player = Main.player[Main.myPlayer];
            player.position = newPos;
            player.velocity = Vector2.Zero;
            player.fallStart = (int)(player.position.Y / 16f);
        }

        /// <summary>
        /// Set the Delta Time
        /// </summary>
        /// <param name="gameTime">Games current Game Time</param>
        public static void SetDeltaTime(/*GameTime gameTime*/)
        {
            _deltaTime = 1f / 60f;
        }

        public static void SetDeltaTime(float deltaTime)
        {
            _deltaTime = deltaTime;
        }

        public static bool StringStartsWith(string str, string startStr)
        {
            if (str.Length >= startStr.Length)
            {
                if (str.Substring(0, startStr.Length) == startStr) return true;
            }
            return false;
        }

        public static string GetEndOfString(string str, string startStr)
        {
            return str.Substring(startStr.Length, str.Length - startStr.Length);
        }

        public static Vector2 CursorPosition
        {
            get
            {
                return new Vector2(Main.mouseX, Main.mouseY);
            }
        }

        public static Vector2 CursorWorldCoords
        {
            get
            {
                return CursorPosition + Main.screenPosition;
            }
        }

        public static Vector2 CursorTileCoords
        {
            get
            {
                return GetTileCoordsFromWorldCoords(CursorWorldCoords);
            }
        }

        //public static Vector2 GetCursorWorldCoords()
        //{
        //    return new Vector2((int)Main.screenPosition.X + Main.mouseX, (int)Main.screenPosition.Y + Main.mouseY);
        //}

        public static Vector2 GetTileCoordsFromWorldCoords(Vector2 worldCoords)
        {
            return new Vector2((int)worldCoords.X / 16, (int)worldCoords.Y / 16);
        }

        public static Vector2 GetWorldCoordsFromTileCoords(Vector2 tileCoords)
        {
            return new Vector2((int)tileCoords.X * 16, (int)tileCoords.Y * 16);
        }

        public static void DrawBorderedRect(SpriteBatch spriteBatch, Color color, Color borderColor, Vector2 position, Vector2 size, int borderWidth)
        {
            size *= 16;
            Vector2 pos = GetWorldCoordsFromTileCoords(position) - Main.screenPosition;
            spriteBatch.Draw(DummyTexture, new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y), color);

            spriteBatch.Draw(DummyTexture, new Rectangle((int)pos.X - borderWidth, (int)pos.Y - borderWidth, (int)size.X + borderWidth * 2, borderWidth), borderColor);
            spriteBatch.Draw(DummyTexture, new Rectangle((int)pos.X - borderWidth, (int)pos.Y + (int)size.Y, (int)size.X + borderWidth * 2, borderWidth), borderColor);
            spriteBatch.Draw(DummyTexture, new Rectangle((int)pos.X - borderWidth, (int)pos.Y, (int)borderWidth, (int)size.Y), borderColor);
            spriteBatch.Draw(DummyTexture, new Rectangle((int)pos.X + (int)size.X, (int)pos.Y, (int)borderWidth, (int)size.Y), borderColor);
        }

        public static void DrawBorderedRect(SpriteBatch spriteBatch, Color color, Vector2 position, Vector2 size, int borderWidth)
        {
            Color fillColor = color * .3f;
            DrawBorderedRect(spriteBatch, fillColor, color, position, size, borderWidth);
        }

        public static void DrawBorderedRect(SpriteBatch spriteBatch, Color color, Vector2 position, Vector2 size, int borderWidth, string text)
        {
            DrawBorderedRect(spriteBatch, color, position, size, borderWidth);
            Vector2 pos = GetWorldCoordsFromTileCoords(position) - Main.screenPosition;
            pos.X += 2;
            pos.Y += 2;
            spriteBatch.DrawString(Main.fontMouseText, text, pos, Color.White, 0f, Vector2.Zero, .7f, SpriteEffects.None, 0);
        }

        public static void DrawStringBorder(SpriteBatch spriteBatch, SpriteFont font, Vector2 position, string text, Color borderColor, float boarderSize, Vector2 origin, float scale)
        {
            Vector2 pos = Vector2.Zero;
            int i = 0;
            while (i < 4)
            {
                switch (i)
                {
                    case 0:
                        pos.X = position.X - boarderSize;
                        pos.Y = position.Y;
                        break;

                    case 1:
                        pos.X = position.X + boarderSize;
                        pos.Y = position.Y;
                        break;

                    case 2:
                        pos.X = position.X;
                        pos.Y = position.Y - boarderSize;
                        break;

                    case 3:
                        pos.X = position.X;
                        pos.Y = position.Y + boarderSize;
                        break;
                }
                spriteBatch.DrawString(font, text, pos, borderColor, 0f, origin, scale, SpriteEffects.None, 0f);
                i++;
            }
        }

        private static Dictionary<int, Color> rarityColors = new Dictionary<int, Color>()
        {
            {-11, new Color(255, 175, 0) },
            {-1, new Color(130, 130, 130) },
            {1, new Color(150, 150, 255) },
            {2, new Color(150, 255, 150) },
            {3, new Color(255, 200, 150) },
            {4, new Color(255, 150, 150) },
            {5, new Color(255, 150, 255) },
            {6, new Color(210, 160, 255) },
            {7, new Color(150, 255, 10) },
            {8, new Color(255, 255, 10) },
            {9, new Color(5, 200, 255) },
        };

        public static Color GetItemColor(Item item)
        {
            if (rarityColors.ContainsKey(item.rare)) return rarityColors[item.rare];
            return Color.White;
        }

        private static bool debug = true;

        public static void DebugText(string message)
        {
            if (debug)
            {
                string header = "HERO's Mod: ";
                if (Main.dedServ)
                {
                    System.IO.StreamWriter file = new System.IO.StreamWriter("G:/terraria-debug-server.txt", true, System.Text.Encoding.UTF8);
                    Console.WriteLine(header + message);
                    file.WriteLine("Server: " + message);
                    file.Flush();
                    file.Close();
                }
                else
                {
                    System.IO.StreamWriter file = new System.IO.StreamWriter("G:/terraria-debug-client.txt", true, System.Text.Encoding.UTF8);
                    if (Main.gameMenu)
                    {
                        ErrorLogger.Log(header + Main.myPlayer + ": " + message);
                    }
                    else
                    {
                        Main.NewText(header + message);
                    }
                    file.WriteLine("Client: " + message);
                    file.Flush();
                    file.Close();
                }
            }
        }

        public static Rectangle GetClippingRectangle(SpriteBatch spriteBatch, Rectangle r)
        {
            Vector2 vector = new Vector2(r.X, r.Y);
            Vector2 position = new Vector2(r.Width, r.Height) + vector;
            vector = Vector2.Transform(vector, Main.UIScaleMatrix);
            position = Vector2.Transform(position, Main.UIScaleMatrix);
            Rectangle result = new Rectangle((int)vector.X, (int)vector.Y, (int)(position.X - vector.X), (int)(position.Y - vector.Y));
            int width = spriteBatch.GraphicsDevice.Viewport.Width;
            int height = spriteBatch.GraphicsDevice.Viewport.Height;
            result.X = Utils.Clamp<int>(result.X, 0, width);
            result.Y = Utils.Clamp<int>(result.Y, 0, height);
            result.Width = Utils.Clamp<int>(result.Width, 0, width - result.X);
            result.Height = Utils.Clamp<int>(result.Height, 0, height - result.Y);
            return result;
        }

        public static bool NumberIsOneOfThese(long number, long[] numbers2) {
            foreach (long number2 in numbers2) {
                if (number == number2) {
                    return true;
                }
            }
            return false;
        }

    }

    public enum NetworkMode : byte
    {
        None,
        Client,
        Server
    }
}