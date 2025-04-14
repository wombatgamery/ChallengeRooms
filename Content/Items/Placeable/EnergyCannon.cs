using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChallengeRooms.Content.Items.Placeable
{
    public class EnergyCannon : ModItem
    {
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.DartTrap);
            Item.placeStyle = 0;
            Item.rare = ItemRarityID.Green;
            Item.createTile = ModContent.TileType<Tiles.EnergyCannon>();
        }
    }
}