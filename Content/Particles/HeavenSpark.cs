using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleEngine;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace BlockContent.Content.Particles
{
    public class HeavenSpark : Particle
    {
        public override void OnSpawn()
        {
            scale *= Main.rand.NextFloat(0.8f, 1.3f);
            rotation = Main.rand.NextFloat(-1f, 1f) * 0.1f;
        }

        public override void Update()
        {
            scale *= 0.99f;
            velocity *= 0.9f;
            if (Main.rand.NextBool(4))
                velocity += Main.rand.NextVector2Circular(0.3f, 0.2f);

            if (scale < 0.8f)
                scale *= 0.95f;

            if (scale < 0.15f)
                Active = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Asset<Texture2D> star = ModContent.Request<Texture2D>($"{nameof(BlockContent)}/Assets/Textures/GlowStar");
            Asset<Texture2D> bloom = ModContent.Request<Texture2D>($"{nameof(BlockContent)}/Assets/Textures/GlowSoft");
            Color bloomColor = color * 0.6f * MathHelper.Min(1f, scale);
            bloomColor.A = 0;
            Color shineColor = Color.Lerp(Color.White, color, 0.3f) * Utils.GetLerpValue(0.1f, 0.5f, scale, true);
            shineColor.A = 0; 

            spriteBatch.Draw(star.Value, position - Main.screenPosition, null, bloomColor, rotation, star.Size() * new Vector2(0.5f, 0.5f), scale, SpriteEffects.None, 0);
            spriteBatch.Draw(star.Value, position - Main.screenPosition, null, shineColor, rotation, star.Size() * new Vector2(0.5f, 0.5f), scale * 0.5f, SpriteEffects.None, 0);
            spriteBatch.Draw(bloom.Value, position - Main.screenPosition, null, bloomColor * 0.3f, rotation, bloom.Size() * new Vector2(0.5f, 0.5f), scale, SpriteEffects.None, 0);
        }
    }
}
