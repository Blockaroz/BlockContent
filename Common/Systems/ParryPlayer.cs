using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace BlockContent.Common.Systems
{
    public class ParryPlayer : ModPlayer
    {
        public bool parry;

        private int parryTime;

        public override bool PreItemCheck()
        {
            if (parry)
            {
                Player.TryTogglingShield(parry);

                if (Player.shieldParryTimeLeft == 1)
                    parryTime = Player.shieldParryTimeLeft;

                if (parryTime > 0 && ++parryTime > 20)
                    parryTime = 0;

                Player.shieldParryTimeLeft = parryTime;
            }

            return true;
        }
    }
}
