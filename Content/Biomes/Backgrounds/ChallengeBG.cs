using Terraria;
using Terraria.ModLoader;
using SubworldLibrary;

namespace ChallengeRooms.Content.Biomes.Backgrounds
{
    public class ChallengeBG : ModUndergroundBackgroundStyle
    {
        public override void FillTextureArray(int[] textureSlots)
        {
            textureSlots[0] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Content/Biomes/Backgrounds/ChallengeBG");
            textureSlots[1] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Content/Biomes/Backgrounds/ChallengeBG");
            textureSlots[2] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Content/Biomes/Backgrounds/ChallengeBG");
            textureSlots[3] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Content/Biomes/Backgrounds/ChallengeBG");
        }
    }
}