using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleEngine;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace BlockContent.Content.Particles
{
    public class CinderFlame : Particle
    {
        private int frame;
        private float time;

        public override void OnSpawn()
        {
            frame = Main.rand.Next(8);
            time = 1f + Main.rand.NextFloat(0.1f);
        }

        public override void Update()
        {
            scale *= 0.98f;
            velocity *= 1.01f;
            velocity += Main.rand.NextVector2Circular(0.1f, 0.1f);
            time *= 0.96f;
            if (time < 0.9f)
                velocity.Y -= 0.05f;
            rotation = velocity.ToRotation() - MathHelper.PiOver2;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Asset<Texture2D> flame = ModContent.Request<Texture2D>(Texture);
            Rectangle flameRec = flame.Frame(1, 9, 0, frame);

            Color flameColor = color;
            flameColor.A = 0;
            Color drawColor = Color.Lerp(flameColor, new Color(10, 10, 10, 40), Utils.GetLerpValue(0.7f, 0.33f, time, true)) * Utils.GetLerpValue(0.3f, 0.5f, time, true) * Utils.GetLerpValue(1f, 0.9f, time, true);
            Vector2 fadeScale = new Vector2(1.3f, 1f + Utils.GetLerpValue(0.75f, 0f, time, true)) * scale;

            spriteBatch.Draw(flame.Value, position - Main.screenPosition, flameRec, drawColor, rotation, flameRec.Size() * 0.5f, fadeScale, SpriteEffects.None, 0);
        }
    }
}
