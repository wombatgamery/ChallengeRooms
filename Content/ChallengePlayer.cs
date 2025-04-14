using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ChallengeRooms.Content.World;
using SubworldLibrary;
using Terraria.ModLoader.IO;
using ChallengeRooms.Content.Items;
using Terraria.DataStructures;
using Terraria.WorldBuilding;
using ChallengeRooms.Content.Dusts;

namespace ChallengeRooms.Content
{

    public class ChallengePlayer : ModPlayer
	{
        public bool playerLocationSaving;

        public bool modularEnhancer;

        public int moduleSlot1 = -1;
        public int moduleSlot2 = -1;
        public int moduleSlot3 = -1;

        public int guaranteedCritTimer;
        public int healCounter;
        public float healCap;

        public override void PostUpdateEquips()
        {
            if (modularEnhancer)
            {
                if (guaranteedCritTimer > 0)
                {
                    Player.GetCritChance(DamageClass.Generic) += 96;
                    guaranteedCritTimer--;
                }

                int maxHealPerSecond = Player.statLifeMax2 / 20;
                if (healCap < maxHealPerSecond)
                {
                    healCap += maxHealPerSecond / 60f;
                    if (healCap > maxHealPerSecond)
                    {
                        healCap = maxHealPerSecond;
                    }
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (modularEnhancer)
            {
                if (target.damage > 0 || target.lifeMax > 5)
                {
                    if (target.life <= 0)
                    {
                        int critTimerMultiplier = 0;
                        for (int i = 0; i < 3; i++)
                        {
                            if (GetModuleSlot(i) == ModuleID.KillBoost)
                            {
                                critTimerMultiplier++;
                            }
                        }

                        if (critTimerMultiplier > 0)
                        {
                            guaranteedCritTimer = 60 * critTimerMultiplier;
                        }
                    }

                    if (hit.Crit)
                    {
                        int healingMultiplier = 0;
                        for (int i = 0; i < 3; i++)
                        {
                            if (GetModuleSlot(i) == ModuleID.Healing)
                            {
                                healingMultiplier++;
                            }
                        }

                        if (healingMultiplier > 0)
                        {
                            int heal = (int)MathHelper.Min((healCounter + hit.Damage) / 10 * healingMultiplier, healCap);

                            healCounter = hit.Damage % 10;

                            if (heal > 0)
                            {
                                Player.Heal(heal);
                                healCap -= heal;
                            }

                            Vector2 direction = target.Center - Player.Center;

                            for (int i = 0; i < direction.Length() / 2; i++)
                            {
                                Dust dust = Dust.NewDustPerfect(Player.Center + (direction / direction.Length()) * i * 2, DustID.DryadsWard);
                                dust.scale = 0.5f;
                                dust.velocity = Main.rand.NextVector2Circular(0.5f, 0.5f);
                                dust.noGravity = true;
                            }
                        }
                    }
                }
            }
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            if (modularEnhancer)
            {
                int critTimerMultiplier = 0;
                for (int i = 0; i < 3; i++)
                {
                    if (GetModuleSlot(i) == ModuleID.HurtBoost)
                    {
                        critTimerMultiplier++;
                    }
                }

                if (critTimerMultiplier > 0)
                {
                    guaranteedCritTimer = 60 * critTimerMultiplier;
                }
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (modularEnhancer)
            {
                int critDamageMultiplier = 0;
                for (int i = 0; i < 3; i++)
                {
                    if (GetModuleSlot(i) == ModuleID.CritDamage)
                    {
                        critDamageMultiplier++;
                    }
                }

                modifiers.CritDamage += 0.25f * critDamageMultiplier;
            }
        }

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (guaranteedCritTimer > 0 && !Player.DeadOrGhost)
            {
                Dust dust = Dust.NewDustDirect(Player.position, Player.width, Player.height, ModContent.DustType<CritBoostEffect>());
                dust.noGravity = true;
            }
        }

        public override void ResetEffects()
        {
            modularEnhancer = false;
        }

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            if (SubworldSystem.IsActive<ChallengeRoom>())
            {
                Player.respawnTimer = 300;
            }
        }

        public void EditModuleSlot(int index, int value)
        {
            if (index == 0)
            {
                moduleSlot1 = value;
            }
            else if (index == 1)
            {
                moduleSlot2 = value;
            }
            else if (index == 2)
            {
                moduleSlot3 = value;
            }
        }

        public int GetModuleSlot(int index)
        {
            return index == 2 ? moduleSlot3 : index == 1 ? moduleSlot2 : moduleSlot1;
        }

        public int ToModuleID(int type)
        {
            if (type == ModContent.ItemType<ModuleKillBoost>())
            {
                return ModuleID.KillBoost;
            }
            else if (type == ModContent.ItemType<ModuleHealing>())
            {
                return ModuleID.Healing;
            }
            else if (type == ModContent.ItemType<ModuleCritDamage>())
            {
                return ModuleID.CritDamage;
            }
            else if (type == ModContent.ItemType<ModuleHurtBoost>())
            {
                return ModuleID.HurtBoost;
            }
            //else if (type == ModContent.ItemType<ModuleCloseRange>())
            //{
            //    return ModuleID.CloseRange;
            //}
            //else if (type == ModContent.ItemType<ModuleMobility>())
            //{
            //    return ModuleID.Mobility;
            //}
            return -1;
        }

        public int FromModuleID(int id)
        {
            if (id == ModuleID.KillBoost)
            {
                return ModContent.ItemType<ModuleKillBoost>();
            }
            else if (id == ModuleID.Healing)
            {
                return ModContent.ItemType<ModuleHealing>();
            }
            else if (id == ModuleID.CritDamage)
            {
                return ModContent.ItemType<ModuleCritDamage>();
            }
            else if (id == ModuleID.HurtBoost)
            {
                return ModContent.ItemType<ModuleHurtBoost>();
            }
            //else if (id == ModuleID.CloseRange)
            //{
            //    return ModContent.ItemType<ModuleCloseRange>();
            //}
            //else if (id == ModuleID.Mobility)
            //{
            //    return ModContent.ItemType<ModuleMobility>();
            //}
            return -1;
        }

        public override void SaveData(TagCompound tag)
        {
            tag["ModuleSlot1"] = moduleSlot1;
            tag["ModuleSlot2"] = moduleSlot2;
            tag["ModuleSlot3"] = moduleSlot3;
        }

        public override void LoadData(TagCompound tag)
        {
            moduleSlot1 = -1;
            moduleSlot2 = -1;
            moduleSlot3 = -1;

            if (tag.TryGet("ModuleSlot1", out int mod1) && FromModuleID(mod1) != -1)
            {
                moduleSlot1 = mod1;
            }
            if (tag.TryGet("ModuleSlot2", out int mod2) && FromModuleID(mod2) != -1)
            {
                moduleSlot2 = mod2;
            }
            if (tag.TryGet("ModuleSlot3", out int mod3) && FromModuleID(mod3) != -1)
            {
                moduleSlot3 = mod3;
            }
        }

        public override void OnRespawn()
        {
            if (SubworldSystem.IsActive<ChallengeRoom>())
            {
                challengeDoorEnterPosition = Vector2.Zero;
                challengeDoorExitPosition = Vector2.Zero;
                SubworldSystem.Exit();
            }
        }

        public Vector2 challengeDoorEnterPosition;
        public Vector2 challengeDoorExitPosition;

        public override void OnEnterWorld()
        {
            playerLocationSaving = ModLoader.TryGetMod("PlayerLocationSaving", out Mod pls);

            if (SubworldSystem.IsActive<ChallengeRoom>())
            {
                Player.accWatch = 0;
                Player.accDepthMeter = 0;
                Player.accCompass = 0;
            }
            else
            {
                Player.RefreshInfoAccs();

                if (!playerLocationSaving && challengeDoorEnterPosition != Vector2.Zero)
                {
                    Player.position.X = challengeDoorEnterPosition.X * 16 + 8 - Player.width / 2;
                    Player.position.Y = challengeDoorEnterPosition.Y * 16 - Player.height;
                }
            }
        }

        public override void PreUpdate()
        {
            if (playerLocationSaving)
            {
                if (SubworldSystem.IsActive<ChallengeRoom>())
                {
                    if (Player.position.X / 16 > Main.maxTilesX - 50 && Player.position.Y / 16 > Main.maxTilesY - 50)
                    {
                        Player.position.X = Main.spawnTileX * 16 + 8 - Player.width / 2;
                        Player.position.Y = Main.spawnTileY * 16 - Player.height;
                    }
                }
                else if (Player.position == challengeDoorExitPosition)
                {
                    if (challengeDoorExitPosition != Vector2.Zero && challengeDoorEnterPosition != Vector2.Zero)
                    {
                        Player.position.X = challengeDoorEnterPosition.X * 16 + 8 - Player.width / 2;
                        Player.position.Y = challengeDoorEnterPosition.Y * 16 - Player.height;
                    }
                    else
                    {
                        Player.position.X = Main.spawnTileX * 16 + 8 - Player.width / 2;
                        Player.position.Y = Main.spawnTileY * 16 - Player.height;
                    }

                    Player.fallStart = (int)(Player.position.Y / 16);
                }
            }
        }
    }

