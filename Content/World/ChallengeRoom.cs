using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using SubworldLibrary;
using StructureHelper;
using Terraria.DataStructures;
using System;
using ChallengeRooms.Content.Tiles;
using ChallengeRooms.Content.Walls;
using Terraria.Localization;

namespace ChallengeRooms.Content.World
{
    public class ChallengeRoom : Subworld
    {
        public override int Width => 250 + 66 * 3 + 18;
        public override int Height => 150 + 66 + 200;

        public override bool ShouldSave => false;
        public override bool NormalUpdates => true;

        public override List<GenPass> Tasks => new List<GenPass>()
        {
            new WorldSetup("World Setup", 1),
            new Rooms("Rooms", 1),
            new Objects("Objects", 1),
        };

        public override void OnExit()
        {
            foreach (Player player in Main.ActivePlayers)
            {
                player.GetModPlayer<ChallengePlayer>().challengeDoorExitPosition = player.position;
                player.noFallDmg = true;
            }
        }
    }

    public class WorldSetup : GenPass
    {
        public WorldSetup(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Setting things up";

            Main.worldSurface = 0;
            Main.rockLayer = 0;

            for (int y = 20; y < Main.maxTilesY - 20; y++)
            {
                for (int x = 20; x < Main.maxTilesX - 20; x++)
                {
                    Tile tile = Main.tile[x, y];
                    tile.HasTile = true;
                    tile.TileType = (ushort)ModContent.TileType<ChallengeBrick>();
                    tile.WallType = (ushort)ModContent.WallType<ChallengeBrickWallUnsafe>();
                }
            }
        }
    }

    public class Rooms : GenPass
    {
        public Rooms(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        int roomWidth => 66;
        int roomHeight => 66;

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Placing rooms";

            Rectangle structure = new Rectangle(0, 0, roomWidth * 3, roomHeight);
            structure.X = Main.maxTilesX / 2 - structure.Width / 2;
            structure.Y = (Main.maxTilesY - 200) / 2 - structure.Height / 2;
            Vector2 structurePos = structure.TopLeft();

            Generator.GenerateStructure("Content/World/Structures/Rooms/Entrance " + (Random.Shared.Next(3) + 1), (structurePos + new Vector2(-36, 36)).ToPoint16(), ModContent.GetInstance<ChallengeRooms>());

            Main.spawnTileY = structure.Bottom - 6;
            Main.spawnTileX = structure.Left - 29;

            List<int> rooms = new List<int>();
            for (int i = 0; i < 6; i++)
            {
                rooms.Add(i);
            }

            for (int i = 0; i < 3; i++)
            {
                Vector2 roomPos = new Vector2(structurePos.X + i * roomWidth, structurePos.Y);

                if (i == 2)
                {
                    int room = Random.Shared.Next(2);

                    Generator.GenerateStructure("Content/World/Structures/Rooms/Combat " + (room + 1), roomPos.ToPoint16(), ModContent.GetInstance<ChallengeRooms>());
                }
                else
                {
                    int room = rooms[Random.Shared.Next(rooms.Count)];
                    rooms.Remove(room);

                    Generator.GenerateStructure("Content/World/Structures/Rooms/Parkour " + (room + 1), roomPos.ToPoint16(), ModContent.GetInstance<ChallengeRooms>());
                }
            }

            Generator.GenerateStructure("Content/World/Structures/Rooms/Exit", new Point16((int)structurePos.X + roomWidth * 3 - 6, structure.Bottom - 11), ModContent.GetInstance<ChallengeRooms>());

            //for (int y = 40; y < Main.maxTilesY - 40; y++)
            //{
            //    for (int x = 40; x < Main.maxTilesX - 40; x++)
            //    {
            //        Tile tile = Main.tile[x, y];
            //        if (tile.HasTile && (!CanHoldWall(x, y + 1) && Main.tile[x, y + 1].WallType == ModContent.WallType<ChallengeBackground>() || !CanHoldWall(x, y - 1) && Main.tile[x, y - 1].WallType == 0))
            //        {
            //            tile.WallType = 0;
            //        }
            //        else if (tile.WallType == ModContent.WallType<ChallengeBackground>())
            //        {
            //            tile.WallType = 0;
            //        }
            //    }
            //}
        }

