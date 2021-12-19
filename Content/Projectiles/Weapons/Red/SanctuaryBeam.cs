using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using BlockContent.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContent.Content.Projectiles.Weapons.Red
{
    public class SanctuaryBeam : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sanctuary");
        }
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 9;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 4;
        }

        public float BeamLength;

        public override void AI()
        {
            Projectile.velocity *= 0.01f;

            const int sampleCount = 3;
            Collision.AimingLaserScan(Projectile.Center, Projectile.Center + new Vector2(2400, 0).RotatedBy(Projectile.rotation), 10f, sampleCount, out Vector2 vector, out float[] samples);
            float length = 0;
            for (int i = 0; i < samples.Length; i++)
                length += samples[i];

            if (Projectile.timeLeft == 8)
                BeamLength = (length / sampleCount);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 180, true);
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 180, true);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0;
            bool collision = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + new Vector2(BeamLength, 0).RotatedBy(Projectile.rotation), 2f, ref collisionPoint);
            if (collision)
                BeamLength = collisionPoint;
            return collision;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> beamTexture = Mod.Assets.Request<Texture2D>("Content/Projectiles/Weapons/Red/SanctuaryBeam");
            Rectangle headFrame = new Rectangle(0, 0, 38, 28);
            Rectangle bodyFrame = new Rectangle(0, 30, 38, 12);
            Rectangle tailFrame = new Rectangle(0, 44, 38, 34);

            Color glowColor = Color2.Sanguine;
            glowColor.A /= 2;
            Color glowInnerColor = Color2.PaleGray;
            glowInnerColor.A = 0;

            float rotation = Projectile.rotation - MathHelper.PiOver2;
            float beamScale = Utils.GetLerpValue(0, 6, Projectile.timeLeft, true) * 0.7f;
            float beamInnerScale = Utils.GetLerpValue(4, 8, Projectile.timeLeft, true) * 0.36f;

            Main.EntitySpriteDraw(beamTexture.Value, Projectile.Center - Main.screenPosition, headFrame, glowColor, rotation, headFrame.Size() * new Vector2(0.5f, 0), new Vector2(beamScale, 0.5f), SpriteEffects.None, 0);
            Main.EntitySpriteDraw(beamTexture.Value, Projectile.Center + new Vector2(0, headFrame.Height * 0.5f).RotatedBy(rotation) - Main.screenPosition, bodyFrame, glowColor, rotation, bodyFrame.Size() * new Vector2(0.5f, 0), new Vector2(beamScale, (BeamLength / bodyFrame.Height)), SpriteEffects.None, 0);
            Main.EntitySpriteDraw(beamTexture.Value, Projectile.Center + new Vector2(0, BeamLength).RotatedBy(rotation) - Main.screenPosition, tailFrame, glowColor, rotation, tailFrame.Size() * new Vector2(0.5f, 0), new Vector2(beamScale, 0.5f), SpriteEffects.None, 0);

            Main.EntitySpriteDraw(beamTexture.Value, Projectile.Center - Main.screenPosition, headFrame, glowInnerColor, rotation, headFrame.Size() * new Vector2(0.5f, 0), new Vector2(beamInnerScale, 0.5f), SpriteEffects.None, 0);
            Main.EntitySpriteDraw(beamTexture.Value, Projectile.Center + new Vector2(0, headFrame.Height * 0.5f).RotatedBy(rotation) - Main.screenPosition, bodyFrame, glowInnerColor, rotation, bodyFrame.Size() * new Vector2(0.5f, 0), new Vector2(beamInnerScale, (BeamLength / bodyFrame.Height)), SpriteEffects.None, 0);
            Main.EntitySpriteDraw(beamTexture.Value, Projectile.Center + new Vector2(0, BeamLength).RotatedBy(rotation) - Main.screenPosition, tailFrame, glowInnerColor, rotation, tailFrame.Size() * new Vector2(0.5f, 0), new Vector2(beamInnerScale, 0.4f), SpriteEffects.None, 0);

            Vector2 particlePos = Vector2.Lerp(Projectile.Center, Projectile.Center + new Vector2(BeamLength, 0).RotatedBy(Projectile.rotation), Main.rand.NextFloat()) + Main.rand.NextVector2Circular(4, 4);
            ParticlePool.NewParticle(new Particles.SanctuaryEmber(), particlePos, Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.Next(4, 10), Color2.Sanguine, Projectile.rotation, 1f + Main.rand.NextFloat());

            return false;
        }
    }
}