    public class ChallengeTeleportCheck : ModPlayer
    {
        public override void Load()
        {
            On_Player.TeleportationPotion += TeleportPotionCheck;
            On_Player.Spawn += RecallPotionCheck;
            On_Player.MagicConch += MagicConchCheck;
            On_Player.DemonConch += DemonConchCheck;
        }

        private void TeleportPotionCheck(On_Player.orig_TeleportationPotion orig, Player self)
        {
            if (SubworldSystem.IsActive<ChallengeRoom>())
            {
                SubworldSystem.Exit();
                return;
            }

            orig(self);
        }

        private void RecallPotionCheck(On_Player.orig_Spawn orig, Player self, PlayerSpawnContext context)
        {
            if (SubworldSystem.IsActive<ChallengeRoom>())
            {
                if (context == PlayerSpawnContext.RecallFromItem)
                {
                    SubworldSystem.Exit();
                    return;
                }
            }

            orig(self, context);
        }

        private void MagicConchCheck(On_Player.orig_MagicConch orig, Player self)
        {
            if (SubworldSystem.IsActive<ChallengeRoom>())
            {
                SubworldSystem.Exit();
                return;
            }

            orig(self);
        }

        private void DemonConchCheck(On_Player.orig_DemonConch orig, Player self)
        {
            if (SubworldSystem.IsActive<ChallengeRoom>())
            {
                SubworldSystem.Exit();
                return;
            }

            orig(self);
        }
    }
}
