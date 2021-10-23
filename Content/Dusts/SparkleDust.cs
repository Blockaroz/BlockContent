using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace BlockContent.Content.Dusts
{
    public class SparkleDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
        }

        public override bool Update(Dust dust)
        {
            if (Main.rand.Next(4) == 0)
                dust.velocity += Main.rand.NextVector2Circular(1, 1);
            return true;
        }
        public override Color? GetAlpha(Dust dust, Color lightColor) => dust.color;
    }
}
