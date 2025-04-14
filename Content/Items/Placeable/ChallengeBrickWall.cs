using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChallengeRooms.Content.Items.Placeable
{
    public class ChallengeBrickWall : ModItem
    {
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.StoneWall);
            Item.createWall = ModContent.WallType<Walls.ChallengeBrickWall>();
        }

        public override void AddRecipes()
        {
            Recipe recipe;

            recipe = Recipe.Create(Type, 4);
            recipe.AddIngredient(ModContent.ItemType<ChallengeBrick>());
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();

            recipe = Recipe.Create(ModContent.ItemType<ChallengeBrick>());
            recipe.AddIngredient(Type, 4);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}