using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using BlockContent.Core;

namespace BlockContent.Content.Projectiles.NegastaffMinions.Seeksery
{
    public class MegidoMeteor : ModProjectile
    {
        public override string Texture => "BlockContent/Assets/Textures/Empty";

        public override void SetDefaults()
        {
            Projectile.width = 70;
            Projectile.height = 70;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.timeLeft = 240;
        }

        public override void AI()
        {
            Vector2 pos = Projectile.Center + Projectile.velocity + Main.rand.NextVector2Circular(40, 40);
            float scale = Utils.GetLerpValue(240, 200, Projectile.timeLeft, true);
            Particle.NewParticle(Particle.ParticleType<Particles.LargeExplosion>(), pos, Vector2.Zero, Color.White, scale);

            if (Projectile.ai[0] > -1)
            {
                NPC target = Main.npc[(int)Projectile.ai[0]];
                if (!target.active)
                    Projectile.Kill();
                else
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(target.Center) * 30f, 0.3f);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Projectile.Kill();
        }
    }
}
