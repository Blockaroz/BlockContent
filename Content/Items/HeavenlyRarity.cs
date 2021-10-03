using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace BlockContent.Content.Items
{
    public class Heavenly : ModRarity
    {
        public override Color RarityColor
        {
            get
            {
                float function = (float)(Math.Sin(MathHelper.Pi * Main.GlobalTimeWrappedHourly) + 1f) * 0.5f;
                Color lerpColor = Color.Lerp(new(40, 170, 180, 130), new(255, 130, 210, 120), function);
                return lerpColor;
            }
        }
        public override int GetPrefixedRarity(int offset, float valueMult)
        {
            return offset;
        }
    }
}
