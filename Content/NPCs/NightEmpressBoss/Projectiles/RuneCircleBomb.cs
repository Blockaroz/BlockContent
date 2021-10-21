using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.GameContent;
using Terraria.ID;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using SoundType = Terraria.Audio.SoundType;

namespace BlockContent.Content.NPCs.NightEmpressBoss.Projectiles
{
    public class RuneCircleBomb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("ROON");
            ProjectileID.Sets.TrailCacheLength[Type] = 130;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 120;
        }

        private ref float _safeRadius => ref Projectile.ai[0];
        private ref float _explodeTime => ref Projectile.ai[1];


        public override void AI()
        {
            if (Projectile.timeLeft >= 110)
            {
                for (int i = 0; i < 45; i++)
            {
                Vector2 vector2 = new Vector2(0, _safeRadius).RotatedBy((MathHelper.TwoPi / 45) * i).RotatedByRandom(0.5f);
                Dust dust = Dust.NewDustPerfect(Projectile.Center + vector2, DustID.FireworksRGB, Vector2.Zero, 0, NightEmpress.NightColor(Main.rand.Next(0, 1)), 2f);
                dust.noGravity = true;
                dust.scale *= 0.87f;
            }    
            }

            if (Projectile.timeLeft == _explodeTime)
                SoundEngine.PlaySound(SoundID.Item9, Projectile.Center);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float distance = targetHitbox.Distance(Projectile.Center);
            if (distance > _safeRadius && Projectile.timeLeft <= _explodeTime)
                return true;

            return false;
        }

        public override bool PreDraw(ref Color lightColor) => false; //The circle's drawing effects will be handled by the empress herself
    }
}
