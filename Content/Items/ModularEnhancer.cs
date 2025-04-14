using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChallengeRooms.Content.Items
{
    [AutoloadEquip(EquipType.Back)]

    public class ModularEnhancer : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 16 * 2;
            Item.height = 24 * 2;
            Item.accessory = true;
            Item.value = Item.sellPrice(gold: 5);
            Item.rare = ItemRarityID.Green;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetModPlayer<ChallengePlayer>().modularEnhancer = true;
        }
    }
}