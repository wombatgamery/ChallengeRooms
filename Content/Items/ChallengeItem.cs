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
    public class ChallengeItem : GlobalItem
	{
        public override bool CanUseItem(Item item, Player player)
        {
            if (SubworldSystem.IsActive<ChallengeRoom>())
            {
                if (item.createTile != -1 || item.type == ItemID.DirtBomb || item.type == ItemID.DirtStickyBomb)//|| item.type == ItemID.RopeCoil || item.type == ItemID.VineRopeCoil || item.type == ItemID.SilkRopeCoil || item.type == ItemID.WebRopeCoil
                {
                    return false;
                }
            }

            return base.CanUseItem(item, player);
        }

        //public override bool? UseItem(Item item, Player player)
        //{
        //    if (SubworldSystem.IsActive<ChallengeRoom>())
        //    {
        //        if (item.type == ItemID.RecallPotion || item.type == ItemID.MagicMirror || item.type == ItemID.IceMirror || item.type == ItemID.TeleportationPotion || item.type == ItemID.CellPhone)
        //        {
        //            SubworldSystem.Exit();
        //        }
        //    }
        //    return null;
        //}
    }
}
