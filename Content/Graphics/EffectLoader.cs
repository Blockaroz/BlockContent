using Terraria.GameContent.Shaders;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;

namespace BlockContent.Content.Graphics
{
    public static class EffectLoader
    {
        public static void LoadEffects()
        {
            SkyManager.Instance["BlockContent:ChadSky"] = new ChadEffectSky();
        }
    }
}
