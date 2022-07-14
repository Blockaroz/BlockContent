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
        private float internalScale;

        public override void OnSpawn()
        {
            internalScale = 1f;
            scale *= Main.rand.NextFloat(0.8f, 1.3f);
            rotation = Main.rand.NextFloat(-1f, 1f) * 0.1f;
        }

        public override void Update()
        {
            internalScale *= 0.975f;
            velocity *= 0.9f;
            if (Main.rand.NextBool(4))
                velocity += Main.rand.NextVector2Circular(0.3f, 0.2f);

            if (data is Entity offset)
                position += (offset.position - offset.oldPosition) * Utils.GetLerpValue(0.3f, 0.9f, internalScale, true);

            if (internalScale < 0.66f)
                internalScale *= 0.88f;

            if (internalScale < 0.2f)
                Active = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Asset<Texture2D> star = ModContent.Request<Texture2D>($"{nameof(BlockContent)}/Assets/Textures/GlowStar");
            Asset<Texture2D> bloom = ModContent.Request<Texture2D>($"{nameof(BlockContent)}/Assets/Textures/GlowSoft");
            Color bloomColor = color * 0.77f * Utils.GetLerpValue(0.3f, 0.8f, internalScale);
            bloomColor.A = 0;
            Color shineColor = Color.Lerp(Color.White, color, 0.2f) * Utils.GetLerpValue(0.1f, 0.5f, scale, true);
            shineColor.A = 0;
            float realScale = scale * internalScale * Utils.GetLerpValue(1f, 0.95f, internalScale, true);

            spriteBatch.Draw(star.Value, position - Main.screenPosition, null, bloomColor, rotation, star.Size() * 0.5f, realScale, SpriteEffects.None, 0);
            spriteBatch.Draw(star.Value, position - Main.screenPosition, null, shineColor, rotation, star.Size() * 0.5f, realScale * 0.5f, SpriteEffects.None, 0);
            spriteBatch.Draw(bloom.Value, position - Main.screenPosition, null, bloomColor * 0.15f, rotation, bloom.Size() * 0.5f, realScale * 1.7f, SpriteEffects.None, 0);
        }
    }
}
