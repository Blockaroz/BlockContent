using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace BlockContent.Nihilus.Content.NPCs
{
    public class NihilusRenderer : ILoadable
    {
        public class NihilusFlameParticle
        {
            public NihilusFlameParticle(Vector2 position, Vector2 velocity, float scale, bool follow)
            {
                this.position = position;
                this.velocity = velocity;
                this.scale = scale;
                this.follow = follow;
                active = true;
                time = 0;
                frame = Main.rand.Next(7);
                rotation = Main.rand.NextFloat() * MathHelper.TwoPi;
            }

            public Vector2 position;
            public Vector2 velocity;
            public float scale;
            public bool active;
            public bool follow;
            public float time;
            public int frame;
            public float rotation;

            public void Update() 
            {
                rotation = velocity.ToRotation();

                velocity *= 0.94f;

                velocity.Y -= 0.014f;

                if (time > 10)
                    scale *= 0.92f;

                if (scale < 0.2f)
                    active = false;

                time++;
            }
        }

        public IList<NihilusFlameParticle> flames = new List<NihilusFlameParticle>();

        public void Update(NPC host, float flameSpeed)
        {
            foreach (NihilusFlameParticle particle in flames.ToList())
            {
                if (particle.follow)
                    particle.position += new Vector2(host.velocity.X * 0.8f, host.velocity.Y * 0.85f);
                else
                    particle.velocity += host.velocity * 0.02f;
                particle.position += particle.velocity;
                particle.Update();
                if (!particle.active)
                    flames.Remove(particle);
            }

            for (int i = 0; i < 8; i++)
            {
                Vector2 velocity = new Vector2(flameSpeed * 1.5f, 0).RotatedBy(MathHelper.TwoPi / 8f * i);
                velocity.Y -= flameSpeed * 0.5f;
                velocity.X *= 0.98f;
                flames.Add(new NihilusFlameParticle(host.Center + host.velocity * 1.5f, velocity.RotatedByRandom(0.05f), 3f, !Main.rand.NextBool(4)));
            }
        }

        private RenderTarget2D flameTarget { get; set; }

        public void DrawFlames(SpriteBatch spriteBatch, Vector2 screenPos, GraphicsDevice graphicsDevice)
        {
            if (flameTarget is null)
                flameTarget = new RenderTarget2D(graphicsDevice, Main.screenWidth, Main.screenHeight);

            spriteBatch.End();

            graphicsDevice.SetRenderTarget(flameTarget);
            graphicsDevice.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, default, Main.Transform);

            spriteBatch.End();

            graphicsDevice.SetRenderTarget(null);
            graphicsDevice.Clear(Color.Transparent);

            Effect edges = ModContent.Request<Effect>($"{nameof(BlockContent)}/Nihilus/Assets/Effects/EdgeShader", AssetRequestMode.ImmediateLoad).Value;
            edges.Parameters["uColor"].SetValue(Color.Cyan.ToVector3());
            edges.Parameters["uAlpha"].SetValue(0f);
            edges.Parameters["uSize"].SetValue(new Vector2(Main.screenWidth / 2f, Main.screenHeight / 2f));

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);

            spriteBatch.Draw(flameTarget, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 2f, 0, 0);

            spriteBatch.End();
            spriteBatch.Begin(default, default, default, default, default, default, Main.Transform);
        }

        private void DrawParticles(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>($"{nameof(BlockContent)}/Nihilus/Assets/Textures/NihilusParticle");
            foreach(NihilusFlameParticle particle in flames)
            {
                Rectangle frame = texture.Frame(1, 8, 0, particle.frame);
                Vector2 stretch = new Vector2(2f / particle.velocity.Length(), 0.5f + particle.velocity.Length());
                Color color = (particle.follow ? Color.Lerp(Color.Green, Color.Red, particle.time / 50f) : Color.Red) * 0.5f;
                color.A = 0;
                spriteBatch.Draw(texture.Value, (particle.position - screenPos) / 2f, frame, color, particle.rotation, frame.Size() * 0.5f, particle.scale, 0, 0);
            }
        }

        public void Load(Mod mod)
        {
            //On.Terraria.Main.DrawPlayers_BehindNPCs += 
        }

        public void Unload()
        {
        }
    }
}
