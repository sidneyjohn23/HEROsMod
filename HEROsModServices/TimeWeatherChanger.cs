using HEROsMod.UIKit;
using HEROsMod.UIKit.UIComponents;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace HEROsMod.HEROsModServices
{
	internal class TimeWeatherChanger : HEROsModService
	{
		public static bool TimePaused { get; set; }
		private static double _pausedTime = 0;
        public TimeSetWindow timeSetWindow;

        public static double PausedTime
		{
			get { return _pausedTime; }
			set { _pausedTime = value; }
		}

		//	public static bool PausedTimeDayTime = false;
		private TimeWeatherControlHotbar timeWeatherHotbar;

        public TimeWeatherChanger()
		{
			IsHotbar = true;

			TimePaused = false;
            _name = "Time Weather Control";
            _hotbarIcon = new UIImage(HEROsMod.instance.GetTexture("Images/timeRain"));
            HotbarIcon.Tooltip = "Change Time/Rain";
            HotbarIcon.onLeftClick += HotbarIcon_onLeftClick;

            timeSetWindow = new TimeSetWindow();
            AddUIView(timeSetWindow);
            timeSetWindow.Visible = false;

            //timeWeatherHotbar = new TimeWeatherControlHotbar();
            //HEROsMod.ServiceHotbar.AddChild(timeWeatherHotbar);

            timeWeatherHotbar = new TimeWeatherControlHotbar()
            {
                HotBarParent = HEROsMod.ServiceHotbar,
                timeSetWindow = timeSetWindow

            };
            timeWeatherHotbar.Hide();
            AddUIView(timeWeatherHotbar);

			Hotbar = timeWeatherHotbar;

			HEROsModNetwork.GeneralMessages.TimePausedOrResumedByServer += GeneralMessages_TimePausedOrResumedByServer;
		}

		private void GeneralMessages_TimePausedOrResumedByServer(bool timePaused)
		{
			TimePaused = timePaused;
			timeWeatherHotbar.TimePausedOfResumed();
		}

		private void HotbarIcon_onLeftClick(object sender, EventArgs e)
		{
			if (timeWeatherHotbar.selected)
			{
				timeWeatherHotbar.selected = false;
				timeWeatherHotbar.Hide();
			}
			else
			{
				timeWeatherHotbar.selected = true;
				timeWeatherHotbar.Show();
			}

			//timeWeatherHotbar.Visible = !timeWeatherHotbar.Visible;
			//if (timeWeatherHotbar.Visible)
			//{
			//	timeWeatherHotbar.X = this._hotbarIcon.X + this._hotbarIcon.Width / 2 - timeWeatherHotbar.Width / 2;
			//	timeWeatherHotbar.Y = -timeWeatherHotbar.Height;
			//}
		}

		public override void MyGroupUpdated()
		{
            HasPermissionToUse = HEROsModNetwork.LoginService.MyGroup.HasPermission("ChangeTimeWeather");
			if (!HasPermissionToUse)
			{
				timeWeatherHotbar.Hide();
			}
			//base.MyGroupUpdated();
		}

		public override void Destroy()
		{
			HEROsModNetwork.GeneralMessages.TimePausedOrResumedByServer -= GeneralMessages_TimePausedOrResumedByServer;
			TimePaused = false;
			HEROsMod.ServiceHotbar.RemoveChild(timeWeatherHotbar);
			base.Destroy();
		}

		public static void ToggleTimePause()
		{
			TimePaused = !TimePaused;
			if (TimePaused)
			{
				PausedTime = Main.time;
			}
		}

		public override void Update()
		{
			if (ModUtils.NetworkMode == NetworkMode.None)
			{
				if (TimePaused)
				{
					Main.time = PausedTime;
				}
			}
			base.Update();
		}
	}

	internal class TimeWeatherControlHotbar : UIHotbar
	{
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
        public static Texture2D playTexture
		{
			get
			{
				if (_playTexture == null) _playTexture = HEROsMod.instance.GetTexture("Images/speed1");
				return _playTexture;
			}
		}

		public static Texture2D pauseTexture
		{
			get
			{
				if (_pauseTexture == null) _pauseTexture = HEROsMod.instance.GetTexture("Images/speed0");
				return _pauseTexture;
			}
		}

		public TimeWeatherControlHotbar()
		{
            buttonView = new UIView();
			Height = 54;
			UpdateWhenOutOfBounds = true;
            //this.Visible = false;

            buttonView.Height = base.Height;
			base.Anchor = AnchorPosition.Top;
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
            //for (int i = 0; i < children.Count; i++)
            //{
            //	if (children[i].Visible)
            //	{
            //		children[i].X = xPos;
            //		xPos += children[i].Width + spacing;
            //		children[i].Y = Height / 2 - children[i].Height / 2;
            //	}
            //}
            //Width = xPos;
		}

		public override void test()
		{
			//	ModUtils.DebugText("TEST " + buttonView.ChildCount);

			Height = 54;
			UpdateWhenOutOfBounds = true;

			UIImage bStopRain = new UIImage(HEROsMod.instance.GetTexture("Images/rainStop"));
            UIImage bStartRain = new UIImage(HEROsMod.instance.GetTexture("Images/rainIcon"))
            {
                Tooltip = "Start Rain"
            };
            bStopRain.Tooltip = "Stop Rain";
			bStartRain.onLeftClick += bStartRain_onLeftClick;
			bStopRain.onLeftClick += bStopRain_onLeftClick;
			buttonView.AddChild(bStopRain);
			buttonView.AddChild(bStartRain);

			UIImage bStopSandstorm = new UIImage(HEROsMod.instance.GetTexture("Images/rainStop"));
            UIImage bStartSandstorm = new UIImage(HEROsMod.instance.GetTexture("Images/rainIcon"))
            {
                Tooltip = "Start Sandstorm"
            };
            bStopSandstorm.Tooltip = "Stop Sandstorm";
			bStartSandstorm.onLeftClick += bStartSandstorm_onLeftClick;
			bStopSandstorm.onLeftClick += bStopSandstorm_onLeftClick;
			buttonView.AddChild(bStopSandstorm);
			buttonView.AddChild(bStartSandstorm);

            UIImage nightButton = new UIImage(HEROsMod.instance.GetTexture("Images/moonIcon"))
            {
                Tooltip = "Night"
            };
            nightButton.onLeftClick += nightButton_onLeftClick;
            UIImage noonButton = new UIImage(HEROsMod.instance.GetTexture("Images/sunIcon"))
            {
                Tooltip = "Noon"
            };
            noonButton.onLeftClick += noonButton_onLeftClick;
			bPause = new UIImage(TimeWeatherChanger.TimePaused ? playTexture : pauseTexture);
			bPause.onLeftClick += bPause_onLeftClick;
			bPause.Tooltip = TimeWeatherChanger.TimePaused ? "Resume Time" : "Pause Time";// "Toggle Freeze Time";

			buttonView.AddChild(nightButton);
			buttonView.AddChild(noonButton);
			buttonView.AddChild(bPause);

            UIImage sundialButton = new UIImage(HEROsMod.instance.GetTexture("Images/timeRain"))
            {
                Tooltip = "Force Enchanted Sundial"
            };
            sundialButton.onLeftClick += sundialButton_onLeftClick;
			buttonView.AddChild(sundialButton);

            UIImage changeTimeButton = new UIImage(HEROsMod.instance.GetTexture("Images/clockIcon"))
            {
                Tooltip = "Change Time...",
            };
            changeTimeButton.onLeftClick += ChangeTimeButton_onLeftClick;
            buttonView.AddChild(changeTimeButton);

            //float xPos = spacing;
            //for (int i = 0; i < children.Count; i++)
            //{
            //	if (children[i].Visible)
            //	{
            //		children[i].X = xPos;
            //		xPos += children[i].Width + spacing;
            //		children[i].Y = Height / 2 - children[i].Height / 2;
            //	}
            //}
            //Width = xPos;

            base.CenterXAxisToParentCenter();
			float num = spacing;
			for (int i = 0; i < buttonView.children.Count; i++)
			{
                buttonView.children[i].Anchor = AnchorPosition.Left;
                buttonView.children[i].Position = new Vector2(num, 0f);
                buttonView.children[i].CenterYAxisToParentCenter();
                buttonView.children[i].Visible = true;
				//this.buttonView.children[i].ForegroundColor = buttonUnselectedColor;
				num += buttonView.children[i].Width + spacing;
			}
			//this.Resize();
			base.Width = num;
            buttonView.Width = base.Width;
		}

		public override void Update()
		{
			DoSlideMovement();
			//base.CenterXAxisToParentCenter();
			base.Update();
		}

		//public void Resize()
		//{
		//	float num = this.spacing;
		//	for (int i = 0; i < this.buttonView.children.Count; i++)
		//	{
		//		if (this.buttonView.children[i].Visible)
		//		{
		//			this.buttonView.children[i].X = num;
		//			num += this.buttonView.children[i].Width + this.spacing;
		//		}
		//	}
		//	base.Width = num;
		//	this.buttonView.Width = base.Width;
		//}

		private void sundialButton_onLeftClick(object sender, EventArgs e)
		{
			if (Main.netMode == 1) // Client
			{
				HEROsModNetwork.GeneralMessages.RequestForcedSundial();
			}
			else // Single
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

		private void bPause_onLeftClick(object sender, EventArgs e)
		{
			if (Main.netMode != 1)
			{
				TimeWeatherChanger.ToggleTimePause();
				UIImage b = (UIImage)sender;
				TimePausedOfResumed();
				Main.NewText("Time has " + (TimeWeatherChanger.TimePaused ? "been paused" : "resumed"));
			}
			else
			{
				HEROsModNetwork.GeneralMessages.ReqestTimeChange(HEROsModNetwork.GeneralMessages.TimeChangeType.Pause);
			}
		}

		private void bStopRain_onLeftClick(object sender, EventArgs e)
		{
			if (Main.netMode == 1)
			{
				HEROsModNetwork.GeneralMessages.RequestStopRain();
				return;
			}
			Main.NewText("Rain has been turned off");

			ModUtils.StopRain();
		}

		private void bStartRain_onLeftClick(object sender, EventArgs e)
		{
			if (Main.netMode == 1)
			{
				HEROsModNetwork.GeneralMessages.RequestStartRain();
				return;
			}
			Main.NewText("Rain has been turned on");
			ModUtils.StartRain();
		}

		private void bStopSandstorm_onLeftClick(object sender, EventArgs e)
		{
			if (Main.netMode == 1)
			{
				HEROsModNetwork.GeneralMessages.RequestStopSandstorm();
				return;
			}
			Main.NewText("Sandstorm has been turned off");

			ModUtils.StopSandstorm();
		}

		private void bStartSandstorm_onLeftClick(object sender, EventArgs e)
		{
			if (Main.netMode == 1)
			{
				HEROsModNetwork.GeneralMessages.RequestStartSandstorm();
				return;
			}
			Main.NewText("Sandstorm has been turned on");
			ModUtils.StartSandstorm();
		}

		public void TimePausedOfResumed()
		{
			if (TimeWeatherChanger.TimePaused)
			{
				bPause.Texture = playTexture;
			}
			else
			{
				bPause.Texture = pauseTexture;
			}
			bPause.Tooltip = TimeWeatherChanger.TimePaused ? "Resume Time" : "Pause Time";
		}

		private void nightButton_onLeftClick(object sender, EventArgs e)
		{
			if (Main.netMode != 1)
			{
				Main.dayTime = false;
				Main.time = 0;// 19:30
			}
			else
			{
				HEROsModNetwork.GeneralMessages.ReqestTimeChange(HEROsModNetwork.GeneralMessages.TimeChangeType.SetToNight);
			}
		}

		private void noonButton_onLeftClick(object sender, EventArgs e)
		{
			if (Main.netMode != 1)
			{
				Main.dayTime = true;
				Main.time = 27000.0; 
			}
			else
			{
				HEROsModNetwork.GeneralMessages.ReqestTimeChange(HEROsModNetwork.GeneralMessages.TimeChangeType.SetToNoon);
			}
		}

		private void TimeControlWindow_onLeftClick(object sender, EventArgs e)
		{
			UIImage b = (UIImage)sender;
			int rate = (int)b.Tag;
			if (rate > 0)
			{
				//pauseTime = false;
				Main.dayRate = (int)b.Tag;
			}
			else
			{
				//pauseTime = true;
				//previousTime = Main.time;
			}
		}

        private void ChangeTimeButton_onLeftClick(object sender, EventArgs e)
        {
            if (Main.netMode != 1)
            {
                timeSetWindow.Visible = !timeSetWindow.Visible;
            } else
            {

            }
        }

		internal static void Unload()
		{
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

    internal class TimeSetWindow : UIWindow
    {
        private static float spacing = 8;
        private UITextbox timeBox;
        private UIButton SetButton;
        private UIButton CancelButton;

        public TimeSetWindow()
        {
            X = 200;
            Y = 250;
            CanMove = true;
            
            Width = 100;
            SetButton = new UIButton("Set Time");
            SetButton.onLeftClick += SetButton_onLeftClick;
            
            CancelButton = new UIButton("Cancel");
            CancelButton.onLeftClick += CancelButton_onLeftClick;
            CancelButton.Position = new Vector2(spacing * 2 + SetButton.Width, this.Height - 40);
            UILabel SetTimeLabel = new UILabel("Set Time")
            {
                X = spacing,
                Y = spacing,
                OverridesMouse = false,
                Scale = .6f
            };
            
            timeBox = new UITextbox()
            {
                Width = 55,
                Position = new Vector2(spacing, SetTimeLabel.Height + spacing)
            };
            UILabel HoursLabel = new UILabel("(24 hours 0:00-23:59)")
            {
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

        private void CancelButton_onLeftClick(object sender, EventArgs e)
        {
            Visible = false;
        }

        private void SetButton_onLeftClick(object sender, EventArgs e)
        {
            ParseTime(timeBox.Text);
            timeBox.Text = "";
            Visible = false;
        }

        private void ParseTime(string time)
        {
            string pattern = "[^0-9:]";
            float gametime = 0; bool daytime = true;
            if (!System.Text.RegularExpressions.Regex.IsMatch(time, pattern) && System.Text.RegularExpressions.Regex.IsMatch(time, "^[0-9]{1,2}:[0-9]{1,2}$"))
            {
                int hour = int.Parse(time.Split(':')[0]);
                int minute = int.Parse(time.Split(':')[1]);
                if (hour >= 0 && hour <= 23 && minute >= 0 && minute <= 59)
                {
                    if (hour >= 0 && hour <= 4)
                    {
                        if (hour == 4 && minute < 30)
                        {
                            gametime = 16200 + hour * 3600 + minute * 60;
                            daytime = false;
                        } else if (hour == 4 && minute >= 30)
                        {
                            gametime = hour * 3600 + minute * 60 - 16200;
                            daytime = true;
                        } else
                        {
                            gametime = 16200 + hour * 3600 + minute * 60;
                            daytime = false;
                        }
                    } else if (hour > 4 && hour < 19)
                    {
                        gametime = hour * 3600 + minute * 60 - 16200;
                        daytime = true;
                    } else if (hour >= 19)
                    {
                        if (hour == 19 && minute < 30)
                        {
                            gametime = hour * 3600 + minute * 60 - 16200;
                            daytime = true;
                        } else if (hour == 19 && minute >= 30)
                        {
                            gametime = hour * 3600 + minute * 60 - 70200;
                            daytime = false;
                        } else
                        {
                            gametime = hour * 3600 + minute * 60 - 70200;
                            daytime = false;
                        }
                    }
                }

                Main.time = gametime;
                Main.dayTime = daytime;
                Main.NewText("Time successfully changed to " + time);
            } else
            {
                Main.NewText("Wrong time format!", Color.Red);
            }
        }
    }
}