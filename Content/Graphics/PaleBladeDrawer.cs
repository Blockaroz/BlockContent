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
            _strip.PrepareStrip(projectile.oldPos, projectile.oldRot, EdgeGlowColor, EdgeGlowWidth, -Main.screenPosition + (projectile.Size / 2),  projectile.oldPos.Length, true);
            _strip.DrawTrail();
            _strip2.PrepareStrip(projectile.oldPos, projectile.oldRot, EdgeColor, EdgeWidth, -Main.screenPosition + (projectile.Size / 2), projectile.oldPos.Length, true);
            _strip2.DrawTrail();
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();

            Player player = Main.player[projectile.owner];
            StripDust(projectile.localAI[0], projectile.Center + (projectile.rotation - MathHelper.PiOver2).ToRotationVector2() * MathHelper.Lerp(0.5f, 3f, Main.rand.NextFloat()), projectile.rotation - MathHelper.PiOver2 + MathHelper.PiOver2 * projectile.spriteDirection, player.velocity);
        }

        private static Color EdgeGlowColor(float progressOnStrip) 
        {
            Color result = Color.Lerp(Color.DimGray, Color.DarkSlateGray, Utils.GetLerpValue(0.2f, 1f, progressOnStrip, true));
            result.A /= 2;
            return result;
        }

        private static float EdgeGlowWidth(float progressOnStrip)
        {
            float num = Utils.GetLerpValue(0, 0.1f, progressOnStrip, true) * Utils.GetLerpValue(1, 0.2f, progressOnStrip);
            return MathHelper.SmoothStep(0f, 48f, num);
        }

        private static Color EdgeColor(float progressOnStrip)
        {
            Color result = Color.Lerp(Color.White, Color.GhostWhite, Utils.GetLerpValue(0.2f, 1f, progressOnStrip, true));
            result.A /= 2;
            return result;
        }

        private static float EdgeWidth(float progressOnStrip)
        {
            float num = Utils.GetLerpValue(0, 0.1f, progressOnStrip, true) * Utils.GetLerpValue(1, 0.2f, progressOnStrip);
            return MathHelper.SmoothStep(0f, 16f, num);
        }

        private void StripDust(float t, Vector2 centerPosition, float rotation, Vector2 velocity)
        {
            int random = Main.rand.Next(1, 3);
            if (t < 65)
            {
                for (int i = 0; i < random; i++)
                {
                    Color dustColor = Color.Lerp(Color.Gainsboro, Color.DimGray, Main.rand.NextFloat());
                    dustColor.A = 51;
                    Dust dust = Dust.NewDustPerfect(centerPosition + Main.rand.NextVector2Square(-1, 1), ModContent.DustType<Dusts.SparkleDust>(), null, 100, dustColor, 1f);
                    dust.fadeIn = 0.8f + Main.rand.NextFloat(-1, 1) * 0.6f;
                    dust.noGravity = true;
                    dust.velocity += Main.rand.NextVector2Circular(2, 2);
                    dust.noLightEmittence = true;
                }
            }
        }
    }
}
