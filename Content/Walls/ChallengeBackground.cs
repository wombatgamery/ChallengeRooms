﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChallengeRooms.Content.Walls
{
    public class ChallengeBackground : ModWall
    {
		public override void SetStaticDefaults()
		{
            Main.wallHouse[Type] = false;
            Main.wallBlend[Type] = 1;

            DustType = DustID.Stone;

            AddMapEntry(new Color(46, 48, 53));
        }

        const int Width = 8;
        const int Height = 6;
        const int ScrollSpeed = 8;

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new(Main.offScreenRange);
            Rectangle frame = new((int)(i * 16 - Main.screenPosition.X / ScrollSpeed) % (Width * 16), (int)(j * 16 - Main.screenPosition.Y / ScrollSpeed) % (Height * 16), 16, 16);
            Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, frame, Lighting.GetColor(i, j), 0f, Vector2.Zero, 1f, 0, 0f);
            return false;
        }

        public override void KillWall(int i, int j, ref bool fail)
        {
            fail = true;
        }

        public override bool CanExplode(int i, int j) => false;
	}
}
