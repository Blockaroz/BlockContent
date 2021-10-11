using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.GameContent;
using Terraria.ID;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using System;

namespace BlockContent.Content.NPCs.NightEmpressBoss.Projectiles
{
    public class BombB : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("UnFini");
            ProjectileID.Sets.TrailCacheLength[Type] = 15;
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
            Projectile.timeLeft = 280;
        }

        private const int _fullStop = 24;

        private const int _explode = 14;

        private int _animationCounter = 0;

        public override void AI()
        {
            int targetIndex = (int)Projectile.ai[0];
            Player target = Main.player[targetIndex];
            if (Projectile.timeLeft > 250)
                Projectile.velocity *= 0.97f;

            if (Projectile.timeLeft < 250 && Projectile.timeLeft > 100)
            {
                Vector2 newVelocity = Projectile.velocity;

                if (Main.player.IndexInRange(targetIndex))
                    newVelocity = Projectile.DirectionTo(target.Center) * 32;

                float lerpValue = MathHelper.Lerp(0.05f, 0.1f, Utils.GetLerpValue(190, 40, Projectile.timeLeft, true));
                Projectile.velocity = Vector2.SmoothStep(Projectile.velocity, newVelocity, lerpValue);

                Projectile.velocity += Main.rand.NextVector2Circular((float)Math.Sin(Projectile.velocity.X) * 2, (float)Math.Cos(Projectile.velocity.Y) * 2);
            }

            if (Projectile.Distance(target.Center) < 70 && Projectile.timeLeft > _fullStop)
                Projectile.timeLeft = _fullStop;

            if (Projectile.timeLeft <= _fullStop)
            {
                Projectile.velocity = Vector2.Zero;

                if (Projectile.timeLeft == _explode)
                {
                    //sound
                    Projectile.Resize(120, 120);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> baseTexture = Mod.Assets.Request<Texture2D>("Content/NPCs/NightEmpressBoss/Projectiles/BombB");
            Asset<Texture2D> explosionTexture = Mod.Assets.Request<Texture2D>("Content/NPCs/NightEmpressBoss/Projectiles/BombB_Explosion");
            Asset<Texture2D> trailTexture = Mod.Assets.Request<Texture2D>("Content/NPCs/NightEmpressBoss/Projectiles/BombB_Trail");

            Vector2 origin = new Vector2(21);
            Rectangle baseFrame = baseTexture.Frame(1, 2, 0, 0);
            Rectangle spotFrame = baseTexture.Frame(1, 2, 0, 1);

            Color nightShade = NightEmpress.NightColor(0, true);
            nightShade.A /= 7;
            Color lightShade = NightEmpress.NightColor(0);
            lightShade.A /= 8;
            Color darkShade = NightEmpress.NightColor(0.5f);
            darkShade.A /= 8;

            //float explosionOpacity = Utils.GetLerpValue(0, _explode, Projectile.timeLeft, true);
            //float explosionScale = Utils.GetLerpValue(_explode, 0, Projectile.timeLeft, true);

            if (Projectile.timeLeft > _explode)
            {
                for (int i = 1; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
                {
                    float scaleValue = Utils.GetLerpValue(ProjectileID.Sets.TrailCacheLength[Type], 0, i, true);
                    float lerpValue = Utils.GetLerpValue(0, 7, i, true);
                    Color trailColor = NightEmpress.NightColor(lerpValue);
                    trailColor.A /= 8;
                    Vector2 oldPosition = Projectile.oldPos[i] + (Projectile.Size / 2);
                    Vector2 trailOrigin = new Vector2(9, 8);
                    Main.EntitySpriteDraw(trailTexture.Value, oldPosition - Main.screenPosition, null, trailColor, Projectile.oldRot[i] - MathHelper.PiOver2, trailOrigin, Projectile.scale * scaleValue, SpriteEffects.None, 0);
                }

                float explosionBuild = Utils.GetLerpValue(_explode, _fullStop, Projectile.timeLeft, true);

                Main.EntitySpriteDraw(baseTexture.Value, Projectile.Center - Main.screenPosition, baseFrame, darkShade, Projectile.rotation, origin, Projectile.scale * 1.25f, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(baseTexture.Value, Projectile.Center - Main.screenPosition, baseFrame, nightShade, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(baseTexture.Value, Projectile.Center - Main.screenPosition, spotFrame, Color.Black, Projectile.rotation, origin, Projectile.scale * explosionBuild, SpriteEffects.None, 0);

                if (Main.rand.Next(2) == 0)
                {
                    Dust trailDust = Dust.NewDustDirect(Projectile.oldPos[2] + (Projectile.Size / 2) - new Vector2(5), 10, 10, DustID.AncientLight, Projectile.velocity.X, Projectile.velocity.Y, 0, darkShade, 1f);
                    trailDust.velocity = Projectile.velocity * 0.7f;
                    trailDust.noGravity = true;
                }
            }
            else if (Projectile.timeLeft <= _explode)
            { 
                if (Projectile.timeLeft == _explode)
                {
                    //Dust explodeDust = Dust.NewDustDirect();
                }
            }

            Projectile.scale = Utils.GetLerpValue(281, 270, Projectile.timeLeft, true);
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

            return false;
        }
    }
}
