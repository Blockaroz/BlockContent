using System;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace BlockContent.SonsAndDaughters
{
    public class RedFilter : ModSystem
    {
        public static bool Active;

        public override void Load()
        {
            Filters.Scene["Blockaroz:SonsAndDaughters"] = new Filter(
                new ScreenShaderData(Main.ScreenShaderRef, "FilterMiniTower")
                .UseColor(1.1f, 0.3f, 0.3f)
                .UseOpacity(0.65f), EffectPriority.VeryHigh);
            Filters.Scene["Blockaroz:SonsAndDaughters"].Load();
        }

        private void DoFilter()
        {
            if (Active)
            {
                if (!Filters.Scene["Blockaroz:SonsAndDaughters"].Active)
                    Filters.Scene.Activate("Blockaroz:SonsAndDaughters");
            }
            else if (Filters.Scene["Blockaroz:SonsAndDaughters"].Active)
                Filters.Scene["Blockaroz:SonsAndDaughters"].Deactivate();
        }

        public override void PostUpdateProjectiles()
        {
            DoFilter();
        }
    }
}
