using System;
using BlockContent.Content.Projectiles.Weapons.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleEngine;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace BlockContent.Content.Particles
{
    public class HarmfulBubbleParticle : Particle
    {
        public override void OnSpawn()
        {
            life = 0;
            frame = Main.rand.Next(3);
            rotation = Main.rand.NextFloat(3.14f);
        }

        private int frame;
        private int life;

        public override void Update()
        {
            scale *= 0.98f;
            velocity *= 0.92f;

            rotation += Main.rand.NextFloat(-0.1f, 0.1f);

            if (scale < 0.15f)
                Active = false;

            life++;
        }

        private static RenderTarget2D bubbleTarget;
        private static RenderTarget2D bubbleTarget2;
        private Vector2 oldScreenSize;

        public override void Load()
        {
            if (!Main.dedServ)
                Main.QueueMainThreadAction(() =>
                {
                    bubbleTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
                    bubbleTarget2 = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
                });
            On.Terraria.Main.SetDisplayMode += RefreshTarget;
            On.Terraria.Main.DrawDust += DrawTarget;
            On.Terraria.Main.CheckMonoliths += DrawToTarget;
        }

        private void RefreshTarget(On.Terraria.Main.orig_SetDisplayMode orig, int width, int height, bool fullscreen)
        {
            orig(width, height, fullscreen);

            if (oldScreenSize != new Vector2(width, height))
            {
                if (!Main.dedServ)
                {
                    bubbleTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, width, height);
                    bubbleTarget2 = new RenderTarget2D(Main.graphics.GraphicsDevice, width, height);
                }
            }
            oldScreenSize = new Vector2(width, height);
        }

        private void DrawToTarget(On.Terraria.Main.orig_CheckMonoliths orig)
        {
            if (Main.gameMenu)
            {
                orig();
                return;
            }

            Main.graphics.GraphicsDevice.SetRenderTarget(bubbleTarget);
            Main.graphics.GraphicsDevice.Clear(Color.Transparent);

            Main.spriteBatch.Begin(default, default, SamplerState.PointClamp, default, default);

            Asset<Texture2D> tex = ModContent.Request<Texture2D>($"{nameof(BlockContent)}/Content/Particles/HarmfulBubbleParticle");
            foreach (Particle bubble in ParticleSystem.particle)
            {
                if (bubble.Type == Type && bubble.Active)
                {
                    float fadeIn = Utils.GetLerpValue(1, 10, (bubble as HarmfulBubbleParticle).life, true) * 0.4f;
                    Rectangle frame = tex.Frame(1, 4, 0, (bubble as HarmfulBubbleParticle).frame);
                    Color lightColor = Lighting.GetColor((int)bubble.position.X / 16, (int)bubble.position.Y / 16);
                    lightColor.A = 180;
                    Main.spriteBatch.Draw(tex.Value, (bubble.position - Main.screenPosition) / 2f, frame, lightColor, bubble.rotation, frame.Size() * 0.5f, fadeIn * bubble.scale, 0, 0);
                }
            }

            Main.spriteBatch.End();

            Main.graphics.GraphicsDevice.SetRenderTarget(bubbleTarget2);
            Main.graphics.GraphicsDevice.Clear(Color.Transparent);

            Effect edges = ModContent.Request<Effect>($"{nameof(BlockContent)}/Assets/Effects/EdgeShader", AssetRequestMode.ImmediateLoad).Value;
            edges.Parameters["uSize"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
            edges.Parameters["useLight"].SetValue(true);
            edges.Parameters["uColor"].SetValue(Color.White.ToVector3());
            edges.Parameters["uAlpha"].SetValue(1f);

            Main.spriteBatch.Begin(default, default, default, default, default, edges);

            Main.spriteBatch.Draw(bubbleTarget, Vector2.Zero, Color.White);

            Main.spriteBatch.End();

            Main.graphics.GraphicsDevice.SetRenderTarget(bubbleTarget);
            Main.graphics.GraphicsDevice.Clear(Color.Transparent);

            Main.spriteBatch.Begin();

            Main.spriteBatch.Draw(bubbleTarget2, Vector2.Zero, Color.White);

            Main.spriteBatch.End();

            Main.graphics.GraphicsDevice.SetRenderTarget(null);

            orig();
        }

        private void DrawTarget(On.Terraria.Main.orig_DrawDust orig, Main self)
        {
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

            Main.spriteBatch.Draw(bubbleTarget, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 2, 0, 0);

            Main.spriteBatch.End();

            orig(self);
        }
    }
}
