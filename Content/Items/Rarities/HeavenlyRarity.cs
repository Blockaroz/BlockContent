using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace BlockContent.Content.Items
{
    public class HeavenlyRarity : ModRarity
    {
        public override Color RarityColor
        {
            get
            {
                Color[] colorArray = new Color[]
                {
                    new Color(90, 255, 220),
                    new Color(255, 100, 230),
                    new Color(255, 255, 70),
                    new Color(50, 120, 255)
                };
                Color result = new GradientColor(colorArray, 0.5f, 0.45f).Value;
                result.A = 180;
                return result;
            }
        }
    }
}
