using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleEngine;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace BlockContent.Content.Particles
{
    public class ImpactSpark : Particle
    {
        private List<float> rotations;
        private List<float> scales;
        private float life;

        public override void OnSpawn()
        {
            rotations = new List<float>();
            scales = new List<float>();
            for (int i = 0; i < Main.rand.Next(4, 6); i++)
            {
                rotations.Add(velocity.RotatedByRandom(1f).ToRotation());
                scales.Add(Main.rand.NextFloat(0.3f, 3f));
            }
            life = 0f;
        }

        public override void Update()
        {
            if (data is Entity offset)
                position += (offset.position - offset.oldPosition);

            life++;
            velocity *= 0;
            if (life > 10)
                Active = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Asset<Texture2D> spark = TextureAssets.Extra[98];

            Color sparkColor = color;
            sparkColor.A = 0;
            Color glowColor = Color.Lerp(Color.White, sparkColor, 0.5f);
            glowColor.A = 0;
            for (int i = 0; i < rotations.Count; i++)
            {
                Vector2 sparkScale = new Vector2(Utils.GetLerpValue(8, 2, life, true) * 1.4f, Utils.GetLerpValue(0, 5, life, true) * scales[i] * 2f) * scale;
                Main.EntitySpriteDraw(spark.Value, position - Main.screenPosition, null, sparkColor * MathHelper.Clamp(scales[i], 0, 1), rotations[i], spark.Size() * 0.5f, sparkScale, 0, 0);
                Main.EntitySpriteDraw(spark.Value, position - Main.screenPosition, null, glowColor* MathHelper.Clamp(scales[i], 0, 1), rotations[i], spark.Size() * 0.5f, sparkScale * 0.3f, 0, 0);
            }
        }
    }
}
