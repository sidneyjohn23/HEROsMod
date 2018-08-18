using HEROsMod.UIKit;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;

namespace HEROsMod.HEROsModServices {
    internal class BuffService : HEROsModService {
        private BuffWindow _buffWindow;

        //public static int[] Buffs = new int[] {
        //	112, 107, 1, 2, 3 , 4, 5, 6, 7, 8, 9, 10, 11, 12, 106, 13, 14,
        //	15, 16 ,17, 18, 19, 26, 27, 29, 93, 48, 63, 59, 58
        //};
        public static int[] SkipBuffs = new int[] {
            BuffID.Pygmies, BuffID.LeafCrystal, BuffID.IceBarrier, BuffID.BabySlime, BuffID.Ravens, BuffID.BeetleEndurance1, BuffID.BeetleEndurance2, BuffID.BeetleEndurance3,
            BuffID.BeetleMight1, BuffID.BeetleMight2, BuffID.BeetleMight3, BuffID.ImpMinion, BuffID.SpiderMinion, BuffID.TwinEyesMinion,
            BuffID.MinecartLeft, BuffID.MinecartLeftMech, BuffID.MinecartLeftWood, BuffID.MinecartRight, BuffID.MinecartRightMech, BuffID.MinecartRightWood,
            BuffID.SharknadoMinion, BuffID.UFOMinion, BuffID.DeadlySphere, BuffID.SolarShield1, BuffID.SolarShield2, BuffID.SolarShield3, BuffID.StardustDragonMinion,
            BuffID.StardustGuardianMinion, BuffID.HornetMinion, BuffID.PirateMinion, BuffID.StardustMinion, BuffID.Oiled, BuffID.SugarRush, BuffID.StardustMinionBleed, BuffID.Daybreak, BuffID.BetsysCurse, BuffID.ShadowFlame, BuffID.Midas, BuffID.VortexDebuff, BuffID.DryadsWardDebuff,  };

		public BuffService()
		{
			_hotbarIcon = new UIImage(HEROsMod.instance.GetTexture("Images/buffs")/*Main.buffTexture[2]*/);
			HotbarIcon.Tooltip = HEROsMod.HeroText("OpenBuffWindow");
			HotbarIcon.OnLeftClick += HotbarIcon_onLeftClick;

            //_buffWindow = new BuffWindow();
            //this.AddUIView(_buffWindow);
            //_buffWindow.Visible = false;
        }

        public override void MyGroupUpdated() {
            HasPermissionToUse = HEROsModNetwork.LoginService.MyGroup.HasPermission("CanUseBuffs");
            if (!HasPermissionToUse && _buffWindow != null) {
                _buffWindow.Visible = false;
            }
        }

        private void HotbarIcon_onLeftClick(object sender, EventArgs e) {
            if (_buffWindow == null) {
                if (!Main.dedServ) {
                    _buffWindow = new BuffWindow();
                    AddUIView(_buffWindow);
                    _buffWindow.Visible = false;

                    _buffWindow.Y = 270;
                    _buffWindow.X = 130;
                }
            }
            _buffWindow.Visible = !_buffWindow.Visible;
        }
    }

    internal class BuffWindow : UIWindow {
        private UITextbox tbSeconds;
        UIButton bAll;
        UIButton bBuffs;
        UIButton bDebuffs;
        List<int> buffs = new List<int>();
        List<int> category = new List<int>();
        UIScrollView buffView = new UIScrollView();
        float yPos = Spacing;

