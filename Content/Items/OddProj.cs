using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.GameContent;
using Terraria.ID;
using BlockContent.Content.Graphics;
using Terraria.Graphics.Effects;

namespace BlockContent.Content.Items
{
    public class OddProj : ModProjectile
    {
        public override string Texture => "BlockContent/Assets/Textures/BlessingChad/BlessingChad1";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 40;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 120;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
