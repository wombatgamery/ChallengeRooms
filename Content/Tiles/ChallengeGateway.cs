using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;
using SubworldLibrary;
using ChallengeRooms.Content.World;
using ChallengeRooms.Content.Items;
using Humanizer;
using ChallengeRooms.Content.Items.Hovers;
using Microsoft.Xna.Framework.Input;
using Terraria.GameContent.ObjectInteractions;
using ChallengeRooms.Content.Projectiles;

namespace ChallengeRooms.Content.Tiles
{
    public class ChallengeGateway : ModTile
    {
		public override void SetStaticDefaults()
		{
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileNoAttach[Type] = true;

            TileID.Sets.HasOutlines[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;
            TileID.Sets.PreventsTileReplaceIfOnTopOfIt[Type] = true;
            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;
            TileID.Sets.CanBeClearedDuringOreRunner[Type] = false;
            TileID.Sets.GetsDestroyedForMeteors[Type] = false;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
            TileObjectData.newTile.Height = 5;
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16, 16, 16 };
            TileObjectData.newTile.CoordinatePadding = 0;
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<TEChallengeGateway>().Hook_AfterPlacement, -1, 0, false);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);

            LocalizedText name = CreateMapEntryName();

            DustType = DustID.Copper;
			HitSound = SoundID.Tink;