        public BuffWindow() {
            CanMove = true;

            UILabel lTitle = new UILabel(HEROsMod.HeroText("Buffs")) {
                Scale = .6f,
                X = Spacing,
                Y = Spacing,
                OverridesMouse = false
            };

            UILabel lSeconds = new UILabel(HEROsMod.HeroText("Seconds"));

            tbSeconds = new UITextbox() {
                Text = "60",
                Numeric = true,
                MaxCharacters = 5,
                Width = 75,
                Y = lTitle.Y + lTitle.Height
            };
            buffView = new UIScrollView() {
                X = lTitle.X,
                Y = tbSeconds.Y + tbSeconds.Height + Spacing,
                Width = 300,
                Height = 250
            };

            bAll = new UIButton("All") {
                X = lTitle.X + buffView.Width + Spacing,
                Y = tbSeconds.Y + tbSeconds.Height + Spacing,
                Width = 300
            };
            bAll.OnLeftClick += BAll_onLeftClick;

            bBuffs = new UIButton("Buffs") {
                X = lTitle.X + buffView.Width + Spacing,
                Y = bAll.Y + bAll.Height + Spacing,
                Width = 300
            };
            bBuffs.OnLeftClick += BBuffs_onLeftClick;

            bDebuffs = new UIButton("Debuffs") {
                X = lTitle.X + buffView.Width + Spacing,
                Y = bBuffs.Y + bBuffs.Height + Spacing,
                Width = 300
            };
            bDebuffs.OnLeftClick += BDebuffs_onLeftClick;

            UIImage bClose = new UIImage(closeTexture) {
                Y = Spacing
            };
            bClose.OnLeftClick += BClose_onLeftClick;

            BuildBuffList();
            BAll_onLeftClick(bAll, new EventArgs());

            Width = buffView.X + buffView.Width + 2 * Spacing + bDebuffs.Width;
            Height = buffView.Y + buffView.Height + Spacing;

            tbSeconds.X = Width - tbSeconds.Width - Spacing;
            bClose.X = Width - bClose.Width - Spacing;

            lSeconds.Scale = .4f;
            lSeconds.Anchor = AnchorPosition.Right;
            lSeconds.X = tbSeconds.X - Spacing;
            lSeconds.Y = tbSeconds.Y + tbSeconds.Height / 2;

            AddChild(lTitle);
            AddChild(lSeconds);
            AddChild(tbSeconds);
            AddChild(buffView);
            AddChild(bClose);
            AddChild(bAll);
            AddChild(bBuffs);
            AddChild(bDebuffs);
        }

        private void BDebuffs_onLeftClick(object sender, EventArgs e) {
            category.Clear();
            foreach (int i in buffs) {
                if (Main.debuff[i])
				{
					category.Add(i);
				}
			}
            BuildImages();
            WhiteAllButtons();
            ((UIButton) sender).SetTextColor(Color.Yellow);
        }

        private void BBuffs_onLeftClick(object sender, EventArgs e) {
            category.Clear();
            foreach (int i in buffs) {
                if (Main.debuff[i] || Main.lightPet[i] || Main.vanityPet[i] || i == 0 || BuffService.SkipBuffs.Contains(i))
				{
					continue;
				}

				category.Add(i);
            }
            BuildImages();
            WhiteAllButtons();
            ((UIButton) sender).SetTextColor(Color.Yellow);
        }

        private void BAll_onLeftClick(object sender, EventArgs e) {
            category.Clear();
            category = buffs.ToList();
            BuildImages();
            WhiteAllButtons();
            ((UIButton) sender).SetTextColor(Color.Yellow);
        }

		private void BClose_onLeftClick(object sender, EventArgs e) => Visible = false;

		private void Bg_onLeftClick(object sender, EventArgs e) {
            UIView view = (UIView) sender;
            int buffType = (int) view.Tag;

            if (tbSeconds.Text.Length == 0) {
                tbSeconds.Text = "60";
            }
            int seconds = int.Parse(tbSeconds.Text);

            Main.player[Main.myPlayer].AddBuff(buffType, seconds * 60);
        }

        void BuildBuffList() {
            buffs.Clear();
            for (int i = 1; i < Main.debuff.Length; i++) {
                buffs.Add(i);
            }
        }

        void WhiteAllButtons() {
            bAll.SetTextColor(Color.White);
            bBuffs.SetTextColor(Color.White);
            bDebuffs.SetTextColor(Color.White);
        }

        void BuildImages() {
            buffView.ClearContent();
            foreach (int i in category) {
                int buffType = i;
				UIRect bg = new UIRect
				{
					ForegroundColor = i % 2 == 0 ? Color.Transparent : Color.Blue * .1f,
					X = Spacing,
					Y = yPos,
					Width = 300 - 20 - Spacing * 2,
					Tag = buffType
				};
				string buffDescription = Lang.GetBuffDescription(buffType);
                bg.Tooltip = (buffDescription ?? "");
                bg.OnLeftClick += Bg_onLeftClick;

                UIImage buffImage = new UIImage(Main.buffTexture[buffType]) {
                    X = Spacing,
                    Y = SmallSpacing / 2,
                    OverridesMouse = false
                };
                bg.Height = buffImage.Height + SmallSpacing;
                yPos += bg.Height;

                UILabel label = new UILabel(Lang.GetBuffName(buffType)) {
                    Scale = .4f,
                    Anchor = AnchorPosition.Left,
                    X = buffImage.X + buffImage.Width + Spacing,
                    Y = buffImage.Y + buffImage.Height / 2,
                    OverridesMouse = false
                };
                bg.AddChild(buffImage);
                bg.AddChild(label);
                buffView.AddChild(bg);
                Main.NewText("build image " + i);
            }
            buffView.ContentHeight = yPos;
            yPos = Spacing;
        }
    }
}