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
        public ref float State => ref NPC.ai[3];

        public override void SendExtraAI(BinaryWriter writer)
        {
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
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
                TryPhaseTwo();
            }

            if (Phase == 0) //Spawn
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

                //We check if the timer is below the limit to allow for 5 frames of stillness.
                if (PhaseCounter > attackLength && PhaseCounter <= attackLength + dashCap)
                    DashToTarget(targetPos + targetPosOffset);

                if (PhaseCounter > attackLength + dashCap + 5)
                {
                    PhaseCounter = -1;
                    Phase++;
                }
            }

            if (Phase == 2)//Shooting Star Barrage
            {
                PhaseCounter++;
                const int attackLength = 270;
                const int dashCap = 10;
                const int blastTime = 100;
                const int blastTimeSecond = 180;

                if (PhaseCounter <= attackLength)
                {
                    float offsetX = (float)Math.Cos((MathHelper.Pi * PhaseCounter) / (attackLength / 3)) * _direction;
                    float offsetY = (float)Math.Sin((MathHelper.Pi * PhaseCounter) / (attackLength / 6));
                    Vector2 followPos = new Vector2(offsetX * 500, (offsetY * 80) - 250);

                    MoveToTarget(targetPos + followPos, 5, 15);

                    if (PhaseCounter <= blastTime)
                    {
                        //sound
                        ShootingStars(6, offsetX);
                    }

                    if (PhaseCounter > blastTime + 20 && PhaseCounter <= blastTimeSecond)
                        ShootingStars(10, offsetX);
                }

                if (PhaseCounter > attackLength && PhaseCounter <= attackLength + dashCap)
                    DashToTarget(targetPos + targetPosOffset);

                if (PhaseCounter > attackLength + dashCap + 5)
                {
                    Phase++;
                    PhaseCounter = -1;
                }
            }

            if (Phase == 3)//Explo
            {
                PhaseCounter++;

                const int attackLength = 390;
                const int begin = 30;
                const int explosion = 180;
                //This is not for the explosion, its just so the empress stops before she does the explosion

                if (PhaseCounter < explosion)
                    MoveToTarget(targetPos + new Vector2(-100 * _direction, -400), 0.5f, 10);
                else
                    NPC.velocity *= 0.2f;

                if (PhaseCounter == begin)
                {
                    SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Assets/Sounds/NightEmpress/charg"), NPC.Center);
                    Projectile radial = Projectile.NewProjectileDirect(NPC.GetProjectileSpawnSource(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<RuneCircle>(), damageValue[1], 0);
                    radial.ai[0] = 180;
                    radial.ai[1] = NPC.whoAmI;
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
                const int attackLength = 200;
                const int doAttack = 20;
                const int doAttackSecond = 50;

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

                if (PhaseCounter > attackLength + 5)
                {
                    Phase = 1;
                    PhaseCounter = -1;
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
                {
                    Phase = float.MinValue;
                    SoundEngine.PlaySound(SoundID.Item162, NPC.Center);
                }

                NPC.velocity.X *= 0.2f;
                NPC.velocity.Y = MathHelper.SmoothStep(0, -5, Utils.GetLerpValue(0, 125, PhaseCounter, true));
                
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

        public void TryPhaseTwo()
        {
            bool shouldDoPhase2 = false;

            if (shouldDoPhase2)
            {
                NPC.dontTakeDamage = true;
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

            if (Phase == 0)
                damageValue[0] = -1;

            NPC.damage = damageValue[0];

            for (int i = 0; i < damageValue.Length; i++)
            {
                if (NightRage() || Main.getGoodWorld)
                    damageValue[i] = 9999;
            }
        }

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

        #region Attacks

        public void ShootingStars(int interval, float velocityX)
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
            Color lightEnrage = new Color(154, 54, 255);
            Color dark = new GradientColor(new Color[] { new Color(40, 40, 128), new Color(73, 30, 123) }, 2, 1).Value;
            //new Color(73, 30, 123);
            //new Color(105, 0, 205);

            if (useSecondColor == true)
                lightEnrage = new Color(255, 200, 100);

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
            Asset<Texture2D> body = Mod.Assets.Request<Texture2D>("Content/NPCs/NightEmpressBoss/NightEmpress");

            Rectangle? frame = body.Frame(1, 2, 0, 0);

            drawColor = Color.White;

            if (Phase == 0)
                DrawSpawnColor(out drawColor);

            if (Phase == 3)
                DrawRuneCircle(spriteBatch, screenPos);

            DrawWings(spriteBatch, screenPos, drawColor);
            if (State == 0)
            {
            }

            spriteBatch.Draw(body.Value, NPC.Center - screenPos, frame, drawColor, NPC.rotation, new Vector2(frame.Value.Width, frame.Value.Height) / 2, NPC.scale, SpriteEffects.None, 0);

            DoDust();

            return false;
        }

        public void DrawWings(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Asset<Texture2D> wings = Mod.Assets.Request<Texture2D>("Content/NPCs/NightEmpressBoss/NightEmpress_Wings");
            Asset<Texture2D> wingsGlow = Mod.Assets.Request<Texture2D>("Content/NPCs/NightEmpressBoss/NightEmpress_WingsGlow");

            NPC.localAI[0]++;
            int wingCount = (int)(NPC.localAI[0] / 3f) % 11;
            Rectangle? frame = wings.Frame(1, 11, 0, wingCount);
            spriteBatch.Draw(wings.Value, NPC.Center - screenPos, frame, drawColor, NPC.rotation, new Vector2(frame.Value.Width / 2, frame.Value.Height / 2), NPC.scale * 2, SpriteEffects.None, 0);
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
            }
        }

        public void DrawRuneCircle(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            Asset<Texture2D>[] runeCircle = new Asset<Texture2D>[3];
            for (int i = 0; i < runeCircle.Length; i++)
                runeCircle[i] = Mod.Assets.Request<Texture2D>("Assets/Textures/NightEmpress/RuneCircle_" + i);
            Asset<Texture2D> font = Mod.Assets.Request<Texture2D>("Assets/Textures/NightEmpress/Runes");
            Asset<Texture2D> blackFade = Mod.Assets.Request<Texture2D>("Assets/Textures/LargeGlowball");

            float scaleLerp = MathHelper.SmoothStep(0, 1, MoreUtils.DualLerp(0, 80, 330, 400, PhaseCounter, true));
            float colorLerp = MathHelper.SmoothStep(0, 1, MoreUtils.DualLerp(20, 80, 300, 360, PhaseCounter, true));

            float cClockwise = (Main.GlobalTimeWrappedHourly % 360) * MathHelper.ToRadians(-15);
            float clockwise = (Main.GlobalTimeWrappedHourly % 360) * MathHelper.ToRadians(28);

            Color lightShade = NightColor(0) * colorLerp;
            lightShade.A /= 4;
            Color darkShade = NightColor(0.66f) * colorLerp;
            darkShade.A /= 4;

            //draw back fade
            spriteBatch.Draw(blackFade.Value, NPC.Center - screenPos, null, Color.Black * colorLerp * 0.9f, 0, blackFade.Size() / 2, 0.5f + scaleLerp, SpriteEffects.None, 0);

            //draw circles
            for (int i = 0; i < 2; i++)
            {
                Vector2 pos = NPC.Center + new Vector2(0.66f, 0).RotatedBy(MoreUtils.GetCircle(i, 4));
                spriteBatch.Draw(runeCircle[0].Value, pos - screenPos, null, lightShade, cClockwise, runeCircle[0].Size() / 2, scaleLerp, SpriteEffects.None, 0);
                spriteBatch.Draw(runeCircle[1].Value, pos - screenPos, null, darkShade, clockwise, runeCircle[0].Size() / 2, scaleLerp, SpriteEffects.None, 0);
                spriteBatch.Draw(runeCircle[2].Value, pos - screenPos, null, lightShade, 0, runeCircle[0].Size() / 2, scaleLerp, SpriteEffects.None, 0);
            }

            //draw text
            int[] character = new int[]
            {
                //moonburn
                //fivetwozeroeight
                //ondiscord
                //isanawesomeandlikeableperson
                13, 15, 15, 14, 2, 21, 18, 14, 
                6, 9, 22, 5, 20, 23, 15, 26, 5, 18, 15, 5, 9, 7, 8, 20, 
                15, 14, 4, 9, 19, 3, 15, 18, 4, 
                9, 19, 1, 14, 1, 23, 5, 19, 15, 13, 5, 1, 14, 4, 12, 9, 11, 5, 1, 2, 12, 5, 16, 5, 18, 19, 15, 14
            };
            for (int i = 0; i < character.Length; i++)
            {
                Rectangle? frame = font.Frame(26, 1, character[i], 0);
                float runeRotation = ((MathHelper.TwoPi / character.Length) * i) + (cClockwise * 2);
                Vector2 circlePos = Vector2.Lerp(Vector2.Zero, Vector2.UnitX * 186, scaleLerp).RotatedBy(runeRotation);
                for (int j = 0; j < 4; j++)
                {
                    Vector2 characterGlowOffset = circlePos + new Vector2(1.5f * scaleLerp, 0).RotatedBy(((MathHelper.TwoPi / 4) * j) + MathHelper.PiOver4);
                    spriteBatch.Draw(font.Value, NPC.Center + characterGlowOffset - screenPos, frame, lightShade, runeRotation + MathHelper.PiOver2, new Vector2(frame.Value.Width / 2, frame.Value.Height / 2) * colorLerp, scaleLerp, SpriteEffects.None, 0);
                }
                spriteBatch.Draw(font.Value, NPC.Center + circlePos - screenPos, frame, new Color(255, 255, 255, 0) * colorLerp, runeRotation + MathHelper.PiOver2, new Vector2(frame.Value.Width / 2, frame.Value.Height / 2), scaleLerp, SpriteEffects.None, 0);
            }
        }

        public static int GlowDustID { get => ModContent.DustType<Dusts.GlowballDust>(); }

        public void DoDust()
        {
            if (Phase <= float.MinValue)
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

            if (Phase == 4)
            {
                const int finish = 70;
                if (PhaseCounter < finish)
                {
                    Color glowColor = NightColor(Utils.GetLerpValue(50, 0, PhaseCounter, true));
                    glowColor.A /= 5;
                    Vector2 circle = Main.rand.NextVector2CircularEdge(80, 80);
                    Vector2 velocity = Main.rand.NextVector2Circular(10, 10);
                    Dust dust = Dust.NewDustDirect(NPC.Center + circle, 2, 2, GlowDustID, -velocity.X, -velocity.Y, 0, glowColor, 1);
                    dust.noGravity = true;
                }
            }
        }

        #endregion
    }
}
