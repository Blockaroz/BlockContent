using BlockContent.Content.Graphics;
using BlockContent.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContent.Content.Projectiles.Weapons.Holy
{
    public class PaleBladeProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Blade");
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
            Projectile.manualDirectionChange = true;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 2;
            Projectile.penetrate = -1;
        }

        private ref float Time => ref Projectile.localAI[0];

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (Time > 45)
                Projectile.Kill();

            float rotation = (MathHelper.SmoothStep(-Projectile.ai[0], Projectile.ai[0], Utils.GetLerpValue(0f, 25f, Time, true))) * Projectile.direction;
            float length = MathHelper.SmoothStep(70f, MathHelper.Clamp(Projectile.ai[1], 70f, 700f), ExtraUtils.DualLerp(2f, 12.5f, 23f, Time, true));
            float thick = MathHelper.SmoothStep(-1, 1, Utils.GetLerpValue(0f, 25f, Time, true)) * (Projectile.direction * 130);
            Vector2 slash = Vector2.SmoothStep(new Vector2(70f, 0f).RotatedBy(rotation), new Vector2(MathHelper.Max(70f, length), thick), ExtraUtils.DualLerp(0f, 10f, 15f, 25f, Time, true));
            Vector2 endPoint = player.MountedCenter + slash.RotatedBy(Projectile.velocity.ToRotation());

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 + rotation;
            Projectile.Center = endPoint;

            Time++;

        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (targetHitbox.Distance(Projectile.Center) < 170)
                return true;

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
