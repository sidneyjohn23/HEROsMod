using HEROsMod.UIKit;
using HEROsMod.UIKit.UIComponents;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using HEROsMod.HEROsModNetwork;
using System.IO;

namespace HEROsMod.HEROsModServices {
    internal class TimeWeatherChanger : HEROsModService {
        public static bool TimePaused { get; set; }

		public TimeSetWindow timeSetWindow;

		public static double PausedTime { get; set; } = 0;

		//	public static bool PausedTimeDayTime = false;
		private TimeWeatherControlHotbar timeWeatherHotbar;

        public TimeWeatherChanger() {
            IsHotbar = true;

			TimePaused = false;
			_name = "Time Weather Control";
			_hotbarIcon = new UIImage(HEROsMod.instance.GetTexture("Images/timeRain"));
			HotbarIcon.Tooltip = HEROsMod.HeroText("ChangeTimeRain");
			HotbarIcon.OnLeftClick += HotbarIcon_onLeftClick;

            timeSetWindow = new TimeSetWindow();
            AddUIView(timeSetWindow);
            timeSetWindow.Visible = false;

            //timeWeatherHotbar = new TimeWeatherControlHotbar();
            //HEROsMod.ServiceHotbar.AddChild(timeWeatherHotbar);

            timeWeatherHotbar = new TimeWeatherControlHotbar() {
                HotBarParent = HEROsMod.ServiceHotbar,
                timeSetWindow = timeSetWindow

            };
            timeWeatherHotbar.Hide();
            AddUIView(timeWeatherHotbar);

            Hotbar = timeWeatherHotbar;

            GeneralMessages.TimePausedOrResumedByServer += GeneralMessages_TimePausedOrResumedByServer;
        }

        private void GeneralMessages_TimePausedOrResumedByServer(bool timePaused) {
            TimePaused = timePaused;
            timeWeatherHotbar.TimePausedOfResumed();
        }

        private void HotbarIcon_onLeftClick(object sender, EventArgs e) {
            if (timeWeatherHotbar.Selected) {
                timeWeatherHotbar.Selected = false;
                timeWeatherHotbar.Hide();
            } else {
                timeWeatherHotbar.Selected = true;
                timeWeatherHotbar.Show();
            }

            //timeWeatherHotbar.Visible = !timeWeatherHotbar.Visible;
            //if (timeWeatherHotbar.Visible)
            //{
            //	timeWeatherHotbar.X = this._hotbarIcon.X + this._hotbarIcon.Width / 2 - timeWeatherHotbar.Width / 2;
            //	timeWeatherHotbar.Y = -timeWeatherHotbar.Height;
            //}
        }

        public override void MyGroupUpdated() {
            HasPermissionToUse = LoginService.MyGroup.HasPermission("ChangeTimeWeather");
            if (!HasPermissionToUse) {
                timeWeatherHotbar.Hide();
            }
            //base.MyGroupUpdated();
        }

        public override void Destroy() {
            GeneralMessages.TimePausedOrResumedByServer -= GeneralMessages_TimePausedOrResumedByServer;
            TimePaused = false;
            HEROsMod.ServiceHotbar.RemoveChild(timeWeatherHotbar);
            base.Destroy();
        }

        public static void ToggleTimePause() {
            TimePaused = !TimePaused;
            if (TimePaused) {
                PausedTime = Main.time;
            }
        }

        public override void Update() {
            if (ModUtils.NetworkMode == NetworkMode.None) {
                if (TimePaused) {
                    Main.time = PausedTime;
                }
            }
            base.Update();
        }
    }

    internal class TimeWeatherControlHotbar : UIHotbar {
        //	static float spacing = 8f;

        public UIImage bPause;
        private static Texture2D _playTexture;
        private static Texture2D _pauseTexture;
        public TimeSetWindow timeSetWindow;
        //static Texture2D _rainTexture;
        //public static Texture2D rainTexture
        //{
        //	get
        //	{
        //		if (_rainTexture == null) _rainTexture = HEROsMod.instance.GetTexture("Images/rainIcon");
        //		return _rainTexture;
        //	}
        //}
        public static Texture2D PlayTexture {
            get {
                if (_playTexture == null)
				{
					_playTexture = HEROsMod.instance.GetTexture("Images/speed1");
				}

				return _playTexture;
            }
        }

