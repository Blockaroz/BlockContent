using BlockContent.Content.Graphics;
using BlockContent.Content.NPCs.NightEmpressBoss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContent.Content.Projectiles.NPCProjectiles.NightEmpressProjectiles
{
    public class LineAttackProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Line");
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 700;
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
            Projectile.timeLeft = _totalTime;
            Projectile.hide = true;
        }

        private const int _totalTime = 40;
        private Vector2 _lineLength;

        public override void AI()
        {
            Projectile.velocity = Vector2.Zero;
            _lineLength = new Vector2(MathHelper.SmoothStep(120, 900, Utils.GetLerpValue(30, 5, Projectile.timeLeft, true)), 0).RotatedBy(Projectile.rotation);
            if (Projectile.timeLeft == 30)
                SoundEngine.PlaySound(SoundID.Item164, Projectile.Center);
            if (Projectile.timeLeft == 28)
                CameraUtils.Screenshake(4, 8);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0;
            if (Collision.CheckAABBvLineCollision(targetHitbox.Center(), targetHitbox.Size(), Projectile.Center, Projectile.Center + _lineLength, 100, ref collisionPoint))
                return true;
            if (Collision.CheckAABBvLineCollision(targetHitbox.Center(), targetHitbox.Size(), Projectile.Center, Projectile.Center - _lineLength, 100, ref collisionPoint))
                return true;

            return false;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Color baseColor = MoreColor.NightSky;
            baseColor.A = 50;
            Color glowColor = NightEmpress.SpecialColor(0);
            glowColor.A = 25;

            Asset<Texture2D> streakTexture = Mod.Assets.Request<Texture2D>("Assets/Textures/Glowball_" + (short)0);
            Asset<Texture2D> ballTexture = Mod.Assets.Request<Texture2D>("Assets/Textures/Glowball_" + (short)1);

            for (int i = 0; i < 2; i++)
            {
                int direction = i == 0 ? 1 : -1;

                //draw black backing
                float indicator = MoreUtils.DualLerp(40, 35, 10, Projectile.timeLeft, true);
                Main.EntitySpriteDraw(ballTexture.Value, Projectile.Center - Main.screenPosition, null, Color.Black * 0.3f * indicator, 0, ballTexture.Size() / 2, 1, SpriteEffects.None, 0);

                //draw empress shades and flare
                for (int j = 0; j < 7; j++)
                {
                    float opacity = MoreUtils.DualLerp(32, 30, 15, 5, Projectile.timeLeft + j, true);
                    float lerp = Utils.GetLerpValue(35, 5, Projectile.timeLeft + (j * 2));
                    Vector2 pos = Projectile.Center + (Vector2.SmoothStep(Vector2.Zero, _lineLength * 1.2f, lerp) * direction);
                    DrawEmpressImage(pos + Main.rand.NextVector2CircularEdge(7, 7), baseColor * 0.5f * opacity, j);
                    DrawEmpressImage(pos + Main.rand.NextVector2CircularEdge(3, 3), glowColor * 0.5f * opacity, j);
                    DrawEmpressImage(pos, glowColor * 0.5f * opacity, j);
                }

                Vector2 streakPos = Vector2.SmoothStep(Vector2.Zero, _lineLength * 1.2f, Utils.GetLerpValue(35, 5, Projectile.timeLeft + 5)) * direction;
                float streakScale = MoreUtils.DualLerp(25, 12, 5, Projectile.timeLeft, true);
                MoreUtils.DrawStreak(streakTexture, SpriteEffects.None, Projectile.Center + streakPos - Main.screenPosition, streakTexture.Size() / 2, streakScale, 3, 9, Projectile.rotation, glowColor, NightEmpress.SpecialColor(0, true)); 

                ParticleOrchestraSettings settings = new ParticleOrchestraSettings()
                {
                    PositionInWorld = Projectile.Center + (_lineLength * direction) + Main.rand.NextVector2Circular(70, 70),
                    MovementVector = Vector2.Zero
                };
                if (Main.rand.Next(2) == 0 && Projectile.timeLeft < 30)
                    ParticleEffects.CreateNightMagic(settings, glowColor);
            }

            return false;
        }

        public void DrawEmpressImage(Vector2 position, Color color, int wingCounter = 0)
        {
            Asset<Texture2D> bodyTexture = Mod.Assets.Request<Texture2D>("Content/NPCs/NightEmpressBoss/NightEmpress");
            Asset<Texture2D> armTexture = Mod.Assets.Request<Texture2D>("Assets/Textures/NightEmpress/NightEmpress_Arms");
            Asset<Texture2D> wingTexture = Mod.Assets.Request<Texture2D>("Assets/Textures/NightEmpress/NightEmpress_Wings");

            Rectangle bodyFrame = bodyTexture.Frame(1, 2, 0, 1);
            Rectangle wingFrame = wingTexture.Frame(1, 11, 0, wingCounter);
            Rectangle armFrameLeft = armTexture.Frame(2, 6, 1, 0);
            Rectangle armFrameRight = armTexture.Frame(2, 6, 0, 0);
            Vector2 armOffsetLeft = new Vector2(31, -26);
            Vector2 armOffsetRight = new Vector2(-31, -26);

            Main.EntitySpriteDraw(wingTexture.Value, position - Main.screenPosition, wingFrame, color, 0, wingFrame.Size() / 2, Projectile.scale * 2, SpriteEffects.None, 0);

            Main.EntitySpriteDraw(bodyTexture.Value, position - Main.screenPosition, bodyFrame, color, 0, bodyFrame.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

            Main.EntitySpriteDraw(armTexture.Value, position + armOffsetLeft - Main.screenPosition, armFrameLeft, color, 0, armFrameLeft.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(armTexture.Value, position + armOffsetRight - Main.screenPosition, armFrameRight, color, 0, armFrameRight.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

        }
    }
}
