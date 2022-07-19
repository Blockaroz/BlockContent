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

namespace BlockContent.Nihilus.Content.NPCs
{
    [AutoloadBossHead]
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
            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(target.Center) * (NPC.Distance(target.Center) - 200) * 0.05f, 0.02f);

            FlameParticles();
        }

        private void FlameParticles()
        {
            Vector2 flamePos = NPC.Center + NPC.velocity * 1.3f;
            Vector2 flameVel = Main.rand.NextVector2Circular(4f, 5f) - Vector2.UnitY.RotatedByRandom(0.33f) * 4 + NPC.velocity * 0.9f;
            Particle.NewParticle(Particle.ParticleType<NihilusFlameParticle>(), flamePos, flameVel, Color.White, 4f + Main.rand.NextFloat());
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //underlays

            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Asset<Texture2D> armsTex = ModContent.Request<Texture2D>(Texture + "Arms");

            NihilusFlameDrawer.DrawFlames(spriteBatch);

            for (int i = 0; i < 3; i++)
            {
                float armRot = (MathHelper.Pi / 4.5f) - (MathHelper.Pi / 1.21f * i / 3f);
                float altArmRot = MathHelper.Pi - armRot;
                Vector2 armOff = new Vector2(42 - i + (float)Math.Sin((Main.GlobalTimeWrappedHourly * 2f - i * 0.8f) % MathHelper.TwoPi) * 4f, 0).RotatedBy(armRot) * NPC.scale;
                armOff.X *= 0.8f;
                armOff.Y -= 13;
                Vector2 altArmOff = new Vector2(-armOff.X, armOff.Y);
                Rectangle armFrame = armsTex.Frame(1, 3, 0, 2 - i);
                Vector2 armOrigin = new Vector2(6, 11);
                Vector2 altArmOrigin = new Vector2(6, 11);
                spriteBatch.Draw(armsTex.Value, NPC.Center - NPC.velocity * 0.33f + armOff.RotatedBy(NPC.rotation) - screenPos, armFrame, Color.White, armRot + NPC.rotation, armOrigin, NPC.scale, 0, 0);
                spriteBatch.Draw(armsTex.Value, NPC.Center - NPC.velocity * 0.33f + altArmOff.RotatedBy(NPC.rotation) - screenPos, armFrame, Color.White, altArmRot + NPC.rotation, altArmOrigin, NPC.scale, SpriteEffects.FlipVertically, 0);
            }

            spriteBatch.Draw(texture.Value, NPC.Center - screenPos, null, Color.White, NPC.rotation, texture.Size() * 0.5f, NPC.scale, 0, 0);

            //overlays

            return false;
        }
    }
}
