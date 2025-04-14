using System.Collections.Generic;
using ChallengeRooms.Content.Dusts;
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
    public class ChallengeProjectile : GlobalProjectile
    {
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (SubworldSystem.IsActive<ChallengeRoom>() && projectile.aiStyle == 7)
            {
                projectile.tileCollide = true;
            }
        }

        public override bool? GrappleCanLatchOnTo(Projectile projectile, Player player, int x, int y)
        {
            return SubworldSystem.IsActive<ChallengeRoom>() ? false : null;
        }


        public override bool OnTileCollide(Projectile projectile, Vector2 oldVelocity)
        {
            if (SubworldSystem.IsActive<ChallengeRoom>() && projectile.aiStyle == 7)
            {
                projectile.ai[0] = 1f;
                projectile.tileCollide = false;

                for (int i = 0; i < 5; i++)
                {
                    Dust dust = Dust.NewDustPerfect(projectile.Center, DustID.MartianSaucerSpark);
                    dust.noGravity = true;
                    dust.velocity = Main.rand.NextVector2Circular(8, 8);
                }

                SoundStyle sound = new SoundStyle("ChallengeRooms/Content/Sounds/Objects/sword_parry");
                sound.Volume = 0.5f; sound.PitchVariance = 0.5f;
                SoundEngine.PlaySound(sound, projectile.Center);

                return false;
            }
            return true;
        }
    }
}