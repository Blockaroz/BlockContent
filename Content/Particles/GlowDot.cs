using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleEngine;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace BlockContent.Content.Particles
{
    public class GlowDot : Particle
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
            internalScale *= 0.99f;
            scale *= 0.99f;
            velocity *= 0.94f;
            if (Main.rand.NextBool(2))
                velocity += Main.rand.NextVector2Circular(0.3f, 0.2f);

            if (data is Entity offset)
                position += (offset.position - offset.oldPosition) * Utils.GetLerpValue(0.77f, 0.9f, internalScale, true);

            if (internalScale < 0.7f)
            {
                internalScale *= 0.92f;
                velocity *= 0.9f;
            }
            if (internalScale * scale < 0.2f)
                Active = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Asset<Texture2D> dot = ModContent.Request<Texture2D>(Texture);
            Color bloomColor = color * 0.77f * Utils.GetLerpValue(0.3f, 0.8f, internalScale);
            bloomColor.A = 0;
            Color shineColor = Color.Lerp(Color.White, color, 0.2f) * Utils.GetLerpValue(0.1f, 0.5f, scale, true);
            shineColor.A = 0;
            float realScale = scale * internalScale * Utils.GetLerpValue(1f, 0.99f, internalScale, true) * 0.4f;

            spriteBatch.Draw(dot.Value, position - Main.screenPosition, null, bloomColor, rotation, dot.Size() * 0.5f, realScale, SpriteEffects.None, 0);
            spriteBatch.Draw(dot.Value, position - Main.screenPosition, null, shineColor, rotation, dot.Size() * 0.5f, realScale * 0.66f, SpriteEffects.None, 0);
        }
    }
}
