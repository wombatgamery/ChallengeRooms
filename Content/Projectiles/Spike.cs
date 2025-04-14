using System.Collections.Generic;
using ChallengeRooms.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChallengeRooms.Content.Projectiles
{
    public class Spike : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 2;
        }

        public override void SetDefaults()
		{
			Projectile.width = 5 * 2;
			Projectile.height = 16 * 2;
			Projectile.aiStyle = -1;
			Projectile.friendly = true;
			Projectile.hostile = true;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 60;
            Projectile.tileCollide = false;
            Projectile.hide = true;

            ProjectileID.Sets.DontAttachHideToAlpha[Type] = true;
		}

        public override void OnSpawn(IEntitySource source)
        {
			Dust dust = Dust.NewDustPerfect(new Vector2(Projectile.Center.X, Projectile.Bottom.Y), DustID.TreasureSparkle);
            dust.noGravity = true;
            dust.velocity = -Vector2.UnitY + Main.rand.NextVector2Circular(1, 1);

            for (int i = 0; i < 5; i++)
            {
                dust = Dust.NewDustPerfect(Projectile.BottomLeft + Vector2.UnitX * Main.rand.NextFloat(Projectile.width * 2), DustID.Smoke, Alpha: 153);
				dust.noGravity = true;
                dust.velocity = -Vector2.UnitY + Main.rand.NextVector2Circular(1, 1);
            }

            //for (int i = 0; i < Main.maxPlayers; i++)
            //{
            //    Player player = Main.player[i];
            //    if (player.active)
            //    {
            //        if (!player.DeadOrGhost)
            //        {
            //            if (Projectile.Hitbox.Intersects(player.Hitbox))
            //            {
            //                Projectile.frame = 1;
            //                break;
            //            }
            //        }
            //    }
            //    else break;
            //}

            if (Vector2.Distance(Main.LocalPlayer.Center, Projectile.Center) < 64 * 16)
            {
                SoundStyle sound = new SoundStyle("ChallengeRooms/Content/Sounds/Objects/spike");
                sound.Volume = 0.75f; //sound.MaxInstances = 3;
                SoundEngine.PlaySound(sound, Projectile.Center);
            }
        }

        public override void AI()
        {
            if (Projectile.timeLeft <= 16)
            {
                Projectile.position.Y += 2;
                Projectile.height -= 2;
                if (Projectile.timeLeft <= 1)
                {
                    Projectile.friendly = false;
                    Projectile.hostile = false;
                }
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Bleeding, 30 * 60);
            Projectile.frame = 1;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Bleeding, 30 * 60);
            Projectile.frame = 1;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }
    }
}