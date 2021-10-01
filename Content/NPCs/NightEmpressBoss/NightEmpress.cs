using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContent.Content.NPCs.NightEmpressBoss
{
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

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
        }

        public override void SetDefaults()
        {
            NPC.width = 102;
            NPC.height = 168;
            NPC.noGravity = true;
            NPC.friendly = false;

            NPC.lifeMax = 90000;
            NPC.defense = 50;
            NPC.damage = NPC.GetAttackDamage_ScaledByStrength(90);
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(gold: 30);

            NPC.boss = true;
            //BossBag = ModContent.ItemType<>();
            NPC.npcSlots = 15f;
            NPC.noTileCollide = true;

            NPC.HitSound = SoundID.NPCHit1;

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

            if (Phase == 0)
            {
                //something
                if (PhaseCounter++ >= 240)
                {
                    PhaseCounter = 0;
                    Phase++;
                }
            }

            if (Phase == 1)
            {
                PhaseCounter++;
                if (PhaseCounter == 0)
                    NPC.TargetClosest();

                const int attackLength = 180;
                const int doAttack = 20;
                const int doAttackAgain = 60;

                if (PhaseCounter <= attackLength)
                {
                    MoveToTarget(target, 0.7f, 10);

                    if (PhaseCounter >= doAttack && PhaseCounter <= doAttackAgain)
                    {
                        NPC.velocity *= 0.1f;

                        float rotationOffset = Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4);
                        Vector2 velocity = new Vector2(7f, 0);

                        if (PhaseCounter == doAttack)
                            FloweringNight(9, rotationOffset, velocity);

                        if (PhaseCounter == doAttackAgain)
                            FloweringNight(9, -rotationOffset + MathHelper.PiOver4, -velocity);
                    }
                }

                if (PhaseCounter > attackLength && PhaseCounter <= attackLength + 10)
                    DashToTarget(target);

                if (PhaseCounter > attackLength + 15)
                {
                    PhaseCounter = 0;
                    Phase++;
                }
            }

            if (Phase == 2)
            {
                PhaseCounter++;
                if (PhaseCounter == 0)
                    NPC.TargetClosest();

                const int attackLength = 120;

                Vector2 targetPos = target.Invalid ? NPC.Center : (target.Center + new Vector2(0, -250));

                if (PhaseCounter <= attackLength)
                {
                    MoveToTarget(target, 5f, 10);
                    //attack
                }

                if (PhaseCounter > attackLength && PhaseCounter <= attackLength + 10)
                    DashToTarget(target);

                if (PhaseCounter > attackLength + 15)
                {
                    PhaseCounter = 0;
                    Phase--;
                }
            }
        }

        #region Movement

        public void MoveToTarget(NPCAimedTarget target, float speed, float minimumDistance)
        {
            Vector2 targetPos = target.Invalid ? NPC.Center : (target.Center + new Vector2(0, -270));
            if (Vector2.Distance(NPC.Center, targetPos) >= minimumDistance)
                NPC.velocity += NPC.DirectionTo(targetPos).SafeNormalize(Vector2.Zero) * 0.25f * speed;
            else
            {
                NPC.velocity *= 0.95f;
                if (NPC.velocity.Length() < 0.0044f)
                    NPC.velocity = Vector2.Zero;
            }

            if (NPC.velocity.Length() > 1.5f)
                NPC.velocity *= 0.93f;
        }

        public void DashToTarget(NPCAimedTarget target)
        {
            NPC.TargetClosest();
            Vector2 targetPos = target.Invalid ? NPC.Center : (target.Center + new Vector2(0, -270));
            if (NPC.Distance(targetPos) > 200f)
                targetPos -= NPC.DirectionTo(targetPos) * 120f;

            Vector2 difference = targetPos - NPC.Center;
            float lerpValue = Utils.GetLerpValue(100f, 700f, difference.Length());
            float speed = difference.Length();
            if (speed > 20f)
                speed = 20f;

            NPC.velocity = Vector2.Lerp(difference.SafeNormalize(Vector2.Zero) * speed, difference / 6f, lerpValue);

            NPC.velocity *= 0.92f;
        }

        #endregion

        #region Attacks

        public void FloweringNight(int totalProjectiles, float rotOffset, Vector2 velocity)
        {
            int projType = ModContent.ProjectileType<Projectiles.NightFlower>();

            //sound
            for (int i = 0; i < totalProjectiles * 2; i++)
            {
                float direction = i > totalProjectiles ? 1 : -1;
                float rotation = ((MathHelper.TwoPi / totalProjectiles) * i) + rotOffset;
                Projectile flowerProj = Main.projectile[Projectile.NewProjectile(NPC.GetProjectileSpawnSource(), NPC.Center + new Vector2(0, 20).RotatedBy(rotation), velocity.RotatedBy(rotation), projType, NPC.GetAttackDamage_ForProjectiles_MultiLerp(120, 240, 360), 0)];
                flowerProj.ai[1] = direction;
            }
        }

        #endregion

        #region Visual

        public static Color NightColor(float t)
        {
            Color light = new Color(162, 95, 234);
            Color dark = new Color(87, 27, 169);
            Color lightEnrage = new Color(0, 0, 0);
            Color darkEnrage = new Color(0, 0, 0);

            if (NightRage())
                return Color.Lerp(lightEnrage, darkEnrage, t);

            return Color.Lerp(light, dark, t);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {

            if (Phase == 0)
            {
                float fadeIn = Utils.GetLerpValue(60, 0, PhaseCounter, true);
                Color fadeColor = NightColor(fadeIn);
                fadeColor.A = 0;
                NPC.Opacity = fadeIn;
                drawColor = Color.Lerp(fadeColor, Color.White, fadeIn);
                
            }

            Asset<Texture2D> body = Mod.Assets.Request<Texture2D>("Content/NPCs/NightEmpressBoss/NightEmpress");
            spriteBatch.Draw(body.Value, NPC.Center - screenPos, null, drawColor, NPC.rotation, body.Size() / 2, NPC.scale, SpriteEffects.None, 0);
            return false;
        }

        #endregion
    }
}