        public static Texture2D PauseTexture {
            get {
                if (_pauseTexture == null)
				{
					_pauseTexture = HEROsMod.instance.GetTexture("Images/speed0");
				}

				return _pauseTexture;
            }
        }

        public TimeWeatherControlHotbar() {
            buttonView = new UIView();
            Height = 54;
            UpdateWhenOutOfBounds = true;
            //this.Visible = false;

            buttonView.Height = base.Height;
			Anchor = AnchorPosition.Top;
            AddChild(buttonView);

            //UIImage bStopRain = new UIImage(HEROsMod.instance.GetTexture("Images/sunIcon"));
            //UIImage bStartRain = new UIImage(rainTexture);
            //bStartRain.Tooltip = "Start Rain";
            //bStopRain.Tooltip = "Stop Rain";
            //bStartRain.onLeftClick += bStartRain_onLeftClick;
            //bStopRain.onLeftClick += bStopRain_onLeftClick;
            //AddChild(bStopRain);
            //AddChild(bStartRain);

            //UIImage nightButton = new UIImage(HEROsMod.instance.GetTexture("Images/moonIcon"));
            //nightButton.Tooltip = "Night";
            //nightButton.onLeftClick += nightButton_onLeftClick;
            //UIImage noonButton = new UIImage(HEROsMod.instance.GetTexture("Images/sunIcon"));
            //noonButton.Tooltip = "Noon";
            //noonButton.onLeftClick += noonButton_onLeftClick;
            //bPause = new UIImage(pauseTexture);
            //bPause.onLeftClick += bPause_onLeftClick;
            //AddChild(nightButton);
            //AddChild(noonButton);
            //AddChild(bPause);

            //float xPos = spacing;
            //for (int i = 0; i < Children.Count; i++)
            //{
            //	if (Children[i].Visible)
            //	{
            //		Children[i].X = xPos;
            //		xPos += Children[i].Width + spacing;
            //		Children[i].Y = Height / 2 - Children[i].Height / 2;
            //	}
            //}
            //Width = xPos;
        }

        public override void Test() {
            //	ModUtils.DebugText("TEST " + buttonView.ChildCount);

            Height = 54;
            UpdateWhenOutOfBounds = true;

			UIImage bStopRain = new UIImage(HEROsMod.instance.GetTexture("Images/rainStop"));
			UIImage bStartRain = new UIImage(HEROsMod.instance.GetTexture("Images/rainIcon"))
			{
				Tooltip = HEROsMod.HeroText("StartRain")
			};
			bStopRain.Tooltip = HEROsMod.HeroText("StopRain");
			bStartRain.OnLeftClick += BStartRain_onLeftClick;
			bStopRain.OnLeftClick += BStopRain_onLeftClick;
			buttonView.AddChild(bStopRain);
			buttonView.AddChild(bStartRain);

			UIImage bStopSandstorm = new UIImage(HEROsMod.instance.GetTexture("Images/rainStop"));
			UIImage bStartSandstorm = new UIImage(HEROsMod.instance.GetTexture("Images/rainIcon"))
			{
				Tooltip = HEROsMod.HeroText("StartSandstorm")
			};
			bStopSandstorm.Tooltip = HEROsMod.HeroText("StopSandstorm");
			bStartSandstorm.OnLeftClick += BStartSandstorm_onLeftClick;
			bStopSandstorm.OnLeftClick += BStopSandstorm_onLeftClick;
			buttonView.AddChild(bStopSandstorm);
			buttonView.AddChild(bStartSandstorm);

			UIImage nightButton = new UIImage(HEROsMod.instance.GetTexture("Images/moonIcon"))
			{
				Tooltip = HEROsMod.HeroText("Night")
			};
			nightButton.OnLeftClick += NightButton_onLeftClick;
			UIImage noonButton = new UIImage(HEROsMod.instance.GetTexture("Images/sunIcon"))
			{
				Tooltip = HEROsMod.HeroText("Noon")
			};
			noonButton.OnLeftClick += NoonButton_onLeftClick;
			bPause = new UIImage(TimeWeatherChanger.TimePaused ? PlayTexture : PauseTexture);
			bPause.OnLeftClick += BPause_onLeftClick;
			bPause.Tooltip = TimeWeatherChanger.TimePaused ? HEROsMod.HeroText("ResumeTime") : HEROsMod.HeroText("PauseTime");// "Toggle Freeze Time";

            buttonView.AddChild(nightButton);
            buttonView.AddChild(noonButton);
            buttonView.AddChild(bPause);

			UIImage sundialButton = new UIImage(HEROsMod.instance.GetTexture("Images/timeRain"))
			{
				Tooltip = HEROsMod.HeroText("ForceEnchantedSundial")
			};
			sundialButton.OnLeftClick += SundialButton_onLeftClick;
			buttonView.AddChild(sundialButton);

            UIImage changeTimeButton = new UIImage(HEROsMod.instance.GetTexture("Images/clockIcon")) {
                Tooltip = "Change Time...",
            };
            changeTimeButton.OnLeftClick += ChangeTimeButton_onLeftClick;
            buttonView.AddChild(changeTimeButton);

			//float xPos = spacing;
			//for (int i = 0; i < Children.Count; i++)
			//{
			//	if (Children[i].Visible)
			//	{
			//		Children[i].X = xPos;
			//		xPos += Children[i].Width + spacing;
			//		Children[i].Y = Height / 2 - Children[i].Height / 2;
			//	}
			//}
			//Width = xPos;

			CenterXAxisToParentCenter();
            float num = spacing;
            for (int i = 0; i < buttonView.Children.Count; i++) {
                buttonView.Children[i].Anchor = AnchorPosition.Left;
                buttonView.Children[i].Position = new Vector2(num, 0f);
                buttonView.Children[i].CenterYAxisToParentCenter();
                buttonView.Children[i].Visible = true;
                //this.buttonView.Children[i].ForegroundColor = buttonUnselectedColor;
                num += buttonView.Children[i].Width + spacing;
            }
			//this.Resize();
			Width = num;
            buttonView.Width = Width;
        }

