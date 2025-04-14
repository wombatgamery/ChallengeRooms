using ChallengeRooms.Content.Projectiles;
using ChallengeRooms.Content.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SubworldLibrary;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace ChallengeRooms.Content.Tiles
{
    public class SpikeTrap : ModTile
    {
		public override void SetStaticDefaults()
		{
            Main.tileFrameImportant[Type] = true;
            Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileLavaDeath[Type] = false;

            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;
			TileID.Sets.CanBeClearedDuringOreRunner[Type] = false;

            //TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.CoordinateHeights = new[] { 16 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.CoordinatePadding = 0;
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<TESpikeTrap>().Hook_AfterPlacement, -1, 0, false);
            TileObjectData.addTile(Type);

            DustType = DustID.Copper;
			HitSound = SoundID.Tink;

            AddMapEntry(new Color(192, 214, 214), CreateMapEntryName());
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (!fail)
            {
                ModContent.GetInstance<TESpikeTrap>().Kill(i, j);
            }
        }

        public override void PlaceInWorld(int i, int j, Item item)
        {
            Tile tile = Main.tile[i, j];
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendTileSquare(-1, i, j, 1, TileChangeType.None);
            }
        }

        public override bool CanKillTile(int i, int j, ref bool blockDamaged) => !SubworldSystem.IsActive<ChallengeRoom>();

        public override bool CanExplode(int i, int j) => !SubworldSystem.IsActive<ChallengeRoom>();

        public override bool Slope(int i, int j)
        {
            return false;
        }

        public override bool IsTileDangerous(int i, int j, Player player) => true;
    }

    public class TESpikeTrap : ModTileEntity
    {
        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.HasTile && tile.TileType == ModContent.TileType<SpikeTrap>();
        }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendTileSquare(Main.myPlayer, i, j, 1, 1);

                NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, i, j, Type);
            }
            return Place(i, j);
        }

        public override void Update()
        {
            if (Main.GameUpdateCount % 120 == ((Position.Y % 4 < 2) ? 90 : 30))
            {
                Tile tile = Main.tile[Position.X, Position.Y];

                Vector2 position = (Position.ToVector2() + Vector2.UnitX / 2 - Vector2.UnitY) * 16;
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_None(), position, Vector2.Zero, ModContent.ProjectileType<Spike>(), 75, 0);
                NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, proj.whoAmI);

                for (int j = Position.Y - 1; j >= Position.Y - 2; j--)
                {
                    if (Main.tile[Position.X, j].HasTile && Main.tile[Position.X, j].TileType != ModContent.TileType<ChallengeBrick>() && !TileID.Sets.Platforms[Main.tile[Position.X, j].TileType])
                    {
                        if (SubworldSystem.IsActive<ChallengeRoom>())
                        {
                            WorldGen.KillTile(Position.X, j);
                        }
                    }
                }
            }
        }
    }
}
