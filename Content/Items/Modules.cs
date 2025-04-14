using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChallengeRooms.Content.Items
{
    public class ModuleKillBoost : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 13 * 2;
            Item.height = 13 * 2;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.Green;
        }
    }

    public class ModuleHealing : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 13 * 2;
            Item.height = 13 * 2;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.Green;
        }
    }

    public class ModuleCritDamage : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 13 * 2;
            Item.height = 13 * 2;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.Green;
        }
    }

    public class ModuleHurtBoost : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 13 * 2;
            Item.height = 13 * 2;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.Green;
        }
    }

    //public class ModuleMobility : ModItem
    //{
    //    public override void SetDefaults()
    //    {
    //        Item.width = 13 * 2;
    //        Item.height = 13 * 2;
    //        Item.rare = ItemRarityID.Green;
    //    }
    //}

    //public class ModuleCloseRange : ModItem
    //{
    //    public override void SetDefaults()
    //    {
    //        Item.width = 13 * 2;
    //        Item.height = 13 * 2;
    //        Item.rare = ItemRarityID.Green;
    //    }
    //}

    public class ModuleID
    {
        public const int KillBoost = 0;

        public const int Healing = 1;

        public const int CritDamage = 2;

        public const int HurtBoost = 3;

        public const int Mobility = 4;

        public const int CloseRange = 5;
    }
}