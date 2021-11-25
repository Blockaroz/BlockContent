using BlockContent.Content.Skies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.Dyes;
using Terraria.GameContent.Shaders;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace BlockContent.Content.Graphics
{
    public static class EffectLoader
    {
        private static Mod Mod
        {
            get { return ModContent.GetInstance<BlockContent>(); }
        }

        public static void LoadEffects()
        {
            LoadShaders();
            LoadSkies();
        }

        public static void LoadSkies()
        {
            SkyManager.Instance["BlockContent:NightEmpressSky"] = new NightEmpressSky();
        }

        public static void LoadShaders()
        {
            GameShaders.Misc["BlockContent:PaleBlade"] = new MiscShaderData(Main.VertexPixelShaderRef, "MagicMissile").UseProjectionMatrix(doUse: true);

            GameShaders.Misc["BlockContent:Grayscale"] = new MiscShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Assets/Effects/Grayscale").Value), "Grayscale");
            
            GameShaders.Misc["BlockContent:NightEmpressWings"] = new MiscShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Assets/Effects/NightEmpressWings").Value), "NightEmpressWings");
            GameShaders.Misc["BlockContent:NightEmpressWings"].UseImage0("Images/Extra_156");
            Asset<Texture2D> wingsTexture = Mod.Assets.Request<Texture2D>("Assets/Textures/NightEmpress/NightEmpress_WingsShader");
            //GameShaders.Misc["BlockContent:NightEmpressWings"].Shader.GraphicsDevice.Textures[0] = wingsTexture.Value;
            //GameShaders.Misc["BlockContent:NightEmpressWings"].Shader.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            //GameShaders.Misc["BlockContent:NightEmpressWings"].Shader.Parameters["uImageSize0"].SetValue(wingsTexture.Size());

        }
    }
}
