using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChallengeRooms.Content.Dusts
{
	public class RiftParticle : ModDust
	{
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;

            dust.scale *= 0.97f;
            dust.velocity *= 0.9f;
            dust.velocity += Main.rand.NextVector2Circular(0.4f, 0.4f);

            if (dust.scale <= 0.2f)
            {
                dust.active = false;
            }

            Lighting.AddLight(dust.position, (new Vector3(200, 50, 100) / 255f) * dust.scale);

            return false;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return Color.White;
        }
    }
}