using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace BlockContent.Content.Graphics
{
    public struct HolyBladeDrawer
    {
        Mod Mod
        {
            get => ModContent.GetInstance<BlockContent>();
        }

        private static VertexStrip trail = new VertexStrip();

        public void Draw(Projectile projectile)
        {
            MiscShaderData shader = GameShaders.Misc["Blockaroz:HolyBlade"];
            shader.UseShaderSpecificData(new Vector4(1, 0, 0, 0.6f));
            shader.UseImage0("Images/Extra_209");
            shader.UseImage1("Images/Misc/noise");
            shader.Apply();
            trail.PrepareStrip(projectile.oldPos, projectile.oldRot, ColorFunc, Width, -Main.screenPosition + (projectile.Size / 2), projectile.oldPos.Length, true);
            trail.DrawTrail();
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
        }

        public float Width(float progressOnStrip)
        {
            return 40f;
        }

        public Color ColorFunc(float progressOnStrip)
        {
            Color color = Color2.HolyMelee;
            color.A = 0;
            return color;
        }
    }
}
