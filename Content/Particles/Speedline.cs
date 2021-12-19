using BlockContent.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace BlockContent.Content.Particles
{
    public class Speedline : Particle
    {
        public override void Update()
        {
            velocity *= 1.05f;
            speed = ExtraUtils.DualLerp(0, 2, 5, 8, misc, true);
            misc++;
            if (misc > 8)
                Active = false;
        }

        private float speed = 0;

        public override void Draw(SpriteBatch spriteBatch)
        {
            Asset<Texture2D> line = Mod.Assets.Request<Texture2D>("Content/Particles/Speedline");
            spriteBatch.Draw(line.Value, position - Main.screenPosition, null, color, rotation, new Vector2(0, line.Height() / 2), new Vector2(speed * scale, 0.5f), SpriteEffects.None, 0);
        }
    }
}
