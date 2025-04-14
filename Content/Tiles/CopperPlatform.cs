using Microsoft.Xna.Framework;
using SubworldLibrary;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using ChallengeRooms.Content.World;

namespace ChallengeRooms.Content.Tiles
{
    [LegacyName("ChallengePlatform")]
    public class CopperPlatform : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileSolidTop[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileTable[Type] = true;

            TileID.Sets.Platforms[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;

            TileObjectData.newTile.FullCopyFrom(TileID.Platforms);
            TileObjectData.newTile.WaterDeath = false;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);

            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsDoor);

			DustType = DustID.Copper;
			HitSound = SoundID.Tink;
            AdjTiles = new int[] { TileID.Platforms };

            AddMapEntry(new Color(207, 117, 74));
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override bool CanKillTile(int i, int j, ref bool blockDamaged) => !SubworldSystem.IsActive<ChallengeRoom>();

        public override bool CanExplode(int i, int j) => !SubworldSystem.IsActive<ChallengeRoom>();
    }
}