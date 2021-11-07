using BlockContent.Content.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContent.Content.Projectiles.Red
{
    public class SanctuaryMissile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sanctuary Missile");
            ProjectileID.Sets.TrailCacheLength[Type] = 15;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 360;
        }

        public override void AI()
        {
            const float speed = 16f;
            bool inRange = MoreUtils.NPCInRange(Projectile, Projectile.Center, 500, out int npcIndex) && Projectile.timeLeft <= 345;
            NPC target = Main.npc[npcIndex];
            if (inRange)
            {
                Projectile.velocity += Projectile.DirectionTo(target.Center).SafeNormalize(Vector2.Zero) * speed * 0.2f;
                if (Projectile.velocity.Length() > 1)
                    Projectile.velocity *= 0.9f;
            }
            else
                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * speed;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }
        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.Center);
            for (int i = 0; i < Main.rand.Next(50, 70); i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.Center - new Vector2(2), 4, 4, 278, 0, 0, 0, MoreColor.Sanguine, 1f);
                dust.noGravity = true;
                dust.velocity += Main.rand.NextVector2Circular(10, 10);
                dust.scale = (Main.rand.NextFloat() * 0.4f) + 0.4f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> missileTexture = Mod.Assets.Request<Texture2D>("Content/Projectiles/Red/SanctuaryMissile");
            Asset<Texture2D> trailTexture = TextureAssets.Extra[98];
            Rectangle baseFrame = missileTexture.Frame(2, 1, 0, 0);
            Rectangle glowFrame = missileTexture.Frame(2, 1, 1, 0);

            Color glowColor = MoreColor.Sanguine;
            glowColor.A /= 3;
            for (int i = 1; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
            {
                Vector2 drawPos = Projectile.oldPos[i] + (Projectile.Size / 2);
                float opacity = Utils.GetLerpValue(ProjectileID.Sets.TrailCacheLength[Type], 0, i, true);
                Main.EntitySpriteDraw(trailTexture.Value, drawPos - Main.screenPosition, null, glowColor * opacity, Projectile.oldRot[i] + MathHelper.PiOver2, trailTexture.Size() / 2, new Vector2(0.2f, 0.6f) * Projectile.scale, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(missileTexture.Value, Projectile.Center - Main.screenPosition, baseFrame, Color.White, Projectile.rotation, baseFrame.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(missileTexture.Value, Projectile.Center - Main.screenPosition, glowFrame, MoreColor.Sanguine, Projectile.rotation, glowFrame.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

            if (Main.rand.Next(7) == 0)
            {
                for (int i = 0; i < Main.rand.Next(0, 4); i++)
                {
                    Dust dust = Dust.NewDustDirect(Projectile.Center - new Vector2(2), 4, 4, 278, 0, 0, 0, MoreColor.Sanguine, 1f);
                    dust.noGravity = true;
                    dust.velocity += Projectile.velocity * 0.2f;
                    dust.scale = (Main.rand.NextFloat() * 0.4f) + 0.4f;
                }
            }

            return false;
        }
    }
}