        public override void Update() {
            DoSlideMovement();
            //base.CenterXAxisToParentCenter();
            base.Update();
        }

        //public void Resize()
        //{
        //	float num = this.spacing;
        //	for (int i = 0; i < this.buttonView.Children.Count; i++)
        //	{
        //		if (this.buttonView.Children[i].Visible)
        //		{
        //			this.buttonView.Children[i].X = num;
        //			num += this.buttonView.Children[i].Width + this.spacing;
        //		}
        //	}
        //	base.Width = num;
        //	this.buttonView.Width = base.Width;
        //}

        private void SundialButton_onLeftClick(object sender, EventArgs e) {
            if (Main.netMode == 1) // Client
            {
                GeneralMessages.RequestForcedSundial();
            } else // Single
              {
                Main.fastForwardTime = true;
                Main.sundialCooldown = 0;
                //NetMessage.SendData(7, -1, -1, "", 0, 0f, 0f, 0f, 0, 0, 0);
            }

            //if (/*!Main.fastForwardTime &&*/ (Main.netMode == 1 || Main.sundialCooldown == 0))
            //{
            //	if (Main.sundialCooldown == 0)
            //	{
            //		if (Main.netMode == 1)
            //		{
            //			NetMessage.SendData(51, -1, -1, "", Main.myPlayer, 3f, 0f, 0f, 0, 0, 0);
            //			return;
            //		}
            //		Main.fastForwardTime = true;
            //		Main.sundialCooldown = 8;
            //		NetMessage.SendData(7, -1, -1, "", 0, 0f, 0f, 0f, 0, 0, 0);
            //	}
            //}
        }

		private void BPause_onLeftClick(object sender, EventArgs e)
		{
			if (Main.netMode != 1)
			{
				TimeWeatherChanger.ToggleTimePause();
				UIImage b = (UIImage)sender;
				TimePausedOfResumed();
				if(TimeWeatherChanger.TimePaused)
				{
					Main.NewText(HEROsMod.HeroText("TimeHasBeenPaused"));
				}
				else
				{
					Main.NewText(HEROsMod.HeroText("TimeHasResumed"));
				}
			}
			else
			{
				GeneralMessages.RequestTimeChange(GeneralMessages.TimeChangeType.Pause);
			}
		}

