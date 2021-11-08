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
            SkyManager.Instance["BlockContent:SubtleDarkSky"] = new NightSky();
        }

        public static void LoadShaders()
        {
            GameShaders.Misc["BlockContent:PaleBlade"] = new MiscShaderData(Main.VertexPixelShaderRef, "MagicMissile").UseProjectionMatrix(doUse: true);

            GameShaders.Misc["BlockContent:Test"] = new MiscShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Assets/Effects/Grayscale", AssetRequestMode.ImmediateLoad).Value), "GrayscaleFunction");
        
        }
    }
}
