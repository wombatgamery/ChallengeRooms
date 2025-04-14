using ChallengeRooms.Content.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Security.Cryptography.X509Certificates;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace ChallengeRooms.Content
{
	public class ModuleSlotUI
	{
		public static Vector2 UITopLeft = new Vector2(570, 175);

        public static Rectangle UIRect => new Rectangle((int)UITopLeft.X, (int)UITopLeft.Y, 138, 78);

        public static Rectangle Slot1Rect => new ((int)UITopLeft.X + 22, (int)UITopLeft.Y + 24, 30, 30);
        public static Rectangle Slot2Rect => new((int)UITopLeft.X + 54, (int)UITopLeft.Y + 24, 30, 30);
        public static Rectangle Slot3Rect => new((int)UITopLeft.X + 86, (int)UITopLeft.Y + 24, 30, 30);

        public static bool dragging;
        public static bool canDrag;
        public static Vector2 dragPosition = Vector2.Zero;

        public static void Draw(SpriteBatch spriteBatch)
        {
            ChallengePlayer modPlayer = Main.LocalPlayer.GetModPlayer<ChallengePlayer>();

            if (Main.playerInventory && modPlayer.modularEnhancer)
            {
                #region interaction
                int selectedSlotIndex = -1;

                if (UIRect.Contains(Main.MouseScreen.ToPoint()))
                {
                    Player player = Main.LocalPlayer;

                    player.mouseInterface = Main.blockMouse = true;

                    for (int slotIndex = 0; slotIndex < 3; slotIndex++)
                    {
                        Rectangle slotRect = slotIndex == 2 ? Slot3Rect : slotIndex == 1 ? Slot2Rect : Slot1Rect;
                        if (slotRect.Contains(Main.MouseScreen.ToPoint()))
                        {
                            selectedSlotIndex = slotIndex;
                            break;
                        }
                    }

                    if (selectedSlotIndex != -1)
                    {
                        if (Main.mouseLeft && Main.mouseLeftRelease)
                        {
                            Item heldItem = Main.mouseItem;

                            int selectedSlotItemID = selectedSlotIndex == 2 ? modPlayer.moduleSlot3 : selectedSlotIndex == 1 ? modPlayer.moduleSlot2 : modPlayer.moduleSlot1;

                            SoundStyle sound = new SoundStyle("ChallengeRooms/Content/Sounds/UI/ModuleSlot");
                            sound.Volume = 0.5f;
                            sound.PitchVariance = 0.25f;
                            sound.MaxInstances = 0;

                            if (selectedSlotItemID == -1)
                            {
                                if (modPlayer.ToModuleID(heldItem.type) != -1)
                                {
                                    modPlayer.EditModuleSlot(selectedSlotIndex, modPlayer.ToModuleID(heldItem.type));
                                    heldItem.TurnToAir();

                                    SoundEngine.PlaySound(sound);
                                }
                            }
                            else if (heldItem.IsAir)
                            {
                                heldItem.SetDefaults(modPlayer.FromModuleID(selectedSlotItemID));
                                heldItem.stack = 1;
                                modPlayer.EditModuleSlot(selectedSlotIndex, -1);

                                SoundEngine.PlaySound(sound);
                            }
                            else if (modPlayer.ToModuleID(heldItem.type) != -1)
                            {
                                int savedID = modPlayer.GetModuleSlot(selectedSlotIndex);
                                modPlayer.EditModuleSlot(selectedSlotIndex, modPlayer.ToModuleID(heldItem.type));
                                heldItem.SetDefaults(modPlayer.FromModuleID(savedID));

                                SoundEngine.PlaySound(sound);
                            }
                        }
                    }
                }
                #endregion

                #region dragging
                if (!Main.mouseLeft)
                {
                    dragging = false;
                }

                if ((canDrag && Main.mouseLeft || dragging))
                {
                    if (canDrag && Main.mouseLeft)
                    {
                        dragging = true;
                        dragPosition = Main.MouseScreen - UITopLeft;
                    }

                    UITopLeft = Main.MouseScreen - dragPosition;

                    UITopLeft.X = MathHelper.Clamp(UITopLeft.X, 0, Main.screenWidth - UIRect.Width);
                    UITopLeft.Y = MathHelper.Clamp(UITopLeft.Y, 0, Main.screenHeight - UIRect.Height);
                }

                if (UIRect.Contains(Main.MouseScreen.ToPoint()) && selectedSlotIndex == -1)
                {
                    canDrag = !Main.mouseLeft;
                }
                else canDrag = false;
                #endregion

                #region drawing
                spriteBatch.Draw(ModContent.Request<Texture2D>("ChallengeRooms/Content/UI/ModuleSlotUI").Value, UITopLeft, null, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);

                for (int slotIndex = 0; slotIndex < 3; slotIndex++)
                {
                    int selectedSlotItemID = modPlayer.FromModuleID(slotIndex == 2 ? modPlayer.moduleSlot3 : slotIndex == 1 ? modPlayer.moduleSlot2 : modPlayer.moduleSlot1);

                    Vector2 position = (slotIndex == 2 ? Slot3Rect.TopLeft() : slotIndex == 1 ? Slot2Rect.TopLeft() : Slot1Rect.TopLeft()) + Vector2.One * 2;
                    if (modPlayer.GetModuleSlot(slotIndex) != -1)
                    {
                        spriteBatch.Draw(ModContent.Request<Texture2D>(ModContent.GetModItem(selectedSlotItemID).Texture).Value, position, null, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);
                        spriteBatch.Draw(ModContent.Request<Texture2D>("ChallengeRooms/Content/UI/ModuleOutline").Value, position, null, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);

                        if (selectedSlotIndex == slotIndex && !dragging)
                        {
                            Item displayItem = new Item(selectedSlotItemID);

                            Main.HoverItem = displayItem.Clone();
                            Main.instance.MouseTextHackZoom(string.Empty);
                        }
                    }

                    if (selectedSlotIndex == slotIndex && !dragging)
                    {
                        spriteBatch.Draw(ModContent.Request<Texture2D>("ChallengeRooms/Content/UI/SelectionOutline").Value, position, null, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);
                    }
                }
                #endregion
            }
        }
    }
}
