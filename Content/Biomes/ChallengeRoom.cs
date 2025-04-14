using Terraria;
using Terraria.ModLoader;
using SubworldLibrary;
using ChallengeRooms.Content.Biomes.Backgrounds;

namespace ChallengeRooms.Content.Biomes
{
	public class ChallengeRoom : ModBiome
	{
		public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

		public override int Music => MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/wba-free-track-devoured");

        public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<ChallengeBG>();

        //public override string BestiaryIcon => "Remnants/Biomes/VaultIcon";

        public override bool IsBiomeActive(Player player)
		{
			int wall = Main.tile[(int)player.Center.X / 16, (int)player.Center.Y / 16].WallType;
			return SubworldSystem.IsActive<World.ChallengeRoom>();
		}
    }
}