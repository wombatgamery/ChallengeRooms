using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChallengeRooms.Content.Items.Placeable
{
    public class CopperPlatform : ModItem
    {
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.WoodPlatform);
            Item.width = 10 * 2;
            Item.height = 7 * 2;
            Item.createTile = ModContent.TileType<Tiles.CopperPlatform>();
        }

        public override void AddRecipes()
        {
            Recipe recipe;

            recipe = Recipe.Create(Type, 10);
            recipe.AddIngredient(ItemID.CopperBar);
            recipe.AddTile(TileID.HeavyWorkBench);
            recipe.Register();
        }
    }
}