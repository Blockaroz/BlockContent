using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleEngine;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace BlockContent.Nihilus.Content.Particles
{
    public class AbyssalEmber : Particle
    {
        public override void OnSpawn()
        {
            life = 1f;
            rotation = velocity.ToRotation();
        }

        public float life;

        public override void Update()
        {
            life *= 0.98f;
            if (life < 0.89f)
                life *= 0.95f;

            if (life < 0.1f)
                Active = false;

            rotation = velocity.ToRotation() - MathHelper.PiOver2;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Asset<Texture2D> tex = ModContent.Request<Texture2D>(Texture);
            float up = Utils.GetLerpValue(1f, 0.98f, life, true);
            Vector2 stretch = new Vector2(1f, 0.8f + velocity.Length() * 0.2f) * scale * up * life;
            Color baseColor = color;
            Color glowColor = Color.Lerp(Color.White, color, 0.5f);
            baseColor.A = 0;
            glowColor.A = 0;

            spriteBatch.Draw(tex.Value, position - Main.screenPosition, null, baseColor, rotation, tex.Size() * 0.5f, stretch, 0, 0);
            spriteBatch.Draw(tex.Value, position - Main.screenPosition, null, glowColor, rotation, tex.Size() * 0.5f, stretch * 0.5f, 0, 0);
        }
    }
}
