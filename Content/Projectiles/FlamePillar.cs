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
    public class FlamePillar : ModProjectile
    {
        public override void SetDefaults()
		{
			Projectile.width = 8 * 2;
			Projectile.height = 40 * 2;
			Projectile.aiStyle = -1;
			Projectile.friendly = true;
			Projectile.hostile = true;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
		}

        public override void OnSpawn(IEntitySource source)
        {
            Dust dust;

            for (int i = 0; i < 32; i++)
            {
                dust = Dust.NewDustPerfect(new Vector2(Projectile.Center.X, Projectile.position.Y + 16 + Main.rand.Next(Projectile.height - 16)), DustID.Torch, Scale: 2);
                dust.noGravity = true;
                dust.velocity = (-Vector2.UnitY + Main.rand.NextVector2Circular(1, 1)) * 4;
            }

            if (Vector2.Distance(Main.LocalPlayer.Center, Projectile.Center) < 64 * 16)
            {
                SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);

                SoundStyle sound = new SoundStyle("ChallengeRooms/Content/Sounds/Objects/flamejet");
                sound.Volume = 0.75f;
                SoundEngine.PlaySound(sound, Projectile.Center);
            }

            Lighting.AddLight(Projectile.Center, 1.2f, 0.4f, 0);
        }

        public override void AI()
        {
            Dust dust;

            for (int i = 0; i < 4; i++)
            {
                dust = Dust.NewDustPerfect(new Vector2(Projectile.Center.X, Projectile.position.Y + 16 + Main.rand.Next(Projectile.height - 16)), DustID.Smoke, Alpha: 153, Scale: 2);
                dust.noGravity = true;
                dust.velocity = (-Vector2.UnitY + Main.rand.NextVector2Circular(1, 1)) * 3;
            }

            for (int i = 0; i < 4; i++)
            {
                dust = Dust.NewDustPerfect(new Vector2(Projectile.Center.X, Projectile.position.Y + 16 + Main.rand.Next(Projectile.height - 16)), DustID.Torch, Scale: 2);
                dust.noGravity = true;
                dust.velocity = (-Vector2.UnitY + Main.rand.NextVector2Circular(1, 1)) * 2;
            }

            for (int i = 0; i < 1; i++)
            {
                dust = Dust.NewDustPerfect(new Vector2(Projectile.Center.X, Projectile.position.Y + Projectile.height), DustID.IceTorch, Scale: 2);
                dust.noGravity = true;
                dust.velocity = (-Vector2.UnitY + Main.rand.NextVector2Circular(1, 1)) * 1;
                dust.noLight = true;
            }

            Lighting.AddLight(Projectile.Center, 1.2f, 0.4f, 0);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.OnFire, 30 * 60);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 30 * 60);
        }
    }
}