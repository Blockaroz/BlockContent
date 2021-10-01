using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContent.Content.NPCs
{
    public class AchromaticAbomination : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Achromatic Abomination");
            NPCID.Sets.CountsAsCritter[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 10;
            NPC.noGravity = true;
        }
        public override void OnKill()
        {
            NPC.SpawnBoss((int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<NightEmpressBoss.NightEmpress>(), 0);
        }
    }
}
