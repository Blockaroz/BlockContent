using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Graphics.Shaders;
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
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Rectangle checkDraw = new Rectangle((int)Main.screenPosition.X - 500, (int)Main.screenPosition.Y - 550, Main.screenWidth + 1000, Main.screenHeight + 1100);

            foreach (Particle particle in particle.Where(p => p.shader == null))
            {
                if (!new Rectangle((int)particle.position.X - 3, (int)particle.position.Y - 3, 6, 6).Intersects(checkDraw))
                    continue;
                if (Main.netMode != NetmodeID.Server)
                    particle.Draw(spriteBatch);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            foreach (Particle particle in particle.Where(p => p.shader != null))
            {
                if (!new Rectangle((int)particle.position.X - 2, (int)particle.position.Y - 2, 4, 4).Intersects(checkDraw))
                    continue;
                if (Main.netMode != NetmodeID.Server)
                {
                    particle.emit = false;
                    particle.shader.Apply(null);
                    particle.Draw(spriteBatch);
                }
            }
            spriteBatch.End();
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
            //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}
