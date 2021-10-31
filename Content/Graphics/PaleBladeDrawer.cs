using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContent.Content.Graphics
{
    public struct PaleBladeDrawer
    {
        private static VertexStrip _strip = new();
        private static VertexStrip _strip2 = new();

        public void Draw(Projectile projectile)
        {
            MiscShaderData shader = GameShaders.Misc["BlockContent:PaleBlade"];
            shader.UseImage0("Images/Extra_197");
            shader.UseImage1("Images/Extra_197");
            shader.UseImage2("Images/Extra_194");
            shader.Apply();
            _strip.PrepareStrip(projectile.oldPos, projectile.oldRot, EdgeGlowColor, EdgeGlowWidth, -Main.screenPosition + (projectile.Size / 2), projectile.oldPos.Length, true);
            _strip.DrawTrail();
            _strip2.PrepareStrip(projectile.oldPos, projectile.oldRot, EdgeColor, EdgeWidth, -Main.screenPosition + (projectile.Size / 2), projectile.oldPos.Length, true);
            _strip2.DrawTrail();
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();

            Player player = Main.player[projectile.owner];
            StripDust(projectile.localAI[0], projectile.Center + (projectile.rotation - MathHelper.PiOver2).ToRotationVector2() * MathHelper.Lerp(0.5f, 3f, Main.rand.NextFloat()), projectile.rotation);
        }

        private static Color EdgeGlowColor(float progressOnStrip) 
        {
            Color result = Color.Lerp(MoreColor.PaleGray, Color.DimGray, Utils.GetLerpValue(0.1f, 0.5f, progressOnStrip, true));
            result.A = 200;
            return result;
        }

        private static float EdgeGlowWidth(float progressOnStrip)
        {
            float num = Utils.GetLerpValue(0, 0.1f, progressOnStrip, true) * Utils.GetLerpValue(1, 0.2f, progressOnStrip);
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
            float num = Utils.GetLerpValue(0, 0.1f, progressOnStrip, true) * Utils.GetLerpValue(1, 0.3f, progressOnStrip);
            return MathHelper.SmoothStep(0f, 21f, num);
        }

        private void StripDust(float t, Vector2 centerPosition, float rotation)
        {
            if (t < 50 && Main.rand.Next(1) == 0)
            {
                Color dustColor = Color.Lerp(MoreColor.PaleGray, Color.DimGray, Main.rand.NextFloat(0, 1));
                dustColor.A = 51;
                Dust dust = Dust.NewDustPerfect(centerPosition + Main.rand.NextVector2Square(-1, 1), ModContent.DustType<Dusts.GlowballDust>(), null, 100, dustColor, 1f);
                dust.fadeIn = 1f + Main.rand.NextFloat(-1, 1) * 0.6f;
                dust.noGravity = true;
                dust.velocity += Main.rand.NextVector2Circular(2, 2);
                dust.velocity += new Vector2(2, 0).RotatedBy(rotation);
                dust.noLightEmittence = true;
            }
        }
    }
}
