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
            velocity.Y -= 0.02f;
            velocity.Y += (float)Math.Cos(misc * 60f) / scale / 20f;
            if (Main.rand.Next(2) == 0)
                velocity.X += Main.rand.NextFloat(-0.3f, 0.3f);
            rotation = velocity.ToRotation();
            if (misc > 15)
            {
                scale *= 0.95f;
                velocity.X *= 0.94f;
            }
            velocity *= 1.01f;
            if (scale < 0.1f)
                active = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            color.A = 20;
            Color glowColor = Color.Lerp(Color.White, color, 0.25f);
            glowColor.A = 0;

            Asset<Texture2D> ember = Mod.Assets.Request<Texture2D>("Content/Particles/Ember");

            Vector2 stretch = new Vector2(scale, scale + Utils.GetLerpValue(0, 2, velocity.Length()) * Utils.GetLerpValue(0, 0.33f, scale, true));

            spriteBatch.Draw(ember.Value, position - Main.screenPosition, null, color * 0.9f, rotation - MathHelper.PiOver2, ember.Size() / 2, stretch, SpriteEffects.None, 0);
            spriteBatch.Draw(ember.Value, position - Main.screenPosition, null, glowColor, rotation - MathHelper.PiOver2, ember.Size() / 2, stretch * 0.45f, SpriteEffects.None, 0);
        }
    }
}
