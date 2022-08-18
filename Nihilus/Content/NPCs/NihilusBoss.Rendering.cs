using BlockContent.Nihilus.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleEngine;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ModLoader;

namespace BlockContent.Nihilus.Content.NPCs
{
    public partial class NihilusBoss
    {
        //private static RenderTarget2D fullTarget;
        private static RenderTarget2D flameTarget;
        private static RenderTarget2D flameTarget2;
        private Vector2 oldScreenSize;

        private void DrawToFlames()
        {
            Main.graphics.GraphicsDevice.SetRenderTarget(flameTarget);
            Main.graphics.GraphicsDevice.Clear(Color.Transparent);

            Main.spriteBatch.Begin(default, default, SamplerState.PointClamp, default, default);
            
            DrawFlameShapes();

            Main.spriteBatch.End();

            Main.graphics.GraphicsDevice.SetRenderTarget(flameTarget2);
            Main.graphics.GraphicsDevice.Clear(Color.Transparent);

            Effect color = ModContent.Request<Effect>($"{nameof(BlockContent)}/Nihilus/Assets/Effects/ColorMapShader", AssetRequestMode.ImmediateLoad).Value;
            color.Parameters["palette"].SetValue(ModContent.Request<Texture2D>($"{nameof(BlockContent)}/Nihilus/Assets/Textures/NihilusFlamePalette").Value);
            color.Parameters["sampleTexture0"].SetValue(ModContent.Request<Texture2D>($"{nameof(BlockContent)}/Nihilus/Assets/Textures/FlamePattern0").Value);
            color.Parameters["sampleTexture1"].SetValue(ModContent.Request<Texture2D>($"{nameof(BlockContent)}/Nihilus/Assets/Textures/FlamePattern1").Value);
            color.Parameters["uSizeImage"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
            color.Parameters["uSize0"].SetValue(new Vector2(128));
            color.Parameters["uSize1"].SetValue(new Vector2(256));
            color.Parameters["uPosition"].SetValue(Main.screenPosition / 2);
            color.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 0.1f % 1f);
            color.Parameters["uAlpha"].SetValue(1f);

            Main.spriteBatch.Begin(default, default, default, default, default, color);

            Main.spriteBatch.Draw(flameTarget, Vector2.Zero, Color.White);

            Main.spriteBatch.End();

            Main.graphics.GraphicsDevice.SetRenderTarget(flameTarget);
            Main.graphics.GraphicsDevice.Clear(Color.Transparent);

            Main.spriteBatch.Begin();

            Main.spriteBatch.Draw(flameTarget2, Vector2.Zero, Color.White);

            Main.spriteBatch.End();

            Main.graphics.GraphicsDevice.SetRenderTarget(null);

        }

        private void DrawToTargets(On.Terraria.Main.orig_CheckMonoliths orig)
        {
            if (Main.gameMenu)
            {
                orig();
                return;
            }

            if (Main.graphics.GraphicsDevice != null)
            {
                if (oldScreenSize != new Vector2(Main.screenWidth, Main.screenHeight))
                {
                    if (!Main.dedServ)
                    {
                        //fullTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, width, height);
                        flameTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
                        flameTarget2 = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
                    }
                }

                DrawToFlames();
            }

            oldScreenSize = new Vector2(Main.screenWidth, Main.screenHeight);

            orig();
        }

        private void DrawFlamesParticles(On.Terraria.Main.orig_DrawNPCs orig, Main self, bool behindTiles)
        {
            if (!behindTiles)
                DrawFlames();

            orig(self, behindTiles);
        }

        private void DrawFlameShapes()
        {
            Asset<Texture2D> flameTex = ModContent.Request<Texture2D>($"{nameof(BlockContent)}/Nihilus/Content/Particles/AbyssalFlame");

            foreach (Particle particle in ParticleSystem.particle)
            {
                if (particle.Type == Particle.ParticleType<AbyssalFlame>() && particle.Active)
                {
                    Vector2 stretch = new Vector2(1f, 0.99f + particle.velocity.Length() * 0.01f);
                    Rectangle frame = flameTex.Frame(1, 8, 0, (particle as AbyssalFlame).frame);
                    float fadeIn = Utils.GetLerpValue(0, 19, (particle as AbyssalFlame).life, true);
                    float colLerp = Utils.GetLerpValue(-20, 40, (particle as AbyssalFlame).life, true);
                    Color flameColor = Color.Lerp(Color.Lerp(new(0, 255, 200), new(0, 255, 0), MathHelper.Clamp(colLerp * 4f, 0, 1f)), new(255, 0, 0), (float)Math.Sqrt(colLerp));
                    flameColor.A /= 3;
                    Main.spriteBatch.Draw(flameTex.Value, (particle.position - Main.screenPosition) / 2f, frame, flameColor, particle.rotation, frame.Size() * 0.5f, stretch * fadeIn * particle.scale, 0, 0);
                }
            }
            //add additional things here. make sure they fade from green to red.
        }