        //private bool CanHoldWall(int x, int y)
        //{
        //    Tile tile = Main.tile[x, y];
        //    return tile.HasTile && tile.TileType == ModContent.TileType<ChallengeBrick>();
        //}
    }

    public class Objects : GenPass
    {
        public Objects(string name, float loadWeight) : base(name, loadWeight)
        {
        }
        
        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Placing objects";

            //for (int y = 40; y < Main.maxTilesY - 40; y++)
            //{
            //    for (int x = 40; x < Main.maxTilesX - 40; x++)
            //    {
            //        Tile tile = Main.tile[x, y];
            //        if (!tile.HasTile)
            //        {
            //            tile = Main.tile[x, y + 1];
            //            if (tile.HasTile && Main.tileSolid[tile.TileType] && Random.Shared.Next(4) == 0 && tile.TileType != ModContent.TileType<CopperPipe>())
            //            {
            //                bool valid = true;
            //                for (int i = x; i <= x + 1; i++)
            //                {
            //                    tile = Main.tile[i, y + 1];
            //                    if (tile.TileType == ModContent.TileType<CopperPlatform>() || tile.TileType == ModContent.TileType<EnergyCannon>() || tile.TileType == ModContent.TileType<SpikeTrap>() || tile.TileType == ModContent.TileType<FlameTrap>())
            //                    {
            //                        valid = false;
            //                    }
            //                    else
            //                    {
            //                        for (int j = y - 2; !WorldGen.SolidTile3(i, j); j--)
            //                        {
            //                            if (Main.tile[i, j].TileType == ModContent.TileType<EnergyCannon>())
            //                            {
            //                                valid = false;
            //                            }
            //                        }
            //                    }

            //                    if (!valid)
            //                    {
            //                        break;
            //                    }

            //                }
            //                if (valid)
            //                {
            //                    WorldGen.PlacePot(x, y, style: Random.Shared.Next(31, 34));
            //                }
            //            }
            //        }
            //    }
            //}

            for (int y = 40; y < Main.maxTilesY - 40; y++)
            {
                for (int x = 40; x < Main.maxTilesX - 40; x++)
                {
                    Tile tile = Main.tile[x, y];
                    if (!tile.HasTile)
                    {
                        tile = Main.tile[x, y + 1];
                        if (tile.HasTile && Main.tileSolid[tile.TileType] && !Main.tile[x, y - 1].HasTile && tile.TileType != ModContent.TileType<CopperPlatform>() && tile.TileType != ModContent.TileType<CopperPipe>() && tile.TileType != ModContent.TileType<SpikeTrap>())
                        {
                            if (WorldGen.genRand.NextBool(24))
                            {
                                WorldGen.Place3x2(x, y, TileID.LargePiles, Random.Shared.Next(7));
                            }
                            if (WorldGen.genRand.NextBool(18))
                            {
                                WorldGen.PlaceSmallPile(x, y, Random.Shared.Next(6, 16), 1);
                            }
                            if (WorldGen.genRand.NextBool(12))
                            {
                                WorldGen.PlaceSmallPile(x, y, Main.rand.NextBool(2) ? Random.Shared.Next(12, 28) : Random.Shared.Next(28, 36), 0);
                            }
                        }
                    }
                }
            }
        }

        private void MediumPile(int x, int y, int Xframe = 0, int Yframe = 0)
        {
            if (!Framing.GetTileSafely(x, y).HasTile && !Framing.GetTileSafely(x + 1, y).HasTile)
            {
                Framing.GetTileSafely(x, y).HasTile = true;
                Framing.GetTileSafely(x + 1, y).HasTile = true;
                Framing.GetTileSafely(x, y).TileType = TileID.SmallPiles;
                Framing.GetTileSafely(x + 1, y).TileType = TileID.SmallPiles;

                Framing.GetTileSafely(x, y).TileFrameX = (short)(Xframe * 36);
                Framing.GetTileSafely(x + 1, y).TileFrameX = (short)(Xframe * 36 + 18);
                Framing.GetTileSafely(x, y).TileFrameY = (short)(18 + Yframe * 18);
                Framing.GetTileSafely(x + 1, y).TileFrameY = (short)(18 + Yframe * 18);
            }
        }
    }
}