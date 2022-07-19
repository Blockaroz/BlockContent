using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleEngine;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace BlockContent.Nihilus.Content.NPCs
{
    public class NihilusFlameDrawer : ILoadable
    {
        private static RenderTarget2D flameTarget;
        private static RenderTarget2D flameTarget2;
        private Vector2 oldScreenSize;

        private bool Active { get => NPC.AnyNPCs(ModContent.NPCType<NihilusBoss>()); }

        private void RefreshTarget(On.Terraria.Main.orig_SetDisplayMode orig, int width, int height, bool fullscreen)
        {
            orig(width, height, fullscreen);

            if (oldScreenSize != new Vector2(width, height))
            {
                if (!Main.dedServ)
                {
                    flameTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, width, height);
                    flameTarget2 = new RenderTarget2D(Main.graphics.GraphicsDevice, width, height);
                }
            }
            oldScreenSize = new Vector2(width, height);
        }

        private void DrawToTarget(On.Terraria.Main.orig_CheckMonoliths orig)
        {
            if (Main.gameMenu || !Active)
            {
                orig();
                return;
            }

            Main.graphics.GraphicsDevice.SetRenderTarget(flameTarget);
            Main.graphics.GraphicsDevice.Clear(Color.Transparent);

            Main.spriteBatch.Begin(default, default, SamplerState.PointClamp, default, default);

            Asset<Texture2D> flameTex = ModContent.Request<Texture2D>($"{nameof(BlockContent)}/Nihilus/Content/NPCs/NihilusFlameParticle");
            foreach (Particle particle in ParticleSystem.particle)
            {
                if (particle.Type == Particle.ParticleType<NihilusFlameParticle>() && particle.Active)
                {
                    Vector2 stretch = new Vector2(1f, 0.99f + particle.velocity.Length() * 0.01f);
                    Rectangle frame = flameTex.Frame(1, 8, 0, (particle as NihilusFlameParticle).frame);
                    float fadeIn = Utils.GetLerpValue(-7, 19, (particle as NihilusFlameParticle).life, true);
                    Color flameColor = Color.Lerp(new(255, 0, 0), new(0, 255, 0), Utils.GetLerpValue(30, 0, (particle as NihilusFlameParticle).life, true));
                    Main.spriteBatch.Draw(flameTex.Value, (particle.position - Main.screenPosition) / 2f, frame, flameColor, particle.rotation, frame.Size() * 0.5f, stretch * fadeIn * particle.scale, 0, 0);
                }
            }

            Main.spriteBatch.End();

            Main.graphics.GraphicsDevice.SetRenderTarget(flameTarget2);
            Main.graphics.GraphicsDevice.Clear(Color.Transparent);

            Effect edges = ModContent.Request<Effect>($"{nameof(BlockContent)}/Nihilus/Assets/Effects/EdgeShader", AssetRequestMode.ImmediateLoad).Value;
            edges.Parameters["uSize"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
            edges.Parameters["useLight"].SetValue(false);
            edges.Parameters["uColor"].SetValue(Color.HotPink.ToVector3());
            edges.Parameters["uAlpha"].SetValue(0f);

            Main.spriteBatch.Begin(default, default, default, default, default, edges);

            Main.spriteBatch.Draw(flameTarget, Vector2.Zero, Color.White);

            Main.spriteBatch.End();

            Main.graphics.GraphicsDevice.SetRenderTarget(flameTarget);
            Main.graphics.GraphicsDevice.Clear(Color.Transparent);

            Main.spriteBatch.Begin();

            Main.spriteBatch.Draw(flameTarget2, Vector2.Zero, Color.White);

            Main.spriteBatch.End();

            Main.graphics.GraphicsDevice.SetRenderTarget(null);

            orig();
        }

        public static void DrawFlames(SpriteBatch spriteBatch)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

            spriteBatch.Draw(flameTarget, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 2, 0, 0);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
        }

        public void Load(Mod mod)
        {
            if (!Main.dedServ)
                Main.QueueMainThreadAction(() =>
                {
                    flameTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
                    flameTarget2 = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
                });
            On.Terraria.Main.SetDisplayMode += RefreshTarget;
            On.Terraria.Main.CheckMonoliths += DrawToTarget;
        }

        public void Unload() { }
    }
}
