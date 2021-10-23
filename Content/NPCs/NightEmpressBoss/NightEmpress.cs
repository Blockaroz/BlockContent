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
        public ref float FreeToUse => ref NPC.ai[3];

        public override void SendExtraAI(BinaryWriter writer)
        {
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
        }

        public override void AI()
        {
            NPCAimedTarget target = NPC.GetTargetData();
            Vector2 targetPos = target.Invalid ? NPC.Center : target.Center;
            Vector2 targetPosOffset = new Vector2(0, -250);

            HandleDamageValues();

            if (PhaseCounter == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                NPC.TargetClosest();

            if (Main.netMode != NetmodeID.MultiplayerClient && Phase != 0)
                TryDespawn(target);

            if (Phase == 0) //spawn
            {
                PhaseCounter++;
                if (PhaseCounter == 5)
                    SoundEngine.PlaySound(SoundID.Item160, NPC.Center);

                float yLerp = Utils.GetLerpValue(0, 140, PhaseCounter, true);

                NPC.velocity.Y = MathHelper.SmoothStep(0.5f, 0f, yLerp);

                NPC.dontTakeDamage = true;

                if (PhaseCounter >= 240)
                {
                    NPC.dontTakeDamage = false;
                    Phase++;
                    PhaseCounter = -1;
                }
            }

            if (Phase == 1)//Shooting Stars
            {
                PhaseCounter++;
                const int attackLength = 270;
                const int blastTime = 100;
                const int blastTimeSecond = 180;

                int direction = Main.rand.NextBool().ToDirectionInt();
                if (PhaseCounter == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                    direction = Main.rand.NextBool().ToDirectionInt();

                if (PhaseCounter <= attackLength)
                {
                    float offsetX = (float)Math.Sin((MathHelper.Pi * PhaseCounter) / (attackLength / 3)) * direction;
                    float offsetY = (float)Math.Cos((MathHelper.Pi * PhaseCounter) / (attackLength / 6));
                    Vector2 followPos = Vector2.Lerp(new Vector2(offsetX * 500, (offsetY * 80) - 250), targetPosOffset, Utils.GetLerpValue(blastTimeSecond, blastTimeSecond + 20, PhaseCounter, true));
                    Vector2 dist = NPC.Center - (targetPos + followPos);
                    float speed = MathHelper.Lerp(1, 3, dist.Length() / 200);

                    MoveToTarget(target, speed, 15, followPos);

                    if (PhaseCounter <= blastTime)
                    {
                        //sound
                        ShootingStars(6, offsetX);
                    }

                    if (PhaseCounter > blastTime + 20 && PhaseCounter <= blastTimeSecond)
                        ShootingStars(9, offsetX);
                }

                if (PhaseCounter > attackLength + 5)
                {
                    Phase++;
                    PhaseCounter = -1;
                }
            }

            if (Phase == 2)//Explo
            {
                PhaseCounter++;

                const int attackLength = 240;
                const int begin = 10;

                if (PhaseCounter < begin)
                    MoveToTarget(target, 10f, 10, targetPosOffset);

                if (PhaseCounter == begin)
                {
                    SoundEngine.PlaySound(Mod.GetLegacySoundSlot(Terraria.ModLoader.SoundType.Item, "Assets/Sounds/Item/NightEmpress/eonExplosionCharge"), NPC.Center);
                    NPC.velocity = Vector2.Zero;
                    Projectile radial = Projectile.NewProjectileDirect(NPC.GetProjectileSpawnSource(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<RuneCircleBomb>(), damageValue[1], 0);
                    radial.ai[0] = 180;
                }

                if (PhaseCounter <= begin - 5)
                    MoveToTarget(target, 20f, 0, targetPosOffset);

                if (PhaseCounter > attackLength)
                {
                    Phase++;
                    PhaseCounter = -1;
                }
            }

            if (Phase == 3)//Flowering Night
            {
                PhaseCounter++;

                const int attackLength = 200;
                const int dashCap = 10;
                const int doAttack = 10;
                const int doAttackSecond = 40;

                if (PhaseCounter <= attackLength)
                {
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

                if (PhaseCounter > attackLength && PhaseCounter <= attackLength + dashCap)
                    DashToTarget(target, targetPosOffset);

                if (PhaseCounter > attackLength + dashCap + 5)
                {
                    Phase = 1;
                    PhaseCounter = -1;
                }
            }

            if (Phase == null)
            {
                PhaseCounter++;

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
        }

        private void Debug(NPCAimedTarget target)
        {
            if (PhaseCounter == 0)
            {
                Main.NewText("Phase: " + Phase, Color.Red);
                Main.NewText("PhaseCounter: " + PhaseCounter, Color.DarkRed);
                Main.NewText(target, Color.DarkGoldenrod);
                for (int i = 0; i < damageValue.Length; i++)
                {
                    Main.NewText("damageValue[" + i + "]: " + damageValue[i], Color.MediumPurple);
                }
            }
        }

        private bool NightRage()
        {
            if (!Main.dayTime)
                NPC.ai[3] = 1;

            return NPC.ai[3] == 1;
        }

        public void TryDespawn(NPCAimedTarget target)
        {
            bool shouldDespawn = false;

            if (!shouldDespawn)
            {
                bool targetInvalid = target.Invalid || NPC.Distance(target.Center) > 7200f;
                bool overTime = false;
                if (NightRage())
                {
                    if (Main.dayTime || (!Main.dayTime && Main.time >= 31800.0))
                        overTime = true;
                }
                shouldDespawn = shouldDespawn || overTime || targetInvalid;
            }

            if (shouldDespawn)
            {
                NPC.dontTakeDamage = true;
                if (PhaseCounter == 0 && Phase > 0)
                    Phase = float.MinValue;

                NPC.velocity *= 0.7f;
                
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

        public int[] damageValue = new int[8];

        public void HandleDamageValues()
        {
            damageValue[0] = 90;//Contact Damage
            //Note: Projectile damage is multiplied by 2 by default.
            damageValue[1] = 95;//Explosion
            damageValue[2] = 52;//Flowering Night
            damageValue[3] = 36; //Star
            damageValue[4] = 0;
            damageValue[5] = 0;
            damageValue[6] = 0;
            damageValue[7] = 0;

            if (Phase == 0 || Phase == 2)
                damageValue[0] = -1;

            NPC.damage = damageValue[0];

            for (int i = 0; i < damageValue.Length; i++)
            {
                if (NightRage())
                    damageValue[i] = 9999;
            }
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
                if (NPC.velocity.Length() < 0.0045f)
                    NPC.velocity = Vector2.Zero;
            }

            if (NPC.velocity.Length() > 1.5f)
                NPC.velocity *= 0.95f;
        }

        public void DashToTarget(NPCAimedTarget target, Vector2 offset)
        {
            NPC.TargetClosest();
            Vector2 targetPos = target.Invalid ? NPC.Center : (target.Center + offset);
            if (NPC.Distance(targetPos + offset) > 200f)
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

        public void ShootingStars(int interval, float velocityX)
        {
            if (PhaseCounter % interval == 0)
            {
                Projectile shootingStar = Projectile.NewProjectileDirect(NPC.GetProjectileSpawnSource(), NPC.Center + new Vector2(0, -8), NPC.velocity + new Vector2(Main.rand.Next(-7, 7) + velocityX, Main.rand.Next(-20, -16)), ModContent.ProjectileType<ShootingStar>(), damageValue[3], 0);
                shootingStar.ai[0] = NPC.target;
                shootingStar.ai[1] = -velocityX;
            }

            //if (PhaseCounter == 0)
            //SoundEngine.PlaySound
        }

        public void FloweringNight(int totalProjectiles, float rotOffset, Vector2 velocity)
        {
            int projType = ModContent.ProjectileType<NightFlower>();
            for (int i = 0; i < totalProjectiles * 2; i++)
            {
                float direction = i > totalProjectiles ? 1 : -1;
                float rotation = ((MathHelper.TwoPi / totalProjectiles) * i) + rotOffset;
                Projectile flowerProj = Main.projectile[Projectile.NewProjectile(NPC.GetProjectileSpawnSource(), NPC.Center + new Vector2(0, -24), velocity.RotatedBy(rotation), projType, damageValue[2], 0)];
                flowerProj.ai[1] = direction;
            }
        }

        #endregion

        #region Visual

        public static Color NightColor(float t, bool useSecondColor = false)
        {
            Color light = new Color(162, 95, 234);
            Color dark = new Color(73, 30, 123);//new Color(105, 0, 205);
            Color lightEnrage = new Color(154, 54, 255);
            Color darkEnrage = new Color(73, 30, 123);

            if (useSecondColor == true)
                lightEnrage = new Color(255, 200, 100);

            if (!Main.dayTime)
                return Color.Lerp(lightEnrage, darkEnrage, t);

            return Color.Lerp(light, dark, t);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Asset<Texture2D> body = Mod.Assets.Request<Texture2D>("Content/NPCs/NightEmpressBoss/NightEmpress");

            Rectangle? frame = body.Frame(1, 2, 0, 0);

            drawColor = Color.White;

            DrawSpawnColor(out drawColor);

            DrawWings(spriteBatch, screenPos, drawColor);
            spriteBatch.Draw(body.Value, NPC.Center - screenPos, frame, drawColor, NPC.rotation, new Vector2(frame.Value.Width, frame.Value.Height) / 2, NPC.scale, SpriteEffects.None, 0);

            return false;
        }

        public void DrawWings(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Asset<Texture2D> wings = Mod.Assets.Request<Texture2D>("Content/NPCs/NightEmpressBoss/NightEmpress_Wings");
            Asset<Texture2D> wingsGlow = Mod.Assets.Request<Texture2D>("Content/NPCs/NightEmpressBoss/NightEmpress_WingsGlow");

            NPC.localAI[0]++;
            int wingCount = (int)(NPC.localAI[0] / 3f) % 11;
            Rectangle? frame = wings.Frame(1, 11, 0, wingCount);

            spriteBatch.Draw(wings.Value, NPC.Center - screenPos, frame, drawColor, NPC.rotation, new Vector2(frame.Value.Width, frame.Value.Height) / 2, NPC.scale * 2f, SpriteEffects.None, 0);
        }

        public void DrawSpawnColor(out Color drawColor)
        {
            drawColor = Color.White;
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
            if (Phase <= float.MinValue)
            {
                //despawn animation
                float colorFade = Utils.GetLerpValue(95, 120, PhaseCounter, true);
                float firstFade = Utils.GetLerpValue(70, 110, PhaseCounter, true);
                float appearFade = Utils.GetLerpValue(0, 70, PhaseCounter, true);
                Color changeColor = Color.Lerp(NightColor(colorFade), Color.Transparent, firstFade);
                changeColor.A = (byte)(Utils.GetLerpValue(90, 120, PhaseCounter, true) * 5);
                NPC.Opacity = appearFade;
                drawColor = Color.Lerp(Color.White, changeColor, appearFade);

                Dust glowDust = Dust.NewDustDirect(NPC.Center - new Vector2(35, 48), 70, 80, DustID.AncientLight, 0, 0, 9, NightColor(colorFade), 1f);
                glowDust.noGravity = true;
                glowDust.noLight = true;
                glowDust.velocity = glowDust.position.DirectionFrom(NPC.Center) * (NPC.Distance(glowDust.position) * 0.2f);
                glowDust.velocity *= 0.93f;

                Dust darkDust = Dust.NewDustDirect(NPC.Center - new Vector2(35, 24), 70, 80, DustID.Wraith, 0, -3, 9, Color.Gray, 1.5f);
                darkDust.noGravity = true;
                darkDust.noLight = true;
                darkDust.velocity = darkDust.position.DirectionFrom(NPC.Center) * (NPC.Distance(glowDust.position) * 0.3f);
            }
        }

        #endregion
    }
}
