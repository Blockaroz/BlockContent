using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContent.Content.Graphics
{
    public struct SanctuaryDrawer
    {
        public static int GetGunIndex()
        {
            int[] gunIndex = new int[]
            {
                ItemID.QuadBarrelShotgun,
                ItemID.PhoenixBlaster,
                ItemID.Flamethrower,
                ItemID.VenusMagnum,
                ItemID.SniperRifle,
                ItemID.ChainGun,
                ItemID.Xenopopper,
                ItemID.SDMG,
                ItemID.Celeb2
            };
            return Main.rand.Next(gunIndex);
        }
        //ModContent.ItemType<Sanctuary>()
        
        public void DrawSingleGun(int index, int direction, Vector2 position, float rotation, Color color)
        {
            Asset<Texture2D> gunTexture = TextureAssets.Item[index];
            Vector2 origin = gunTexture.Size() / 2;
            SpriteEffects effects = direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Main.EntitySpriteDraw(gunTexture.Value, position, null, color, rotation, origin, 1, effects, 0);
        }

        private static Color GetColorOnWheel(float degree)
        {
            Color[] colors = new Color[]
{
                new(233, 33, 33),
                new(233, 30, 100),
                new(255, 70, 0)
};
            Color peakColor = new GradientColor(colors).Value;
            Color dark = Color.Lerp(Color.Transparent, new(119, 0, 17), degree);
            return peakColor;
        }
    }
}
