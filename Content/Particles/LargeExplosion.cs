using System;
using Terraria;
using BlockContent.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace BlockContent.Content.Particles
{
    public class LargeExplosion : Particle
    {
        public override void OnSpawn()
        {
            scale *= Main.rand.NextFloat(0.5f, 1.2f);
            rotation *= 0.1f;
            frame = 0;
            frameCounter = 0;
        }

        private int frame;
        private int frameCounter;

        public override void Update()
        {
            velocity *= 0.8f;
            frameCounter++;
            if (frameCounter > 3)
            {
                frame++;
                frameCounter = 0;
            }
            if (frame >= 6)
                Active = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Asset<Texture2D> explosion = Mod.Assets.Request<Texture2D>("Content/Particles/LargeExplosion");
            Rectangle rect = explosion.Frame(1, 6, 0, frame);
            spriteBatch.Draw(explosion.Value, position - Main.screenPosition, rect, color, rotation, rect.Size() * 0.5f, scale, SpriteEffects.None, 0);
        }
    }
}