			AddMapEntry(new Color(124, 68, 58), CreateMapEntryName());
		}

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Point16 origin = TileUtils.GetTileOrigin(i, j, true);
            ModContent.GetInstance<TEChallengeGateway>().Kill(origin.X, origin.Y);
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public override bool RightClick(int i, int j)
        {
            if (TileUtils.TryGetTileEntityAs(i, j, out TEChallengeGateway tileEntity) && tileEntity.cooldown != tileEntity.initialCooldown)
            {
                if (!tileEntity.open)
                {
                    if (tileEntity.cooldown == 0)
                    {
                        bool inCombatTrial = false;
                        if (SubworldSystem.IsActive<ChallengeRoom>())
                        {
                            foreach (NPC npc in Main.ActiveNPCs)
                            {
                                if (!npc.friendly && npc.damage > 0 && npc.life > 0)
                                {
                                    inCombatTrial = true;
                                    break;
                                }
                            }
                            if (!inCombatTrial)
                            {
                                foreach (Projectile p in Main.ActiveProjectiles)
                                {
                                    if (p.type == ModContent.ProjectileType<Rift>())
                                    {
                                        inCombatTrial = true;
                                        break;
                                    }
                                }
                            }
                        }

                        if (!inCombatTrial)
                        {
                            tileEntity.open = true;
                            tileEntity.animationFrame = 1;
                            tileEntity.animationTimer = 0;

                            NetMessage.SendData(MessageID.TileEntitySharing, number: tileEntity.ID);
                            tileEntity.SendPacket(Main.LocalPlayer.whoAmI);

                            if (Main.netMode == NetmodeID.SinglePlayer)
                            {
                                Main.NewText(Language.GetTextValue("Mods.ChallengeRooms.Tiles.ChallengeGateway.AccessGranted"), B: 0);
                            }
                            else Main.NewText(Language.GetTextValue("Mods.ChallengeRooms.Tiles.ChallengeGateway.MultiplayerMessage"), G: 51, B: 0);
                        }
                        else
                        {
                            Main.NewText(Language.GetTextValue("Mods.ChallengeRooms.Tiles.ChallengeGateway.AccessDenied") + ": " + Language.GetTextValue("Mods.ChallengeRooms.Tiles.ChallengeGateway.CombatTrial"), B: 0);
                        }
                    }
                    else
                    {
                        Main.NewText(Language.GetTextValue("Mods.ChallengeRooms.Tiles.ChallengeGateway.AccessDenied") + ": " + (3 - tileEntity.progress) + " " + ((3 - tileEntity.progress) == 1 ? Language.GetTextValue("Mods.ChallengeRooms.Tiles.ChallengeGateway.DaysLeftSingle") : Language.GetTextValue("Mods.ChallengeRooms.Tiles.ChallengeGateway.DaysLeftMultiple")), B: 0);
                    }
                }
                else if (tileEntity.animationFrame == 12)
                {
                    tileEntity.open = false;
                    tileEntity.cooldown = tileEntity.initialCooldown;

                    NetMessage.SendData(MessageID.TileEntitySharing, number: tileEntity.ID);

                    if (SubworldSystem.IsActive<ChallengeRoom>())
                    {
                        SubworldSystem.Exit();
                    }
                    else
                    {
                        Main.LocalPlayer.GetModPlayer<ChallengePlayer>().challengeDoorEnterPosition = tileEntity.Position.ToVector2() + new Vector2(1, 5);

                        SubworldSystem.Enter<ChallengeRoom>();
                    }
                }
            }
            return false;
        }

        public override void MouseOver(int i, int j)
        {
            if (TileUtils.TryGetTileEntityAs(i, j, out TEChallengeGateway tileEntity) && tileEntity.cooldown != tileEntity.initialCooldown)
            {
                if (!tileEntity.open || tileEntity.animationFrame == 12)
                {
                    Player player = Main.LocalPlayer;
                    player.noThrow = 2;
                    player.cursorItemIconEnabled = true;
                    player.cursorItemIconID = ModContent.ItemType<GatewayIcon>();
                }
            }
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            if (Main.tile[i, j].TileFrameY >= 16 && TileUtils.TryGetTileEntityAs(i, j, out TEChallengeGateway tileEntity))
            {
                if (Main.tile[i, j].TileFrameX == 16)
                {
                    if (tileEntity.animationFrame == 0)
                    {
                        r = tileEntity.progress / 3f;
                        g = 0;
                        b = 0;
                    }
                    else if (tileEntity.animationFrame < 6)
                    {
                        r = 0;
                        g = 1;
                        b = 0;
                    }
                }
            }
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            if (TileUtils.TryGetTileEntityAs(i, j, out TEChallengeGateway tileEntity))
            {
                Tile tile = Main.tile[i, j];

                if (tile.TileFrameX == 16 && tile.TileFrameY == 0 && tileEntity.animationFrame == 0)
                {
                    Main.instance.TilesRenderer.AddSpecialLegacyPoint(i, j);
                }
            }
        }

        public override bool CanKillTile(int i, int j, ref bool blockDamaged) => false;

		public override bool CanExplode(int i, int j) => false;

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];
            if (TileUtils.TryGetTileEntityAs(i, j, out TEChallengeGateway tileEntity))
            {
                if (tileEntity.animationFrame > 0 && tile.TileFrameY >= 16)
                {
                    Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
                    Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.TileFrameX + tileEntity.animationFrame * 48, tile.TileFrameY, 16, 16), Lighting.GetColor(i, j), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.TileFrameX + tileEntity.animationFrame * 48, tile.TileFrameY + 80, 16, 16), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                }
            }
        }

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (TileUtils.TryGetTileEntityAs(i, j, out TEChallengeGateway tileEntity))
            {
                Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
                for (int k = 0; k < tileEntity.progress; k++)
                {
                    Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, new Vector2(i * 16 - (int)Main.screenPosition.X + 8, j * 16 - (int)Main.screenPosition.Y + 74 - k * 8) + zero, new Rectangle(0, 80, 2, 2), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                }
            }
        }
    }
    public class TEChallengeGateway : ModTileEntity
    {
        public int cooldown = 0;
        public bool open = false;
        public byte animationFrame = 0;
        public int animationTimer = 0;

        public int initialCooldown => 24 * 60 * 60 * 3; // 3 in-game days
        public int progress => (int)((float)(1 - (float)(cooldown / (float)initialCooldown)) * 3);

        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.HasTile && tile.TileType == ModContent.TileType<ChallengeGateway>() && tile.TileFrameX == 0 && tile.TileFrameY == 0;
        }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendTileSquare(Main.myPlayer, i, j, 3, 5);
                NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, i, j, Type);

                return -1;
            }
            Point16 tileOrigin = new Point16(1, 3);
            return Place(i - tileOrigin.X, j - tileOrigin.Y);
        }

        public override void OnNetPlace() => NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, ID, Position.X, Position.Y);

        public override void Update()
        {
            if (cooldown > 0)
            {
                animationFrame = 0;
                cooldown -= (int)Main.dayRate;
                if (cooldown < 0)
                {
                    cooldown = 0;
                }

                NetMessage.SendData(MessageID.TileEntitySharing, number: ID);
                SendPacket();
            }
            else if (open)
            {
                if (animationFrame < 12)
                {
                    if (animationTimer == 0)
                    {
                        if (animationFrame == 1)
                        {
                            SoundStyle sound = new SoundStyle("ChallengeRooms/Content/Sounds/Objects/beep");
                            sound.Volume = 0.5f;
                            SoundEngine.PlaySound(sound, (Position.ToVector2() + new Vector2(1.5f, 2.5f)) * 16);
                        }
                        else if (animationFrame == 2)
                        {
                            SoundStyle sound = new SoundStyle("ChallengeRooms/Content/Sounds/Objects/door-sequence-1");
                            sound.Volume = 0.5f;
                            SoundEngine.PlaySound(sound, (Position.ToVector2() + new Vector2(1.5f, 2.5f)) * 16);
                        }
                        else if (animationFrame == 8)
                        {
                            SoundStyle sound = new SoundStyle("ChallengeRooms/Content/Sounds/Objects/door-sequence-2");
                            sound.Volume = 0.5f;
                            SoundEngine.PlaySound(sound, (Position.ToVector2() + new Vector2(1.5f, 2.5f)) * 16);
                        }
                    }

                    if (++animationTimer >= (animationFrame == 1 ? 30 : 4))
                    {
                        animationFrame++;
                        animationTimer = 0;
                    }

                    NetMessage.SendData(MessageID.TileEntitySharing, number: ID);
                    SendPacket();
                }
            }
            else if (SubworldSystem.IsActive<ChallengeRoom>())
            {
                if (Position.X < Main.maxTilesX / 2)
                {
                    open = true;
                    animationFrame = 12;
                }
            }
            //else
            //{
            //    for (int i = 0; i < Main.maxPlayers; i++)
            //    {
            //        Player player = Main.player[i];
            //        if (Vector2.Distance(player.Center, (Position.ToVector2() + new Vector2(1.5f, 2.5f)) * 16) <= 4 * 16)
            //        {
            //            open = true;
            //            animationFrame = 1;
            //            animationTimer = 0;

            //            SendPacket();

            //            break;
            //        }
            //    }
            //}
        }

        public void SendPacket(int toWho = -1, int fromWho = -1)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                return;
            }

            ModPacket packet = Mod.GetPacket();
            packet.Write(0);
            packet.Write(ID);

            packet.Write(cooldown);
            packet.Write(open);
            packet.Write(animationFrame);
            packet.Write(animationTimer);
            packet.Send(toWho, fromWho);
        }

        internal static bool ReadPacket(Mod mod, BinaryReader reader, int fromWho)
        {
            int _id = reader.ReadInt32();

            int _cooldown = reader.ReadInt32();
            bool _open = reader.ReadBoolean();
            byte _animationFrame = reader.ReadByte();
            int _animationTimer = reader.ReadInt32();

            if (ByID.TryGetValue(_id, out TileEntity te) && te is TEChallengeGateway entity)
            {
                entity.cooldown = _cooldown;
                entity.open = _open;
                entity.animationFrame = _animationFrame;
                entity.animationTimer = _animationTimer;

                if (Main.netMode == NetmodeID.Server)
                {
                    entity.SendPacket(-1, fromWho);
                }

                return true;
            }
            return false;
        }

        public override void SaveData(TagCompound tag)
        {
            tag["Cooldown"] = cooldown;
            tag["DoorOpen"] = open;
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.TryGet("Cooldown", out int number))
            {
                cooldown = number;
            }
            if (tag.TryGet("DoorOpen", out bool flag))
            {
                open = flag;
                animationFrame = (byte)(open ? 12 : 0);
            }
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(cooldown);
            writer.Write(open);
            writer.Write(animationFrame);
            writer.Write(animationTimer);
        }

        public override void NetReceive(BinaryReader reader)
        {
            cooldown = reader.ReadInt32();
            open = reader.ReadBoolean();
            animationFrame = reader.ReadByte();
            animationTimer = reader.ReadInt32();
        }
    }
}
