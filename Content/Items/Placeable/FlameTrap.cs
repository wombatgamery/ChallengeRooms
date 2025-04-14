using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChallengeRooms.Content.Items.Placeable
{
    public class FlameTrap : ModItem
    {
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.DartTrap);
            Item.rare = ItemRarityID.Green;
            Item.createTile = ModContent.TileType<Tiles.FlameTrap>();
        }
    }
}