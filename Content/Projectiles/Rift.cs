using System;
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
    public class Rift : ModProjectile
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
			Projectile.friendly = false;
			Projectile.hostile = false;
			Projectile.penetrate = 1;
			Projectile.timeLeft = 120;
		}

		public int monsterToSpawn;

		public override void AI()
		{
			if (Main.rand.NextBool(3))
			{
                Dust dust = Dust.NewDustDirect(Projectile.position, 1, Projectile.height, ModContent.DustType<RiftParticle>());
                dust.velocity = Vector2.Zero;
            }

			if (++Projectile.frameCounter >= 2)
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= Main.projFrames[Type])
				{
					Projectile.frame = 0;
				}
			}


			Lighting.AddLight(Projectile.Center, (new Vector3(255, 50, 85) / 255f));// / 2f);
		}

        public override void OnKill(int timeLeft)
        {
            NPC npc = NPC.NewNPCDirect(NPC.GetSource_None(), Projectile.Center, monsterToSpawn);
            npc.SpawnedFromStatue = true;

			NetMessage.SendData(MessageID.SyncNPC, number: npc.whoAmI);

			for (int i = 0; i < 20; i++)
			{
				Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<RiftBurst>());
				Vector2 direction = Main.rand.NextVector2Circular(4, 4);
				dust.position += direction * 6;
				dust.velocity = -direction;
			}

            SoundStyle sound = new SoundStyle("ChallengeRooms/Content/Sounds/Objects/rift");
            sound.Volume = 0.75f;
            SoundEngine.PlaySound(sound);
        }

        public override bool PreDraw(ref Color lightColor)
		{
			lightColor = Color.White;
			return true;
		}
    }
}