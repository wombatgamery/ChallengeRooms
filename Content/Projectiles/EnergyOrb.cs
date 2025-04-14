using System.Collections.Generic;
using ChallengeRooms.Content.Dusts;
using ChallengeRooms.Content.Tiles;
using ChallengeRooms.Content.World;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChallengeRooms.Content.Projectiles
{
    public class EnergyOrb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
        }

		public override void SetDefaults()
		{
			Projectile.width = 6 * 2;
			Projectile.height = 6 * 2;
			Projectile.aiStyle = -1;
			Projectile.friendly = true;
			Projectile.hostile = true;
			Projectile.penetrate = 1;
			Projectile.timeLeft = 600;
		}

		public override void AI()
		{
			Dust dust = Dust.NewDustDirect(Projectile.position, 1, Projectile.height, ModContent.DustType<EnergyParticle>());
			dust.velocity = Vector2.Zero;

			if (++Projectile.frameCounter >= 2)
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= Main.projFrames[Type])
				{
					Projectile.frame = 0;
				}
			}

			Point16 tileCoords = ((Projectile.Center + Projectile.velocity * 2) / 16).ToPoint16();
			if (Main.tile[tileCoords.X, tileCoords.Y].HasTile && Main.tile[tileCoords.X, tileCoords.Y].TileType != ModContent.TileType<ChallengeBrick>() && Main.tileSolid[Main.tile[tileCoords.X, tileCoords.Y].TileType] && !TileID.Sets.Platforms[Main.tile[tileCoords.X, tileCoords.Y].TileType])
			{
                if (SubworldSystem.IsActive<ChallengeRoom>())
                {
                    WorldGen.KillTile(tileCoords.X, tileCoords.Y);
                }
			}

			Lighting.AddLight(Projectile.Center, (new Vector3(50, 153, 255) / 255f));// / 2f);
		}

        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < 10; i++)
            {
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<EnergyBurst>());
				dust.velocity = (Projectile.velocity + Main.rand.NextVector2Circular(4, 4)) * Main.rand.NextFloat(0, 1);
			}

            if (Vector2.Distance(Main.LocalPlayer.Center, Projectile.Center) < 64 * 16)
            {
                SoundStyle sound = new SoundStyle("ChallengeRooms/Content/Sounds/Objects/energycannon");
                sound.Volume = 0.1f; sound.MaxInstances = 3; sound.SoundLimitBehavior = SoundLimitBehavior.IgnoreNew;
                SoundEngine.PlaySound(sound, Projectile.Center);
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<EnergyBurst>());
                dust.velocity = Main.rand.NextVector2Circular(4, 4);
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
			//for (int i = 0; i < 10; i++)
			//{
			//	Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, Alpha: 153);
			//	dust.velocity = Main.rand.NextVector2Circular(5, 5);
			//}
			//for (int i = 0; i < 10; i++)
			//{
			//	Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.IceTorch);
			//	dust.velocity = Main.rand.NextVector2Circular(5, 5);
			//}

			Projectile.Kill();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
			Projectile.Kill();
		}

        public override bool PreDraw(ref Color lightColor)
		{
			lightColor = Color.White;
			return true;
		}

		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
        }
    }
}