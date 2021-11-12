﻿using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.GameContent;
using Terraria.ID;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using BlockContent.Content.NPCs.NightEmpressBoss;

namespace BlockContent.Content.Projectiles.NPCProjectiles.NightEmpressProjectiles
{
    public class ShootingStar : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shooting Star");
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 45;
            Projectile.height = 45;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 190;
        }

        public override void AI()
        {
            Player target = Main.player[(int)Projectile.ai[0]];
            Vector2 predictedPosition = target.Center + 
                new Vector2(target.velocity.X * 20, (target.velocity.Y * 18) - Math.Abs(target.velocity.X * 0.15f)) + 
                new Vector2(Projectile.ai[1] * 100, 0);

            _distance = Projectile.Distance(predictedPosition);

            if (Projectile.velocity == Vector2.Zero)
                Projectile.rotation = MathHelper.Pi;
            else
                Projectile.rotation = Projectile.velocity.ToRotation();

            if (Projectile.timeLeft > 170 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.velocity += Main.rand.NextVector2Circular(3, 3);
                Projectile.velocity += Projectile.DirectionTo(target.Center).SafeNormalize(Vector2.Zero) * Utils.GetLerpValue(1000, 200, predictedPosition.Distance(Projectile.Center));
                Projectile.hostile = false;
            }
            else
                Projectile.hostile = true;

            if (Projectile.timeLeft >= 140)
            {
                _linePosition = Projectile.Center;
                _lineRotation = Projectile.AngleTo(predictedPosition);

                if (Projectile.timeLeft == 145)
                {
                    Projectile.velocity = Vector2.Zero;
                    Projectile.localAI[0]++;
                }
            }

            if (Projectile.timeLeft == 120)
                Projectile.velocity += new Vector2(40, 0).RotatedBy(_lineRotation);//speed of projectile

            if (Projectile.timeLeft == 69)
                Projectile.localAI[0]++;
        }

        private float _distance;
        private float _lineRotation;
        private Vector2 _linePosition;

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.localAI[0] == 1)
            {
                float timeLerp = MoreUtils.DualLerp(140, 120, 90, Projectile.timeLeft, true);
                float lineLength = Utils.GetLerpValue(0f, 128f, _distance);
                Color lineColor = NightEmpress.GlowColor(0);
                lineColor.A /= 5;
                Main.EntitySpriteDraw(TextureAssets.Extra[178].Value, _linePosition - Main.screenPosition, null, lineColor * timeLerp, _lineRotation, Vector2.One, new Vector2(lineLength, 1.5f), SpriteEffects.None, 0);
            }

            Projectile.scale = Utils.GetLerpValue(190, 175, Projectile.timeLeft, true) * Utils.GetLerpValue(0, 30, Projectile.timeLeft, true);

            Asset<Texture2D> star = Mod.Assets.Request<Texture2D>("Content/Projectiles/NPCProjectiles/NightEmpressProjectiles/ShootingStar");
            Asset<Texture2D> starTrail = Mod.Assets.Request<Texture2D>("Content/Projectiles/NPCProjectiles/NightEmpressProjectiles/ShootingStar_Trail");
            Asset<Texture2D> starSparkle = Mod.Assets.Request<Texture2D>("Assets/Textures/Streak_" + (short)0);
            
            Color starColor = NightEmpress.GlowColor(0, true);
            starColor.A = 25;
            Color starAfterImageColor = NightEmpress.GlowColor(0);
            starAfterImageColor.A = 12;
            Color starTrailColor = NightEmpress.GlowColor(1);
            starTrailColor.A = 12;

            Projectile.localAI[1] += 0.05f * Projectile.direction;
            if (Projectile.localAI[1] >= MathHelper.TwoPi || Projectile.localAI[1] <= -MathHelper.TwoPi)
                Projectile.localAI[1] = 0;

            //startrail
            for (int i = 1; i < Projectile.oldPos.Length; i++)
            {
                float opacity = Utils.GetLerpValue(Projectile.oldPos.Length, 0, i, true);
                Vector2 oldCenter = Projectile.oldPos[i] + (Projectile.Size / 2);
                Main.EntitySpriteDraw(TextureAssets.Extra[98].Value, oldCenter - Main.screenPosition, null, starTrailColor * opacity, Projectile.oldRot[i] + MathHelper.PiOver2, TextureAssets.Extra[98].Size() / 2, new Vector2(Projectile.scale * 0.6f, Projectile.scale), SpriteEffects.None, 0);
            }

            //starfire
            if (Projectile.timeLeft <= 120)
            {
                Vector2 scale = new Vector2(Projectile.scale * Utils.GetLerpValue(120, 110, Projectile.timeLeft, true));
                for (int i = 1; i <= 4; i++)
                {
                    Vector2 offset = new Vector2(8, 0).RotatedBy(Projectile.localAI[1]).RotatedBy((MathHelper.TwoPi / 4) * i);
                    Main.EntitySpriteDraw(starTrail.Value, Projectile.Center + offset - Main.screenPosition, null, starTrailColor * 0.4f, Projectile.oldRot[2] + MathHelper.PiOver2, new Vector2(starTrail.Width() / 2), scale * 1.4f, SpriteEffects.None, 0); ;
                }
                Main.EntitySpriteDraw(starTrail.Value, Projectile.Center - Main.screenPosition, null, starTrailColor, Projectile.oldRot[2] + MathHelper.PiOver2, new Vector2(starTrail.Width() / 2), scale, SpriteEffects.None, 0); ;

            }
            //star
            Vector2 oldPos = Projectile.oldPos[1] + (Projectile.Size / 2);
            Main.EntitySpriteDraw(star.Value, oldPos - Main.screenPosition, null, starAfterImageColor * 0.5f, Projectile.localAI[1] * 3, star.Size() / 2, Projectile.scale * 1.4f, SpriteEffects.None, 0);
            for (int i = 1; i <= 5; i++)
            {
                Vector2 offset = new Vector2(3, 0).RotatedBy((MathHelper.TwoPi / 5) * i).RotatedBy(Projectile.localAI[1]);
                Main.EntitySpriteDraw(star.Value, Projectile.Center + offset - Main.screenPosition, null, starColor, Projectile.localAI[1] * 4, star.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.EntitySpriteDraw(star.Value, Projectile.Center - Main.screenPosition, null, MoreColor.NightSky, Projectile.localAI[1] * 4, star.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

            //flash
            float flashScale = MoreUtils.DualLerp(145, 144, 120, Projectile.timeLeft, true);
            MoreUtils.DrawSparkle(starSparkle, SpriteEffects.None, Projectile.Center - Main.screenPosition, starSparkle.Size() / 2, flashScale, 4, 3, 3, Projectile.localAI[1] * -4, starTrailColor, starColor, alpha: 12);

            return false;
        }
    }
}