        public override void Load()
        {
            if (!Main.dedServ)
                Main.QueueMainThreadAction(() =>
                {
                    //fullTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
                    flameTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
                    flameTarget2 = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
                });
            On.Terraria.Main.DrawNPCs += DrawFlamesParticles;
            On.Terraria.Main.CheckMonoliths += DrawToTargets;
        }

        public void DrawBody(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Asset<Texture2D> armsTex = ModContent.Request<Texture2D>(Texture + "Arms");
            Asset<Texture2D> eyeTex = ModContent.Request<Texture2D>(Texture + "Eye");

            //arms
            for (int i = 0; i < 3; i++)
            {
                float armRot = (MathHelper.Pi / 4.5f) - (MathHelper.Pi / 1.21f * i / 3f);
                float altArmRot = MathHelper.Pi - armRot;
                Vector2 armOff = new Vector2(42 - i + (float)Math.Sin((Main.GlobalTimeWrappedHourly * 2f - i * 0.8f) % MathHelper.TwoPi) * 4f, 0).RotatedBy(armRot);
                armOff.X *= 0.8f;
                armOff.Y -= 13;
                Vector2 altArmOff = new Vector2(-armOff.X, armOff.Y);
                Rectangle armFrame = armsTex.Frame(1, 3, 0, 2 - i);
                Vector2 armOrigin = new Vector2(6, 11);
                Vector2 altArmOrigin = new Vector2(6, 11);
                spriteBatch.Draw(armsTex.Value, NPC.Center - NPC.velocity * 0.33f + armOff.RotatedBy(NPC.rotation) * NPC.scale - screenPos, armFrame, Color.White, armRot + NPC.rotation, armOrigin, NPC.scale, 0, 0);
                spriteBatch.Draw(armsTex.Value, NPC.Center - NPC.velocity * 0.33f + altArmOff.RotatedBy(NPC.rotation) * NPC.scale - screenPos, armFrame, Color.White, altArmRot + NPC.rotation, altArmOrigin, NPC.scale, SpriteEffects.FlipVertically, 0);
            }
            
            //the body
            spriteBatch.Draw(texture.Value, NPC.Center - screenPos, null, Color.White, NPC.rotation, texture.Size() * 0.5f, NPC.scale, 0, 0);

            //eye
            Vector2 eyePos = NPC.Center + new Vector2(0, -10f * NPC.scale) + lookingDirection * new Vector2(1f, 1.3f);
            spriteBatch.Draw(eyeTex.Value, eyePos - screenPos, null, new Color(255, 255, 255, 0), NPC.rotation, eyeTex.Size() * 0.5f, NPC.scale * 0.7f, 0, 0);
        }

        public void DrawShield(float strength)
        {
            //shield shader goes here
            Effect edges = ModContent.Request<Effect>($"{nameof(BlockContent)}/Nihilus/Assets/Effects/EdgeShader", AssetRequestMode.ImmediateLoad).Value;
            edges.Parameters["uSize"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
            edges.Parameters["useLight"].SetValue(0f);
            edges.Parameters["uColor"].SetValue(Color.White.ToVector3() * strength);
            edges.Parameters["uSecColor"].SetValue(Vector3.Zero);
            edges.Parameters["uAlpha"].SetValue(0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, edges);

            Main.spriteBatch.Draw(flameTarget, Vector2.Zero, null, Color.White * strength, 0, Vector2.Zero, 2, 0, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null);
        }

        public void DrawFlames()
        {
            //red and blue bit goes here

            Effect edges = ModContent.Request<Effect>($"{nameof(BlockContent)}/Nihilus/Assets/Effects/EdgeShader", AssetRequestMode.ImmediateLoad).Value;
            edges.Parameters["uSize"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
            edges.Parameters["useLight"].SetValue(1f);
            edges.Parameters["uColor"].SetValue(new Vector3(0.3f));
            //edges.Parameters["useLight"].SetValue(0.8f);
            //edges.Parameters["uColor"].SetValue(Vector3.One);
            edges.Parameters["uSecColor"].SetValue(new Vector4(1f, 1f, 1f, 0.8f));
            edges.Parameters["uAlpha"].SetValue(0.8f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, edges, Main.Transform);

            Main.spriteBatch.Draw(flameTarget, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 2, 0, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            DrawBody(spriteBatch, screenPos);

            //DrawShield(0f);

            return false;
        }
    }
}
