using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;

namespace BlockContent.Core
{
    public static class ParticleSystem
    {
        private static int nextID;

        internal static readonly IList<Particle> particleTypes = new List<Particle>();

        internal static int ReserveParticleID() => nextID++;

        public static Particle GetParticle(int type) => type == -1 ? null : particleTypes[type];

        internal static void Unload()
        {
            particleTypes.Clear();
            nextID = 0;
        }

        public static IList<Particle> particle = new List<Particle>();

        public static void UpdateParticles()
        {
            if (Main.netMode != NetmodeID.Server && !Main.gamePaused)
            {
                foreach (Particle p in particle.ToList())
                {
                    p.position += p.velocity;
                    p.Update();
                    if (!p.Active)
                        particle.Remove(p);
                }
            }
        }

        public static void DrawParticles(SpriteBatch spriteBatch)
        {
            foreach (Particle particle in particle)
            {
                if (Main.netMode != NetmodeID.Server)
                    particle.Draw(spriteBatch);
            }
        }
    }
}
