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
                Color[] colorArray = new Color[4] 
                { 
                    new Color(212, 212, 219), 
                    new Color(0, 255, 205), 
                    new Color(255, 100, 230),
                    new Color(255, 210, 30),
                };
                Color result = new GradientColor(colorArray, 0.75f).Value;
                result.A = 200;
                return result;
            }
        }
        public override int GetPrefixedRarity(int offset, float valueMult)
        {
            return offset;
        }
    }
}
