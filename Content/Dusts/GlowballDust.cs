﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace BlockContent.Content.Dusts
{
    public class GlowballDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.scale *= 1.2f;
            dust.noGravity = true;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor) => dust.color;
    }
}
