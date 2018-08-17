using HEROsMod.HEROsModNetwork;
using HEROsMod.HEROsModServices;
using HEROsMod.UIKit;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
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
		internal static Dictionary<string, ModTranslation> translations; // reference to private field.

		public override void Load()
		{
			// Since we are using hooks not in older versions, and since ItemID.Count changed, we need to do this.
			if (ModLoader.version < new Version(0, 10, 1, 3))
			{
				throw new Exception(HEROsMod.HeroText("UpdateTModLoaderToUse"));
			}

			try
			{
				instance = this;

				FieldInfo translationsField = typeof(Mod).GetField("translations", BindingFlags.Instance | BindingFlags.NonPublic);
				translations = (Dictionary<string, ModTranslation>)translationsField.GetValue(this);
				//LoadTranslations();

				//	AddGlobalItem("HEROsModGlobalItem", new HEROsModGlobalItem());
				// AddPlayer("HEROsModModPlayer", new HEROsModModPlayer());
				//if (ModUtils.NetworkMode != NetworkMode.Server)

				if (!Main.dedServ)
				{
					UIKit.UIButton.buttonBackground = HEROsMod.instance.GetTexture("Images/UIKit/buttonEdge");
					UIKit.UIView.closeTexture = HEROsMod.instance.GetTexture("Images/closeButton");
					UIKit.UITextbox.textboxBackground = HEROsMod.instance.GetTexture("Images/UIKit/textboxEdge");
					UIKit.UISlider.barTexture = HEROsMod.instance.GetTexture("Images/UIKit/barEdge");
					UIKit.UIScrollView.ScrollbgTexture = GetTexture("Images/UIKit/scrollbgEdge");
					UIKit.UIScrollBar.ScrollbarTexture = HEROsMod.instance.GetTexture("Images/UIKit/scrollbarEdge");
					UIKit.UIDropdown.capUp = HEROsMod.instance.GetTexture("Images/UIKit/dropdownCapUp");
					UIKit.UIDropdown.capDown = HEROsMod.instance.GetTexture("Images/UIKit/dropdownCapDown");
					UIKit.UICheckbox.checkboxTexture = HEROsMod.instance.GetTexture("Images/UIKit/checkBox");
					UIKit.UICheckbox.checkmarkTexture = HEROsMod.instance.GetTexture("Images/UIKit/checkMark");
				}

				Init();
			}
			catch (Exception e)
			{
				ModUtils.DebugText("Load:\n" + e.Message + "\n" + e.StackTrace + "\n");
			}
		}

		internal static string HeroText(string key)
		{
			return translations[$"Mods.HEROsMod.{key}"].GetTranslation(Language.ActiveCulture);
			// This isn't good until after load....
			// return Language.GetTextValue($"Mods.HEROsMod.{category}.{key}");
		}

		// Clear EVERYthing, mod is unloaded.
		public override void Unload()
		{
			UIKit.UIComponents.ItemBrowser.Filters = null;
			UIKit.UIComponents.ItemBrowser.DefaultSorts = null;
			UIKit.UIComponents.ItemBrowser.Categories = null;
			UIKit.UIComponents.ItemBrowser.CategoriesLoaded = false;
			UIKit.UIButton.buttonBackground = null;
			UIKit.UIView.closeTexture = null;
			UIKit.UITextbox.textboxBackground = null;
			UIKit.UISlider.barTexture = null;
			UIKit.UIScrollView.ScrollbgTexture = null;
			UIKit.UIScrollBar.ScrollbarTexture = null;
			UIKit.UIDropdown.capUp = null;
			UIKit.UIDropdown.capDown = null;
			UIKit.UICheckbox.checkboxTexture = null;
			UIKit.UICheckbox.checkmarkTexture = null;
			HEROsModServices.Login._loginTexture = null;
			HEROsModServices.Login._logoutTexture = null;
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
				HEROsModNetwork.Network.ResetAllPlayers();
				HEROsModNetwork.Network.ServerUsingHEROsMod = false;
				HEROsModNetwork.Network.Regions.Clear();
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
			ModUtils.previousInventoryItems = null;
			translations = null;
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

		/*
		private void LoadTranslations()
		{
			// 0.10.1.2 already does this
			if (ModLoader.version >= new Version(0, 10, 1, 2))
				return;

			var modTranslationDictionary = new Dictionary<string, ModTranslation>();
			var translationFiles = File.Where(x => Path.GetExtension(x.Key) == ".lang");
			foreach (var translationFile in translationFiles)
			{
				string translationFileContents = System.Text.Encoding.UTF8.GetString(translationFile.Value);
				GameCulture culture = GameCulture.FromName(Path.GetFileNameWithoutExtension(translationFile.Key));

				using (StringReader reader = new StringReader(translationFileContents))
				{
					string line;
					while ((line = reader.ReadLine()) != null)
					{
						int split = line.IndexOf('=');
						if (split < 0)
							continue; // lines witout a = are ignored
						string key = line.Substring(0, split).Trim().Replace(" ", "_");
						string value = line.Substring(split + 1).Trim();
						if (value.Length == 0)
						{
							continue;
						}
						value = value.Replace("\\n", "\n");
						// TODO: Maybe prepend key with filename: en.US.ItemName.lang would automatically assume "ItemName." for all entries.
						//string key = key;
						ModTranslation mt;
						if (!modTranslationDictionary.TryGetValue(key, out mt))
							modTranslationDictionary[key] = mt = CreateTranslation(key);
						mt.AddTranslation(culture, value);
					}
				}
			}

			foreach (var value in modTranslationDictionary.Values)
			{
				AddTranslation(value);
			}
		}
		*/

		//public override Matrix ModifyTransformMatrix(Matrix Transform)
		//{
		//	if (!Main.gameMenu)
		//	{
		//		return Transform *= HEROsModMod.HEROsModServices.ZoomToolsService.OffsetMatrix;
		//	}
		//	return Transform;
		//}

		//public static bool CreateiveDisabled = false;

		private static bool _prevGameMenu = true;
		//internal ModExtensions modExtensions;

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
			ServiceController.AddService(new LightHack(instance.miscOptions.Hotbar));
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