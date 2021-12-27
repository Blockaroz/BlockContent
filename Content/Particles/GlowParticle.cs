﻿using BlockContent.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace BlockContent.Content.Particles
{
    public class GlowParticle : Particle
    {
        private float modifScale = 1f;

        public override void Update()
        {
            velocity *= 0.96f;
            rotation += velocity.X * 0.1f;
            misc++;
            if (misc > 50)
                Active = false;
            modifScale = ExtraUtils.DualLerp(0, 5, 30, 50, misc, true) * scale;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Asset<Texture2D> glow = Mod.Assets.Request<Texture2D>("Content/Particles/GlowParticle");
            color.A /= 2;
            Color inColor = Color.Lerp(Color.White, color, 0.1f);
            inColor.A = 0;
            spriteBatch.Draw(glow.Value, position - Main.screenPosition, null, color, rotation, glow.Size() / 2, modifScale, SpriteEffects.None, 0);
            spriteBatch.Draw(glow.Value, position - Main.screenPosition, null, inColor, rotation, glow.Size() / 2, modifScale * 0.4f, SpriteEffects.None, 0);
        }
    }
}
