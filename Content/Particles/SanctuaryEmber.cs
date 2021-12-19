using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using BlockContent.Core;
using Terraria;
using Terraria.ModLoader;

namespace BlockContent.Content.Particles
{
    public class SanctuaryEmber : Particle
    {
        public override void Update()
        {
            misc++;
            velocity *= 0.98f;
            rotation = velocity.ToRotation();
            if (Collision.SolidCollision(position - new Vector2(2), 4, 4))
            {
                velocity = Vector2.Zero;
                scale *= 0.21f;
            }

            scale *= 0.94f;
            if (scale < 0.01f)
                Active = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Color glowColor = Color2.Sanguine;
            glowColor.A = 80;
            color.A = 0;

            Asset<Texture2D> ember = Mod.Assets.Request<Texture2D>("Content/Particles/Ember");

            spriteBatch.Draw(ember.Value, position - Main.screenPosition, null, glowColor, rotation - MathHelper.PiOver2, ember.Size() / 2, scale, SpriteEffects.None, 0);
            spriteBatch.Draw(ember.Value, position - Main.screenPosition, null, color, rotation - MathHelper.PiOver2, ember.Size() / 2, new Vector2(scale * 0.2f, scale * 0.4f), SpriteEffects.None, 0);
        }
    }
}
