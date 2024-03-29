﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleEngine;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace BlockContent.Content.Particles
{
    public class Ember : Particle
    {
        public override void OnSpawn()
        {
            scale *= Main.rand.NextFloat(0.5f, 1.2f);
            rotation = velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override void Update()
        {
            scale *= 0.98f;
            velocity *= 0.98f;
            if (Main.rand.NextBool(4))
                velocity += Main.rand.NextVector2Circular(0.3f, 0.2f);
            velocity.Y -= 0.013f;

            if (scale < 0.1f)
                Active = false;

            rotation = velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Asset<Texture2D> bloom = ModContent.Request<Texture2D>($"{nameof(BlockContent)}/Assets/Textures/Glow");
            Color bloomColor = color * 0.9f;
            bloomColor.A = 0;
            Vector2 stretch = new Vector2(0.8f, velocity.Length() * 0.1f + 0.9f);
            spriteBatch.Draw(bloom.Value, position - Main.screenPosition, null, Color.Lerp(Color.White, color, MathHelper.Clamp(0.6f - scale * 0.1f, 0f, 1f)), rotation, bloom.Size() * new Vector2(0.5f, 0.5f), scale * stretch, SpriteEffects.None, 0);
            spriteBatch.Draw(bloom.Value, position - Main.screenPosition, null, bloomColor, rotation, bloom.Size() * 0.5f, scale * 0.5f * stretch, SpriteEffects.None, 0);
        }
    }
}
