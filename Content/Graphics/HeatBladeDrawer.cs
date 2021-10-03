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
    public struct HeatBladeDrawer
    {
        private static VertexStrip _edgeGlowStrip = new();
        private static VertexStrip _edgeStrip = new();

        public void Draw(Projectile projectile)
        {
            MiscShaderData shader = GameShaders.Misc["BlockContent:OctaneBlade"];
            shader.UseSaturation(-3);
            shader.UseImage0("Images/Extra_191");
            shader.UseImage1("Images/Extra_197");
            shader.UseImage2("Images/Extra_197");
            shader.Apply();

            _edgeGlowStrip.PrepareStripWithProceduralPadding(projectile.oldPos, projectile.oldRot, EdgeGlowColor, EdgeWidth, -Main.screenPosition + (projectile.Size / 2), true);
            _edgeGlowStrip.DrawTrail();

            _edgeStrip.PrepareStripWithProceduralPadding(projectile.oldPos, projectile.oldRot, EdgeColor, ThinEdgeWidth, -Main.screenPosition + (projectile.Size / 2), true);
            _edgeStrip.DrawTrail();

            Main.pixelShader.CurrentTechnique.Passes[0].Apply();

            Player player = Main.player[projectile.owner];

            StripDust(projectile.localAI[0], projectile.Center + (projectile.rotation - MathHelper.PiOver2).ToRotationVector2() * MathHelper.Lerp(0.5f, 1f, Main.rand.NextFloat()), projectile.rotation - MathHelper.PiOver2 + MathHelper.PiOver2 * projectile.spriteDirection, player.velocity);
        }

        private static Color EdgeGlowColor(float progressOnStrip) 
        {
            Color result = Color.Lerp(new(253, 73, 53), new(252, 28, 37), Utils.GetLerpValue(0.2f, 1f, progressOnStrip, true));
            result.A = 198;
            return result;
        }

        private static Color EdgeColor(float progressOnStrip)
        {
            Color result = Color.Lerp(Color.Goldenrod, new(255, 213, 46), Utils.GetLerpValue(0.3f, 1f, progressOnStrip, true));
            result.A /= 2;
            return result;
        }

        private static float EdgeWidth(float progressOnStrip)
        {
            float num = 1f;
            float lerpValue = Utils.GetLerpValue(0f, 0.2f, progressOnStrip, clamped: true);
            num *= 1f - (1f - lerpValue) * (1f - lerpValue);
            return MathHelper.SmoothStep(0f, 40f, num);
        }
        private static float ThinEdgeWidth(float progressOnStrip) 
        {
            float num = 1f;
            float lerpValue = Utils.GetLerpValue(0f, 0.2f, progressOnStrip, clamped: true);
            num *= 1f - (1f - lerpValue) * (1f - lerpValue);
            return MathHelper.SmoothStep(0f, 18f, num);
        }

        private void StripDust(float t, Vector2 centerPosition, float rotation, Vector2 velocity)
        {
            Color orng = new Color(255, 213, 46);
            int num = Main.rand.Next(1, 4);
            if (t < 65)
            {
                for (int i = 0; i < num; i++)
                {
                    Dust dust = Dust.NewDustPerfect(centerPosition, DustID.AncientLight, null, 100, Color.Lerp(orng, Color.DarkRed, Main.rand.NextFloat() * 0.3f));
                    dust.scale = 0.4f;
                    dust.fadeIn = 0.4f + Main.rand.NextFloat() * 0.3f;
                    dust.noGravity = true;
                    dust.velocity += rotation.ToRotationVector2() * (3f + Main.rand.NextFloat() * 4f);
                    dust.noLightEmittence = true;
                }
            }
        }
    }
}