		private void BStopRain_onLeftClick(object sender, EventArgs e)
		{
			if (Main.netMode == 1)
			{
				GeneralMessages.RequestStopRain();
				return;
			}
			Main.NewText(HEROsMod.HeroText("RainHasBeenTurnedOff"));

            ModUtils.StopRain();
        }

        private void BStartRain_onLeftClick(object sender, EventArgs e) {
            if (Main.netMode == 1) {
                GeneralMessages.RequestStartRain();

                StreamWriter file = new StreamWriter("G:/terraria-chat2.txt", true);
				Terraria.UI.Chat.ChatLine[] chatLines = Main.chatLine;
                for (int i = 0; i < Main.numChatLines; i++) {
                    if (chatLines[i].text == "whatever") {
                        string a = "";
                        foreach (Terraria.UI.Chat.TextSnippet j in chatLines[i].parsedText) {
                            a += j.Text;
                            a += ";";
                        }
                        file.WriteLine("Snippets:: " + a);
                    } else {
                        file.WriteLine(chatLines[i].text);
                    }
                }
                file.Flush();
                file.Close();

                return;
            }
			Main.NewText(HEROsMod.HeroText("RainHasBeenTurnedOn"));
			ModUtils.StartRain();


        }

		private void BStopSandstorm_onLeftClick(object sender, EventArgs e)
		{
			if (Main.netMode == 1)
			{
				GeneralMessages.RequestStopSandstorm();
				return;
			}
			Main.NewText(HEROsMod.HeroText("SandstormHasBeenTurnedOff"));

            ModUtils.StopSandstorm();
        }

		private void BStartSandstorm_onLeftClick(object sender, EventArgs e)
		{
			if (Main.netMode == 1)
			{
				GeneralMessages.RequestStartSandstorm();
				return;
			}
			Main.NewText(HEROsMod.HeroText("SandstormHasBeenTurnedOn"));
			ModUtils.StartSandstorm();
		}

		public void TimePausedOfResumed()
		{
			if (TimeWeatherChanger.TimePaused)
			{
				bPause.Texture = PlayTexture;
			}
			else
			{
				bPause.Texture = PauseTexture;
			}
			bPause.Tooltip = TimeWeatherChanger.TimePaused ? HEROsMod.HeroText("ResumeTime") : HEROsMod.HeroText("PauseTime");
		}

        private void NightButton_onLeftClick(object sender, EventArgs e) {
            if (Main.netMode != 1) {
                Main.dayTime = false;
                Main.time = 0;// 19:30
            } else {
                GeneralMessages.RequestTimeChange(GeneralMessages.TimeChangeType.SetToNight);
            }
        }

        private void NoonButton_onLeftClick(object sender, EventArgs e) {
            if (Main.netMode != 1) {
                Main.dayTime = true;
                Main.time = 27000.0;
            } else {
                GeneralMessages.RequestTimeChange(GeneralMessages.TimeChangeType.SetToNoon);
            }
        }

        private void TimeControlWindow_onLeftClick(object sender, EventArgs e) {
            UIImage b = (UIImage) sender;
            int rate = (int) b.Tag;
            if (rate > 0) {
                //pauseTime = false;
                Main.dayRate = (int) b.Tag;
            } else {
                //pauseTime = true;
                //previousTime = Main.time;
            }
        }

		private void ChangeTimeButton_onLeftClick(object sender, EventArgs e) => timeSetWindow.Visible = !timeSetWindow.Visible;

		internal static void Unload() {
            _playTexture = null;
            _pauseTexture = null;
        }

        //public override void Update()
        //{
        //	if (this.Visible)
        //	{
        //		if (!MouseInside)
        //		{
        //			int mx = Main.mouseX;
        //			int my = Main.mouseY;
        //			float right = DrawPosition.X + Width;
        //			float left = DrawPosition.X;
        //			float top = DrawPosition.Y;
        //			float bottom = DrawPosition.Y + Height;
        //			float dist = 75f;
        //			bool outsideBounds = (mx > right && mx - right > dist) ||
        //								 (mx < left && left - mx > dist) ||
        //								 (my > bottom && my - bottom > dist) ||
        //								 (my < top && top - my > dist);
        //			if ((UIKit.UIView.MouseLeftButton && !MouseInside) || outsideBounds) this.Visible = false;
        //		}
        //	}
        //	base.Update();
        //}
    }

