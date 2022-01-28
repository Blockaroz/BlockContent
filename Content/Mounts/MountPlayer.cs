using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace BlockContent.Content.Mounts
{
    public class MountPlayer : ModPlayer
    {
        public bool exosuit;

        //specific suits
        public bool protonaut;

        public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
        {
            if (exosuit)
            {
            }
        }

        public override void ResetEffects()
        {
            exosuit = false;
            protonaut = false;
        }
    }
}
