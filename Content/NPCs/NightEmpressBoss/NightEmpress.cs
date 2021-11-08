using BlockContent.Content.Graphics;
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
            NPC.noGravity = true;
            NPC.friendly = false;

            NPC.lifeMax = 90000;
            NPC.damage = damageValue[0];
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
                _direction = NPC.Center.X > targetPos.X ? 1 : -1;
            }

            if (Main.netMode != NetmodeID.MultiplayerClient && Phase != 0)
            {
                TryDespawn(target);
                TryPhaseTwo(targetPos + targetPosOffset);
            }

            if (Phase == 0) //Spawn
            {
                PhaseCounter++;
                if (PhaseCounter == 10)
                    SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Assets/Sounds/NightEmpress/EmpressSpawn"), NPC.Center);

                float yLerp = Utils.GetLerpValue(0, 140, PhaseCounter, true);

                NPC.velocity.Y = MathHelper.SmoothStep(0.5f, 0f, yLerp);

                NPC.dontTakeDamage = true;

                if (PhaseCounter >= 240)
                {
                    NPC.dontTakeDamage = false;
                    Phase = 1;
                    PhaseCounter = -1;
                }
            }

            if (Phase == 1)//Swoop
            {
                PhaseCounter++;

                const int attackLength = 360;
                const int dashStart = 300;
                const int dashCap = 10;

                if (PhaseCounter < dashStart)
                {
                    float speed = Utils.GetLerpValue(300, 290, PhaseCounter, true);
                    NPC.velocity += NPC.DirectionTo(targetPos).SafeNormalize(Vector2.Zero) * Utils.GetLerpValue(100, 550, targetPos.Distance(NPC.Center));
                    NPC.velocity *= Utils.GetLerpValue(300, 290, PhaseCounter, true);
                    if (NPC.Distance(targetPos) > 800)
                        MoveToTarget(targetPos, 5, 10);
                }

                //We check if the timer is below the limit to allow for dashCap frames of stillness
                if (PhaseCounter > attackLength && PhaseCounter <= attackLength + dashCap)
                    DashToTarget(targetPos + targetPosOffset);

                if (PhaseCounter > attackLength + dashCap + 10)
                {
                    PhaseCounter = -1;
                    Phase++;
                }
            }

            if (Phase == 2)//Shooting Star Barrage
            {
                PhaseCounter++;
                const int attackLength = 185;
                const int dashCap = 10;
                const int blastTime = 100;
                const int blastTimeSecond = 180;

                if (PhaseCounter <= attackLength && PhaseCounter > 0)//For some reason, it shoots a star at PhaseCounter = 0, which conflicts with things like despawning and phase two
                {
                    float offsetX = (float)Math.Cos((MathHelper.Pi * PhaseCounter) / (attackLength / 3)) * _direction;
                    float offsetY = (float)Math.Sin((MathHelper.Pi * PhaseCounter) / (attackLength / 6));
                    Vector2 followPos = new Vector2(offsetX * 500, (offsetY * 80) - 250);

                    MoveToTarget(targetPos + followPos, 5, 15);

                    if (PhaseCounter == 10)
                        SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Assets/Sounds/NightEmpress/EmpressShootingStars"), NPC.Center);

                    if (PhaseCounter <= blastTime)
                        ShootingStarBarrage(6, offsetX);

                    if (PhaseCounter > blastTime + 20 && PhaseCounter <= blastTimeSecond)
                        ShootingStarBarrage(10, offsetX);
                }

                if (PhaseCounter > attackLength && PhaseCounter <= attackLength + dashCap)
                    DashToTarget(targetPos + targetPosOffset);

                if (PhaseCounter > attackLength + dashCap + 10)
                {
                    Phase++;
                    PhaseCounter = -1;
                }
            }

            if (Phase == 3)//Explo
            {
                PhaseCounter++;

                const int attackLength = 390;
                const int createCircle = 30;
                const int explode = 240;
                const int stopMovement = 220;

                if (PhaseCounter < stopMovement)
                {
                    float speed = Utils.GetLerpValue(190, 0, PhaseCounter, true) * 0.5f;
                    MoveToTarget(targetPos + new Vector2(-200 * _direction, -360), speed, 10);
                }
                else
                    NPC.velocity = Vector2.Zero;

                if (PhaseCounter == createCircle)
                    SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Assets/Sounds/NightEmpress/EmpressRuneCharge"), NPC.Center);

                if (PhaseCounter == explode)
                {
                    SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Assets/Sounds/NightEmpress/EmpressRuneExplosion"), NPC.Center);
                    Projectile radial = Projectile.NewProjectileDirect(NPC.GetProjectileSpawnSource(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<RuneCircle>(), damageValue[3], 0);
                    radial.ai[0] = 190;
                    radial.ai[1] = NPC.whoAmI;
                }
                if (PhaseCounter >= explode)
                {
                    float scale = Utils.GetLerpValue(380, 240, PhaseCounter, true) * 13;
                    CameraUtils.Screenshake(scale, 140);
                }

                if (PhaseCounter > attackLength)
                {
                    Phase++;
                    PhaseCounter = -1;
                }
            }

            if (Phase == 4)//Flowering Night
            {
                PhaseCounter++;
                const int attackLength = 120;
                const int doAttack = 5;
                const int doAttackSecond = 40;

                if (PhaseCounter <= attackLength)
                {
                    if (PhaseCounter < doAttackSecond)
                        MoveToTarget(targetPos + targetPosOffset, 3f, 0);
                    else
                        NPC.velocity = NPC.DirectionFrom(targetPos).SafeNormalize(Vector2.Zero);

                    if (PhaseCounter >= doAttack && PhaseCounter <= doAttackSecond)
                    {
                        NPC.velocity *= 0.1f;
                        Vector2 velocity = new Vector2(11f, 0);

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
                if (NightRage())
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

                NPC.velocity *= 0;

                if (PhaseCounter > 100 && PhaseCounter <= 120)
                {
                    State = 1;
                    DashToTarget(targetPosition);
                }

                if (PhaseCounter >= 240)
                {
                    Phase = _oldPhase;
                    NPC.dontTakeDamage = false;
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
            if (NPC.Distance(targetPos) > 200f)
                targetPos -= NPC.DirectionTo(targetPos) * 180f;

            Vector2 difference = targetPos - NPC.Center;
            float lerpValue = Utils.GetLerpValue(100f, 550f, difference.Length());
            float speed = difference.Length();
            if (speed > 24f)
                speed = 24f;

            NPC.velocity = Vector2.Lerp(difference.SafeNormalize(Vector2.Zero) * speed, difference / 7f, lerpValue);
        }

        #endregion

        public int[] damageValue = new int[8];

        public void HandleDamageValues()
        {
            damageValue[0] = 90;//Contact Damage
            //Note: Projectile damage is multiplied by 2 by default.
            damageValue[1] = 95;
            damageValue[2] = 36;//Shooting Star Barrage
            damageValue[3] = 95;//Explosi
            damageValue[4] = 52;//Flowering Night
            damageValue[5] = 0;
            damageValue[6] = 0;
            damageValue[7] = 0;

            if (Phase == 0)
                damageValue[0] = -1;

            NPC.damage = damageValue[0];

            for (int i = 0; i < damageValue.Length; i++)
            {
                if (NightRage() || Main.getGoodWorld)
                    damageValue[i] = 9999;
            }
        }

        #region Attacks

        public void ShootingStarBarrage(int interval, float velocityX)
        {
            if (PhaseCounter % interval == 0)
            {
                Projectile shootingStar = Projectile.NewProjectileDirect(NPC.GetProjectileSpawnSource(), NPC.Center + new Vector2(0, -8), new Vector2(Main.rand.Next(-7, 7) + velocityX, Main.rand.Next(-14, -7)), ModContent.ProjectileType<ShootingStar>(), damageValue[2], 0);
                shootingStar.ai[0] = NPC.target;
                shootingStar.ai[1] = -velocityX;
            }
        }

        public void FloweringNight(int totalProjectiles, float rotOffset, Vector2 velocity)
        {
            int projType = ModContent.ProjectileType<NightFlower>();
            for (int i = 0; i < totalProjectiles * 2; i++)
            {
                float direction = i > totalProjectiles ? 1 : -1;
                float rotation = ((MathHelper.TwoPi / totalProjectiles) * i) + rotOffset;
                Projectile flowerProj = Main.projectile[Projectile.NewProjectile(NPC.GetProjectileSpawnSource(), NPC.Center + new Vector2(0, -24), velocity.RotatedBy(rotation), projType, damageValue[4], 0)];
                flowerProj.ai[1] = direction;
            }
        }

        #endregion

        #region Visual

        public static Color NightColor(float t, bool useSecondColor = false)
        {
            Color light = new Color(162, 95, 234);
            Color lightEnrage = new Color(154, 54, 255);
            Color dark = new Color(73, 30, 123);
            //new Color(73, 30, 123);
            //new Color(105, 0, 205);

            if (useSecondColor == true)
                lightEnrage = new GradientColor(new Color[] { new Color(255, 200, 100), new Color(255, 155, 255), new Color(100, 255, 200) }, 0.5f).Value;

            if (!Main.dayTime)
                return Color.Lerp(lightEnrage, dark, t);

            return Color.Lerp(light, dark, t);
        }
        public static Color NightBlack 
        {
            get => new Color(20, 16, 28);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            drawColor = Color.White;
            Color glowColor = Color.Transparent;
            //Handle Logic
            HandleDrawColor(out drawColor, out glowColor);

            //Phases custom drawing
            if (Phase == 3)
                DrawPhaseRuneCircle(spriteBatch, screenPos);

            //Draw the Empress
            DrawEmpress(spriteBatch, screenPos, drawColor, glowColor);

            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (Phase == 3)
            {
                Vector2 flareOrigin = TextureAssets.Extra[98].Size() / 2;
                Vector2 handPosition = new Vector2(0, -14);
                float[] flareScale = new float[]
                {
                    MoreUtils.DualLerp(90, 240, 270, PhaseCounter, true),
                    MoreUtils.DualLerp(239, 240, 300, PhaseCounter, true)
                };
                MoreUtils.DrawSparkle(TextureAssets.Extra[98], SpriteEffects.None, NPC.Center + handPosition - screenPos, flareOrigin, flareScale[0], 1, 8 * flareScale[0], 2, MathHelper.PiOver2, NightColor(1), NightColor(0), 0.3f);
                MoreUtils.DrawSparkle(TextureAssets.Extra[98], SpriteEffects.None, NPC.Center + handPosition - screenPos, flareOrigin, flareScale[1], 1, 60 * flareScale[1], 10 * flareScale[1], MathHelper.PiOver2, NightColor(1), NightColor(0, true), alpha: 51);
            }

            CreateMagicParticles();
            CreateDusts();
        }

        public void DrawEmpress(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor, Color glowColor, bool includeGlows = true)
        {
            Asset<Texture2D> body = Mod.Assets.Request<Texture2D>("Content/NPCs/NightEmpressBoss/NightEmpress");
            Asset<Texture2D> bodyGlow = Mod.Assets.Request<Texture2D>("Content/NPCs/NightEmpressBoss/NightEmpress_Glow");
            //Asset<Texture2D> spikes
            Rectangle? frame = body.Frame(1, 2, 0, (int)State);

            DrawWings(spriteBatch, screenPos, drawColor, includeGlows);
            if (State == 1)
            {
                //draw spikes
                //if (includeGlows) { }
            }
            spriteBatch.Draw(body.Value, NPC.Center - screenPos, frame, drawColor, NPC.rotation, new Vector2(frame.Value.Width, frame.Value.Height) / 2, NPC.scale, SpriteEffects.None, 0);
            if (State == 1 && includeGlows)
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector2 offset = new Vector2(MathHelper.Max(0, (float)Math.Sin(Main.GlobalTimeWrappedHourly % 50)), 0).RotatedBy(MathHelper.TwoPi / 4 * i);
                    spriteBatch.Draw(bodyGlow.Value, NPC.Center + offset - screenPos, null, glowColor, NPC.rotation, bodyGlow.Size() / 2, NPC.scale, SpriteEffects.None, 0);
                }
            }

            //DrawArms

        }

        public void DrawWings(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor, bool includeOverlay = true)
        {
            Asset<Texture2D> wings = Mod.Assets.Request<Texture2D>("Content/NPCs/NightEmpressBoss/NightEmpressWings");
            Asset<Texture2D> wingsMask = Mod.Assets.Request<Texture2D>("Content/NPCs/NightEmpressBoss/NightEmpressWings_Glow");

            NPC.localAI[0]++;
            int wingCount = (int)(NPC.localAI[0] / 3f) % 11;
            Rectangle? frame = wings.Frame(1, 11, 0, wingCount);
            spriteBatch.Draw(wings.Value, NPC.Center - screenPos, frame, drawColor, NPC.rotation, new Vector2(frame.Value.Width / 2, frame.Value.Height / 2), NPC.scale * 2, SpriteEffects.None, 0);
        }

        //Arms methods

        public void DrawPhaseRuneCircle(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            Asset<Texture2D>[] runeCircle = new Asset<Texture2D>[3];
            for (int i = 0; i < runeCircle.Length; i++)
                runeCircle[i] = Mod.Assets.Request<Texture2D>("Assets/Textures/NightEmpress/RuneCircle_" + i);
            Asset<Texture2D> font = Mod.Assets.Request<Texture2D>("Assets/Textures/NightEmpress/Runes");
            Asset<Texture2D> blackFade = Mod.Assets.Request<Texture2D>("Assets/Textures/Glowball_" + (short)1);

            float scaleValue = MathHelper.SmoothStep(0, 1, Utils.GetLerpValue(0, 80, PhaseCounter, true));
            float opacity = MathHelper.SmoothStep(0, 1, MoreUtils.DualLerp(20, 80, 330, 390, PhaseCounter, true));

            float cClockwise = (Main.GlobalTimeWrappedHourly % 360) * MathHelper.ToRadians(-13);
            float clockwise = (Main.GlobalTimeWrappedHourly % 360) * MathHelper.ToRadians(28);

            Color lightShade = NightColor(0) * opacity;
            lightShade.A /= 4;
            Color nightShade = NightColor(0, true) * opacity;
            nightShade.A /= 4;
            Color darkShade = NightColor(1) * opacity;
            darkShade.A /= 4;

            //draw back fade
            spriteBatch.Draw(blackFade.Value, NPC.Center - screenPos, null, Color.Black * opacity * 0.87f, 0, blackFade.Size() / 2, 0.5f + scaleValue, SpriteEffects.None, 0);

            //draw circles
            for (int i = 0; i < 2; i++)
            {
                Vector2 pos = NPC.Center + new Vector2(0.66f, 0).RotatedBy(MoreUtils.GetCircle(i, 2));
                spriteBatch.Draw(runeCircle[0].Value, pos - screenPos, null, lightShade, cClockwise, runeCircle[0].Size() / 2, scaleValue, SpriteEffects.None, 0);
                spriteBatch.Draw(runeCircle[1].Value, pos - screenPos, null, darkShade, clockwise, runeCircle[0].Size() / 2, scaleValue, SpriteEffects.None, 0);
                spriteBatch.Draw(runeCircle[2].Value, pos - screenPos, null, lightShade, 0, runeCircle[0].Size() / 2, scaleValue, SpriteEffects.None, 0);
            }

            //draw text
            int[] character = new int[]
            {
                13, 15, 15, 14, 2, 21, 18, 14, 
                6, 9, 22, 5, 20, 23, 15, 26, 5, 18, 15, 5, 9, 7, 8, 20, 
                15, 14, 4, 9, 19, 3, 15, 18, 4, 
                9, 19, 1, 14, 1, 23, 5, 19, 15, 13, 5, 1, 14, 4, 12, 9, 11, 5, 1, 2, 12, 5, 16, 5, 18, 19, 15, 14
            };
            for (int i = 0; i < character.Length; i++)
            {
                Rectangle? frame = font.Frame(26, 1, character[i], 0);
                float runeRotation = (MathHelper.TwoPi / character.Length * i) + (cClockwise * 2);
                Vector2 placeInSentence = new Vector2(186 * scaleValue, 0).RotatedBy(runeRotation);
                for (int j = 0; j < 4; j++)
                {
                    Vector2 placeInSentenceGlow = placeInSentence + new Vector2(2 * scaleValue, 0).RotatedBy((MathHelper.TwoPi / 4 * j) + MathHelper.PiOver4);
                    spriteBatch.Draw(font.Value, NPC.Center + placeInSentenceGlow - screenPos, frame, darkShade, runeRotation + MathHelper.PiOver2, frame.Value.Size() / 2, scaleValue, SpriteEffects.None, 0);
                }
                spriteBatch.Draw(font.Value, NPC.Center + placeInSentence - screenPos, frame, new Color(255, 255, 255, 0) * opacity, runeRotation + MathHelper.PiOver2, frame.Value.Size() / 2, scaleValue, SpriteEffects.None, 0);
            }
        }

        public void HandleDrawColor(out Color drawColor, out Color glowColor)
        {
            drawColor = Color.White;
            glowColor = Color.Transparent;
            if (Phase == 0)
            {
                //spawn animation
                float colorFade = Utils.GetLerpValue(0, 30, PhaseCounter, true);
                float firstFade = Utils.GetLerpValue(10, 45, PhaseCounter, true);
                float appearFade = Utils.GetLerpValue(50, 120, PhaseCounter, true);
                Color changeColor = Color.Lerp(Color.Transparent, NightColor(colorFade), firstFade);
                changeColor.A = (byte)(Utils.GetLerpValue(0, 18, PhaseCounter, true) * 5);
                NPC.Opacity = appearFade;
                drawColor = Color.Lerp(changeColor, Color.White, appearFade);
            }
            
            if (Phase <= short.MinValue)
            {
                //despawn animation
                float colorFade = Utils.GetLerpValue(95, 120, PhaseCounter, true);
                float firstFade = Utils.GetLerpValue(70, 110, PhaseCounter, true);
                float appearFade = Utils.GetLerpValue(0, 70, PhaseCounter, true);
                Color changeColor = Color.Lerp(NightColor(colorFade), Color.Transparent, firstFade);
                changeColor.A = (byte)(Utils.GetLerpValue(90, 120, PhaseCounter, true) * 5);
                NPC.Opacity = appearFade;
                drawColor = Color.Lerp(Color.White, changeColor, appearFade);
                glowColor = Color.Lerp(Color.Transparent, changeColor, appearFade);
            }
        }

        public static int GlowDustID { get => ModContent.DustType<Dusts.HaloDust>(); }

        public void CreateMagicParticles()
        {
            if (Phase <= short.MinValue)
            {
                ParticleOrchestraSettings despawnSettings = new ParticleOrchestraSettings()
                {
                    PositionInWorld = NPC.Center + new Vector2(0, -20) + Main.rand.NextVector2Circular(90, 90),
                    MovementVector = new Vector2(Main.rand.NextFloat(-2, 2), Main.rand.NextFloat(-7, -4))
                };
                for (int i = 0; i < Main.rand.Next(3); i++)
                    ParticleEffects.CreateNightMagic(despawnSettings, NightColor(Main.rand.NextFloat()));
            }
        }

        public void CreateDusts()
        {
            if (Phase <= short.MinValue)
            {
                Color glowColor = NightColor(Utils.GetLerpValue(95, 120, PhaseCounter, true));
                glowColor.A /= 5;
                Dust glowDust = Dust.NewDustDirect(NPC.Center - new Vector2(35, 48), 70, 80, GlowDustID, 0, 0, 0, glowColor, 1f);
                glowDust.noGravity = true;
                glowDust.velocity = glowDust.position.DirectionFrom(NPC.Center) * (NPC.Distance(glowDust.position) * 0.2f);
                glowDust.velocity *= 0.93f;

                Dust darkDust = Dust.NewDustDirect(NPC.Center - new Vector2(35, 24), 70, 80, DustID.Wraith, 0, -3, 9, NightBlack, 1f);
                darkDust.noGravity = true;
                darkDust.velocity = darkDust.position.DirectionFrom(NPC.Center) * (NPC.Distance(glowDust.position) * 0.3f);
            }
            if (Phase == 3)
            {
                Color color = NightColor(0);
                color.A = 0;
                for (int i = 0; i < Main.rand.Next(70, 120); i++)
                {
                    if (PhaseCounter <= 220 && PhaseCounter > 40 && PhaseCounter % 54 == 0)
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
            if (Phase == 4)
            {
                Color glowColor = NightColor(Utils.GetLerpValue(50, 0, PhaseCounter, true));
                glowColor.A /= 5;
                Vector2 circle = Main.rand.NextVector2CircularEdge(80, 80);
                Vector2 velocity = Main.rand.NextVector2Circular(10, 10);
                Dust dust = Dust.NewDustDirect(NPC.Center + circle, 2, 2, GlowDustID, -velocity.X, -velocity.Y, 0, glowColor, 1);
                dust.noGravity = true;
            }
        }

        #endregion
    }
}
