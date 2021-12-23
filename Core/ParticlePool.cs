using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContent.Core
{
    public static class ParticlePool
    {
        public static List<Particle> particles = new List<Particle>();
        public static List<Particle> particlesBehindEntities = new List<Particle>();

        public static Particle NewParticle(Particle type, Vector2 position, Vector2 velocity, Color color, float scale = 1f, bool drawBehindEntities = false)
        {
            if (!Main.gamePaused)
            {
                type.SetStaticDefaults();
                type.position = position;
                type.velocity = velocity;
                type.color = color;
                type.scale = scale;
                type.rotation = velocity.ToRotation() + (Main.rand.NextFloat(-0.2f, 0.2f) * MathHelper.TwoPi);

                type.Active = true;
                if (drawBehindEntities)
                    particlesBehindEntities.Add(type);
                else
                    particles.Add(type);
            }
            return type;
        }

        public static void UpdateParticles()
        {
            for (int i = 0; i < particles.Count; i++)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    particles[i].position += particles[i].velocity;
                    particles[i].Update();
                    if (!particles[i].Active)
                    {
                        particles.RemoveAt(i);
                        i--;
                    }
                }
            }
            for (int i = 0; i < particles.Count; i++)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    particles[i].position += particles[i].velocity;
                    particles[i].Update();
                    if (!particles[i].Active)
                    {
                        particles.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        public static void DrawParticles(SpriteBatch spriteBatch)
        {
            foreach (Particle particle in particles)
            {
                if (Main.netMode != NetmodeID.Server && !Main.GlobalTimerPaused)
                    particle.Draw(spriteBatch);
            }
        }
        
        public static void DrawParticlesBehind(SpriteBatch spriteBatch)
        {
            foreach (Particle particle in particlesBehindEntities)
            {
                if (Main.netMode != NetmodeID.Server && !Main.GlobalTimerPaused)
                    particle.Draw(spriteBatch);
            }
        }
    }
}
