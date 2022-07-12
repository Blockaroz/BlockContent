using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContent.Content.Items.Weapons
{
    public class RoseRarity : ModRarity
    {
        public override string Name => "Rose";

        public override Color RarityColor
        {
            get
            {
                Color color = new GradientColor(new Color[] 
                { 
                    new Color(255, 26, 183),
                    new Color(255, 120, 43),
                    new Color(255, 26, 183),
                    new Color(190, 89, 255)
                }, 0.66f, 0.65f).Value;
                color.A /= 2;
                return color;
            }
        }

        public override int GetPrefixedRarity(int offset, float valueMult)
        {
            if (offset < 0)
                return ItemRarityID.Purple;
            return Type;
        }
    }
}
