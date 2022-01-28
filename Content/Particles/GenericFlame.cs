using System;
using Terraria;
using Microsoft.Xna.Framework;
using BlockContent.Core;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace BlockContent.Content.Particles
{
    public class GenericFlame : Particle
    {
        private Point particleFrame;

        public override void OnSpawn()
        {
            particleFrame = new Point(Main.rand.Next(3), Main.rand.Next(2));
            rotation = Main.rand.NextFloat() * MathHelper.TwoPi;
        }

        public override void Update()
        {
            velocity.Y -= 0.01f;
            velocity *= 0.98f;
            rotation += velocity.X * 0.07f;

            misc++;
            if (misc > 4)
                scale *= 0.82f;
            if (scale < 0.3f)
                active = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Asset<Texture2D> texture = Mod.Assets.Request<Texture2D>("Content/Particles/GenericFlame");
            Rectangle texFrame = texture.Frame(4, 3, particleFrame.X, particleFrame.Y);
            color.A = 0;
            Color drawColor = Color.Lerp(color, Color.Black * 0.1f, misc / 5f) * Utils.GetLerpValue(7, 3, misc, true);
            spriteBatch.Draw(texture.Value, position - Main.screenPosition, texFrame, drawColor, rotation, texFrame.Size() / 2, scale, SpriteEffects.None, 0);
        }
    }
}
