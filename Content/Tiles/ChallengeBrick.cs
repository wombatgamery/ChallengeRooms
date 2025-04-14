using Microsoft.Xna.Framework;
using SubworldLibrary;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ChallengeRooms.Content.World;

namespace ChallengeRooms.Content.Tiles
{
    public class ChallengeBrick : ModTile
    {
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;

			TileID.Sets.CanBeClearedDuringGeneration[Type] = false;
			TileID.Sets.CanBeClearedDuringOreRunner[Type] = false;

			MinPick = 55;
			MineResist = 2;
			DustType = DustID.Stone;
			HitSound = SoundID.Tink;

			AddMapEntry(new Color(95, 102, 104));
		}

        public override bool CanKillTile(int i, int j, ref bool blockDamaged) => !SubworldSystem.IsActive<ChallengeRoom>();

        public override bool CanExplode(int i, int j) => false;

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            if (BlendingTile(i - 1, j) && BlendingTile(i + 1, j) && BlendingTile(i, j - 1) && BlendingTile(i, j + 1))
            {
                bool top = !BlendingTile(i - 1, j - 1) || !BlendingTile(i + 1, j - 1);
                bool bottom = !BlendingTile(i - 1, j + 1) || !BlendingTile(i + 1, j + 1);

                if (top ^ bottom)
                {
                    bool left = !BlendingTile(i - 1, j + (bottom ? 1 : -1));
                    bool right = !BlendingTile(i + 1, j + (bottom ? 1 : -1));

                    if (left ^ right)
                    {
                        Tile tile = Main.tile[i, j];

                        tile.TileFrameX = (short)(Main.rand.Next(3) * 18 * 2);
                        tile.TileFrameY = (short)(18 * 5 + (bottom ? 18 : 0));

                        if (right)
                        {
                            tile.TileFrameX += 18;
                        }

                        return false;
                    }
                }
            }
            return true;
        }

        private bool BlendingTile(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.HasTile && (tile.TileType == Type || Main.tileBlendAll[tile.TileType]);
        }
	}
}
