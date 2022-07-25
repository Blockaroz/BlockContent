using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace BlockContent.SonsAndDaughters.Content
{
    public partial class DoomBall
    {
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> shape = ModContent.Request<Texture2D>($"{nameof(BlockContent)}/SonsAndDaughters/Assets/Textures/Cell");

            Effect souls = ModContent.Request<Effect>($"{nameof(BlockContent)}/SonsAndDaughters/Assets/Effects/DoomInnerSoulsShader", AssetRequestMode.ImmediateLoad).Value;

            souls.Parameters["soulTexture"].SetValue(ModContent.Request<Texture2D>($"{nameof(BlockContent)}/SonsAndDaughters/Assets/Textures/SoulTexture").Value);
            souls.Parameters["distortionTexture"].SetValue(ModContent.Request<Texture2D>($"{nameof(BlockContent)}/SonsAndDaughters/Assets/Textures/EdgeNoise1").Value);
            souls.Parameters["uTime"].SetValue(Projectile.localAI[0] * 0.002f);
            souls.Parameters["uSize"].SetValue(Projectile.scale);
            souls.Parameters["uColor"].SetValue(new Color(50, 0, 0, 255).ToVector4());
            souls.Parameters["uSecColor"].SetValue(new Color(20, 10, 20, 255).ToVector4());

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, souls, Main.Transform);

            Main.EntitySpriteDraw(shape.Value, Projectile.Center - Main.screenPosition, null, Color.Black, 0, shape.Size() * 0.5f, Projectile.scale * 0.95f, 0, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

            DrawSoulLayers();
            DrawOuterRing();
            DrawStar();

            return false;
        }

        private void DrawStar()
        {
            Asset<Texture2D> streak = ModContent.Request<Texture2D>($"{nameof(BlockContent)}/SonsAndDaughters/Assets/Textures/EyeStar");
            Color color = new Color(240, 0, 30) * 0.8f;
            color.A = 0;
            Color glow = Color.White;
            glow.A = 0;
            float starScale = (1.5f + (float)Math.Sin(Projectile.localAI[0] * 0.05f % MathHelper.TwoPi) * 0.2f) * Projectile.scale;

            Main.EntitySpriteDraw(streak.Value, Projectile.Center - Main.screenPosition, null, color * 0.8f, -Projectile.localAI[0] * 0.04f, streak.Size() * 0.5f, starScale * 0.66f, 0, 0);

            Main.EntitySpriteDraw(streak.Value, Projectile.Center - Main.screenPosition, null, color, 0, streak.Size() * 0.5f, starScale, 0, 0);

            Main.EntitySpriteDraw(streak.Value, Projectile.Center - Main.screenPosition, null, glow, 0, streak.Size() * 0.5f, starScale * 0.5f, 0, 0);
        }

        private void DrawOuterRing()
        {
            Asset<Texture2D> shape = ModContent.Request<Texture2D>($"{nameof(BlockContent)}/SonsAndDaughters/Assets/Textures/Cell");
            Asset<Texture2D> glow = ModContent.Request<Texture2D>($"{nameof(BlockContent)}/SonsAndDaughters/Assets/Textures/CellGlow");

            Effect ring = ModContent.Request<Effect>($"{nameof(BlockContent)}/SonsAndDaughters/Assets/Effects/DoomOuterRingShader", AssetRequestMode.ImmediateLoad).Value;

            ring.Parameters["distortionTexture0"].SetValue(ModContent.Request<Texture2D>($"{nameof(BlockContent)}/SonsAndDaughters/Assets/Textures/EdgeNoise0").Value);
            ring.Parameters["distortionTexture1"].SetValue(ModContent.Request<Texture2D>($"{nameof(BlockContent)}/SonsAndDaughters/Assets/Textures/EdgeNoise1").Value);
            ring.Parameters["uTime"].SetValue(Projectile.localAI[0] * 0.001f);
            ring.Parameters["uSize"].SetValue(Projectile.scale);
            ring.Parameters["uColor"].SetValue(new Color(255, 179, 255, 20).ToVector4());
            ring.Parameters["uSecColor"].SetValue(new Color(111, 0, 138, 20).ToVector4());

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, ring, Main.Transform);

            float mainScale = 1f + (float)Math.Sin(Projectile.localAI[0] * 0.07f % MathHelper.TwoPi) * 0.01f;
            Main.EntitySpriteDraw(shape.Value, Projectile.Center - Main.screenPosition, null, Color.White, 0, shape.Size() * 0.5f, mainScale * Projectile.scale, 0, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

            Main.EntitySpriteDraw(glow.Value, Projectile.Center - Main.screenPosition, null, new Color(111, 0, 138, 0) * 0.33f, 0, glow.Size() * 0.5f, mainScale * 1.05f * Projectile.scale, 0, 0);
        }

        private void DrawSoulLayers()
        {
            Asset<Texture2D> shape = ModContent.Request<Texture2D>($"{nameof(BlockContent)}/SonsAndDaughters/Assets/Textures/Cell");
            
            Effect souls = ModContent.Request<Effect>($"{nameof(BlockContent)}/SonsAndDaughters/Assets/Effects/DoomSoulsShader", AssetRequestMode.ImmediateLoad).Value;

            souls.Parameters["soulTexture"].SetValue(ModContent.Request<Texture2D>($"{nameof(BlockContent)}/SonsAndDaughters/Assets/Textures/SoulTexture").Value);
            souls.Parameters["distortionTexture"].SetValue(ModContent.Request<Texture2D>($"{nameof(BlockContent)}/SonsAndDaughters/Assets/Textures/EdgeNoise0").Value);
            souls.Parameters["uTime"].SetValue(Projectile.localAI[0] * 0.0015f);
            souls.Parameters["uSize"].SetValue(0.1f);
            souls.Parameters["uColor"].SetValue(new Color(80, 5, 27, 120).ToVector4());

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, souls, Main.Transform);

            Main.EntitySpriteDraw(shape.Value, Projectile.Center - Main.screenPosition, null, Color.White, 0, shape.Size() * 0.5f, Projectile.scale, 0, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

            souls.Parameters["distortionTexture"].SetValue(ModContent.Request<Texture2D>($"{nameof(BlockContent)}/SonsAndDaughters/Assets/Textures/EdgeNoise1").Value);
            souls.Parameters["uTime"].SetValue(Projectile.localAI[0] * 0.001f);
            souls.Parameters["uSize"].SetValue(0.5f);
            souls.Parameters["uColor"].SetValue(new Color(165, 37, 219, 120).ToVector4());

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, souls, Main.Transform);

            Main.EntitySpriteDraw(shape.Value, Projectile.Center - Main.screenPosition, null, Color.White, 0, shape.Size() * 0.5f, Projectile.scale, 0, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

        }

        public void DoParticles()
        {

        }
    }
}
