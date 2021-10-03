using BlockContent.Content.Graphics;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContent.Content.Projectiles
{
    public class OctaneBladeProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Octane Blade");
            ProjectileID.Sets.TrailCacheLength[Type] = 30;
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
            Projectile.usesLocalNPCImmunity = true;
            Projectile.manualDirectionChange = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.penetrate = -1;
            Projectile.noEnchantmentVisuals = true;
        }

        private ref float Time => ref Projectile.localAI[0];

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            float lerpValue = Utils.GetLerpValue(900f, 0f, Projectile.velocity.Length() * 2f, true);
            float num = MathHelper.Lerp(0.7f, 2f, lerpValue);
            Time += num;
            if (Time >= 120f)
            {
                Projectile.Kill();
                return;
            }

            float lerpValue2 = Utils.GetLerpValue(0f, 1f, Time / 60f, true);
            float projAngle = Projectile.velocity.ToRotation();
            int direction = (Projectile.velocity.X > 0f) ? 1 : (-1);
            float rotation = MathHelper.Pi + direction * lerpValue2 * MathHelper.TwoPi;

            float modifiedLength = Projectile.velocity.Length() + Utils.GetLerpValue(0.5f, 1f, lerpValue2, true) * 40f;
            if (modifiedLength < 60f)
                modifiedLength = 60f;

            Vector2 spinningpoint = new Vector2(1f, 0f).RotatedBy(rotation) * new Vector2(modifiedLength, Projectile.ai[0] * MathHelper.Lerp(2f, 1f, lerpValue));
            Vector2 value2 = (player.MountedCenter) + spinningpoint.RotatedBy(projAngle);
            Vector2 value3 = (1f - Utils.GetLerpValue(0f, 0.5f, lerpValue2, clamped: true)) * new Vector2(((Projectile.velocity.X > 0f) ? 1 : (-1)) * (0f - modifiedLength) * 0.1f, (0f - Projectile.ai[0]) * 0.3f);
            Projectile.rotation = rotation + projAngle + MathHelper.PiOver2;
            Projectile.Center = value2 + value3;
            Projectile.spriteDirection = (direction = ((Projectile.velocity.X > 0f) ? 1 : (-1)));
            if (Projectile.ai[0] < 0f)
            {
                Projectile.rotation = MathHelper.Pi + direction * lerpValue2 * (-MathHelper.TwoPi) + projAngle;
                Projectile.rotation += MathHelper.PiOver2;
                Projectile.spriteDirection = (direction = ((!(Projectile.velocity.X > 0f)) ? 1 : (-1)));
            }
        }

        private Rectangle _hitBox = new Rectangle(0, 0, 300, 300);

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            float scaleFactor = 40f;
            for (int i = 14; i < Projectile.oldPos.Length; i += 15)
            {
                float num2 = Projectile.localAI[0] - i;
                if (!(num2 < 0f) && !(num2 > 60f))
                {
                    Vector2 value2 = Projectile.oldPos[i] + Projectile.Size / 2f;
                    Vector2 value3 = (Projectile.oldRot[i] + (float)Math.PI / 2f).ToRotationVector2();
                    _hitBox.X = (int)value2.X - _hitBox.Width / 2;
                    _hitBox.Y = (int)value2.Y - _hitBox.Height / 2;
                    if (_hitBox.Intersects(targetHitbox) && Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), value2 - value3 * scaleFactor, value2 + value3 * scaleFactor, 20f, ref collisionPoint))
                        return true;
                }
            }
            Vector2 value4 = (Projectile.rotation + (float)Math.PI / 2f).ToRotationVector2();
            _hitBox.X = (int)Projectile.position.X - _hitBox.Width / 2;
            _hitBox.Y = (int)Projectile.position.Y - _hitBox.Height / 2;
            if (_hitBox.Intersects(targetHitbox) && Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - value4 * scaleFactor, Projectile.Center + value4 * scaleFactor, 20f, ref collisionPoint))
                return true;

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            default(OctaneBladeDrawer).Draw(Projectile);
            return false;
        }
    }
}
