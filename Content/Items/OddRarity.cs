using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace BlockContent.Content.Items
{
    public class OddRarity : ModRarity
    {
        public override Color RarityColor => new(0, 255, 255, 64);

        public override int GetPrefixedRarity(int offset, float valueMult) => Type;

        public override string Name => "Odd";
    }
}
