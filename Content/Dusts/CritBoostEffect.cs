using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChallengeRooms.Content.Dusts
{
	public class CritBoostEffect : ModDust
	{
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;

            dust.scale *= 0.95f;
            dust.velocity *= 0.8f;
            dust.velocity += Main.rand.NextVector2Circular(0.8f, 0.8f);

            if (dust.scale <= 0.2f)
            {
                dust.active = false;
            }

            Lighting.AddLight(dust.position, new Vector3(0.5f, 0, 0) * dust.scale);

            return false;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return Color.White;
        }
    }
}