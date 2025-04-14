using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChallengeRooms.Content.Walls
{
    public class ChallengeBrickWall : ModWall
    {
		public override void SetStaticDefaults()
		{
            Main.wallHouse[Type] = false;

            DustType = DustID.Stone;

            AddMapEntry(new Color(59, 63, 69));
        }

        public override bool CanExplode(int i, int j) => false;
	}

    public class ChallengeBrickWallUnsafe : ModWall
    {
        public override string Texture => "ChallengeRooms/Content/Walls/ChallengeBrickWall";

        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;

            DustType = DustID.Stone;

            AddMapEntry(new Color(59, 63, 69));
        }

        public override void KillWall(int i, int j, ref bool fail)
        {
            fail = true;
        }

        public override bool CanExplode(int i, int j) => false;
    }
}
