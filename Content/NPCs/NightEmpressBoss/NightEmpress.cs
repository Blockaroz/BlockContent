using BlockContent.Content.NPCs.NightEmpressBoss.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContent.Content.NPCs.NightEmpressBoss
{
    [AutoloadBossHead]
    public class NightEmpress : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Empress of Night");

            NPCID.Sets.TrailingMode[Type] = 1;
            NPCID.Sets.TrailCacheLength[Type] = 10;

            NPCID.Sets.MPAllowedEnemies[Type] = true;

            NPCDebuffImmunityData debuffData = new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] {
                    BuffID.Confused
                }
            };
            NPCID.Sets.DebuffImmunitySets.Add(Type, debuffData);

            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }

        public override string BossHeadTexture => "BlockContent/Content/NPCs/NightEmpressBoss/NightEmpress_Head";

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
        }

        public override void SetDefaults()
        {
            NPC.width = 118;
            NPC.height = 166;
            NPC.noGravity = true;
            NPC.friendly = false;

            NPC.lifeMax = 90000;
            NPC.damage = 90;
            NPC.defense = 50;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(gold: 30);

            NPC.boss = true;
            //BossBag = ModContent.ItemType<>();
            NPC.npcSlots = 15f;
            NPC.noTileCollide = true;
            NPC.dontTakeDamage = true;

            NPC.HitSound = SoundID.NPCHit1;

            NPC.position.Y -= 60;

            if (!Main.dedServ)
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Boss_NightEmpress");
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }

        private static bool NightRage()
        {
            return !Main.dayTime;
        }

        public ref float Phase => ref NPC.ai[0];
        public ref float PhaseCounter => ref NPC.ai[1];
        public ref float AttackCounter => ref NPC.ai[2];
        public ref float MovementCounter => ref NPC.ai[3];

        //public ref float DrawCounter => ref NPC.localAI[0];

        public override void SendExtraAI(BinaryWriter writer)
        {
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
        }

        public override void AI()
        {
            NPCAimedTarget target = NPC.GetTargetData();

            Vector2 targetPosOffset = new Vector2(0, -250);

            if (PhaseCounter == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                NPC.TargetClosest();

            if (Phase == 0) //spawn
            {
                PhaseCounter++;
                float yLerp = Utils.GetLerpValue(0, 140, PhaseCounter, true);

                NPC.velocity.Y = MathHelper.SmoothStep(0.5f, 0f, yLerp);

                NPC.dontTakeDamage = true;

                if (PhaseCounter >= 240)
                {
                    NPC.dontTakeDamage = false;
                    PhaseCounter = 0;
                    Phase++;
                }
            }

            if (Phase == 1)
            {
                PhaseCounter++;

                const int attackLength = 540;
                const int blastTime = 265;

                float offsetX = (float)Math.Sin((MathHelper.Pi * PhaseCounter) / (attackLength / 3));
                float offsetY = (float)Math.Cos((MathHelper.Pi * PhaseCounter) / (attackLength / 6));

                Vector2 followPos = new(offsetX * 700, (offsetY * 100) - 300);

                if (PhaseCounter <= attackLength)
                {
                    if (PhaseCounter <= blastTime)
                    {
                        MoveToTarget(target, 6f, 10, followPos);
                        BombAttack(target, 9);
                    }
                    if (PhaseCounter >= attackLength - blastTime)
                    {
                        MoveToTarget(target, 6f, 10, followPos);
                        BombAttack(target, 15);
                    }
                }


                if (PhaseCounter > attackLength && PhaseCounter <= attackLength + 50)
                    DashToTarget(target, targetPosOffset);


                if (PhaseCounter > attackLength + 50)
                {
                    PhaseCounter = 0;
                    Phase++;
                }
            }

            if (Phase == 2)
            {
                PhaseCounter++;
                if (PhaseCounter == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.TargetClosest();

                const int attackLength = 230;
                const int doAttack = 30;
                const int doAttackSecond = 60;

                if (PhaseCounter <= attackLength)
                {
                    Vector2 targetPos = target.Invalid ? NPC.Center : target.Center;

                    if (PhaseCounter < doAttackSecond)
                        MoveToTarget(target, 3f, 0, targetPosOffset);
                    else
                        NPC.velocity = NPC.DirectionFrom(targetPos).SafeNormalize(Vector2.Zero);

                    if (PhaseCounter >= doAttack && PhaseCounter <= doAttackSecond)
                    {
                        NPC.velocity *= 0.1f;
                        Vector2 velocity = new Vector2(10f, 0);

                        float angle = 0;
                        if (PhaseCounter == doAttack - 1)
                            angle = NPC.AngleTo(targetPos) + Main.rand.NextFloat(0.1f, 0.1f);

                        if (PhaseCounter == doAttack)
                        {
                            SoundEngine.PlaySound(SoundID.Item163, NPC.Center);
                            FloweringNight(10, angle - MathHelper.PiOver4, velocity);
                        }

                        if (PhaseCounter == doAttackSecond)
                            FloweringNight(10, angle + MathHelper.PiOver4, -velocity);
                    }
                }

                if (PhaseCounter > attackLength && PhaseCounter <= attackLength + 10)
                    DashToTarget(target, targetPosOffset);

                if (PhaseCounter > attackLength + 15)
                {
                    PhaseCounter = 0;
                    Phase = 1;
                    //Phase++;
                }
            }

            if (Phase == -1)
            {
                PhaseCounter++;
                if (PhaseCounter == 0)
                    NPC.TargetClosest();

                const int attackLength = 120;

                

                if (PhaseCounter <= attackLength)
                {
                    //attack
                }

                if (PhaseCounter > attackLength && PhaseCounter <= attackLength + 10)
                    DashToTarget(target, targetPosOffset);

                if (PhaseCounter > attackLength + 15)
                {
                    PhaseCounter = 0;
                    Phase++;
                }
            }

            HandleDamageValues();
        }

        private float[] damageValue = new float[8];

        public void HandleDamageValues()
        {

        }

        #region Movement

        public void MoveToTarget(NPCAimedTarget target, float speed, float minimumDistance, Vector2 offset)
        {
            Vector2 targetPos = target.Invalid ? NPC.Center : (target.Center + offset);
            if (Vector2.Distance(NPC.Center, targetPos) >= minimumDistance)
                NPC.velocity += NPC.DirectionTo(targetPos).SafeNormalize(Vector2.Zero) * 0.25f * speed;
            else
            {
                NPC.velocity *= 0.95f;
                if (NPC.velocity.Length() < 0.0044f)
                    NPC.velocity = Vector2.Zero;
            }

            if (NPC.velocity.Length() > 1.5f)
                NPC.velocity *= 0.95f;
        }

        public void DashToTarget(NPCAimedTarget target, Vector2 offset)
        {
            NPC.TargetClosest();
            Vector2 targetPos = target.Invalid ? NPC.Center : (target.Center + offset);
            if (NPC.Distance(targetPos) > 150f)
                targetPos -= NPC.DirectionTo(targetPos) * 150f;

            Vector2 difference = targetPos - NPC.Center;
            float lerpValue = Utils.GetLerpValue(100f, 550f, difference.Length());
            float speed = difference.Length();
            if (speed > 24f)
                speed = 24f;

            NPC.velocity = Vector2.Lerp(difference.SafeNormalize(Vector2.Zero) * speed, difference / 7f, lerpValue);
        }

        #endregion

        #region Attacks

        public void BombAttack(NPCAimedTarget target, float interval)
        {
            int projType = ModContent.ProjectileType<BombB>();
            Vector2 targetPos = target.Invalid ? NPC.Center : target.Center;
            Vector2 velocity = new Vector2(10, 0).RotatedBy(NPC.AngleTo(targetPos)).RotatedByRandom(1);

            float hand = (PhaseCounter % 2 == 0) ? -1 : 1;
            Vector2 spawnPos = NPC.Center + new Vector2(40 * hand, -24);

            if (PhaseCounter % interval == 0)
            {
                Projectile bombProj = Main.projectile[Projectile.NewProjectile(NPC.GetProjectileSpawnSource(), spawnPos, velocity + NPC.velocity, projType, NPC.GetAttackDamage_ForProjectiles_MultiLerp(120, 240, 360), 0)];
                bombProj.ai[0] = NPC.target;
            }
        }

        public void FloweringNight(int totalProjectiles, float rotOffset, Vector2 velocity)
        {
            int projType = ModContent.ProjectileType<NightFlower>();
            for (int i = 0; i < totalProjectiles * 2; i++)
            {
                float direction = i > totalProjectiles ? 1 : -1;
                float rotation = ((MathHelper.TwoPi / totalProjectiles) * i) + rotOffset;
                Projectile flowerProj = Main.projectile[Projectile.NewProjectile(NPC.GetProjectileSpawnSource(), NPC.Center + new Vector2(0, -24), velocity.RotatedBy(rotation), projType, NPC.GetAttackDamage_ForProjectiles_MultiLerp(120, 240, 360), 0)];
                flowerProj.ai[1] = direction;
            }
        }

        #endregion

        #region Visual

        public static Color NightColor(float t, bool useSecondColor = false)
        {
            Color light = new Color(162, 95, 234);
            Color dark = new Color(87, 27, 169);
            Color lightEnrage = new Color(174, 74, 255);
            Color darkEnrage = new Color(63, 0, 123);

            if (useSecondColor == true)
                lightEnrage = new Color(132, 221, 255);

            if (NightRage())
                return Color.Lerp(lightEnrage, darkEnrage, t);

            return Color.Lerp(light, dark, t);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Asset<Texture2D> body = Mod.Assets.Request<Texture2D>("Content/NPCs/NightEmpressBoss/NightEmpress");

            Rectangle? frame = body.Frame(1, 2, 0, 0);

            Color color = Color.White;

            if (Phase == 0)
            {
                //spawn animation
                float purpleFadeIn = Utils.GetLerpValue(0, 45, PhaseCounter, true);
                float fadeIn = Utils.GetLerpValue(50, 120, PhaseCounter, true);
                Color fadeColor = Color.Lerp(Color.Transparent, NightColor(purpleFadeIn), purpleFadeIn);
                fadeColor.A = 0;
                NPC.Opacity = fadeIn;
                color = Color.Lerp(fadeColor, Color.White, fadeIn);
            }

            spriteBatch.Draw(body.Value, NPC.Center - screenPos, frame, color, NPC.rotation, new Vector2(frame.Value.Width, frame.Value.Height) / 2, NPC.scale, SpriteEffects.None, 0);

            return false;
        }

        #endregion
    }
}
