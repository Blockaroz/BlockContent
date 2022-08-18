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
    public class ImpactParry : Particle
    {
        private List<float> rotations;
        private List<float> scales;
        private float life;

        public override void OnSpawn()
        {
            rotations = new List<float>();
            scales = new List<float>();
            for (int i = 0; i < Main.rand.Next(4, 5); i++)
            {
                rotations.Add(velocity.RotatedByRandom(2f).ToRotation());
                scales.Add(Main.rand.NextFloat(1f, 2f));
            }
            life = 0f;
            rotation = velocity.ToRotation();
        }

        public override void Update()
        {
            if (data is Entity offset)
                position += (offset.position - offset.oldPosition);

            life++;
            velocity *= 0;
            if (life > 20)
                Active = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Asset<Texture2D> spark = TextureAssets.Extra[98];
            Asset<Texture2D> ring = TextureAssets.Extra[174];

            Color glowColor = color;
            glowColor.A = 0;

            float ringScale = (float)Math.Pow(life / 30f, 1.75f) * scale * 5f;
            float ringFade = (float)Math.Sqrt(Utils.GetLerpValue(16, 2, life, true));
            Main.EntitySpriteDraw(ring.Value, position - Main.screenPosition, null, glowColor * ringFade * 0.7f, rotation, ring.Size() * 0.5f, new Vector2(0.3f, 1f) * ringScale, 0, 0);

            for (int i = 0; i < rotations.Count; i++)
            {
                Vector2 sparkScale = new Vector2(Utils.GetLerpValue(15, 5, life, true) * 1.5f, Utils.GetLerpValue(0, 5, life, true) * scales[i] * 2f) * scale;
                Main.EntitySpriteDraw(spark.Value, position - Main.screenPosition, null, glowColor * MathHelper.Clamp(scales[i], 0, 1), rotations[i], spark.Size() * 0.5f, sparkScale, 0, 0);
                Main.EntitySpriteDraw(spark.Value, position - Main.screenPosition, null, new Color(255, 255, 255, 0), rotations[i], spark.Size() * 0.5f, sparkScale * 0.6f, 0, 0);
            }
        }
    }
}
