using HEROsMod.UIKit;
using HEROsMod.UIKit.UIComponents;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;

namespace HEROsMod.HEROsModServices {
    internal class WorldPurifier : HEROsModService {
        public static WorldPurifier instance;

        public WorldPurifier(UIHotbar hotbar) {
            IsInHotbar = true;
            HotbarParent = hotbar;
            _name = "World Purifier";
            _hotbarIcon = new UIKit.UIImage(HEROsMod.instance.GetTexture("Images/map"));
            _hotbarIcon.onLeftClick += _hotbarIcon_onLeftClick;
            HotbarIcon.Tooltip = "Purify World";
            instance = this;
        }

		public override void MyGroupUpdated() => HasPermissionToUse = HEROsModNetwork.LoginService.MyGroup.HasPermission("PurifyWorld");

		private void _hotbarIcon_onLeftClick(object sender, EventArgs e) {
            UIMessageBox mb = new UIMessageBox("Are you sure you want to purify the world?\nThis cannot be undone!", UIMessageBoxType.YesNo, true);
            mb.yesClicked += Mb_yesClicked;
        }

        private void Mb_yesClicked(object sender, EventArgs e) {
            if (Main.netMode != 1) {
                Main.NewText("Please wait, the world is going to be purified, this may take a few moments to minutes...", Color.Crimson);
                PurifyWorld();
                Main.NewText("Purifying the world complete!", Color.Crimson);
            } else {
                HEROsModNetwork.GeneralMessages.RequestPurifyWorld();
            }
        }

        public static void PurifyWorld() {
            for (int i = 0; i < Main.maxTilesX; i++) {
                for (int j = 0; j < Main.maxTilesY; j++) {
                    if (WorldGen.InWorld(i, j) && Main.tile[i,j] != null) {
                        Tile tile = Main.tile[i, j];
                        if (ModUtils.NumberIsOneOfThese(tile.type, new long[] { TileID.Crimsand, TileID.Pearlsand, TileID.Ebonsand })) {
                            tile.type = TileID.Sand;
                        } else if (ModUtils.NumberIsOneOfThese(tile.type, new long[] { TileID.Ebonstone, TileID.Crimstone, TileID.Pearlstone })) {
                            tile.type = TileID.Stone;
                        } else if (ModUtils.NumberIsOneOfThese(tile.type, new long[] { TileID.CrimsonHardenedSand, TileID.CorruptHardenedSand, TileID.HallowHardenedSand})) {
                            tile.type = TileID.HardenedSand;
                        } else if (ModUtils.NumberIsOneOfThese(tile.type, new long[] { TileID.CrimsonSandstone, TileID.CorruptSandstone, TileID.HallowSandstone})) {
                            tile.type = TileID.Sandstone;
                        } else if (ModUtils.NumberIsOneOfThese(tile.type, new long[] { TileID.CorruptIce, TileID.FleshIce, TileID.HallowedIce})) {
                            tile.type = TileID.IceBlock;
                        } else if (ModUtils.NumberIsOneOfThese(tile.type, new long[] { TileID.CorruptThorns, TileID.CrimtaneThorns })) {
                            tile.type = TileID.JungleThorns;
                        } else if (ModUtils.NumberIsOneOfThese(tile.type, new long[] { TileID.CrimsonVines, TileID.HallowedVines })) {
                            tile.type = TileID.Vines;
                        } else if (ModUtils.NumberIsOneOfThese(tile.type, new long[] { TileID.CorruptGrass, TileID.FleshGrass, TileID.HallowedGrass })) {
                            tile.type = TileID.Grass;
                        } else if (ModUtils.NumberIsOneOfThese(tile.type, new long[] { TileID.CorruptPlants, TileID.HallowedPlants, TileID.HallowedPlants2, TileID.FleshWeeds })) {
                            tile.type = TileID.Plants;
                        }
                        if (tile.wall != 0) {
                            if (ModUtils.NumberIsOneOfThese(tile.wall, new long[] { WallID.CorruptGrassUnsafe, WallID.HallowedGrassUnsafe, WallID.CrimsonGrassUnsafe })) {
                                tile.wall = WallID.GrassUnsafe;
                            } else if (ModUtils.NumberIsOneOfThese(tile.wall, new long[] { WallID.EbonstoneUnsafe, WallID.PearlstoneBrickUnsafe, WallID.CrimstoneUnsafe})) {
                                tile.wall = WallID.Stone;
                            } else if (ModUtils.NumberIsOneOfThese(tile.wall, new long[] { WallID.CorruptionUnsafe1, WallID.HallowUnsafe1, WallID.CrimsonUnsafe1} )) {
                                tile.wall = WallID.DirtUnsafe1;
                            } else if (ModUtils.NumberIsOneOfThese(tile.wall, new long[] { WallID.CorruptionUnsafe2, WallID.HallowUnsafe2, WallID.CrimsonUnsafe2 })) {
                                tile.wall = WallID.DirtUnsafe2;
                            } else if (ModUtils.NumberIsOneOfThese(tile.wall, new long[] { WallID.CorruptionUnsafe3, WallID.HallowUnsafe3, WallID.CrimsonUnsafe3 })) {
                                tile.wall = WallID.DirtUnsafe3;
                            } else if (ModUtils.NumberIsOneOfThese(tile.wall, new long[] { WallID.CorruptionUnsafe4, WallID.HallowUnsafe4, WallID.CrimsonUnsafe4 })) {
                                tile.wall = WallID.DirtUnsafe4;
                            } else if (ModUtils.NumberIsOneOfThese(tile.wall, new long[] { WallID.CorruptHardenedSand, WallID.CrimsonHardenedSand, WallID.HallowHardenedSand})) {
                                tile.wall = WallID.HardenedSand;
                            }
                        }
                    }
                }
            }
        }
    }
}
