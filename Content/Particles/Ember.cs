using System;
using BlockContent.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;

namespace BlockContent.Content.Particles
{
    public class Ember : Particle
    {
        public override void OnSpawn()
        {
            velocity += Main.rand.NextVector2Circular(1, 1);
        }

        public override void Update()
        {
            velocity *= 0.98f;
            scale *= 0.98f;
            if (Main.rand.Next(2) == 0)
                velocity += Main.rand.NextVector2Circular(0.3f, 0.2f);
            velocity.Y -= 0.02f;
            if (scale < 0.1f)
                Active = false;

            rotation = velocity.ToRotation();

            if (emit)
                Lighting.AddLight(position, color.ToVector3() * 0.6f);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Asset<Texture2D> texture = Mod.Assets.Request<Texture2D>("Assets/Textures/Glow");
            Color drawColor = color;
            drawColor.A = 0;
            Color light = Color.Lerp(Color.White, Color.LightGoldenrodYellow, (float)(Math.Sin(Main.GlobalTimeWrappedHourly % MathHelper.TwoPi * scale) * 0.2f) + 0.8f);
            Color inColor = Color.Lerp(drawColor, light, 0.3f);
            inColor.A = 0;
            Vector2 stretch = new Vector2(1f, 0.7f + (velocity.Length() * 0.3f)) * scale * 0.5f;
            spriteBatch.Draw(texture.Value, position - Main.screenPosition, null, drawColor, rotation, texture.Size() * 0.5f, stretch, SpriteEffects.None, 0);
            spriteBatch.Draw(texture.Value, position - Main.screenPosition, null, inColor, rotation, texture.Size() * 0.5f, stretch * 0.3f, SpriteEffects.None, 0);
        }
    }
}
