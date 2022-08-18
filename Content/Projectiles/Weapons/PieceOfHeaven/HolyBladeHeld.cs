using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleEngine;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.Graphics;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContent.Content.Projectiles.Weapons.PieceOfHeaven
{
    public class HolyBladeHeld : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Blade");
            ProjectileID.Sets.TrailingMode[Type] = 4;
            ProjectileID.Sets.TrailCacheLength[Type] = 7;
            ProjectileID.Sets.CanDistortWater[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.manualDirectionChange = true;
            Projectile.noEnchantmentVisuals = true;
            Projectile.hide = true;

            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.netImportant = true;
        }

        private Player Player { get => Main.player[Projectile.owner]; }
        private bool allowKill = false;
        private float slashProgress;

        private float SpeedMod { get => Player.itemAnimationMax / 18f; }

        public override void AI()
        {
            Projectile.timeLeft = 5;
            Player.ChangeDir(Projectile.spriteDirection);
            Player.heldProj = Projectile.whoAmI;

            if (Player.dead || Player.whoAmI != Projectile.owner)
                Projectile.Kill();

            if (Projectile.ai[1] == 0)
            {
                Projectile.direction *= -1;
                Projectile.velocity = Projectile.DirectionTo(Main.MouseWorld).RotatedByRandom(0.1f) * 5;
                Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
                Projectile.localNPCHitCooldown = (int)(18f * SpeedMod);
                Projectile.netUpdate = true;
            }

            switch (Projectile.ai[0])
            {
                case 0:
                    Projectile.ai[1]++;
                    Slash_Default();
                    break;

                case 1:
                    if (Projectile.ai[1] < 1)
                        Projectile.ai[1] = 1;
                    Parrying();
                    break;

                case 2:
                    Projectile.ai[1]++;
                    Slash_Enhanced();
                    break;
            }

            if (allowKill)
            {
                if (!Player.channel || Player.CCed || Player.noItems)
                    Projectile.Kill();

                else
                {
                    Projectile.ai[1] = 0;
                    allowKill = false;
                }
            }

            if (Projectile.localAI[0] > 0)
                Projectile.localAI[0]--;

            Lighting.AddLight(Projectile.Center + Projectile.velocity, HeavenColors.Melee.ToVector3() * 0.3f);
        }

        private float slashEase(float x) //basically ark sword curve
        {
            float[] prog = new float[4];
            prog[0] = 0.15f * (1f - (float)Math.Pow(10f, -3f * x));
            prog[1] = 4f * (float)Math.Pow(x - 0.15f, 2.95f) + 0.1f;
            prog[2] = 0.95f - ((float)Math.Pow(-1.75f * x + 2f, 4f) / 2f);
            prog[3] = 0.4f * x + 0.58f;

            if (x <= 0f)
                return 0f;
            if (x < 0.16f)
                return prog[0];
            else if (x < 0.55f)
                return prog[1];
            else if (x < 0.85f)
                return prog[2];

            return prog[3];
        }

        private void Slash_Default()
        {
            Player.SetDummyItemTime(5);

            slashProgress = slashEase(Utils.GetLerpValue(0, Player.itemAnimationMax, Projectile.ai[1], true));
            float swordRot = (slashProgress - (Projectile.direction > 0 ? 0.8f : 0.5f)) * MathHelper.ToRadians(230) * Projectile.direction * Projectile.spriteDirection;

            Projectile.rotation = Projectile.velocity.ToRotation() + swordRot;
            Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.velocity.ToRotation() + swordRot * Player.gravDir - MathHelper.PiOver2 - Player.fullRotation);
            Projectile.Center = Player.RotatedRelativePointOld(Player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2 - Player.fullRotation), true);

            Vector2 bladeEnd = Projectile.Center + new Vector2(80, 0).RotatedBy(Projectile.velocity.ToRotation() + swordRot * 0.7f);
            hitbox.Width = (int)(220 * Projectile.scale);
            hitbox.Height = (int)(220 * Projectile.scale);
            hitbox.X = (int)bladeEnd.X - hitbox.Width / 2;
            hitbox.Y = (int)bladeEnd.Y - hitbox.Height / 2;

            if (Projectile.ai[1] == (int)(8 * SpeedMod))
            {
                SoundStyle swingSound = SoundID.DD2_MonkStaffSwing;
                swingSound.Pitch = -SpeedMod + 1f;
                swingSound.PitchVariance = 0.3f;
                swingSound.MaxInstances = 0;
                SoundEngine.PlaySound(swingSound, Projectile.Center);
            }

            if (slashProgress > 0.3f && slashProgress < 0.95f && Main.rand.NextBool())
            {
                Color randColor = Color.Lerp(HeavenColors.Melee, HeavenColors.MeleeDark, Main.rand.Next(2));
                Vector2 starPos = Projectile.Center + new Vector2(80, -11 * Projectile.direction * Projectile.spriteDirection).RotatedBy(Projectile.rotation) * visualScale;
                Particle.NewParticle(Particle.ParticleType<Particles.HeavenSpark>(), starPos, Projectile.rotation.ToRotationVector2() * 3f, randColor, Main.rand.NextFloat(0.3f, 0.6f));
            }

            if (Projectile.ai[1] >= Player.itemAnimationMax)
                allowKill = true;

        }

        private void Slash_Enhanced()
        {
            Player.SetDummyItemTime(5);
            int extraTime = (int)(22 * SpeedMod);

            float drawBackProg = slashEase(Utils.GetLerpValue(0, (Player.itemAnimationMax + extraTime) / 2f, Projectile.ai[1], true));
            float drawBack = drawBackProg * MathHelper.TwoPi - MathHelper.Lerp(MathHelper.ToRadians(-80), MathHelper.ToRadians(20), drawBackProg);
            float swingProg = slashEase(Utils.GetLerpValue((Player.itemAnimationMax + extraTime) / 3f, Player.itemAnimationMax + extraTime, Projectile.ai[1], true));
            float swing = (swingProg - 0.5f) * MathHelper.ToRadians(300);
            slashProgress = Utils.GetLerpValue(0, 2f, (drawBackProg + swingProg), true);

            float swordRot = (drawBack + swing) * Projectile.direction * Projectile.spriteDirection;

            Projectile.rotation = Projectile.velocity.ToRotation() + swordRot;
            Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.velocity.ToRotation() + swordRot * Player.gravDir - MathHelper.PiOver2 - Player.fullRotation);
            Projectile.Center = Player.RotatedRelativePointOld(Player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2 - Player.fullRotation), true);

            Vector2 bladeEnd = Projectile.Center + new Vector2(80, 0).RotatedBy(Projectile.velocity.ToRotation() + swordRot);
            hitbox.Width = (int)(255 * Projectile.scale);
            hitbox.Height = (int)(255 * Projectile.scale);
            hitbox.X = (int)bladeEnd.X - hitbox.Width / 2;
            hitbox.Y = (int)bladeEnd.Y - hitbox.Height / 2;

            if (Projectile.ai[1] == (int)(8 * SpeedMod))
            {
                SoundStyle swingSound = SoundID.DD2_MonkStaffSwing;
                swingSound.Volume *= 0.2f;
                swingSound.Pitch = -SpeedMod + 0.8f;
                swingSound.PitchVariance = 0.3f;
                swingSound.MaxInstances = 0;
                SoundEngine.PlaySound(swingSound, Projectile.Center);
            }
            if (Projectile.ai[1] == (int)(25 * SpeedMod))
            {
                SoundStyle swingSound = SoundID.DD2_MonkStaffSwing; //slicier
                swingSound.Pitch = -SpeedMod * 0.5f + 1f;
                swingSound.PitchVariance = 0.3f;
                swingSound.MaxInstances = 0;
                SoundEngine.PlaySound(swingSound, Projectile.Center);
            }

            if (slashProgress > 0.2f && slashProgress < 0.95f)
            {
                Color randColor = Color.Lerp(HeavenColors.Melee, HeavenColors.MeleeDark, Main.rand.Next(2));
                Vector2 starPos = Projectile.Center + new Vector2(80, -11 * Projectile.direction * Projectile.spriteDirection).RotatedBy(Projectile.rotation) * visualScale * Main.rand.NextFloat(0.9f, 1.1f);
                Particle.NewParticle(Particle.ParticleType<Particles.HeavenSpark>(), starPos, Projectile.rotation.ToRotationVector2() * 3f, randColor, Main.rand.NextFloat(0.3f, 0.6f));
            }

            if (Projectile.ai[1] >= Player.itemAnimationMax + extraTime)
            {
                allowKill = true;
                Projectile.ai[0] = 0;
            }
        }

        private void Parrying()
        {
            Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.velocity.ToRotation() * Player.gravDir - MathHelper.PiOver2 + MathHelper.PiOver4 * Player.direction);
            Projectile.Center = Player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.velocity.ToRotation() - MathHelper.PiOver2 + MathHelper.PiOver4 * Player.direction * Player.gravDir);

            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Main.MouseWorld) * 5, 0.1f);
            Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);

            Projectile.direction = (int)Player.gravDir;
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver4 * Projectile.direction * Player.direction;

            Vector2 parryHitboxPosition = Player.Center + new Vector2(20, 0).RotatedBy(Projectile.velocity.ToRotation());
            hitbox.Width = 120;
            hitbox.Height = 120;
            hitbox.X = (int)parryHitboxPosition.X - hitbox.Width / 2;
            hitbox.Y = (int)parryHitboxPosition.Y - hitbox.Height / 2;

            bool canGuard = !Player.mount.Active && Player.hasRaisableShield && Main.mouseRight && Projectile.ai[1] < 2;
            if (Player.whoAmI != Main.myPlayer)
                canGuard = Player.shieldRaised;

            if (!canGuard)
                Projectile.ai[1]++;

            if (Player.HasBuff(BuffID.ParryDamageBuff) && Player.channel)
            {
                Projectile.ai[0] = 2;
                Projectile.ai[1] = 0;
                Projectile.direction *= -1;
                canGuard = false;
            }

            Player.GetModPlayer<Common.Systems.ParryPlayer>().parry = canGuard;

            if (Projectile.ai[1] > 10f || Player.dead || Player.CCed || Player.noItems || Player.whoAmI != Projectile.owner)
                Projectile.Kill();
        }

        public override void Load() => On.Terraria.Player.CanParryAgainst += ParryConditions;

        private bool ParryConditions(On.Terraria.Player.orig_CanParryAgainst orig, Player self, Rectangle blockingPlayerRect, Rectangle enemyRect, Vector2 enemyVelocity) => orig(self, blockingPlayerRect, enemyRect, enemyVelocity) || orig(self, hitbox, enemyRect, enemyVelocity);

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(allowKill);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            allowKill = reader.ReadBoolean();
        }

        private Rectangle hitbox = new Rectangle(0, 0, 220, 220);

        public override void ModifyDamageHitbox(ref Rectangle hitbox) => hitbox = this.hitbox;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            bool defaultHit = Projectile.ai[0] == 0 && Projectile.ai[1] > 12 * SpeedMod;
            bool parryHit = Projectile.ai[0] == 1 && Player.shieldParryTimeLeft < Player.SHIELD_PARRY_DURATION / 2f && Player.shieldParryTimeLeft > 0;
            bool enhancedHit = Projectile.ai[0] == 2 && Projectile.ai[1] > 24 * SpeedMod;
            if (defaultHit || parryHit || enhancedHit)
                return projHitbox.Intersects(targetHitbox);

            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Projectile.ai[0] == 1)
            {   
                Player.velocity += Player.DirectionFrom(target.Center) * new Vector2(1.25f, 0.5f) * 5;
                Player.AddBuff(BuffID.ParryDamageBuff, 300, true);
                Player.parryDamageBuff = true;
                Player.immuneTime += 30;

                if (Projectile.localAI[0] == 0)
                {
                    Particle.NewParticle(Particle.ParticleType<Particles.ImpactParry>(), Player.Center + Projectile.rotation.ToRotationVector2() * 30, Projectile.velocity, HeavenColors.Melee, 1.5f);

                    SoundStyle parrySound = SoundID.Item81;
                    parrySound.MaxInstances = 0;
                    SoundEngine.PlaySound(parrySound, Projectile.Center);
                }
            }
            else
            {
                Projectile proj = Projectile.NewProjectileDirect(Terraria.Entity.InheritSource(Projectile), target.Center, Vector2.Zero, ModContent.ProjectileType<HolyBladeLight>(), Projectile.damage, Projectile.knockBack, Main.myPlayer, target.whoAmI);
                proj.rotation = Projectile.AngleTo(target.Center);

                if (Projectile.localAI[0] == 0)
                {
                    SoundStyle hitSound = SoundID.Item71;
                    hitSound.MaxInstances = 0;
                    SoundEngine.PlaySound(hitSound, Projectile.Center);
                }
            }

            if (Projectile.localAI[0] == 0)
                Projectile.localAI[0] = 5;
        }

        public override string Texture => $"{nameof(BlockContent)}/Content/Items/Weapons/PieceOfHeaven/HolyBlade";

        private Vector2 visualScale;

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Asset<Texture2D> shadow = ModContent.Request<Texture2D>(Texture + "Shadow");
            Asset<Texture2D> bloom = ModContent.Request<Texture2D>(Texture + "Bloom");
            Vector2 origin = texture.Size() * new Vector2(0.5f - 0.34f * Projectile.spriteDirection * Projectile.direction, 0.875f);
            SpriteEffects spriteDir = Projectile.spriteDirection * Projectile.direction < 0 ? SpriteEffects.FlipHorizontally : 0;
            float rotation = Projectile.rotation - MathHelper.PiOver4 * Projectile.spriteDirection * Projectile.direction + MathHelper.PiOver2;

            lightColor = Color.Lerp(Lighting.GetColor((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16), Color.White, 0.7f);
            Color glowColor = Color.Lerp(Color.GhostWhite, HeavenColors.Melee, 0.3f) * 0.3f;
            glowColor.A = 0;

            visualScale = Vector2.One;

            switch (Projectile.ai[0])
            {
                case 0:

                    visualScale = new Vector2(1.1f + slashProgress * (1f - slashProgress) * 3f) * Utils.GetLerpValue(-0.08f, 0.18f, slashProgress, true) * Projectile.scale;

                    Main.EntitySpriteDraw(shadow.Value, Projectile.Center - Main.screenPosition, null, Color.Black * 0.3f, rotation, origin + new Vector2(14), visualScale, spriteDir, 0);

                    if (Projectile.ai[1] > 6 * SpeedMod)
                        DrawTrail();

                    //Main.EntitySpriteDraw(TextureAssets.BlackTile.Value, hitbox.Center.ToVector2() - Main.screenPosition, hitbox, HeavenColors.Silver * 0.2f, 0, hitbox.Size() / 2f, 1f, 0, 0);

                    Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, null, lightColor, rotation, origin, visualScale, spriteDir, 0);
                    Main.EntitySpriteDraw(bloom.Value, Projectile.Center - Main.screenPosition, null, glowColor, rotation, origin + new Vector2(14), visualScale, spriteDir, 0);

                    break;

                case 1:

                    float holdOutOpacity = 1.2f - (float)Math.Sqrt(Projectile.ai[1] / 10f) * 0.5f;
                    Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, null, lightColor, rotation, origin, visualScale * holdOutOpacity, spriteDir, 0);
                    Main.EntitySpriteDraw(bloom.Value, Projectile.Center - Main.screenPosition, null, glowColor, rotation, origin + new Vector2(14), visualScale * holdOutOpacity, spriteDir, 0);

                    if (Player.shieldParryTimeLeft > 0)
                    {
                        float shieldScale = 1f + Player.shieldParryTimeLeft / (float)Player.SHIELD_PARRY_DURATION;
                        float shieldOpacity = (float)Math.Sqrt(1f - (Player.shieldParryTimeLeft / (float)Player.SHIELD_PARRY_DURATION));
                        Vector2 shieldOff = new Vector2(24 * Projectile.spriteDirection * Projectile.direction, -32).RotatedBy(rotation);
                        Main.EntitySpriteDraw(texture.Value, Projectile.Center + shieldOff - Main.screenPosition, null, glowColor * shieldOpacity * 1.5f, rotation, texture.Size() * 0.5f, visualScale * shieldScale, spriteDir, 0);
                    }

                    break;

                case 2:

                    float visTime = Utils.GetLerpValue(0.4f, 1f, slashProgress, true);
                    visualScale = new Vector2(1.2f + visTime * (1f - visTime) * 4f) * Utils.GetLerpValue(0f, 0.2f, slashProgress, true) * Projectile.scale;

                    if (Projectile.ai[1] > 3 * SpeedMod)
                        DrawTrail();

                    Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, null, lightColor, rotation, origin, visualScale, spriteDir, 0);
                    Main.EntitySpriteDraw(bloom.Value, Projectile.Center - Main.screenPosition, null, glowColor, rotation, origin + new Vector2(14), visualScale, spriteDir, 0);

                    break;
            }

            DrawSwipe();

            DrawStar();

            return false;
        }

        private void DrawTrail()
        {
            Asset<Texture2D> trail = ModContent.Request<Texture2D>(Texture + "Trail");
            Vector2 origin = trail.Size() * new Vector2(0.5f - 0.34f * Projectile.spriteDirection * Projectile.direction, 0.875f);
            SpriteEffects spriteDir = Projectile.spriteDirection * Projectile.direction < 0 ? SpriteEffects.FlipHorizontally : 0;

            int trailLength = ProjectileID.Sets.TrailCacheLength[Type];
            for (int i = 0; i < trailLength - 3; i++)
            {
                float betweens = 3;
                for (int j = 0; j < betweens; j++)
                {
                    Color fade = Color.Lerp(HeavenColors.Melee * 0.45f, Color.Transparent, Utils.GetLerpValue(0, trailLength - 3f, i + (j / betweens), true)) * Utils.GetLerpValue(0.15f, 0.5f, slashProgress, true);
                    fade.A = 0;
                    float oldRot = MathHelper.Lerp(Projectile.oldRot[i], Projectile.oldRot[i + 1], j / betweens) - MathHelper.PiOver4 * Projectile.spriteDirection * Projectile.direction + MathHelper.PiOver2;
                    Main.EntitySpriteDraw(trail.Value, Projectile.Center - Main.screenPosition, null, fade, oldRot, origin, visualScale, spriteDir, 0);
                }
            }
        }

        private void DrawStar()
        {
            Asset<Texture2D> star = TextureAssets.Extra[98];

            Color starColor = HeavenColors.Melee * 0.6f;
            starColor.A = 0;
            Color starGlowColor = Color.GhostWhite * 0.5f;
            starGlowColor.A = 0;
            int dir = Projectile.direction * Projectile.spriteDirection;
            Vector2 starPos = Projectile.Center + (new Vector2(80, -6 * dir) * visualScale).RotatedBy(Projectile.rotation);
            float starScale = Projectile.scale;

            switch (Projectile.ai[0])
            {
                case 0:

                    starScale = Projectile.scale * Utils.GetLerpValue(0.9f, 0.7f, slashProgress, true) * Utils.GetLerpValue(0.2f, 0.3f, slashProgress, true);
                    break;

                case 1:

                    return;

                case 2:

                    starScale = Projectile.scale * Utils.GetLerpValue(0.9f, 0.8f, slashProgress, true) * Utils.GetLerpValue(0.4f, 0.5f, slashProgress, true);
                    break;
            }

            Main.EntitySpriteDraw(star.Value, starPos - Main.screenPosition, null, starColor, 0f, star.Size() * 0.5f, new Vector2(0.7f, 1.7f) * starScale, 0, 0);
            Main.EntitySpriteDraw(star.Value, starPos - Main.screenPosition, null, starColor, MathHelper.PiOver2, star.Size() * 0.5f, new Vector2(0.7f, 2.5f) * starScale, 0, 0);
            Main.EntitySpriteDraw(star.Value, starPos - Main.screenPosition, null, starGlowColor, 0f, star.Size() * 0.5f, new Vector2(0.5f, 1f) * starScale, 0, 0);
            Main.EntitySpriteDraw(star.Value, starPos - Main.screenPosition, null, starGlowColor, MathHelper.PiOver2, star.Size() * 0.5f, new Vector2(0.5f, 1f) * starScale, 0, 0);
        }

        private void DrawSwipe()
        {
            Asset<Texture2D> swipe = ModContent.Request<Texture2D>($"{nameof(BlockContent)}/Content/Projectiles/Weapons/PieceOfHeaven/HolyBladeSlash");

            Color swipeColor = HeavenColors.MeleeDark;
            swipeColor.A = 10;
            SpriteEffects swipeDir = Projectile.direction < 0 ? SpriteEffects.FlipHorizontally : 0;

            switch (Projectile.ai[0])
            {
                case 0:

                    swipeColor *= Utils.GetLerpValue(0.6f, 0.7f, slashProgress, true) * Utils.GetLerpValue(0.95f, 0.8f, slashProgress, true);
                    float swipeRot = Projectile.velocity.ToRotation() + MathHelper.PiOver2 - (Projectile.direction * Projectile.spriteDirection > 0 ? MathHelper.ToRadians(40) : -MathHelper.ToRadians(20));
                    Vector2 swipePos = Projectile.Center + new Vector2(0, -10).RotatedBy(swipeRot);
                    float swipeScale = (1f + slashProgress * (1f - slashProgress) * 2.5f) * Projectile.scale;

                    Main.EntitySpriteDraw(swipe.Value, swipePos - Main.screenPosition, null, swipeColor, swipeRot, swipe.Size() * 0.5f, swipeScale * 0.7f, swipeDir, 0);

                    break;                
                
                case 2:
                    swipeColor = HeavenColors.Melee * 0.6f * Utils.GetLerpValue(0.3f, 0.7f, slashProgress, true) * Utils.GetLerpValue(1f, 0.9f, slashProgress, true);
                    swipeColor.A = 10;
                    swipeRot = Projectile.velocity.ToRotation() + MathHelper.PiOver2 + 7 * slashProgress * Projectile.direction * Projectile.spriteDirection;
                    swipePos = Projectile.Center + new Vector2(0, -10).RotatedBy(swipeRot);

                    float visTime = Utils.GetLerpValue(0.4f, 1f, slashProgress, true);
                    swipeScale = (1f + visTime * (1f - visTime) * 2.8f) * Projectile.scale;

                    Main.EntitySpriteDraw(swipe.Value, swipePos - Main.screenPosition, null, swipeColor, swipeRot, swipe.Size() * 0.5f, swipeScale * 0.7f, swipeDir, 0);

                    break;
            }
        }
    }
}
