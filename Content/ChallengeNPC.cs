using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ChallengeRooms.Content.World;
using SubworldLibrary;
using Terraria.ModLoader.IO;
using ChallengeRooms.Content.Items;
using Terraria.DataStructures;
using Terraria.WorldBuilding;
using System.Collections.Generic;

namespace ChallengeRooms.Content
{
    public class ChallengeNPC : GlobalNPC
	{
        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            if (SubworldSystem.IsActive<ChallengeRoom>())
            {
                pool.Clear();
            }
        }
    }
}
