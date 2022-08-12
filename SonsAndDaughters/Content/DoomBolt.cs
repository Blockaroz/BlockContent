using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleEngine;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace BlockContent.SonsAndDaughters.Content
{
    public class DoomBolt : Particle
    {
        public override string Texture => $"{nameof(BlockContent)}/Assets/Textures/Glow";

        private float life;
        private List<Vector2> points;
        private List<Vector2> offsets;

        public override void OnSpawn()
        {
            scale *= Main.rand.NextFloat(1f, 1.1f);
            velocity *= 0;
            life = 1f;
            offsets = new List<Vector2>();
        }

        public override void Update()
        {
            if (data is List<Vector2> list)
            {
                if (life == 1f)
                {
                    points = list;
                    for (int i = 0; i < points.Count; i++)
                        offsets.Add(Main.rand.NextVector2Circular(1, 1));
                }
                for (int i = 1; i < points.Count - 1; i++)
                    points[i] += offsets[i] * points.Count * 0.03f;
            }
            life *= 0.97f;
            scale *= 0.97f;

            if (life < 0.8f)
                life *= 0.8f;

            if (life < 0.12f)
                Active = false;

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Color drawColor = color;
            drawColor.A /= 2;
            if (data is List<Vector2> points)
            {
                List<float> rotations = new List<float>();
                for (int i = 0; i < points.Count - 1; i++)
                    rotations.Add(points[i].AngleTo(points[i + 1]));

                rotations.Add(points[0].AngleTo(points[points.Count - 1]));

                Effect lightningShader = ModContent.Request<Effect>($"{nameof(BlockContent)}/Assets/Effects/TextureShader", AssetRequestMode.ImmediateLoad).Value;
                lightningShader.Parameters["uTexture"].SetValue(TextureAssets.BlackTile.Value);
                lightningShader.Parameters["uColor"].SetValue(drawColor.ToVector4());
                lightningShader.Parameters["uProgress"].SetValue(life * scale);
                lightningShader.CurrentTechnique.Passes[0].Apply();

                //Main.pixelShader.CurrentTechnique.Passes[0].Apply();
                VertexStrip strip = new VertexStrip();
                strip.PrepareStrip(points.ToArray(), rotations.ToArray(), ColorFunction, WidthFunction, -Main.screenPosition, points.Count, true);
                strip.DrawTrail();
            }
        }

        private Color ColorFunction(float progress) => Color.Red;

        private float WidthFunction(float progress) => 20f * life * scale * (progress * (1f - progress)) * Utils.GetLerpValue(1f, 0.97f, life + (progress * 0.04f), true);
    }
}
