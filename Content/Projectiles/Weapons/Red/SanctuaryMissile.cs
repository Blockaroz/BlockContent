﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContent.Content.Projectiles.Weapons.Red
{
    public class SanctuaryMissile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sanctuary Missile");
            ProjectileID.Sets.TrailCacheLength[Type] = 18;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
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

            if (Projectile.ai[0] == 1)
                Projectile.velocity = Vector2.Zero;
            else
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Projectile.ai[0] == 0)
                Projectile.timeLeft = 15;
            Projectile.Resize(64, 64);
            Projectile.ai[0] = 1;
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.Center);
            for (int i = 0; i < Main.rand.Next(50, 70); i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.Center - new Vector2(2), 4, 4, 278, 0, 0, 50, MoreColor.Sanguine, 1f);
                dust.noGravity = true;
                dust.color.A = 50;
                dust.velocity += Main.rand.NextVector2Circular(10, 10);
                dust.scale = (Main.rand.NextFloat() * 0.4f) + 0.4f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> missileTexture = Mod.Assets.Request<Texture2D>("Content/Projectiles/Weapons/Red/SanctuaryMissile");
            Asset<Texture2D> streakTexture = TextureAssets.Extra[98];
            Asset<Texture2D> ballTexture = Mod.Assets.Request<Texture2D>("Assets/Textures/Glowball_" + (short)0);
            Rectangle baseFrame = missileTexture.Frame(3, 1, 0, 0);
            Rectangle glowFrame = missileTexture.Frame(3, 1, 1, 0);
            Rectangle haloFrame = missileTexture.Frame(3, 1, 2, 0);

            Color glowColor = MoreColor.Sanguine;
            glowColor.A /= 3;
            if (Projectile.ai[0] == 0)
            {
                for (int i = 1; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
                {
                    Vector2 drawPos = Projectile.oldPos[i] + (Projectile.Size / 2);
                    float opacity = Utils.GetLerpValue(ProjectileID.Sets.TrailCacheLength[Type], 0, i, true);
                    Main.EntitySpriteDraw(streakTexture.Value, drawPos - Main.screenPosition, null, glowColor * opacity, Projectile.oldRot[i] + MathHelper.PiOver2, streakTexture.Size() / 2, new Vector2(0.2f, 0.6f) * Projectile.scale, SpriteEffects.None, 0);
                }
                Main.EntitySpriteDraw(missileTexture.Value, Projectile.Center - Main.screenPosition, haloFrame, glowColor * 0.2f, Projectile.rotation, haloFrame.Size() / 2, Projectile.scale * 1.5f, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(missileTexture.Value, Projectile.Center - Main.screenPosition, baseFrame, Color.White, Projectile.rotation, baseFrame.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(missileTexture.Value, Projectile.Center - Main.screenPosition, glowFrame, MoreColor.Sanguine, Projectile.rotation, glowFrame.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

            }
            else if (Projectile.ai[0] == 1)
            {
                float scale = MoreUtils.DualLerp(15, 12, 10, 1, Projectile.timeLeft, true);
                MoreUtils.DrawStreak(ballTexture, SpriteEffects.None, Projectile.Center - Main.screenPosition, ballTexture.Size() / 2, scale, 3, 3, 0, MoreColor.Sanguine, Color.White);
                MoreUtils.DrawStreak(streakTexture, SpriteEffects.None, Projectile.Center - Main.screenPosition, streakTexture.Size() / 2, scale, 1, 7, Projectile.rotation, MoreColor.Sanguine, Color.White);
                PunchCameraModifier punch = new PunchCameraModifier(Projectile.Center, Projectile.rotation.ToRotationVector2(), 6f, 12f, 20, 2000f, "Sanctuary");
                Main.instance.CameraModifiers.Add(punch);
            }

            if (Main.rand.Next(3) == 0)
            {
                for (int i = 0; i < Main.rand.Next(1, 5); i++)
                {
                    Dust dust = Dust.NewDustDirect(Projectile.Center - new Vector2(2), 4, 4, 278, 0, 0, 0, MoreColor.Sanguine, 1f);
                    dust.noGravity = true;
                    dust.velocity += Projectile.velocity * 0.2f;
                    dust.scale = (Main.rand.NextFloat() * 0.5f) + 0.5f;
                }
            }

            return false;
        }
    }
}