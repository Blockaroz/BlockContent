﻿using BlockContent.Content.Graphics;
using BlockContent.Content.Projectiles.NPCProjectiles.NightEmpressProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Drawing;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
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
            NPCID.Sets.TrailCacheLength[Type] = 15;

            NPCID.Sets.MPAllowedEnemies[Type] = true;

            NPCDebuffImmunityData debuffData = new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] {
                    BuffID.Confused
                }
            };
            NPCID.Sets.DebuffImmunitySets.Add(Type, debuffData);

            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.ShouldBeCountedAsBoss[Type] = true;
        }

        public override string BossHeadTexture => "BlockContent/Content/NPCs/NightEmpressBoss/NightEmpress_BossHead";

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
        }

        public override void SetDefaults()
        {
            NPC.width = 118;
            NPC.height = 166;
            NPC.friendly = false;

            NPC.lifeMax = 90000;
            NPC.damage = damageValue[0];
            NPC.defense = 80;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(gold: 30);

            NPC.boss = true;
            //BossBag = ModContent.ItemType<>();
            NPC.npcSlots = 15f;
            NPC.noTileCollide = true;
            NPC.dontTakeDamage = true;
            NPC.noGravity = true;

            NPC.HitSound = SoundID.NPCHit1;

            if (!Main.dedServ)
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Boss_NightEmpress");
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }

        public ref float Phase => ref NPC.ai[0];
        public ref float PhaseCounter => ref NPC.ai[1];
        public ref float AttackCounter => ref NPC.ai[2];
        public ref float State => ref NPC.ai[3];
        public bool Rage { get; set; }
        public bool EnterPhaseTwo { get; set; } = false;
        public bool ShouldDespawn { get; set; } = false;

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Rage);
            writer.Write(EnterPhaseTwo);
            writer.Write(ShouldDespawn);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Rage = reader.ReadBoolean();
            EnterPhaseTwo = reader.ReadBoolean();
            ShouldDespawn = reader.ReadBoolean();
        }

        public int[] damageValue = new int[8];

        public void HandleDamageValues()
        {
            damageValue[0] = 90;//Contact Damage
            damageValue[1] = 120;//Dash Contact Damage
            //Note: Projectile damage is multiplied by 2 by default.
            damageValue[2] = 40;//Moon Dance
            damageValue[3] = 36;//Shooting Star Barrage
            damageValue[4] = 95;//Explosi
            damageValue[5] = 52;//Flowering Night
            damageValue[6] = 0;
            damageValue[7] = 0;

            if (Phase == 0 || Phase == 1)
                damageValue[0] = -1;

            NPC.damage = damageValue[0];

            for (int i = 0; i < damageValue.Length; i++)
            {
                if (NightRage() || Main.getGoodWorld)
                    damageValue[i] = 9999;
            }
        }

        private static int _direction;

        public override void AI()
        {
            NPCAimedTarget target = NPC.GetTargetData();
            Vector2 targetPos = target.Invalid ? NPC.Center : target.Center;
            Vector2 targetPosOffset = new Vector2(0, -250);

            HandleDamageValues();

            if (PhaseCounter == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.TargetClosest();
                _direction = (NPC.Center.X > targetPos.X) ? 1 : (-1);
            }

            if (Main.netMode != NetmodeID.MultiplayerClient && Phase != 0)
            {
                TryDespawn(target);
                TryPhaseTwo(targetPos + targetPosOffset);
            }

            if (Phase == 0)//Spawn
            {
                PhaseCounter++;
                //if (PhaseCounter == 10)
                //    SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Assets/Sounds/NightEmpress/EmpressSpawn"), NPC.Center);

                float yLerp = Utils.GetLerpValue(0, 140, PhaseCounter, true);

                NPC.velocity = new Vector2(0, MathHelper.SmoothStep(0.5f, 0f, yLerp));

                NPC.dontTakeDamage = true;

                if (PhaseCounter >= 240)
                {
                    NPC.dontTakeDamage = false;
                    Phase = 1;
                    PhaseCounter = -1;
                }
            }

            NPC.velocity.Y *= 0.97f;

            if (Phase == 1)//Moon Dance
            {
                PhaseCounter++;

                const int attackLength = 330;
                const int interval = 70;

                if (PhaseCounter == 0)
                    SoundEngine.PlaySound(SoundID.Item165, NPC.Center);
                if (PhaseCounter <= 200)
                {
                    if ((PhaseCounter % interval) - 1 == 0)
                        MoonDanceProjectiles(9, Main.rand.NextFloat() * MathHelper.TwoPi);

                    if (PhaseCounter % interval <= 50 && NPC.Distance(targetPos) > 250)
                        NPC.velocity += NPC.DirectionTo(targetPos).SafeNormalize(Vector2.Zero) * Utils.GetLerpValue(20, 200, NPC.Distance(targetPos)) * 0.4f;
                    else
                        NPC.velocity = NPC.velocity.RotatedBy(MathHelper.ToRadians(0.1f) * _direction) * 0.98f;
                }
                if (PhaseCounter == 280)
                    NPC.velocity = NPC.DirectionFrom(targetPos).SafeNormalize(Vector2.Zero) * 12;

                if (PhaseCounter > attackLength)
                {
                    PhaseCounter = -1;
                    Phase++;
                }
            }

            if (Phase == 2)//Dash
            {
                PhaseCounter++;

                const int attackLength = 120;
                const int beginCharge = 30;
                const int dashCap = 10;

                if (PhaseCounter <= beginCharge)
                {
                    MoveToTarget(targetPos + targetPosOffset, 8, 20);
                    if (PhaseCounter == beginCharge / 2)
                        SoundEngine.PlaySound(SoundID.Item160, NPC.Center);
                }

                if (PhaseCounter == beginCharge)
                    NPC.velocity *= 0.33f;

                if (PhaseCounter <= attackLength && PhaseCounter > beginCharge)
                {
                    NPC.damage = damageValue[1];
                    NPC.dontTakeDamage = true;
                    NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(targetPos), 0.08f);
                    if (PhaseCounter == attackLength)
                    {
                        NPC.damage = damageValue[0];
                        NPC.dontTakeDamage = false;
                        NPC.velocity *= 0.5f;
                    }
                }

                //We check if the timer is below the limit to allow for dashCap frames of stillness
                if (PhaseCounter > attackLength && PhaseCounter <= attackLength + dashCap)
                    DashToTarget(targetPos + targetPosOffset);

                if (PhaseCounter > attackLength + dashCap)
                {
                    PhaseCounter = -1;
                    Phase++;
                }
            }

            if (Phase == 3)//Shooting Star Barrage
            {
                PhaseCounter++;
                const int attackLength = 185;
                int dashCap = Phase == 3 ? 10 : 0;
                const int blastTime = 100;
                const int blastTimeSecond = 180;

                if (PhaseCounter <= attackLength && PhaseCounter > 0)//It shoots a star at PC 0, due to modulo funky
                {
                    float offsetX = (float)Math.Cos((MathHelper.Pi * PhaseCounter) / (attackLength / 3)) * _direction;
                    float offsetY = (float)Math.Sin((MathHelper.Pi * PhaseCounter) / (attackLength / 6));
                    Vector2 followPos = new Vector2(offsetX * 500, (offsetY * 80) - 250);

                    MoveToTarget(targetPos + followPos, 5, 15);

                    if (PhaseCounter == 10)
                        SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Assets/Sounds/NightEmpress/EmpressShootingStars"), NPC.Center);

                    if (PhaseCounter <= blastTime)
                        ShootingStarBarrage(5, offsetX);

                    if (PhaseCounter > blastTime + 20 && PhaseCounter <= blastTimeSecond)
                        ShootingStarBarrage(7, offsetX);
                }

                if (PhaseCounter > attackLength && PhaseCounter <= attackLength + dashCap)
                    DashToTarget(targetPos + targetPosOffset);

                if (PhaseCounter > attackLength + 10 + dashCap)
                {
                    Phase++;
                    PhaseCounter = -1;
                }
            }

            if (Phase == 4)//Explo
            {
                PhaseCounter++;

                const int attackLength = 400;
                const int charge = 30;
                const int explode = 240;

                if (PhaseCounter < explode)
                    MoveToTarget(targetPos + new Vector2(-200 * _direction, -360), 0.2f, 2);
                else
                    NPC.velocity *= 0.9f;

                if (PhaseCounter == charge - 10)
                    NPC.velocity = NPC.DirectionFrom(targetPos).SafeNormalize(Vector2.Zero) * 12;

                if (PhaseCounter == charge)
                    SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Assets/Sounds/NightEmpress/EmpressRuneCharge"), NPC.Center);

                if (PhaseCounter == explode)
                {
                    SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Assets/Sounds/NightEmpress/EmpressRuneExplosion"), NPC.Center);
                    Projectile radial = Projectile.NewProjectileDirect(NPC.GetProjectileSpawnSource(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<RuneCircle>(), damageValue[4], 0);
                    radial.ai[0] = 190;
                    radial.ai[1] = NPC.whoAmI;
                    //Main.LocalPlayer.AddBuff(BuffID.Obstructed, 100);
                }
                if (PhaseCounter >= explode)
                {
                    float scale = Utils.GetLerpValue(380, 240, PhaseCounter, true) * 13;
                    CameraUtils.Screenshake(scale, 155);
                }

                if (PhaseCounter > attackLength)
                {
                    Phase++;
                    PhaseCounter = -1;
                }
            }

            if (Phase == 5)//Flowering Night
            {
                PhaseCounter++;
                const int attackLength = 120;
                const int End = 75;

                if (PhaseCounter <= attackLength)
                {
                    if (PhaseCounter < End)
                        MoveToTarget(targetPos + targetPosOffset, 3f, 0);
                    else
                        NPC.velocity = NPC.DirectionFrom(targetPos).SafeNormalize(Vector2.Zero);

                    if (PhaseCounter <= End)
                    {
                        NPC.velocity *= 0.1f;
                        Vector2 velocity = new Vector2(11, 0);

                        if (PhaseCounter == 5)
                            SoundEngine.PlaySound(SoundID.Item163, NPC.Center);

                        if ((PhaseCounter % 30) - 5 == 0)
                            FloweringNight(10, Utils.GetLerpValue(0, 60, PhaseCounter, true) * (MathHelper.Pi / 3), velocity);
                    }
                }

                if (PhaseCounter > attackLength + 10)
                {
                    Phase = 1;
                    PhaseCounter = -1;
                }
            }
        }

        #region Phase Checks

        private bool NightRage()
        {
            if (!Main.dayTime)
                Rage = true;

            return Rage;
        }

        public void TryDespawn(NPCAimedTarget target)
        {
            if (!ShouldDespawn)
            {
                bool targetInvalid = target.Invalid || NPC.Distance(target.Center) > 7200f;
                bool overTime = false;
                if (NightRage() && PhaseCounter <= 0)
                {
                    if (Main.dayTime || (!Main.dayTime && Main.time >= 31800.0))
                        overTime = true;
                }
                ShouldDespawn = ShouldDespawn || overTime || targetInvalid;
            }

            if (ShouldDespawn)
            {
                NPC.dontTakeDamage = true;
                if (PhaseCounter == 0 && Phase > 0)
                {
                    Phase = short.MinValue;
                    SoundEngine.PlaySound(SoundID.Item162, NPC.Center);
                }

                if (Phase <= short.MinValue)
                {
                    NPC.velocity.X *= 0.2f;
                    NPC.velocity.Y = MathHelper.Lerp(-3, 0, Utils.GetLerpValue(0, 125, PhaseCounter, true));
                }

                PhaseCounter++;

                if (PhaseCounter > 125 && Phase < 0)
                {
                    NPC.active = false;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, NPC.whoAmI);

                    return;
                }
            }
        }

        private float _oldPhase = 0;

        public void TryPhaseTwo(Vector2 targetPosition)
        {
            if (!EnterPhaseTwo)
            {
                bool notAttacking = PhaseCounter == 0;
                bool lifeCheck = NPC.life < (NPC.lifeMax / 2);
                EnterPhaseTwo = EnterPhaseTwo || (notAttacking && lifeCheck && !ShouldDespawn && !(State == 1));
            }

            if (EnterPhaseTwo)
            {
                if (PhaseCounter == 0 && Phase > 0)
                {
                    _oldPhase = Phase;
                    Phase = short.MaxValue; 
                    NPC.dontTakeDamage = true;
                }
                PhaseCounter++;

                NPC.velocity *= 0.1f;

                if (PhaseCounter > 100 && PhaseCounter <= 120)
                {
                    State = 1;
                    DashToTarget(targetPosition);
                }

                if (PhaseCounter >= 210)
                    NPC.dontTakeDamage = false;

                if (PhaseCounter >= 240)
                {
                    Phase = _oldPhase;
                    EnterPhaseTwo = false;
                    PhaseCounter = 0;
                }
            }
        }

        #endregion

        #region Movement

        public void MoveToTarget(Vector2 targetPos, float speed, float minimumDistance)
        {
            if (Vector2.Distance(NPC.Center, targetPos) >= minimumDistance)
                NPC.velocity += NPC.DirectionTo(targetPos).SafeNormalize(Vector2.Zero) * 0.25f * speed;
            else
            {
                NPC.velocity *= 0.95f;
                if (NPC.velocity.Length() < 0.0045f)
                    NPC.velocity = Vector2.Zero;
            }

            if (NPC.velocity.Length() > 1.5f)
                NPC.velocity *= 0.95f;
        }

        public void DashToTarget(Vector2 targetPos)
        {
            NPC.TargetClosest();
            if (NPC.Distance(targetPos) > 210f)
                targetPos -= NPC.DirectionTo(targetPos) * 180f;

            Vector2 difference = targetPos - NPC.Center;
            float lerpValue = Utils.GetLerpValue(100f, 550f, difference.Length());
            float speed = difference.Length();
            if (speed > 24f)
                speed = 24f;

            NPC.velocity = Vector2.Lerp(difference.SafeNormalize(Vector2.Zero) * speed, difference / 7f, lerpValue);
        }

        #endregion

        #region Attacks

        public void MoonDanceProjectiles(int totalProjectiles, float rotation = 0)
        {
            for (int i = 0; i < totalProjectiles; i++)
            {
                Projectile moonPetal = Projectile.NewProjectileDirect(NPC.GetProjectileSpawnSource(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<MoonDancePetal>(), damageValue[2], 0);
                moonPetal.ai[0] = (MathHelper.TwoPi / totalProjectiles * i) + rotation;
                moonPetal.ai[1] = NPC.whoAmI;
            }
        }

        public void ShootingStarBarrage(int interval, float velocityX)
        {
            if (PhaseCounter % interval == 0)
            {
                Projectile shootingStar = Projectile.NewProjectileDirect(NPC.GetProjectileSpawnSource(), NPC.Center + new Vector2(0, -8), new Vector2(Main.rand.Next(-7, 7) + velocityX, Main.rand.Next(-14, -7)), ModContent.ProjectileType<ShootingStar>(), damageValue[3], 0);
                shootingStar.ai[0] = NPC.target;
                shootingStar.ai[1] = -velocityX;
            }
        }

        public void FloweringNight(int totalProjectiles, float rotOffset, Vector2 velocity)
        {
            int projType = ModContent.ProjectileType<NightFlower>();
            for (int i = 0; i < totalProjectiles * 2; i++)
            {
                int direction = i > totalProjectiles ? 1 : -1;
                float rotation = ((MathHelper.TwoPi / totalProjectiles) * i) + (rotOffset * direction);
                Projectile flowerProj = Main.projectile[Projectile.NewProjectile(NPC.GetProjectileSpawnSource(), NPC.Center + new Vector2(0, -24), velocity.RotatedBy(rotation), projType, damageValue[5], 0)];
                flowerProj.ai[1] = direction;
            }
        }

        #endregion

        #region Visual

        public static Color SpecialColor(float t, bool useSecondColor = false)
        {
            Color light = new Color(162, 95, 234);
            Color lightEnrage = new Color(145, 85, 255);
            Color dark = new Color(60, 30, 123);
            //new Color(73, 30, 123);

            if (useSecondColor == true)
                lightEnrage = new GradientColor(new Color[] { new Color(255, 200, 100), new Color(255, 200, 200), new Color(100, 255, 200) }, 0.33f).Value;

            if (!Main.dayTime)
                return Color.Lerp(lightEnrage, dark, t);

            return Color.Lerp(light, dark, t);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            drawColor = Color.White;
            Color glowColor = Color.Transparent;
            //Handle Logic
            HandleDrawColor(out drawColor, out glowColor);

            NPC.localAI[0]++;
            
            //Phases custom drawing
            if (Phase == 4)
            {
                Color borderColor = SpecialColor(0);
                borderColor.A = 0;
                DrawPhaseRuneCircle(spriteBatch, screenPos);
                for (int i = 0; i < 8; i++)
                {
                    Vector2 offset = new Vector2(MoreUtils.DualLerp(0, 50, 350, 400, PhaseCounter, true) * 4, 0).RotatedBy((MathHelper.TwoPi / 8 * i) + NPC.rotation);
                    DrawEmpress(spriteBatch, screenPos - offset, borderColor, Color.Transparent, false);
                }
            }

            //Draw the Empress
            DrawEmpress(spriteBatch, screenPos, drawColor, glowColor, true);

            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Asset<Texture2D> streak = TextureAssets.Extra[98];
            if (Phase == 4)
            {
                Vector2 flareOrigin = streak.Size() / 2;
                Vector2 handPosition = new Vector2(0, -14);
                float[] flareScale = new float[]
                {
                    MoreUtils.DualLerp(90, 240, 270, PhaseCounter, true),
                    MoreUtils.DualLerp(239, 240, 300, PhaseCounter, true)
                };
                MoreUtils.DrawSparkle(streak, SpriteEffects.None, NPC.Center + handPosition - screenPos, flareOrigin, flareScale[0], 1.2f, 8 * flareScale[0], 2, MathHelper.PiOver2, SpecialColor(1), SpecialColor(0), 0.3f);
                MoreUtils.DrawSparkle(streak, SpriteEffects.None, NPC.Center + handPosition - screenPos, flareOrigin, flareScale[1], 1.2f, 60 * flareScale[1], 10 * flareScale[1], MathHelper.PiOver2, SpecialColor(1), SpecialColor(0, true), alpha: 51);
            }

            CreateMagicParticles();
            CreateDusts();
        }

        public void DrawEmpress(SpriteBatch spriteBatch, Vector2 offset, Color baseColor, Color glowColor, bool includeGlows)
        {
            Asset<Texture2D> body = TextureAssets.Npc[NPC.type];
            Asset<Texture2D> bodyGlow = Mod.Assets.Request<Texture2D>("Assets/Textures/NightEmpress/NightEmpress_Glow");
            //Asset<Texture2D> spikes
            Rectangle? frame = body.Frame(1, 2, 0, (int)State);

            DrawWings(spriteBatch, offset, baseColor, includeGlows);
            if (State == 1)
            {
                //draw spikes
                //if (includeGlows) { }
            }
            spriteBatch.Draw(body.Value, NPC.Center - offset, frame, baseColor, NPC.rotation, new Vector2(frame.Value.Width, frame.Value.Height) / 2, NPC.scale, SpriteEffects.None, 0);
            if (State == 1 && includeGlows)
            {
                for (int i = 0; i < 4; i++)
                {
                    float sine = MathHelper.Max((float)Math.Sin(Main.GlobalTimeWrappedHourly % 3), 0);
                    Vector2 cloakOffset = new Vector2(sine * 4, 0).RotatedBy(MathHelper.TwoPi / 4 * i);
                    spriteBatch.Draw(bodyGlow.Value, NPC.Center + cloakOffset - offset, null, glowColor * ((sine * 0.5f) + 0.3f), NPC.rotation, bodyGlow.Size() / 2, NPC.scale, SpriteEffects.None, 0);
                }
            }

            DrawArms(spriteBatch, offset, baseColor);

        }

        public void DrawWings(SpriteBatch spriteBatch, Vector2 offset, Color baseColor, bool includeOverlay = true)
        {
            Asset<Texture2D> wings = Mod.Assets.Request<Texture2D>("Assets/Textures/NightEmpress/NightEmpress_Wings");
            Asset<Texture2D> wingsMask = Mod.Assets.Request<Texture2D>("Assets/Textures/NightEmpress/NightEmpress_WingsMask");

            int wingCount = (int)(NPC.localAI[0] / 3f) % 11;
            Rectangle? frame = wings.Frame(1, 11, 0, wingCount);
            spriteBatch.Draw(wings.Value, NPC.Center - offset, frame, baseColor, NPC.rotation, new Vector2(frame.Value.Width / 2, frame.Value.Height / 2), NPC.scale * 2, SpriteEffects.None, 0);
            if (includeOverlay)
            {
                
            }
        }

        public void DrawArms(SpriteBatch spriteBatch, Vector2 offset, Color baseColor)
        {
            HandleArmFrames(out int left, out int right);
            Asset<Texture2D> arms = Mod.Assets.Request<Texture2D>("Assets/Textures/NightEmpress/NightEmpress_Arms");

            Rectangle leftFrame = arms.Frame(2, 4, 1, left);
            Vector2 leftOrigin = leftFrame.Size() / 2;
            Vector2 leftPos = new Vector2(31, -22);
            Main.EntitySpriteDraw(arms.Value, NPC.Center + leftPos - offset, leftFrame, baseColor, NPC.rotation, leftOrigin, NPC.scale, SpriteEffects.None, 0);

            Rectangle rightFrame = arms.Frame(2, 4, 0, right);
            Vector2 rightOrigin = rightFrame.Size() / 2;
            Vector2 rightPos = new Vector2(-31, -22);
            Main.EntitySpriteDraw(arms.Value, NPC.Center + rightPos - offset, rightFrame, baseColor, NPC.rotation, rightOrigin, NPC.scale, SpriteEffects.None, 0);
        }

        public void HandleArmFrames(out int left, out int right)
        {
            left = 0;
            right = 0;
            if (Phase == 0 && PhaseCounter < 200)
            {
                left = 3;
                right = 3;
            }
            if (Phase == 3)
            {
                if (_direction == -1)
                {
                    left = 3;
                    right = 1;
                }
                else
                {
                    left = 1;
                    right = 3;
                }
            }
            if (Phase == 4)
            {
                if (PhaseCounter < 70)
                {
                    left = 2;
                    right = 2;
                }
                else if (PhaseCounter < 90)
                {
                    left = 1;
                    right = 1;
                }
                else if (PhaseCounter < 380)
                {
                    left = 3;
                    right = 3;
                }
            }
        }

        public void DrawPhaseRuneCircle(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            Asset<Texture2D>[] runeCircle = new Asset<Texture2D>[3];
            for (int i = 0; i < runeCircle.Length; i++)
                runeCircle[i] = Mod.Assets.Request<Texture2D>("Assets/Textures/NightEmpress/RuneCircle_" + i);
            Asset<Texture2D> font = Mod.Assets.Request<Texture2D>("Assets/Textures/NightEmpress/Runes");
            Asset<Texture2D> blackFade = Mod.Assets.Request<Texture2D>("Assets/Textures/Glowball_" + (short)1);

            float scaleValue = MathHelper.SmoothStep(0, 1, MoreUtils.DualLerp(0, 70, 360, 400, PhaseCounter, true));
            float opacity = MathHelper.SmoothStep(0, 1, MoreUtils.DualLerp(20, 80, 330, 390, PhaseCounter, true));

            float cClockwise = (Main.GlobalTimeWrappedHourly % 360) * MathHelper.ToRadians(-13);
            float clockwise = (Main.GlobalTimeWrappedHourly % 360) * MathHelper.ToRadians(28);

            Color lightShade = SpecialColor(0) * opacity;
            lightShade.A /= 4;
            Color nightShade = SpecialColor(0, true) * opacity;
            nightShade.A /= 4;
            Color darkShade = SpecialColor(1) * opacity;
            darkShade.A /= 4;

            //draw back fade
            spriteBatch.Draw(blackFade.Value, NPC.Center - screenPos, null, Color.Black * opacity * 0.7f, 0, blackFade.Size() / 2, 0.5f + scaleValue, SpriteEffects.None, 0);

            //draw circles
            for (int i = 0; i < 2; i++)
            {
                Vector2 pos = NPC.Center + new Vector2(0.5f + (Main.rand.NextFloat() * 0.2f), 0).RotatedBy(MoreUtils.GetCircle(i, 2));
                spriteBatch.Draw(runeCircle[0].Value, pos - screenPos, null, lightShade, cClockwise, runeCircle[0].Size() / 2, scaleValue, SpriteEffects.None, 0);
                spriteBatch.Draw(runeCircle[1].Value, pos - screenPos, null, darkShade, clockwise, runeCircle[0].Size() / 2, scaleValue, SpriteEffects.None, 0);
                spriteBatch.Draw(runeCircle[2].Value, pos - screenPos, null, lightShade, 0, runeCircle[0].Size() / 2, scaleValue, SpriteEffects.None, 0);
            }

            //draw text
            int[] character = new int[]
            {
                20, 5, 18, 18, 1, 6, 9, 1, 18, 9, 14, 1, 14, 9, 19, 4, 5, 9, 14, 19, 1, 3, 18, 1, 20, 21, 19, 5, 14, 20, 9, 1
            };
            for (int i = 0; i < character.Length; i++)
            {
                Rectangle? frame = font.Frame(26, 1, character[i], 0);
                float runeRotation = (MathHelper.TwoPi / character.Length * i) + (cClockwise * 2);
                Vector2 placeInSentence = new Vector2(186 * scaleValue, 0).RotatedBy(runeRotation);
                for (int j = 0; j < 4; j++)
                {
                    Vector2 placeInSentenceGlow = placeInSentence + new Vector2(Main.rand.NextFloat(-4, 4) * scaleValue, Main.rand.NextFloat(-1, 1) * scaleValue).RotatedBy((MathHelper.TwoPi / 4 * j) + MathHelper.PiOver4);
                    spriteBatch.Draw(font.Value, NPC.Center + placeInSentenceGlow - screenPos, frame, lightShade, runeRotation + MathHelper.PiOver2, frame.Value.Size() / 2, scaleValue, SpriteEffects.None, 0);
                }
                spriteBatch.Draw(font.Value, NPC.Center + placeInSentence - screenPos, frame, nightShade * opacity, runeRotation + MathHelper.PiOver2, frame.Value.Size() / 2, scaleValue, SpriteEffects.None, 0);
            }
        }

        public void HandleDrawColor(out Color drawColor, out Color glowColor)
        {
            drawColor = Color.White;
            glowColor = SpecialColor(0.5f, true);
            Color glowReal = SpecialColor(0);
            if (Phase == 0)
            {
                //spawn animation
                float colorFade = Utils.GetLerpValue(0, 30, PhaseCounter, true);
                float firstFade = Utils.GetLerpValue(10, 40, PhaseCounter, true);
                float appearFade = Utils.GetLerpValue(50, 120, PhaseCounter, true);
                Color changeColor = Color.Lerp(Color.Transparent, SpecialColor(colorFade), firstFade);
                changeColor.A = (byte)(Utils.GetLerpValue(0, 18, PhaseCounter, true) * 5);
                NPC.Opacity = appearFade;
                drawColor = Color.Lerp(changeColor, Color.White, appearFade);
                glowColor = Color.Lerp(changeColor, SpecialColor(1), appearFade);
            }
            if (Phase <= short.MinValue)
            {
                //despawn animation
                float colorFade = Utils.GetLerpValue(95, 120, PhaseCounter, true);
                float firstFade = Utils.GetLerpValue(70, 110, PhaseCounter, true);
                float appearFade = Utils.GetLerpValue(0, 70, PhaseCounter, true);
                Color changeColor = Color.Lerp(SpecialColor(colorFade), Color.Transparent, firstFade);
                changeColor.A = (byte)(Utils.GetLerpValue(90, 120, PhaseCounter, true) * 5);
                NPC.Opacity = appearFade;
                drawColor = Color.Lerp(Color.White, changeColor, appearFade);
                glowColor = Color.Lerp(glowReal, changeColor, appearFade);
            }
            if (Phase == 2)
            {
                float fade = MoreUtils.DualLerp(200, 250, 300, 360, PhaseCounter, true);
                drawColor = Color.Lerp(Color.White, MoreColor.NightSky, fade);
                glowColor = Color.Lerp(glowReal, Color.Transparent, fade);
            }
            if (Phase == 4)
            {
                float fade = MoreUtils.DualLerp(200, 250, 300, 360, PhaseCounter, true);
                drawColor = Color.Lerp(Color.White, MoreColor.NightSky, fade);
            }
            glowReal.A = 0;
            glowColor.A = 0;
        }

        public static int GlowDustID { get => ModContent.DustType<Dusts.HaloDust>(); }

        public void CreateMagicParticles()
        {
            ParticleOrchestraSettings settings = new ParticleOrchestraSettings();
            if (Phase <= short.MinValue)
            {
                settings.PositionInWorld = NPC.Center + new Vector2(0, -20) + Main.rand.NextVector2Circular(90, 90);
                settings.MovementVector = new Vector2(Main.rand.NextFloat(-2, 2), Main.rand.NextFloat(-7, -4));
                for (int i = 0; i < Main.rand.Next(3); i++)
                    ParticleEffects.CreateNightMagic(settings, SpecialColor(Main.rand.NextFloat()));
            }
            if (Phase == 2 || Phase == 2)
            {
                settings.PositionInWorld = NPC.Center + Main.rand.NextVector2Circular(70, 70);
                settings.MovementVector = NPC.velocity * 0.2f;
                for (int i = 0; i < Main.rand.Next(5); i++)
                    ParticleEffects.CreateNightMagic(settings, SpecialColor(Main.rand.NextFloat()));
            }
            if (Phase == 3)
            {
                settings.PositionInWorld = NPC.Center + new Vector2(80 * _direction, -20) + Main.rand.NextVector2Circular(2, 2);
                settings.MovementVector = NPC.velocity * 0.7f;
                if (Main.rand.Next(2) == 0)
                    ParticleEffects.CreateNightMagic(settings, SpecialColor(Main.rand.NextFloat()));
            }
        }

        public void CreateDusts()
        {
            if (Phase == 4)
            {
                Color color = SpecialColor(0);
                color.A = 0;
                for (int i = 0; i < Main.rand.Next(70, 120); i++)
                {
                    if (PhaseCounter <= 220 && PhaseCounter > 40 && PhaseCounter % 51 == 0)
                    {
                        float size = Main.rand.Next(250, 300);
                        Vector2 position = Main.rand.NextVector2CircularEdge(size, size);
                        Dust dust = Dust.NewDustPerfect(NPC.Center + position, GlowDustID, Vector2.Zero, 0, color, 1f);
                        dust.noGravity = true;
                        dust.velocity = NPC.velocity + dust.position.DirectionTo(NPC.Center) * Main.rand.NextFloat(7, 9);
                    }
                    if (PhaseCounter == 240)
                    {
                        Vector2 speed = Main.rand.NextVector2Circular(100, 100);
                        Dust dust = Dust.NewDustDirect(NPC.Center - new Vector2(10), 20, 20, GlowDustID, speed.X, speed.Y, 0, color, 1f);
                        dust.noGravity = true;
                    }
                }
            }
        }

        #endregion
    }
}
