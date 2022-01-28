using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContent.Core
{
    public static class ParticleLoader
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
                for (int i = 0; i < particle.Count; i++)
                {
                    particle[i].position += particle[i].velocity;
                    particle[i].Update();
                    if (!particle[i].active)
                    {
                        particle.RemoveAt(i);
                        i--;
                    }
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
