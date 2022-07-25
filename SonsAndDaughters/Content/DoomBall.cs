using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace BlockContent.SonsAndDaughters.Content
{
    public partial class DoomBall : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Sons and Daughters of Blockaroz");
            //ProjectileID.Sets.DrawScreenCheckFluff[Type] = 2000;
        }

        public override void SetDefaults()
        {
            Projectile.width = 512;
            Projectile.height = 512;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 4800;
        }

        public override void AI()
        {
            if (Main.rand.NextBool(5))
                Projectile.velocity += Main.rand.NextVector2Circular(1f, 1f);

            Player closest = Main.player[Player.FindClosest(Projectile.Center, 360, 360)];
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(closest.Center).RotatedByRandom(0.5f) * (Projectile.Distance(closest.Center)) * 0.3f, 0.002f);
            Projectile.velocity *= 0.9f;

            int follow = -1;
            foreach (Projectile other in Main.projectile)
                if (other.type == Type && other.active && other.Distance(Projectile.Center) < 450 * Projectile.scale && other.whoAmI != Projectile.whoAmI)
                    follow = other.whoAmI;

            if (follow >= 0)
                Projectile.velocity += Projectile.DirectionFrom(Main.projectile[follow].Center) * Projectile.Distance(Main.projectile[follow].Center) * 0.001f * Projectile.scale;

            Projectile.ai[0]++;
            Projectile.localAI[0]++;

            Projectile.scale = Utils.GetLerpValue(0, 10, (float)Math.Sqrt(Projectile.localAI[0]), true) * Utils.GetLerpValue(0, 8, (float)Math.Sqrt(Projectile.timeLeft), true);
            DoParticles();
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => Projectile.Center.Distance(targetHitbox.Center.ToVector2()) < 250f * Projectile.scale;

    }
}
