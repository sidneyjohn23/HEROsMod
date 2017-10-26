using HEROsMod.HEROsModNetwork;
using HEROsMod.HEROsModServices;
using HEROsMod.UIKit;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

// TODO, freeze is bypassable.
// TODO, regions prevent all the chest movement and right click.
// TODO -- Should I have all services use the same Global hooks?
namespace HEROsMod
{
    internal class HEROsMod : Mod
    {
        public static HEROsMod instance;

        public override void Load()
        {
            if (ModLoader.version < new Version(0, 10))
            {
                throw new Exception("\nThis mod uses functionality only present in the latest tModLoader. Please update tModLoader to use this mod\n\n");
            }

            try
            {
                instance = this;

                if (!Main.dedServ)
                {
                    UIButton.buttonBackground = instance.GetTexture("Images/UIKit/buttonEdge");
                    UIView.closeTexture = instance.GetTexture("Images/closeButton");
                    UITextbox.textboxBackground = instance.GetTexture("Images/UIKit/textboxEdge");
                    UISlider.barTexture = instance.GetTexture("Images/UIKit/barEdge");
                    UIScrollView.ScrollbgTexture = GetTexture("Images/UIKit/scrollbgEdge");
                    UIScrollBar.ScrollbarTexture = instance.GetTexture("Images/UIKit/scrollbarEdge");
                    UIDropdown.capUp = instance.GetTexture("Images/UIKit/dropdownCapUp");
                    UIDropdown.capDown = instance.GetTexture("Images/UIKit/dropdownCapDown");
                    UICheckbox.checkboxTexture = instance.GetTexture("Images/UIKit/checkBox");
                    UICheckbox.checkmarkTexture = instance.GetTexture("Images/UIKit/checkMark");
                }
                ModUtils.DebugText("Mod Load");
                Init();
            }
            catch (Exception e)
            {
                ModUtils.DebugText("Load:\n" + e.Message + "\n" + e.StackTrace + "\n");
            }
        }

        // Clear EVERYthing, mod is unloaded.
        public override void Unload()
        {
            UIKit.UIComponents.ItemBrowser.Filters = null;
            UIKit.UIComponents.ItemBrowser.DefaultSorts = null;
            UIButton.buttonBackground = null;
            UIView.closeTexture = null;
            UITextbox.textboxBackground = null;
            UISlider.barTexture = null;
            UIScrollView.ScrollbgTexture = null;
            UIScrollBar.ScrollbarTexture = null;
            UIDropdown.capUp = null;
            UIDropdown.capDown = null;
            UICheckbox.checkboxTexture = null;
            UICheckbox.checkmarkTexture = null;
            Login._loginTexture = null;
            Login._logoutTexture = null;
            try
            {
                KeybindController.bindings.Clear();
                if (ServiceController != null)
                {
                    if (ServiceController.Services != null)
                    {
                        foreach (var service in ServiceController.Services)
                        {
                            service.Unload();
                        }
                    }
                    ServiceController.RemoveAllServices();
                }
                Network.ResetAllPlayers();
                Network.ServerUsingHEROsMod = false;
                Network.Regions.Clear();
                MasterView.ClearMasterView();
            }
            catch (Exception e)
            {
                ModUtils.DebugText("Unload:\n" + e.Message + "\n" + e.StackTrace + "\n");
            }
            extensionMenuService = null;
            miscOptions = null;
            _hotbar = null;
            ServiceController = null;
            TimeWeatherControlHotbar.Unload();
            instance = null;
        }

        public override void PostSetupContent()
        {
            ModUtils.DebugText("Post Setup Content");
            if (!Main.dedServ)
            {
                foreach (var service in ServiceController.Services)
                {
                    service.PostSetupContent();
                }
            }
        }

