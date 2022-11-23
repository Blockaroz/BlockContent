using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContent.Content.Bosses.Plantera
{
    public class Plantera : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plantera");
            NPCID.Sets.ShouldBeCountedAsBoss[Type] = true;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCDebuffImmunityData debuffData = new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] {
                    BuffID.Poisoned,

                    BuffID.Confused // Most NPCs have this
				}
            };
            NPCID.Sets.DebuffImmunitySets.Add(Type, debuffData);
        }

        public override void SetDefaults()
        {
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.hide = true;
            NPC.aiStyle = -1;
            NPC.width = 80;
            NPC.height = 80;
            NPC.SpawnWithHigherTime(40);
            NPC.boss = true;
            NPC.npcSlots = 20f;

            NPC.damage = 60;
            NPC.defense = 24;
            NPC.lifeMax = 4500;
            NPC.knockBackResist = 0f;
            NPC.dontTakeDamage = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;

            if (!Main.dedServ)
                Music = MusicID.Plantera;

        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            position += new Vector2(0, 30);
            return true;
        }

        public bool Enraged { get; set; }

        public ref float Phase => ref NPC.ai[0];
        public ref float Timer => ref NPC.ai[1];

        private bool PhaseII { get; set; }

        public ref float HeadValue => ref NPC.localAI[0];

        private float JawQuiver => 0.2f + (float)Math.Sin(NPC.localAI[1] * 0.15f) * 0.02f;
        private bool hasChomped;

        private float speed;
        private float slide;
        private float follow;

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(speed);
            writer.Write(slide);
            writer.Write(follow);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            speed = reader.ReadSingle();
            slide = reader.ReadSingle();
            follow = reader.ReadSingle();
        }

        public override void AI()
        {
            NPC.TargetClosestUpgraded();
            NPCAimedTarget target = NPC.GetTargetData();
            if (target.Invalid)
                NPC.TargetClosestUpgraded();

            //if (target.Type == Terraria.Enums.NPCTargetType.Player)
            //{
            //    if (!Main.player[NPC.target].ZoneJungle || Main.player[NPC.target].Center.Y < Main.worldSurface * 16 || Main.player[NPC.target].Center.Y > Main.UnderworldLayer * 16)
            //        Enraged = true;
            //}

            speed = 3f;
            slide = 0.2f;
            follow = 0.33f;
            NPC.defense = 24;

            if (PhaseII)
            {
                speed = 5f;
                slide = 0.15f;
                follow = 0.1f;
                NPC.defense = 40;
            }

            if (Enraged)
            {
                speed = 6f;
                slide = 0.3f;
                follow = 0.6f;

                NPC.defense += 10;
                NPC.damage = 100;
                NPC.localAI[1] += 0.5f; 
            }

            if ((NPC.life < NPC.lifeMax * 0.33f && Enraged) || target.Invalid)
            {
                follow = 0f;
                NPC.velocity += Vector2.UnitY * 0.1f;
                NPC.EncourageDespawn(60);
            }

            NPC.rotation = NPC.rotation.AngleLerp(NPC.AngleTo(target.Center), 0.2f);

            if (NPC.Distance(target.Center) > 800 - target.Velocity.LengthSquared())
            {
                NPC.dontTakeDamage = true;
                outOfRangeProg = MathHelper.Min(outOfRangeProg + 0.1f, 1);
                NPC.localAI[1] += 0.2f;
            }
            else
            {
                NPC.dontTakeDamage = false;
                outOfRangeProg = MathHelper.Max(outOfRangeProg - 0.1f, 0);
            }

            switch (Phase)
            {
                case 0:

                    PetalBurst();

                    break;

                case 1:

                    Phase = 0;
                    HeadValue = JawQuiver;
                    break;
            }

            if (!PhaseII && NPC.life < NPC.lifeMax * 0.5f)
            {
                if (Timer > 0)
                    NPC.dontTakeDamage = true;
                else
                {
                    Timer = 0;
                    PhaseII = true;
                    NPC.localAI[0] = -1;

                    Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center, NPC.rotation.ToRotationVector2(), 378, 1f);
                    Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center, NPC.rotation.ToRotationVector2(), 379, 1f);
                    Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center, NPC.rotation.ToRotationVector2(), 380, 1f);

                    SoundStyle crackle = SoundID.NPCDeath22;
                    SoundEngine.PlaySound(crackle, NPC.Center);

                }
            }

            Timer++;
            NPC.localAI[1]++;

            Vector2 grassVelocity = -NPC.rotation.ToRotationVector2() * 4f;
            int grassType = Main.rand.NextBool(3) ? DustID.GrassBlades : DustID.JunglePlants;
            Dust grassDust = Dust.NewDustDirect(NPC.Center - new Vector2(95, 0).RotatedBy(NPC.rotation) - new Vector2(30), 60, 60, grassType, grassVelocity.X, grassVelocity.Y, 0, Color.White, 1.5f);
            grassDust.noGravity = true;
        }

        private void BasicMovement(float speed, float slide, float follow)
        {
            NPCAimedTarget target = NPC.GetTargetData();
            if (NPC.Distance(target.Center) > 10 && !target.Invalid)
                NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(target.Center + target.Velocity * 8f) * Utils.GetLerpValue(-300, 300, NPC.Distance(target.Center)) * speed + target.Velocity * follow, slide);
        }

        private void ResetAttack()
        {
            Timer = 0;
            Phase++;
        }

        private void PetalBurst()
        {
            const int chargeUp = 50;
            if (Timer < 70 + chargeUp && Timer > chargeUp)
                BasicMovement(-0.2f, 0.05f, follow * 0.3f);
            else
                BasicMovement(speed, slide * 0.3f, follow + 0.5f);

            if (Timer == 40 + chargeUp)
            {
                NPC.velocity -= NPC.rotation.ToRotationVector2() * 4f;

                //for (int i = 0; i < Main.rand.Next(5, 8); i++)
                //    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(18, 0).RotatedBy(NPC.rotation), NPC.rotation.ToRotationVector2().RotatedByRandom(0.5f) * Main.rand.Next(8, 12), ProjectileID.VenomArrow, 40, 0);
                for (int i = 0; i < 30; i++)
                {
                    float rotAway = Main.rand.NextFloat(-1.5f, 1.5f);
                    Vector2 dustVelocity = NPC.rotation.ToRotationVector2().RotatedBy(rotAway) * Main.rand.NextFloat(16) * Utils.GetLerpValue(2f, 0f, Math.Abs(rotAway), true);
                    Dust d = Dust.NewDustPerfect(NPC.Center + new Vector2(10, 0).RotatedBy(NPC.rotation), DustID.BloodWater, dustVelocity, 100, Color.White, 1f);
                    d.noGravity = true;
                    d.fadeIn = Main.rand.NextFloat(1f, 2f);
                }

                SoundEngine.PlaySound(SoundID.Item171, NPC.Center);

            }

            if (!PhaseII)
                HeadValue = JawQuiver + (Utils.GetLerpValue(40, 50, Timer - chargeUp, true) - MathHelper.SmoothStep(0f, 0.5f, Utils.GetLerpValue(0, 40, Timer - chargeUp, true))) * MathHelper.SmoothStep(0f, 1f, Utils.GetLerpValue(70, 60, Timer - chargeUp, true)) * 1.5f;
            else
                HeadValue = JawQuiver + (Utils.GetLerpValue(40, 50, Timer - chargeUp, true) * 0.5f - MathHelper.SmoothStep(0f, 0.2f, Utils.GetLerpValue(0, 40, Timer - chargeUp, true))) * MathHelper.SmoothStep(0f, 1f, Utils.GetLerpValue(70, 60, Timer - chargeUp, true));

            if (Timer > 80 + chargeUp)
                ResetAttack();
        }

        private void VenomBall()
        {
            if (Timer == 60)
            {
                for (int i = 0; i < 40; i++)
                {
                    float rotAway = Main.rand.NextFloat(-1f, 1f);
                    Vector2 dustVelocity = NPC.rotation.ToRotationVector2().RotatedBy(rotAway) * Main.rand.NextFloat(16) * Utils.GetLerpValue(1.5f, 0f, Math.Abs(rotAway), true);
                    Dust d = Dust.NewDustPerfect(NPC.Center + new Vector2(10, 0).RotatedBy(NPC.rotation), DustID.Venom, dustVelocity, 0, Color.White, 1f);
                    d.noGravity = true;
                    d.fadeIn = Main.rand.NextFloat(1f, 2f);
                }
            }
        }

        private void Chomping()
        {
            float totalBiteTime = 54f;
            float biteTime = (NPC.localAI[1] % totalBiteTime);
            HeadValue = JawCurve(biteTime / (totalBiteTime * 0.9f));

            if ((int)(HeadValue * totalBiteTime) < 0 && !hasChomped)
            {
                SoundStyle chompSound = SoundID.Item171;
                chompSound.MaxInstances = 0;
                chompSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(chompSound, NPC.Center);
                hasChomped = true;
            }

            if ((int)(HeadValue * totalBiteTime) > (totalBiteTime * 0.9f))
                hasChomped = false;

        }

        private static float JawCurve(float x)
        {
            float[] prog = new float[2];
            prog[0] = 0.5f * (float)Math.Sin(5f * x - 1.5f) + 0.5f;
            prog[1] = 4f * (float)Math.Pow(x - 0.9f, 2f) - 0.09f;

            if (x < 0)
                return 0;
            else if (x < 0.9f)
                return MathHelper.Lerp(prog[0], prog[1], Utils.GetLerpValue(0.7f, 0.8f, x, true));
            else if (x < 1f)
                return MathHelper.Lerp(prog[1], 0f, Utils.GetLerpValue(0.9f, 1f, x, true));
            else
                return 0f;
        }

        public override void DrawBehind(int index) => Main.instance.DrawCacheNPCsOverPlayers.Add(index);

        private float outOfRangeProg;

        public override Color? GetAlpha(Color drawColor)
        {
            if (outOfRangeProg > 0)
                return Color.Lerp(NPC.GetNPCColorTintedByBuffs(drawColor), Lighting.GetColorClamped((int)NPC.Center.X / 16, (int)NPC.Center.Y / 16, Color.Purple * 0.66f) * 0.9f, outOfRangeProg);
            return NPC.GetNPCColorTintedByBuffs(drawColor);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Vector2 realCenter = NPC.Center - new Vector2(40, 0).RotatedBy(NPC.rotation);

            if (outOfRangeProg > 0f)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 offset = new Vector2(18 * outOfRangeProg, 0).RotatedBy(i / 3f * MathHelper.TwoPi + Main.GlobalTimeWrappedHourly * 4f * NPC.direction);
                    DrawPlantera(realCenter + offset, spriteBatch, screenPos, NPC.GetAlpha(drawColor) * 0.4f);
                }
            }

            DrawPlantera(realCenter, spriteBatch, screenPos, NPC.GetAlpha(drawColor));

            return false;
        }

        private void DrawPlantera(Vector2 center, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Asset<Texture2D> body = ModContent.Request<Texture2D>(Texture);
            Asset<Texture2D> tongue = ModContent.Request<Texture2D>(Texture + "JawsMouth");
            Asset<Texture2D> bulb = ModContent.Request<Texture2D>(Texture + "Bulb");
            Asset<Texture2D> jaws = ModContent.Request<Texture2D>(Texture + "Jaws");
            Asset<Texture2D> jawsBack = ModContent.Request<Texture2D>(Texture + "JawsBack");

            Rectangle bodyBack = body.Frame(2, 1, 0, 0);
            Rectangle bodyFront = body.Frame(2, 1, 1, 0);

            spriteBatch.Draw(body.Value, center - screenPos, bodyBack, drawColor, NPC.rotation + MathHelper.PiOver2, bodyBack.Size() * 0.5f, NPC.scale, 0, 0);

            if (PhaseII)
            {
                float jawRotation = HeadValue * 1.1f;
                Vector2 jawStretch = new Vector2(1f, 1.1f - HeadValue * 0.2f);
                spriteBatch.Draw(jawsBack.Value, center - screenPos, null, drawColor, NPC.rotation + MathHelper.PiOver2, jawsBack.Size() * new Vector2(0.5f, 0.95f), new Vector2(1f, 1.2f - HeadValue * 0.8f) * NPC.scale, 0, 0);

                Rectangle tongueFrame = tongue.Frame(1, 1, 0, 0);
                spriteBatch.Draw(tongue.Value, center - screenPos, tongueFrame, drawColor, NPC.rotation + MathHelper.PiOver2, tongueFrame.Size() * 0.5f, NPC.scale, 0, 0);

                Rectangle jawL = jaws.Frame(2, 1, 0, 0);
                Rectangle jawR = jaws.Frame(2, 1, 1, 0);
                spriteBatch.Draw(jaws.Value, center + new Vector2(22 * HeadValue - 8, 0).RotatedBy(NPC.rotation) - screenPos, jawL, drawColor, NPC.rotation + MathHelper.PiOver2 - jawRotation, jawL.Size() * new Vector2(0.95f, 1f), jawStretch * NPC.scale, 0, 0);
                spriteBatch.Draw(jaws.Value, center + new Vector2(22 * HeadValue - 8, 0).RotatedBy(NPC.rotation) - screenPos, jawR, drawColor, NPC.rotation + MathHelper.PiOver2 + jawRotation, jawR.Size() * new Vector2(0.05f, 1f), jawStretch * NPC.scale, 0, 0);

            }
            else
            {
                Vector2 bulbScale = new Vector2(1f - HeadValue * 0.2f, 1f + HeadValue * 0.2f);
                spriteBatch.Draw(bulb.Value, center + new Vector2(-14, 0).RotatedBy(NPC.rotation) - screenPos, null, drawColor, NPC.rotation + MathHelper.PiOver2, bulb.Size() * new Vector2(0.5f, 1.05f), bulbScale * NPC.scale, 0, 0);
            }

            spriteBatch.Draw(body.Value, center - screenPos, bodyFront, drawColor, NPC.rotation + MathHelper.PiOver2, bodyFront.Size() * 0.5f, NPC.scale, 0, 0);
        }
    }
}
