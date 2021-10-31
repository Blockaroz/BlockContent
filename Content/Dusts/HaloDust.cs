using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContent.Content.Dusts
{
    public class HaloDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.scale *= 1.1f;
            dust.noGravity = true;
            dust.fadeIn += 0.5f;
        }

        public override bool Update(Dust dust)
        {
            dust.velocity *= 1.2f;
            dust.velocity += Vector2.UnitY.RotatedBy(dust.rotation);
            dust.velocity += Main.rand.NextVector2Circular(2, 2);
            return true;
        }

        public override void SetStaticDefaults()
        {
            UpdateType = DustID.Vortex;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor) => dust.color;
    }
}
