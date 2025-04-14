using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChallengeRooms.Content.Items.Hovers
{
    public class GatewayIcon : ModItem
    {
        public override void SetDefaults()
        {
            ItemID.Sets.Deprecated[Type] = true;
        }
    }

}