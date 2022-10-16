using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
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
            DisplayName.SetDefault("Plantera II");
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.ShouldBeCountedAsBoss[Type] = true;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.Plantera);
            NPC.SpawnWithHigherTime(400);
            NPC.aiStyle = -1;
            NPC.boss = true;
            NPC.hide = true;

            if (!Main.dedServ)
            {
                Music = MusicID.Plantera;
                //NPC.HitSound = SoundID.Grass;
            }
        }

        public bool Enraged { get; set; }

        public ref float JawProgress => ref NPC.localAI[0];
        private bool hasChomped;

        public override void AI()
        {
            NPC.TargetClosestUpgraded();
            NPCAimedTarget target = NPC.GetTargetData();

            if (target.Type == Terraria.Enums.NPCTargetType.Player)
            {
                if (!Main.player[NPC.target].ZoneJungle)
                    Enraged = true;
            }

            float speed = 4f;
            if (Enraged)
            {
                speed = 11f;
                NPC.defense = 80;
                NPC.localAI[1] += 0.5f;
            }

            if (NPC.Distance(target.Center) > 10)
                BasicMovement(speed);

            NPC.rotation = NPC.rotation.AngleLerp(NPC.AngleTo(target.Center), 0.15f);

            if (NPC.Distance(target.Center) > 480)
            {
                NPC.dontTakeDamage = true;
                outOfRangeProg = MathHelper.Min(outOfRangeProg + 0.05f, 1);
                NPC.localAI[1] += 0.2f;
            }
            else
            {
                NPC.dontTakeDamage = false;
                outOfRangeProg = MathHelper.Max(outOfRangeProg - 0.05f, 0);
            }

            NPC.localAI[1]++;

            //JawProgress = 0.2f + (float)Math.Sin(NPC.localAI[1] * 0.15f) * 0.02f;
            Chomping();

            Vector2 grassVelocity = -NPC.rotation.ToRotationVector2() * 0.5f;
            Dust grassDust = Dust.NewDustDirect(NPC.Center - new Vector2(95, 0).RotatedBy(NPC.rotation) - new Vector2(30), 60, 60, DustID.JunglePlants, grassVelocity.X, grassVelocity.Y, 0, NPC.GetAlpha(Color.White), 1.5f);
            grassDust.noGravity = true;
        }

        private void BasicMovement(float speed)
        {
            NPCAimedTarget target = NPC.GetTargetData();
            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(target.Center + target.Velocity * 10f) * Utils.GetLerpValue(-300, 300, NPC.Distance(target.Center)) * speed + target.Velocity.RotatedByRandom(0.3f) * 0.2f, 0.18f);
        }

        private void Chomping()
        {
            float biteTime = (NPC.localAI[1] % 50f);
            JawProgress = JawCurve(biteTime / 45f);

            if ((int)(JawProgress * 50) < 0 && !hasChomped)
            {
                SoundStyle chompSound = SoundID.Item171;
                chompSound.MaxInstances = 0;
                chompSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(chompSound, NPC.Center);
                hasChomped = true;
            }

            if ((int)(JawProgress * 50) > 45)
                hasChomped = false;

        }

        private float outOfRangeProg;

        private static float JawCurve(float x)
        {
            float[] prog = new float[2];
            prog[0] = 0.5f * (float)Math.Sin(5f * x - 1.5f) + 0.5f;
            prog[1] = 4f * (float)Math.Pow(x - 0.9f, 2f) - 0.08f;

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

        public override Color? GetAlpha(Color drawColor)
        {
            if (outOfRangeProg > 0)
                return Color.Lerp(drawColor, Lighting.GetColorClamped((int)NPC.Center.X / 16, (int)NPC.Center.Y / 16, Color.Chartreuse * 0.5f), outOfRangeProg);
            return null;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Vector2 realCenter = NPC.Center - new Vector2(60, 0).RotatedBy(NPC.rotation);

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
            Asset<Texture2D> jaws = ModContent.Request<Texture2D>(Texture + "Jaws");
            Asset<Texture2D> jawsBack = ModContent.Request<Texture2D>(Texture + "JawsBack");

            Rectangle bodyBack = body.Frame(2, 1, 0, 0);
            Rectangle bodyFront = body.Frame(2, 1, 1, 0);

            spriteBatch.Draw(body.Value, center - screenPos, bodyBack, drawColor, NPC.rotation + MathHelper.PiOver2, bodyBack.Size() * 0.5f, NPC.scale, 0, 0);

            //jaws
            float jawRotation = JawProgress * 1.1f;
            Vector2 jawStretch = new Vector2(1f, 1.3f - JawProgress * 0.2f);
            spriteBatch.Draw(jawsBack.Value, center + new Vector2(16 * JawProgress, 0).RotatedBy(NPC.rotation) - screenPos, null, drawColor, NPC.rotation + MathHelper.PiOver2, jawsBack.Size() * new Vector2(0.5f, 0.95f), new Vector2(1f, 1.5f - JawProgress * 0.9f) * NPC.scale, 0, 0);

            Rectangle tongueFrame = tongue.Frame(1, 1, 0, 0);
            spriteBatch.Draw(tongue.Value, center - screenPos, tongueFrame, drawColor, NPC.rotation + MathHelper.PiOver2, tongueFrame.Size() * 0.5f, NPC.scale, 0, 0);

            Rectangle jawL = jaws.Frame(2, 1, 0, 0);
            Rectangle jawR = jaws.Frame(2, 1, 1, 0);
            spriteBatch.Draw(jaws.Value, center + new Vector2(24 * JawProgress - 8, 0).RotatedBy(NPC.rotation) - screenPos, jawL, drawColor, NPC.rotation + MathHelper.PiOver2 - jawRotation, jawL.Size() * new Vector2(0.95f, 1f), jawStretch * NPC.scale, 0, 0);
            spriteBatch.Draw(jaws.Value, center + new Vector2(24 * JawProgress - 8, 0).RotatedBy(NPC.rotation) - screenPos, jawR, drawColor, NPC.rotation + MathHelper.PiOver2 + jawRotation, jawR.Size() * new Vector2(0.05f, 1f), jawStretch * NPC.scale, 0, 0);


            spriteBatch.Draw(body.Value, center - screenPos, bodyFront, drawColor, NPC.rotation + MathHelper.PiOver2, bodyFront.Size() * 0.5f, NPC.scale, 0, 0);
        }
    }
}
