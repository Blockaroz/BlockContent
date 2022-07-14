using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContent.Content.Items.Weapons
{
    public class DeepBlueRarity : ModRarity
    {
        public override string Name => "Deep Blue";

        public override Color RarityColor
        {
            get
            {
                Color[] colorArray = new Color[]
                {
                    new Color(15, 100, 255),
                    new Color(50, 50, 255),
                };
                Color result = new GradientColor(colorArray, 2f, 1.8f).Value;
                result.A = 220;
                return result;
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