        public override void PostDrawFullscreenMap(ref string mouseText)
        {
            Teleporter.instance.PostDrawFullScreenMap();
            MapRevealer.instance.PostDrawFullScreenMap();
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int inventoryLayerIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (inventoryLayerIndex != -1)
            {
                layers.Insert(inventoryLayerIndex, new LegacyGameInterfaceLayer(
                    "HerosMod: UI",
                    delegate
                    {
                        try
                        {
                            Update();

                            ServiceHotbar.Update();

                            DrawBehindUI(Main.spriteBatch);

                            Draw(Main.spriteBatch);

                            KeybindController.DoPreviousKeyState();
                        }
                        catch (Exception e)
                        {
                            ModUtils.DebugText("PostDrawInInventory Error: " + e.Message + e.StackTrace);
                        }
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }

        public override void HotKeyPressed(string name)
        {
            KeybindController.HotKeyPressed(name);
        }

        public override void UpdateMusic(ref int music)
        {
            CheckIfGameEnteredOrLeft();
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            Network.HEROsModMessaged(reader, whoAmI);
        }

        public override bool HijackGetData(ref byte messageType, ref BinaryReader reader, int playerNumber)
        {
            //ModUtils.DebugText("Data: " + messageType);
            if (Network.CheckIncomingDataForHEROsModMessage(ref messageType, ref reader, playerNumber))
            {
                return true;
            }
            return false;
        }

        private static bool _prevGameMenu = true;

        // Holds all the loaded services.
        public static ServiceController ServiceController;

        public static RenderTarget2D RenderTarget { get; set; }

        private static ServiceHotbar _hotbar;
        public static ServiceHotbar ServiceHotbar
        {
            get { return _hotbar; }
        }

        public static void Init()
        {
            ModUtils.DebugText("Mod Init");
            ModUtils.Init();
            Network.Init();

            if (!Main.dedServ)
            {
                UIView.exclusiveControl = null;
                InventoryManager.SetKeyBindings();
                ServiceController = new ServiceController();
                _hotbar = new ServiceHotbar();
                SelectionTool.Init();
                
                UIColorPicker colorPicker = new UIColorPicker()
                {
                    X = 200
                };
                MasterView.menuScreen.AddChild(colorPicker);
                
                LoadAddServices();
            }
        }

        private MiscOptions miscOptions;
        private ExtensionMenuService extensionMenuService;

        // TODO, is this ok to do on load rather than on enter?
        public static void LoadAddServices()
        {
            ServiceController.AddService(new ItemBrowser());
            ServiceController.AddService(new InfiniteReach());
            ServiceController.AddService(new FlyCam());
            ServiceController.AddService(new EnemyToggler());
            ServiceController.AddService(new ItemClearer());
            ServiceController.AddService(new TimeWeatherChanger());
            ServiceController.AddService(new Waypoints());
            ServiceController.AddService(new InventoryManager());
            ServiceController.AddService(new MobSpawner());
            ServiceController.AddService(new BuffService());
            ServiceController.AddService(new GodModeService());
            ServiceController.AddService(new PrefixEditor());
            ServiceController.AddService(new InvasionService());
            ServiceController.AddService(new Teleporter());
            ServiceController.AddService(new RegionService());
            ServiceController.AddService(new CheckTileModificationTool());
            ServiceController.AddService(new PlayerList());

            instance.miscOptions = new MiscOptions();
            ServiceController.AddService(instance.miscOptions);
            ServiceController.AddService(new SpawnPointSetter(instance.miscOptions.Hotbar));
            ServiceController.AddService(new MapRevealer(instance.miscOptions.Hotbar));
            ServiceController.AddService(new ItemBanner(instance.miscOptions.Hotbar));
            ServiceController.AddService(new ToggleGravestones(instance.miscOptions.Hotbar));
            ServiceController.AddService(new GroupInspector(instance.miscOptions.Hotbar));
            ServiceController.AddService(new WorldPurifier(instance.miscOptions.Hotbar));
            ServiceController.AddService(new HardmodeEnemyToggler(instance.miscOptions.Hotbar));
            //ServiceController.AddService(new HellevatorBuilder(instance.miscOptions.Hotbar));

            instance.extensionMenuService = new ExtensionMenuService();
            ServiceController.AddService(instance.extensionMenuService);

            ServiceController.AddService(new Login());
            ServiceHotbar.Visible = true;
        }

        public static void Update(/*GameTime gameTime*/)
        {
            if (ModUtils.NetworkMode != NetworkMode.Server)
            {
                ModUtils.PreviousKeyboardState = Main.keyState;
                ModUtils.PreviousMouseState = ModUtils.MouseState;
                ModUtils.MouseState = Mouse.GetState();

                ModUtils.SetDeltaTime(/*gameTime*/);
                ModUtils.Update();
                //HEROsModVideo.Services.MobHUD.MobInfo.Update();
                //Update all services in the ServiceController
                foreach (var service in ServiceController.Services)
                {
                    service.Update();
                }
                MasterView.UpdateMaster();
                SelectionTool.Update();
            }
            Network.Update();
        }

        //Not working since update not called in title screen.
        private static void CheckIfGameEnteredOrLeft()
        {
            if (!Main.gameMenu && _prevGameMenu)
            {
                try
                {
                    GameEntered();
                }
                catch (Exception e)
                {
                    ModUtils.DebugText(e.Message + "\n" + e.StackTrace);
                }
            }
            _prevGameMenu = Main.gameMenu;
        }

        public static void GameEntered()
        {
            ModUtils.DebugText("Game Entered");

            if (ModUtils.NetworkMode == NetworkMode.None)
            {
                foreach (HEROsModService service in ServiceController.Services)
                {
                    service.HasPermissionToUse = !service.MultiplayerOnly;
                }
                ServiceController.ServiceRemovedCall();
            }
            else
            {
                foreach (HEROsModService service in ServiceController.Services)
                {
                    service.HasPermissionToUse = true;
                }
                ServiceController.MyGroupChanged();
            }
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            MasterView.DrawMaster(spriteBatch);
            foreach (var service in ServiceController.Services)
            {
                service.Draw(spriteBatch);
            }

            float x = Main.fontMouseText.MeasureString(UIView.HoverText).X;
            Vector2 vector = new Vector2(Main.mouseX, Main.mouseY) + new Vector2(16f);
            if (vector.Y > Main.screenHeight - 30)
            {
                vector.Y = Main.screenHeight - 30;
            }
            if (vector.X > Main.screenWidth - x)
            {
                vector.X = Main.screenWidth - x - 30;
            }
            Utils.DrawBorderStringFourWay(spriteBatch, Main.fontMouseText, UIView.HoverText, vector.X, vector.Y, new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor), Color.Black, Vector2.Zero, 1f);
        }

        public static void DrawBehindUI(SpriteBatch spriteBatch)
        {
            if (!Main.gameMenu)
            {
                HEROsModVideo.Services.MobHUD.MobInfo.Draw(spriteBatch);
                SelectionTool.Draw(spriteBatch);
                if (RegionService.RegionsVisible)
                    RegionService.DrawRegions(spriteBatch);
                CheckTileModificationTool.DrawBoxOnCursor(spriteBatch);
            }
        }

        public override void PreSaveAndQuit()
        {
            switch (Network.NetworkMode)
            {
                case NetworkMode.None:
                    break;
                case NetworkMode.Client:
                    ModUtils.DebugText("Game left");
                    GeneralMessages.TellServerILeft();
                    Login.LoggedIn = false;
                    ServiceController.MyGroupChanged();

                    Network.Players2.Clear();
                    Network.Groups.Clear();
                    Network.Regions.Clear();

                    /*StreamWriter file = new StreamWriter("G:/terraria-chat2.txt", true);
                    var chatLines = Main.chatLine;
                    for (int i = 0; i < Main.numChatLines; i++)
                    {
                        if (chatLines[i].text == "whatever")
                        {
                            string a = "";
                            foreach (var j in chatLines[i].parsedText)
                            {
                                a += j.Text;
                                a += ";";
                            }
                            file.WriteLine("Snippets:: " + a);
                        }
                        else
                        {
                            file.WriteLine(chatLines[i].text);
                        }
                    }
                    file.Flush();
                    file.Close();*/
                    break;
                case NetworkMode.Server:
                    break;
                default:
                    break;
            }
        }
    }
}