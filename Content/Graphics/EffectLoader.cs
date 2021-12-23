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
            SkyManager.Instance["Blockaroz:NightEmpressSky"] = new NightEmpressSky();
        }

        public static void LoadShaders()
        {
            GameShaders.Misc["Blockaroz:HolyBlade"] = new MiscShaderData(Main.VertexPixelShaderRef, "FinalFractalVertex").UseProjectionMatrix(true); 

            //Ref<Effect> grayscaleRef = new Ref<Effect>(Mod.Assets.Request<Effect>("Assets/Effects/Grayscale").Value);
            //GameShaders.Misc["BlockContent:Grayscale"] = new MiscShaderData(grayscaleRef, "Grayscale");
            GameShaders.Misc["Blockaroz:Sanctuary"] = new MiscShaderData(Main.PixelShaderRef, "ArmorWisp").UseColor(Color2.Sanguine).UseSecondaryColor(Color.DarkRed);

            //Ref<Effect> nightRef = new Ref<Effect>(Mod.Assets.Request<Effect>("Assets/Effects/NightEmpress").Value);
            GameShaders.Misc["Blockaroz:NightEmpress"] = new MiscShaderData(Main.PixelShaderRef, "HallowBoss").UseImage0("Images/Extra_156");
            //Asset<Texture2D> wingsTexture = Mod.Assets.Request<Texture2D>("Assets/Textures/NightEmpress/NightEmpress_WingsShader");
            //GameShaders.Misc["BlockContent:NightEmpress"].Shader.GraphicsDevice.Textures[0] = wingsTexture.Value;
            //GameShaders.Misc["BlockContent:NightEmpress"].Shader.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            //GameShaders.Misc["BlockContent:NightEmpress"].Shader.Parameters["uImageSize0"].SetValue(wingsTexture.Size());

        }
    }
}
