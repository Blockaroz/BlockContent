using Microsoft.Xna.Framework;
using ParticleEngine;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContent.Content.Projectiles.Weapons.Ranged
{
    public class HarmfulBubble : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Harmful Bubble");
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = true;
        }

        public override void AI()
        {
            int search = Projectile.FindTargetWithLineOfSight(600);
            if (search >= 0)
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Main.npc[search].Center).RotatedByRandom(1f) * 20, 0.03f);
            else if (Projectile.ai[0] > 10)
                Projectile.velocity *= 0.95f;

            int follow = -1;
            foreach (Projectile other in Main.projectile)
            {
                if (other.type == Type && other.active && other.owner == Projectile.owner && other.whoAmI != Projectile.whoAmI && Projectile.Distance(other.Center) < 700)
                {
                    follow = other.whoAmI;
                    break;
                }
            }
            if (follow >= 0)
            {
                if (Main.projectile[follow].Center.Distance(Projectile.Center) > 54)
                    Projectile.velocity += Projectile.DirectionTo(Main.projectile[follow].Center).RotatedByRandom(0.5f) * 0.1f;
            }

            float bubbleScale = (1.5f + Main.rand.NextFloat()) * Projectile.scale;
            Vector2 bubblePos = Projectile.Center + Projectile.velocity;
            Particle bubble = Particle.NewParticle(Particle.ParticleType<Particles.HarmfulBubbleParticle>(), bubblePos, Main.rand.NextVector2Circular(2, 2) + Projectile.velocity * 0.5f, Color.White, bubbleScale);
            bubble.data = Projectile.whoAmI;

            Projectile.scale = Utils.GetLerpValue(-10, 10, Projectile.ai[0], true);
            Projectile.ai[0]++;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > 0)
                Projectile.velocity.X = -oldVelocity.X;            
            if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > 0)
                Projectile.velocity.Y = -oldVelocity.Y;

            if (Main.rand.NextBool(10))
                Projectile.Kill(); 
            
            return false;
        }

        public override void Kill(int timeLeft)
        {
            SoundStyle bubbleNoise = SoundID.Item54;
            bubbleNoise.MaxInstances = 0;
            bubbleNoise.PitchVariance = 0.4f;
            SoundEngine.PlaySound(bubbleNoise, Projectile.Center);
        }

        public override bool PreDraw(ref Color lightColor) => false;
    }
}
