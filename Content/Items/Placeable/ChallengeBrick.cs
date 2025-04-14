using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChallengeRooms.Content.Items.Placeable
{
    public class ChallengeBrick : ModItem
    {
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.StoneBlock);
            Item.createTile = ModContent.TileType<Tiles.ChallengeBrick>();
        }

        public override void AddRecipes()
        {
            Recipe recipe;

            recipe = Recipe.Create(Type, 5);
            recipe.AddIngredient(ItemID.StoneBlock);
            recipe.AddIngredient(ItemID.Obsidian);
            recipe.AddTile(TileID.Furnaces);
            recipe.Register();
        }
    }
}