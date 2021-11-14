using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.Graphics;
using Terraria.Graphics.Renderers;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContent.Content.Graphics
{
    public struct PaleBladeDrawer
    {
        private static VertexStrip _strip = new();
        private static VertexStrip _strip2 = new();

        public void Draw(Projectile proj)
        {
            MiscShaderData shader = GameShaders.Misc["BlockContent:PaleBlade"];
            shader.UseImage0("Images/Extra_195");
            shader.UseImage1("Images/Extra_197");
            shader.UseImage2("Images/Extra_194");
            shader.Apply();
            _strip.PrepareStrip(proj.oldPos, proj.oldRot, EdgeGlowColor, EdgeGlowWidth, -Main.screenPosition + (proj.Size / 2), proj.oldPos.Length, true);
            _strip.DrawTrail();
            _strip2.PrepareStrip(proj.oldPos, proj.oldRot, EdgeColor, EdgeWidth, -Main.screenPosition + (proj.Size / 2), proj.oldPos.Length, true);
            _strip2.DrawTrail();
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();

            ParticleOrchestraSettings settings = new ParticleOrchestraSettings()
            {
                PositionInWorld = proj.Center,
                MovementVector = proj.velocity * 0.02f
            };
            if (proj.localAI[0] > 30 && proj.localAI[0] < 50)
                ParticleEffects.CreatePaleSparkles(settings, Color.Gainsboro);

            CreateDust(proj);
        }

        private static Color EdgeGlowColor(float progressOnStrip) 
        {
            Color result = Color.Lerp(MoreColor.PaleGray, Color.DimGray, Utils.GetLerpValue(0.1f, 0.5f, progressOnStrip, true));
            result.A /= 2;
            return result;
        }

        private static float EdgeGlowWidth(float progressOnStrip)
        {
            float num = 1f;
            float lerpValue = Utils.GetLerpValue(0f, 0.2f, progressOnStrip, clamped: true);
            num *= 1f - (1f - lerpValue) * (1f - lerpValue);
            //float num = Utils.GetLerpValue(0, 0.1f, progressOnStrip, true) * Utils.GetLerpValue(1, 0.2f, progressOnStrip);
            return MathHelper.SmoothStep(0f, 36f, num);
        }

        private static Color EdgeColor(float progressOnStrip)
        {
            Color result = Color.Lerp(Color.GhostWhite, MoreColor.PaleGray, Utils.GetLerpValue(0.3f, 1f, progressOnStrip, true));
            result.A /= 2;
            return result;
        }

        private static float EdgeWidth(float progressOnStrip)
        {
            float num = 1f;
            float lerpValue = Utils.GetLerpValue(0f, 0.2f, progressOnStrip, clamped: true);
            num *= 1f - (1f - lerpValue) * (1f - lerpValue);
            //float num = Utils.GetLerpValue(0, 0.1f, progressOnStrip, true) * Utils.GetLerpValue(1, 0.3f, progressOnStrip);
            return MathHelper.SmoothStep(0f, 21f, num);
        }

        private void CreateDust(Projectile proj)
        {
            Vector2 centerPosition = proj.Center + (proj.rotation - MathHelper.PiOver2).ToRotationVector2() * MathHelper.Lerp(0.5f, 3f, Main.rand.NextFloat());
            if (proj.localAI[0] < 50 && Main.rand.Next(4) == 0)
            {
                Color dustColor = Color.Lerp(MoreColor.PaleGray, Color.DimGray, Main.rand.NextFloat(0, 1));
                dustColor.A = 51;
                Dust dust = Dust.NewDustPerfect(centerPosition + Main.rand.NextVector2Square(-1, 1), ModContent.DustType<Dusts.GlowballDust>(), null, 100, dustColor, 1.2f);
                dust.fadeIn = 1f + Main.rand.NextFloat(-1, 1) * 0.6f;
                dust.noGravity = true;
                dust.velocity += Main.rand.NextVector2Circular(2, 2);
                dust.velocity += new Vector2(2, 0).RotatedBy(proj.rotation);
                dust.noLightEmittence = true;
            }
        }
    }
}
