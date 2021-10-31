﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContent.Content.Dusts
{
    public class GlowballDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.scale *= 1.1f;
            dust.noGravity = true;
        }

        public override void SetStaticDefaults()
        {
            UpdateType = DustID.RainbowRod;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor) => dust.color;
    }
}
