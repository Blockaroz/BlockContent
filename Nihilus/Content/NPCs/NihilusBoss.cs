using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria.DataStructures;
using ParticleEngine;
using BlockContent.Nihilus.Content.Particles;

namespace BlockContent.Nihilus.Content.NPCs
{
    [AutoloadBossHead]
    public partial class NihilusBoss : ModNPC
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
            NPC.lifeMax = 80000;
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
            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(target.Center) * (NPC.Distance(target.Center) - 200) * 0.05f, 0.02f);

            FlameParticles();

            lookingDirection = NPC.DirectionTo(target.Center);
        }

        private Vector2 lookingDirection;

        private void FlameParticles()
        {
            for (int i = 0; i < 2; i++)
            {
                Vector2 flamePos = NPC.Center + Vector2.UnitY * 22 + NPC.velocity * 2f;
                float rand = Main.rand.NextFloat(5f);
                Vector2 flameVel = Main.rand.NextVector2Circular(4f, 3f) - Vector2.UnitY.RotatedByRandom(rand * 0.25f).RotatedBy(NPC.velocity.ToRotation() * 0.05f) * Math.Abs(6 - rand) + NPC.velocity * 0.9f;
                Particle.NewParticle(Particle.ParticleType<AbyssalFlame>(), flamePos, flameVel, Color.White, 4f + Main.rand.NextFloat(1.5f));
            }

            if (Main.rand.NextBool())
            {
                Vector2 emberPos = NPC.Center + Main.rand.NextVector2Circular(100, 100);
                Vector2 emberVel = emberPos.DirectionFrom(NPC.Center + Vector2.UnitY * 60) * Main.rand.NextFloat(5f);
                emberVel.Y *= 0.6f;
                Particle.NewParticle(Particle.ParticleType<AbyssalEmber>(), emberPos, emberVel, DragonFruit, Main.rand.NextFloat(2f));
            }
        }

        public static readonly Color DragonFruit = new Color(234, 56, 100);
    }
}
