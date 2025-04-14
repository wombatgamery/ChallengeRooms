using System.IO;
using Terraria.ModLoader;
using ChallengeRooms.Content.Tiles;

namespace ChallengeRooms
{
	// Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
	public class ChallengeRooms : Mod
	{
        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            byte messageType = reader.ReadByte();

            if (messageType == 0)
            {
                TEChallengeGateway.ReadPacket(this, reader, whoAmI);
            }
        }
    }
}
