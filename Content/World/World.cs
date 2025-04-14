using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using ChallengeRooms.Content.Tiles;
using StructureHelper;

namespace ChallengeRooms.Content.World
{
    public class World : ModSystem
    {
        public override void PostWorldGen()
        {
            int count = 0;
            while (count < (Main.maxTilesX * (Main.maxTilesY / 1200f)) / 200 * ModContent.GetInstance<ChallengeConfig>().Frequency)
            {
                int x = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.2f), (int)(Main.maxTilesX * 0.8f));
                int y = WorldGen.genRand.Next((int)(Main.rockLayer), Main.maxTilesY - 200);
                Rectangle area = new Rectangle(x - 8, y - 9, 15, 19);

                if (GenVars.structures.CanPlace(area))
                {
                    bool valid = true;
                    bool[] validTiles = TileID.Sets.Factory.CreateBoolSet(false, TileID.Dirt, TileID.Grass, TileID.Stone, TileID.GreenMoss, TileID.BrownMoss, TileID.RedMoss, TileID.BlueMoss, TileID.PurpleMoss, TileID.LavaMoss, TileID.XenonMoss, TileID.KryptonMoss, TileID.ArgonMoss, TileID.VioletMoss, TileID.Silt, TileID.SnowBlock, TileID.IceBlock, TileID.Slush, TileID.Mud, TileID.JungleGrass, TileID.MushroomGrass, TileID.MushroomBlock, TileID.Sand, TileID.HardenedSand, TileID.Sandstone, TileID.Marble, TileID.Granite);
                    for (int j = area.Top - 3; j < area.Bottom + 3; j++)
                    {
                        for (int i = area.Left - 1; i < area.Right + 1; i++)
                        {
                            Tile tile = Main.tile[i, j];
                            if (Main.tileSolid[tile.TileType] && !validTiles[tile.TileType] && !TileID.Sets.Ore[tile.TileType] || TileID.Sets.BasicChest[tile.TileType] || tile.TileType == TileID.MinecartTrack || tile.TileType == TileID.Trees || tile.TileType == TileID.MushroomTrees || tile.RedWire || tile.BlueWire || tile.GreenWire || tile.YellowWire || tile.HasActuator)
                            {
                                valid = false; break;
                            }
                        }
                        if (!valid) { break; }
                    }
                    for (int j = area.Top - 100; j < area.Bottom + 100; j++)
                    {
                        for (int i = area.Left - 100; i < area.Right + 100; i++)
                        {
                            Tile tile = Main.tile[i, j];
                            if (tile.TileType == ModContent.TileType<ChallengeBrick>())
                            {
                                valid = false; break;
                            }
                        }
                        if (!valid) { break; }
                    }
                    for (int i = area.Left - 1; i < area.Right + 1; i++)
                    {
                        if (!WorldGen.SolidTile(i, area.Bottom - 1))// || !WorldGen.SolidTile3(i, area.Top - 1))
                        {
                            valid = false; break;
                        }
                    }
                    for (int j = y - 2; j <= y + 2; j++)
                    {
                        if (WorldGen.SolidTile(area.Left - 1, j) || WorldGen.SolidTile(area.Right, j))
                        {
                            valid = false; break;
                        }
                        else
                        for (int i = area.Left; i < area.Right; i++)
                        {
                            if (Main.tile[i, j].LiquidAmount > 0)
                            {
                                valid = false; break;
                            }
                        }
                    }

                    if (valid)
                    {
                        for (int j = area.Top; j < area.Bottom; j++)
                        {
                            for (int i = area.Left; i < area.Right; i++)
                            {
                                WorldGen.KillTile(i, j, noItem: true);
                            }
                        }

                        for (int j = area.Top + 4; j < area.Bottom - 4; j++)
                        {
                            if (WorldGen.SolidOrSlopedTile(area.Left - 1, j))
                            {
                                WorldGen.KillTile(area.Left - 1, j, noItem: true);
                            }
                            if (WorldGen.SolidOrSlopedTile(area.Right, j))
                            {
                                WorldGen.KillTile(area.Right, j, noItem: true);
                            }
                        }

                        Generator.GenerateStructure("Content/World/Structures/Gateway", area.TopLeft().ToPoint16(), ModContent.GetInstance<ChallengeRooms>());

                        count++;
                    }
                }
            }
        }
    }
}
