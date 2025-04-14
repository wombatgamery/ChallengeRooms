using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChallengeRooms.Content.Items.Placeable
{
    public class CopperLight : ModItem
    {
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.Switch);
            Item.createTile = ModContent.TileType<Tiles.CopperLight>();
        }

        public override void AddRecipes()
        {
            Recipe recipe;

            recipe = Recipe.Create(Type, 5);
            recipe.AddIngredient(ItemID.CopperBar);
            recipe.AddIngredient(ItemID.Wire, 5);
            recipe.AddTile(TileID.HeavyWorkBench);
            recipe.Register();
        }
    }
}