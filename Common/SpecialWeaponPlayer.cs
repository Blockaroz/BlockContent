using BlockContent.Content.Graphics;
using BlockContent.Content.Particles;
using BlockContent.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContent.Common
{
    public class SpecialWeaponPlayer : ModPlayer
    {
        public int sanctuaryMode;

        public override void ResetEffects()
        {
            sanctuaryMode = 0;
        }
    }
}
