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
            if (proj.localAI[0] > 20 && proj.localAI[0] < 60)
                ParticleEffects.CreatePaleSpeckles(settings);
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
    }
}
