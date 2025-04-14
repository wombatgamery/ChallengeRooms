using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChallengeRooms.Content.Items.Placeable
{
    public class CopperPipe : ModItem
    {
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.StoneBlock);
            Item.createTile = ModContent.TileType<Tiles.CopperPipe>();
        }

        public override void AddRecipes()
        {
            Recipe recipe;

            recipe = Recipe.Create(Type, 5);
            recipe.AddIngredient(ItemID.CopperBar);
            recipe.AddTile(TileID.HeavyWorkBench);
            recipe.Register();
        }
    }
}