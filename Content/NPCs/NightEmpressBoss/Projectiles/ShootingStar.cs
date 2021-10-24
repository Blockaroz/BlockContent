using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.GameContent;
using Terraria.ID;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BlockContent.Content.NPCs.NightEmpressBoss.Projectiles
{
    public class ShootingStar : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shooting Star");
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
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
                new Vector2(target.velocity.X * 25, (target.velocity.Y * 25) - Math.Abs(target.velocity.X * 0.15f)) + 
                new Vector2(Projectile.ai[1] * 170, 0);

            _distance = Projectile.Distance(predictedPosition);

            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Projectile.velocity == Vector2.Zero)
                Projectile.rotation = MathHelper.Pi;

            if (Projectile.timeLeft > 170 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.velocity += Main.rand.NextVector2Circular(5, 5);
                Projectile.velocity += Projectile.DirectionTo(target.Center + new Vector2(0, -10)) * Utils.GetLerpValue(20, 1000, (predictedPosition - Projectile.Center).Length());
                Projectile.hostile = false;
            }
            else
                Projectile.hostile = true;

            if (Projectile.timeLeft >= 140)
            {
                _linePosition = Projectile.Center;
                _lineRotation = Projectile.AngleTo(predictedPosition);
            }

            if (Projectile.timeLeft == 140)
            {
                Projectile.localAI[0]++;
                Projectile.velocity *= 0.01f;
            }

            if (Projectile.timeLeft == 125)
                Projectile.velocity += Projectile.DirectionTo(predictedPosition) * 28;

            if (Projectile.timeLeft == 69)
                Projectile.localAI[0]++;
        }

        private float _distance;
        private float _lineRotation;
        private Vector2 _linePosition;

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < Main.rand.Next(10, 20); i++)
            {
                Color color = NightEmpress.NightColor(Main.rand.NextFloat(0, 1));
                Vector2 circular = Main.rand.NextVector2CircularEdge(10, 10);
                Dust starDust = Dust.NewDustDirect(Projectile.Center, 0, 0, 278, circular.X, circular.Y, 0, color, 1f);
                starDust.noGravity = true;
                starDust.velocity += Main.rand.NextVector2Circular(2, 2);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.localAI[0] == 1)
            {
                float timeLerp = Utils.GetLerpValue(70, 95, Projectile.timeLeft, true);
                float lineLength = Utils.GetLerpValue(0f, 128f, _distance);
                Color lineColor = NightEmpress.NightColor(0);
                lineColor.A /= 5;
                Main.EntitySpriteDraw(TextureAssets.Extra[178].Value, _linePosition - Main.screenPosition, null, lineColor * timeLerp, _lineRotation, Vector2.One, new Vector2(lineLength, 1.5f), SpriteEffects.None, 0);
            }

            Projectile.scale = Utils.GetLerpValue(190, 175, Projectile.timeLeft, true);

            Asset<Texture2D> star = Mod.Assets.Request<Texture2D>("Content/NPCs/NightEmpressBoss/Projectiles/ShootingStar");
            Asset<Texture2D> starTrail = Mod.Assets.Request<Texture2D>("Content/NPCs/NightEmpressBoss/Projectiles/ShootingStar_Trail");

            Color starColor = NightEmpress.NightColor(0, true);
            starColor.A /= 5;
            Color starAfterImageColor = NightEmpress.NightColor(0);
            starAfterImageColor.A /= 7;
            Color starTrailColor = NightEmpress.NightColor(1);
            starTrailColor.A /= 7;

            Projectile.localAI[1] += 0.05f * Projectile.direction;
            if (Projectile.localAI[1] >= MathHelper.TwoPi || Projectile.localAI[1] <= -MathHelper.TwoPi)
                Projectile.localAI[1] = 0;

            float xSquish = MathHelper.Lerp(1f, 0.5f, Utils.GetLerpValue(0, 90, Projectile.velocity.Length()));
            float ySquish = MathHelper.Lerp(1.8f, 0.2f, Utils.GetLerpValue(70, 0, Projectile.velocity.Length()));
            Vector2 trailSquish = new Vector2(Projectile.scale * xSquish, Projectile.scale * ySquish);

            for (int i = 1; i <= 4; i++)
            {
                Vector2 offset = new Vector2(8, 0).RotatedBy(Projectile.localAI[1]).RotatedBy((MathHelper.TwoPi / 4) * i);
                Main.EntitySpriteDraw(starTrail.Value, Projectile.Center + offset - Main.screenPosition, null, starTrailColor * 0.4f, Projectile.oldRot[2] + MathHelper.PiOver2, new Vector2(19), trailSquish * 1.4f, SpriteEffects.None, 0); ;
            }
            Main.EntitySpriteDraw(starTrail.Value, Projectile.Center - Main.screenPosition, null, starTrailColor, Projectile.oldRot[2] + MathHelper.PiOver2, new Vector2(19), trailSquish, SpriteEffects.None, 0); ;

            Vector2 oldPos = Projectile.oldPos[1] + (Projectile.Size / 2);
            Main.EntitySpriteDraw(star.Value, oldPos - Main.screenPosition, null, starAfterImageColor * 0.5f, Projectile.localAI[1] * 3, star.Size() / 2, Projectile.scale * 1.4f, SpriteEffects.None, 0);

            for (int i = 1; i <= 5; i++)
            {
                Vector2 offset = new Vector2(4, 0).RotatedBy((MathHelper.TwoPi / 5) * i).RotatedBy(Projectile.localAI[1]);
                Main.EntitySpriteDraw(star.Value, Projectile.Center + offset - Main.screenPosition, null, starColor, Projectile.localAI[1] * 4, star.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(star.Value, Projectile.Center - Main.screenPosition, null, NightEmpress.NightBlack, Projectile.localAI[1] * 4, star.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

            Dust starDust = Dust.NewDustDirect(Projectile.Center - new Vector2(2), 4, 4, DustID.RainbowMk2, 0, 0, 0, NightEmpress.NightColor(1), 1.5f);
            starDust.noGravity = true;
            starDust.velocity += Main.rand.NextVector2Circular(2, 2);

            return false;
        }
    }
}
