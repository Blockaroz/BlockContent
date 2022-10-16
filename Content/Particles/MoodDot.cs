using System;
using Terraria;
using Terraria.ModLoader;
using ParticleEngine;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ReLogic.Content;
using Terraria.GameContent;

namespace BlockContent.Content.Particles
{
    public sealed class MoodDot : Particle
    {
        private float life;

        public override void OnSpawn()
        {
            life = 0;
            rotation = 0;
        }

        public override void Update()
        {
            life += 1f / scale;

            velocity = Vector2.Lerp(velocity, Main.rand.NextVector2Circular(25, 25) * scale * (float)Math.Sin(life * 0.03f % MathHelper.TwoPi), 0.015f);

            rotation = MathHelper.Lerp(rotation, Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4), 0.001f);

            float fade = Utils.GetLerpValue(0, 30, life, true) * Utils.GetLerpValue(200, 120, life, true);

            if (emit)
                Lighting.AddLight(position, color.ToVector3() * 0.12f * fade);

            if (life > 200)
                Active = false;

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            float fade = Utils.GetLerpValue(0, 30, life, true) * Utils.GetLerpValue(200, 120, life, true);
            float visualScale = Utils.GetLerpValue(-0.7f, 1, scale);
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, position - Main.screenPosition, new Rectangle(0, 0, 2, 2), color * fade, rotation, Vector2.One, visualScale, 0, 0);
            if (emit)
            {
                Asset<Texture2D> bloom = ModContent.Request<Texture2D>(Texture);
                Color bloomColor = color;
                bloomColor.A = 0;
                spriteBatch.Draw(bloom.Value, position - Main.screenPosition, null, bloomColor * 0.4f * fade, rotation * 0.05f, bloom.Size() * 0.5f, visualScale, 0, 0);
            }
        }
    }
}
