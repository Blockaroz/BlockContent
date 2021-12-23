using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using BlockContent.Core;
using Terraria.ModLoader;

namespace BlockContent.Content.Particles
{
    public class Ember : Particle
    {
        public override void Update()
        {
            misc++;
            velocity.Y -= 0.04f;
            velocity.Y += (float)Math.Cos(misc * 100f) / scale / 100f;
            if (Main.rand.Next(2) == 0)
                velocity.X += Main.rand.NextFloat(-0.2f, 0.2f);
            rotation = velocity.ToRotation();
            if (misc > 20)
            {
                scale *= 0.96f;
                velocity.X *= 0.99f;
            }
            if (scale < 0.1f)
                Active = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            color.A /= 2;
            Color glowColor = Color.Lerp(Color.White, color, 0.25f);
            glowColor.A = 0;

            Asset<Texture2D> ember = Mod.Assets.Request<Texture2D>("Content/Particles/Ember");

            spriteBatch.Draw(ember.Value, position - Main.screenPosition, null, color * 0.9f, rotation - MathHelper.PiOver2, ember.Size() / 2, scale, SpriteEffects.None, 0);
            spriteBatch.Draw(ember.Value, position - Main.screenPosition, null, glowColor, rotation - MathHelper.PiOver2, ember.Size() / 2, scale * 0.45f, SpriteEffects.None, 0);
        }
    }
}