    internal class TimeSetWindow : UIWindow {
        private static float spacing = 8;
        private UITextbox timeBox;
        private UIButton SetButton;
        private UIButton CancelButton;

        public TimeSetWindow() {
            X = 200;
            Y = 250;
            CanMove = true;

            Width = 100;
            SetButton = new UIButton("Set Time");
            SetButton.OnLeftClick += SetButton_onLeftClick;

            CancelButton = new UIButton("Cancel");
            CancelButton.OnLeftClick += CancelButton_onLeftClick;
            CancelButton.Position = new Vector2(spacing * 2 + SetButton.Width, Height - 40);
            UILabel SetTimeLabel = new UILabel("Set Time") {
                X = spacing,
                Y = spacing,
                OverridesMouse = false,
                Scale = .6f
            };

            timeBox = new UITextbox() {
                Width = 55,
                Position = new Vector2(spacing, SetTimeLabel.Height + spacing)
            };
            UILabel HoursLabel = new UILabel("(24 hours 0:00-23:59)") {
                Position = new Vector2(timeBox.Width + 2 * spacing, SetTimeLabel.Height + 12),
                Scale = .3f
            };
            SetButton.Position = new Vector2(spacing, spacing + timeBox.Y + timeBox.Height);
            CancelButton.Position = new Vector2(spacing * 2 + SetButton.Width, spacing + timeBox.Y + timeBox.Height);
            Height = SetButton.Y + SetButton.Height + spacing;
            Width = spacing * 3 + SetButton.Width + CancelButton.Width;
            AddChild(SetTimeLabel);
            AddChild(timeBox);
            AddChild(CancelButton);
            AddChild(SetButton);
            AddChild(HoursLabel);

        }

		private void CancelButton_onLeftClick(object sender, EventArgs e) => Visible = false;

		private void SetButton_onLeftClick(object sender, EventArgs e) {
            if (Main.netMode != 1) {
                ParseTime(timeBox.Text, ref Main.time, ref Main.dayTime);
                timeBox.Text = "";
                Visible = false;
            } else {
                double gametime = 0; bool daytime = true;
                ParseTime(timeBox.Text, ref gametime, ref daytime);
                GeneralMessages.RequestTimeChange(GeneralMessages.TimeChangeType.SpecificTime, gametime, daytime);
                timeBox.Text = "";
                Visible = false;
            }
        }

        private void ParseTime(string time, ref double timeI, ref bool dayTime) {
            double gametime = 0;
            bool daytime = true;
            if (System.Text.RegularExpressions.Regex.IsMatch(time, "^[0-9]{1,2}:[0-9]{1,2}$")) {
                int hour = int.Parse(time.Split(':')[0]);
                int minute = int.Parse(time.Split(':')[1]);
                if (hour >= 0 && hour <= 23 && minute >= 0 && minute <= 59) {
                    if (hour >= 0 && hour <= 4) {
                        if (hour == 4 && minute < 30) {
                            gametime = 16200 + hour * 3600 + minute * 60;
                            daytime = false;
                        } else if (hour == 4 && minute >= 30) {
                            gametime = hour * 3600 + minute * 60 - 16200;
                            daytime = true;
                        } else {
                            gametime = 16200 + hour * 3600 + minute * 60;
                            daytime = false;
                        }
                    } else if (hour > 4 && hour < 19) {
                        gametime = hour * 3600 + minute * 60 - 16200;
                        daytime = true;
                    } else if (hour >= 19) {
                        if (hour == 19 && minute < 30) {
                            gametime = hour * 3600 + minute * 60 - 16200;
                            daytime = true;
                        } else if (hour == 19 && minute >= 30) {
                            gametime = hour * 3600 + minute * 60 - 70200;
                            daytime = false;
                        } else {
                            gametime = hour * 3600 + minute * 60 - 70200;
                            daytime = false;
                        }
                    }
                }

                timeI = gametime;
                dayTime = daytime;
                Main.NewText("Time successfully changed to " + time);
            } else {
                Main.NewText("Wrong time format!", Color.Red);
            }
        }
    }
}