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
    public class EnergyCannon : ModTile
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
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<TEEnergyCannon>().Hook_AfterPlacement, -1, 0, false);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);

            DustType = DustID.Copper;
			HitSound = SoundID.Tink;

            RegisterItemDrop(ModContent.ItemType<Items.Placeable.EnergyCannon>(), new int[] { 0, 1, 2, 3 });

            AddMapEntry(new Color(50, 186, 255), CreateMapEntryName());
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (!fail)
            {
                ModContent.GetInstance<TEEnergyCannon>().Kill(i, j);
            }
        }

        public override void PlaceInWorld(int i, int j, Item item)
        {
            Tile tile = Main.tile[i, j];
            if (Main.LocalPlayer.direction == 1)
            {
                tile.TileFrameX += 32;
            }
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendTileSquare(-1, i, j, 1, TileChangeType.None);
            }
        }

        public override bool CanKillTile(int i, int j, ref bool blockDamaged) => !SubworldSystem.IsActive<ChallengeRoom>();

        public override bool CanExplode(int i, int j) => !SubworldSystem.IsActive<ChallengeRoom>();

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 89f / 255f;
            g = 89f / 255f;
            b = 178f / 255f;
        }

        public override bool Slope(int i, int j)
        {
            if (SubworldSystem.IsActive<ChallengeRoom>())
            {
                return false;
            }

            Tile tile = Main.tile[i, j];
            tile.TileFrameX += 16;
            if (tile.TileFrameX >= 64)
            {
                tile.TileFrameX = 0;
            }

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendTileSquare(-1, i, j, 1, TileChangeType.None);
            }

            return false;
        }

        public override bool IsTileDangerous(int i, int j, Player player) => true;

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (TileUtils.TryGetTileEntityAs(i, j, out TEEnergyCannon tileEntity))
            {
                long animationTimer = Main.GameUpdateCount - tileEntity.shotOffset;
                if (animationTimer % 120 >= 0)
                {
                    if (animationTimer < 12 + tileEntity.shotOffset)
                    {
                        Tile tile = Main.tile[i, j];
                        Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
                        Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.TileFrameX, 16, 16, 16), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    }
                    else if (animationTimer < 16 + tileEntity.shotOffset)
                    {
                        Tile tile = Main.tile[i, j];
                        Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
                        Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.TileFrameX, 32, 16, 16), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    }
                }
            }
        }
    }

    public class TEEnergyCannon : ModTileEntity
    {
        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.HasTile && tile.TileType == ModContent.TileType<EnergyCannon>();
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

        public int shotOffset;

        public override void Update()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Tile tile = Main.tile[Position.X, Position.Y];

                shotOffset = tile.TileFrameX % 32 == 0 ? ((Position.Y % 12 < 6) ? 60 : 0) : ((Position.X % 6 < 3) ? 60 : 0);

                if (Main.GameUpdateCount % 120 == shotOffset)
                {
                    bool vertical = tile.TileFrameX % 32 == 16;
                    int direction = tile.TileFrameX < 32 ? -1 : 1;
                    Vector2 position = (Position.ToVector2() + Vector2.One / 2) * 16;
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_None(), position, new Vector2(vertical ? 0 : 1, vertical ? 1 : 0) * direction * 8, ModContent.ProjectileType<EnergyOrb>(), 50, 0);
                    if (vertical)
                    {
                        position.Y = Position.Y * 16 + 8 - proj.height / 2 + (8 + proj.height / 2) * direction;
                    }
                    else position.X = Position.X * 16 + 8 - proj.width / 2 + (8 + proj.width / 2) * direction;
                    NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, proj.whoAmI);
                }
            }
        }
    }
}
