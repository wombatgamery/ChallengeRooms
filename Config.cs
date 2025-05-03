using System.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using ChallengeRooms.Content.Tiles;
using System.ComponentModel;

namespace ChallengeRooms
{
	public class ChallengeConfig : ModConfig
	{
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [Range(0.25f, 1f)]
        [Increment(.25f)]
        [DrawTicks]
        [DefaultValue(1f)]
        public float Frequency;
    }
}
