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
                if (!SkyManager.Instance["BlockContent:NightEmpressSky"].IsActive())
                    SkyManager.Instance.Activate("BlockContent:NightEmpressSky", default(Vector2));
            }
            else if (SkyManager.Instance["BlockContent:NightEmpressSky"].IsActive())
                SkyManager.Instance.Deactivate("BlockContent:NightEmpressSky");
        }
    }
}
