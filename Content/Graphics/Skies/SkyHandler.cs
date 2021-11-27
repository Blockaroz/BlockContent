using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace BlockContent.Content.Graphics.Skies
{
    public class SkyHandler : ModSystem
    {
        public override void PostUpdateNPCs()
        {
            if (NPC.AnyNPCs(ModContent.NPCType<NPCs.NightEmpressBoss.NightEmpress>()))
            {
                if (!SkyManager.Instance["Blockaroz:NightEmpressSky"].IsActive())
                    SkyManager.Instance.Activate("Blockaroz:NightEmpressSky", default(Vector2));
            }
            else if (SkyManager.Instance["Blockaroz:NightEmpressSky"].IsActive())
                SkyManager.Instance.Deactivate("Blockaroz:NightEmpressSky");
        }
    }
}
