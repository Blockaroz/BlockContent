using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria.DataStructures;

namespace BlockContent.Nihilus.Content.NPCs
{
    public class NihilusBoss : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nihilus, Abyssal Flame");
            NPCID.Sets.TrailingMode[Type] = 0;
            NPCID.Sets.TrailCacheLength[Type] = 15;
        }

        public override void SetDefaults()
        {
            NPC.width = 120;
            NPC.height = 100;
            NPC.boss = true;
            NPC.lifeMax = 60000;
            NPC.defense = 20;
            NPC.HitSound = SoundID.DD2_CrystalCartImpact;
            NPC.DeathSound = SoundID.DD2_EtherianPortalOpen;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.SpawnWithHigherTime(50);
            NPC.knockBackResist = 0f;

            if (Main.dedServ)
                return;
        }

        public override void AI()
        {
            NPC.TargetClosestUpgraded();
            NPCAimedTarget target = NPC.GetTargetData();
            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(target.Center) * NPC.Distance(target.Center) * 0.05f, 0.1f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //underlays

            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);

            spriteBatch.Draw(texture.Value, NPC.Center - screenPos, null, Color.White, NPC.rotation, texture.Size() * 0.5f, NPC.scale, 0, 0);

            //overlays

            return false;
        }
    }
}
