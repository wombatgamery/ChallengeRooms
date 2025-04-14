using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace ChallengeRooms.Content
{
	public class UISystem : ModSystem
	{
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer("Modular Enhancer UI", () =>
                {
                    ModuleSlotUI.Draw(Main.spriteBatch);
                    return true;
                },
                InterfaceScaleType.UI));
            }
        }
    }
}
