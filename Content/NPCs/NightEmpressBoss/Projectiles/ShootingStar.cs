using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.GameContent;
using Terraria.ID;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using SoundType = Terraria.Audio.SoundType;
using System;

namespace BlockContent.Content.NPCs.NightEmpressBoss.Projectiles
{
    public class ShootingStar : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shooting Star");
            ProjectileID.Sets.TrailCacheLength[Type] = 130;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 38;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 170;
        }

        public override void AI()
        {
            Player target = Main.player[(int)Projectile.ai[0]];
            Vector2 predictedPosition = target.Center + new Vector2(target.velocity.X * 20, (target.velocity.Y * 20) - Math.Abs(target.velocity.X * 0.15f));

            if (Projectile.timeLeft > 140 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.velocity *= 0.87f;
                Projectile.velocity += Main.rand.NextVector2Circular(1, 2);
                Projectile.rotation += Main.rand.NextVector2Circular(1, 2).ToRotation() * Projectile.direction;
            }
            if (Projectile.timeLeft <= 150 && Projectile.timeLeft > 125)
            {
                Projectile.rotation = Projectile.AngleTo(predictedPosition);

                if (Projectile.timeLeft == 150)
                {
                    Projectile.localAI[0]++;
                    Projectile.velocity = Vector2.Zero;
                }
            }
            if (Projectile.timeLeft == 125)
            {
                Projectile.velocity += Projectile.DirectionTo(predictedPosition) * 40;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = Color.White;
            return true;
        }
    }
}
