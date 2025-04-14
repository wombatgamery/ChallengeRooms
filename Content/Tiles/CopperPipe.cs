using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ChallengeRooms.Content.World;
using SubworldLibrary;

namespace ChallengeRooms.Content.Tiles
{
	public class CopperPipe : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;

			TileID.Sets.DrawsWalls[Type] = true;
			TileID.Sets.CanBeClearedDuringGeneration[Type] = false;
			TileID.Sets.CanBeClearedDuringOreRunner[Type] = false;

			DustType = DustID.Copper;
			HitSound = SoundID.Item52;

            AddMapEntry(new Color(207, 117, 74));
		}

        public override bool CanKillTile(int i, int j, ref bool blockDamaged) => !SubworldSystem.IsActive<ChallengeRoom>();

        public override bool CanExplode(int i, int j) => !SubworldSystem.IsActive<ChallengeRoom>();
    }
}
