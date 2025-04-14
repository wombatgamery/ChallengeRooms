using ChallengeRooms.Content.Dusts;
using ChallengeRooms.Content.Items;
using ChallengeRooms.Content.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace ChallengeRooms.Content.Tiles
{
    public class ChallengeSpawner : ModTile
    {
		public override void SetStaticDefaults()
		{
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileNoAttach[Type] = true;

            TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;
            TileID.Sets.PreventsTileReplaceIfOnTopOfIt[Type] = true;
            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;
            TileID.Sets.CanBeClearedDuringOreRunner[Type] = false;
            TileID.Sets.GetsDestroyedForMeteors[Type] = false;
            TileID.Sets.DisableSmartCursor[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16 };
            TileObjectData.newTile.CoordinatePadding = 0;
            TileObjectData.newTile.Origin = new Point16(0, 3);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<TEChallengeSpawner>().Hook_AfterPlacement, -1, 0, false);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);

            LocalizedText name = CreateMapEntryName();

            DustType = DustID.Copper;
			HitSound = SoundID.Tink;

			AddMapEntry(new Color(207, 117, 74), CreateMapEntryName());
		}


        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Point16 origin = TileUtils.GetTileOrigin(i, j, true);
            ModContent.GetInstance<TEChallengeSpawner>().Kill(origin.X, origin.Y);
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            if (TileUtils.TryGetTileEntityAs(i, j, out TEChallengeSpawner tileEntity) && tileEntity.awake)
            {
                r = 200f / 255f;
                g = 50f / 255f;
                b = 100f / 255f;
            }
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            if (TileUtils.TryGetTileEntityAs(i, j, out TEChallengeSpawner tileEntity))
            {
                Tile tile = Main.tile[i, j];

                if (tile.TileFrameX == 0 && tile.TileFrameY == 0)
                {
                    Main.instance.TilesRenderer.AddSpecialLegacyPoint(i, j);
                }
            }
        }

        public override bool CanKillTile(int i, int j, ref bool blockDamaged) => false;

		public override bool CanExplode(int i, int j) => false;

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (TileUtils.TryGetTileEntityAs(i, j, out TEChallengeSpawner tileEntity))
            {
                Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
                Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y + 16 - (tileEntity.awake || tileEntity.animationFrame == 0 ? 0 : ((tileEntity.animationFrame == 1 ? 2 : tileEntity.animationFrame == 2 ? 5 : tileEntity.animationFrame == 3 ? 7 : 8) * 2))) + zero, new Rectangle(tileEntity.animationFrame == 0 ? 32 : 64, 0, 32, 38), Lighting.GetColor(i, j + 1), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

                if (tileEntity.awake)
                {
                    Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, new Rectangle(0, 64, 32, 64), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                }
            }
        }
    }
    public class TEChallengeSpawner : ModTileEntity
    {
        public bool awake = false;
        public int spawnTimer = 0;
        public bool spawning = false;
        public int wavesLeft = 5;
        public int itemsSpawned;

        public byte animationFrame = 0;
        public int animationTimer = 0;

        int level => 5 - wavesLeft;

        int randomSkeletonID;
        bool randomBool;

        int randomModuleID;
        int randomTrapItemID;
        int randomTrapStackSize;
        int randomPotionItemID;

        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.HasTile && tile.TileType == ModContent.TileType<ChallengeSpawner>() && tile.TileFrameX == 0 && tile.TileFrameY == 0;
        }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                int width = 2;
                int height = 4;
                NetMessage.SendTileSquare(Main.myPlayer, i, j, width, height);

                NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, i, j, Type, 0f, 0, 0, 0);
                return -1;
            }
            Point16 tileOrigin = new Point16(0, 3);
            return Place(i - tileOrigin.X, j - tileOrigin.Y);
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(awake);

            writer.Write(wavesLeft);

            writer.Write(RandomSkeleton());
            writer.Write(Main.rand.NextBool(2));

            writer.Write(Main.rand.Next(4));
            if (Main.rand.NextBool(3))
            {
                writer.Write(ModContent.ItemType<Items.Placeable.EnergyCannon>());
                writer.Write(Main.rand.Next(4, 7));
            }
            else
            {
                writer.Write(Main.rand.NextBool(2) ? ModContent.ItemType<Items.Placeable.SpikeTrap>() : ModContent.ItemType<Items.Placeable.FlameTrap>());
                writer.Write(Main.rand.Next(6, 10));
            }

            if (Main.rand.NextBool(2))
            {
                if (Main.rand.NextBool(2))
                {
                    writer.Write(ItemID.EndurancePotion);
                }
                else writer.Write(ItemID.LifeforcePotion);
            }
            else
            {
                if (Main.rand.NextBool(2))
                {
                    writer.Write(ItemID.WrathPotion);
                }
                else writer.Write(ItemID.RagePotion);
            }
        }

        public override void NetReceive(BinaryReader reader)
        {
            awake = reader.ReadBoolean();

            wavesLeft = reader.ReadInt32();

            randomSkeletonID = reader.ReadInt32();
            randomBool = reader.ReadBoolean();

            randomModuleID = reader.ReadInt32();
            randomTrapItemID = reader.ReadInt32();
            randomTrapStackSize = reader.ReadInt32();
            randomPotionItemID = reader.ReadInt32();
        }

        public override void Update()
        {
            if (awake)
            {
                if (spawning)
                {
                    animationTimer = 0;

                    for (int spawnsLeft = level + 5; spawnsLeft > 0; spawnsLeft--)
                    {
                        if (Main.netMode != NetmodeID.Server)
                        {
                            randomSkeletonID = RandomSkeleton();
                            randomBool = Main.rand.NextBool(2);
                        }

                        int npcType;
                        if (level > 2 && spawnsLeft == 1)
                        {
                            npcType = NPCID.Demon;
                        }
                        else if (level > 0 && spawnsLeft % 3 == 0)
                        {
                            npcType = NPCID.CaveBat;
                        }
                        else if (randomBool)
                        {
                            npcType = randomSkeletonID;
                        }
                        else
                        {
                            if (level > 3)
                            {
                                npcType = NPCID.MotherSlime;
                            }
                            else npcType = NPCID.BlackSlime;
                        }

                        int x = Position.X + 1 + (int)(Math.Sin(Main.GameUpdateCount * spawnsLeft) * 29);
                        int y = Position.Y + 2;
                        while (!CanSpawnOn(x - 1, y) && !CanSpawnOn(x, y) && !CanSpawnOn(x + 1, y))
                        {
                            y++;
                        }
                        y--;
                        while (CanSpawnOn(x - 1, y) || CanSpawnOn(x, y) || CanSpawnOn(x + 1, y))
                        {
                            y--;
                        }
                        y -= 2;

                        Projectile proj = Projectile.NewProjectileDirect(new EntitySource_TileEntity(this), new Vector2(x, y) * 16 + Vector2.One * 8, Vector2.Zero, ModContent.ProjectileType<Rift>(), 0, 0);
                        if (proj.ModProjectile is Rift rift)
                        {
                            rift.monsterToSpawn = npcType;
                        }
                        NetMessage.SendData(MessageID.SyncProjectile, number: proj.whoAmI);
                    }

                    spawning = false;
                    wavesLeft--;
                }
                else
                {
                    bool hostilesPresent = false;
                    foreach (NPC npc in Main.ActiveNPCs)
                    {
                        if (!npc.friendly && npc.damage > 0 && npc.life > 0)
                        {
                            hostilesPresent = true;
                            break;
                        }
                    }
                    if (!hostilesPresent)
                    {
                        bool riftsPresent = false;
                        foreach (Projectile p in Main.ActiveProjectiles)
                        {
                            if (p.type == ModContent.ProjectileType<Rift>())
                            {
                                riftsPresent = true;
                                break;
                            }
                        }

                        if (!riftsPresent)
                        {
                            if (wavesLeft > 0)
                            {
                                spawning = true;
                            }
                            else
                            {
                                awake = false;

                                SoundEngine.PlaySound(new SoundStyle("ChallengeRooms/Content/Sounds/Objects/spawner-terminate"), (Position.ToVector2() + new Vector2(2, 1)) * 16);

                                animationTimer = -15;
                                itemsSpawned = 0;
                            }
                        }
                    }
                }

                animationTimer++;
            }
            else
            {
                if (wavesLeft > 0)
                {
                    if (!awake)
                    {
                        foreach (Player player in Main.ActivePlayers)
                        {
                            if (!player.DeadOrGhost && MathHelper.Distance(player.Center.X, (Position.X + 2) * 16) < 16 * 16)
                            {
                                awake = true;
                                SoundEngine.PlaySound(new SoundStyle("ChallengeRooms/Content/Sounds/Objects/spawner-awake"), (Position.ToVector2() + new Vector2(1, 2)) * 16);

                                wavesLeft = 5;

                                NetMessage.SendData(MessageID.TileEntitySharing, number: ID);

                                break;
                            }
                        }
                    }
                }
                else if (animationFrame < 4)
                {
                    if (++animationTimer >= 4)
                    {
                        animationFrame++;
                        animationTimer = 0;
                    }
                }
                else
                {
                    if (itemsSpawned < 5)
                    {
                        spawnTimer--;
                        if (spawnTimer <= 0)
                        {
                            int type;
                            int stack = 1;
                            if (itemsSpawned % 5 == 0)
                            {
                                type = ModContent.ItemType<ModularEnhancer>();
                            }
                            else if (itemsSpawned % 5 == 1)
                            {
                                ChallengePlayer modPlayer = Main.LocalPlayer.GetModPlayer<ChallengePlayer>();

                                if (Main.netMode != NetmodeID.Server)
                                {
                                    type = modPlayer.FromModuleID(Main.rand.Next(4));
                                }
                                else
                                {
                                    type = modPlayer.FromModuleID(randomModuleID);
                                }
                            }
                            else if (itemsSpawned % 5 == 2)
                            {
                                if (Main.netMode != NetmodeID.Server)
                                {
                                    if (Main.rand.NextBool(2))
                                    {
                                        if (Main.rand.NextBool(2))
                                        {
                                            type = ItemID.EndurancePotion;
                                        }
                                        else type = ItemID.LifeforcePotion;
                                    }
                                    else
                                    {
                                        if (Main.rand.NextBool(2))
                                        {
                                            type = ItemID.WrathPotion;
                                        }
                                        else type = ItemID.RagePotion;
                                    }
                                }
                                else
                                {
                                    type = randomPotionItemID;
                                }
                                stack = Main.rand.Next(2, 4);
                            }
                            else if (itemsSpawned % 5 == 3)
                            {
                                if (Main.netMode != NetmodeID.Server)
                                {
                                    if (Main.rand.NextBool(3))
                                    {
                                        type = ModContent.ItemType<Items.Placeable.EnergyCannon>();
                                        stack = Main.rand.Next(4, 7);
                                    }
                                    else
                                    {
                                        type = Main.rand.NextBool(2) ? ModContent.ItemType<Items.Placeable.SpikeTrap>() : ModContent.ItemType<Items.Placeable.FlameTrap>();
                                        stack = Main.rand.Next(6, 10);
                                    }
                                }
                                else
                                {
                                    type = randomTrapItemID;
                                    stack = randomTrapStackSize;
                                }
                            }
                            else
                            {
                                type = ItemID.GoldCoin;
                                stack = 20;
                            }

                            Vector2 pos = new Vector2(Position.X + 1, Position.Y + 3) * 16;
                            Item.NewItem(new EntitySource_TileBreak(Position.X, Position.Y), pos, new Item(type, stack));

                            Dust dust = Dust.NewDustPerfect(pos, DustID.TreasureSparkle);
                            dust.noGravity = true;
                            dust.velocity = -Vector2.UnitY + Main.rand.NextVector2Circular(1, 1);

                            for (int i = 0; i < 10; i++)
                            {
                                dust = Dust.NewDustPerfect(pos, DustID.Cloud, Alpha: 153);
                                dust.noGravity = true;
                                dust.velocity = -Vector2.UnitY * 2 + Main.rand.NextVector2Circular(2, 2);
                            }

                            SoundEngine.PlaySound(SoundID.Item10, Position.ToVector2() * 16);

                            spawnTimer = 15;
                            itemsSpawned++;
                        }
                    }
                }
            }
        }

        private bool CanSpawnOn(int x, int y)
        {
            return Main.tile[x, y].HasTile && (Main.tileSolid[Main.tile[x, y].TileType] || TileID.Sets.Platforms[Main.tile[x, y].TileType]);
        }

        private int RandomSkeleton()
        {
            if (Main.rand.NextBool(4))
            {
                return NPCID.Skeleton;
            }
            return Main.rand.Next(201, 204);
        }
    }
}
