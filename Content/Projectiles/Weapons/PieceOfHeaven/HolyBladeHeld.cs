using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleEngine;
using ReLogic.Content;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
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
            ProjectileID.Sets.TrailingMode[Type] = 2;
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
        public override void AI()
        {
            Projectile.timeLeft = 3;
            Player.SetDummyItemTime(2);
            Player.ChangeDir(Projectile.spriteDirection);
            Player.heldProj = Projectile.whoAmI;

            int dir = Projectile.direction * Projectile.spriteDirection;

            if (allowKill)
            {
                if (!Player.channel || Player.dead || Player.CCed || Player.noItems || Player.whoAmI != Projectile.owner)
                    Projectile.Kill();

                else
                {
                    Projectile.ai[1] = 0;
                    allowKill = false;
                }
            }

            if (Projectile.ai[1] == 0)
            {
                Projectile.direction *= -1;
                Projectile.velocity = Projectile.DirectionTo(Main.MouseWorld).RotatedByRandom(0.3f) * 5;
                Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
                Projectile.localNPCHitCooldown = (int)(20f * SpeedMod);
                Projectile.netUpdate = true;
            }

            switch (Projectile.ai[0])
            {
                case 0:

                    slashProgress = slashEase(Utils.GetLerpValue(0, Player.itemAnimationMax * 1.07f, Projectile.ai[1], true));
                    float swordRot = (slashProgress - (Projectile.direction > 0 ? 0.8f : 0.5f)) * MathHelper.ToRadians(220) * dir;
                    Projectile.rotation = Projectile.velocity.ToRotation() + swordRot;
                    Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.velocity.ToRotation() + swordRot * Player.gravDir * 0.97f - MathHelper.PiOver2 - Player.fullRotation);
                    Projectile.Center = Player.RotatedRelativePointOld(Player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2 - Player.fullRotation), true);
                    
                    if (Projectile.ai[1] > Player.itemAnimationMax)
                        allowKill = true;

                    if (Projectile.ai[1] == (int)(8 * SpeedMod))
                    {         
                        SoundStyle swingSound = SoundID.DD2_MonkStaffSwing;
                        swingSound.Pitch = -SpeedMod * 1.5f + 1.5f;
                        swingSound.PitchVariance = 0.3f;
                        swingSound.MaxInstances = 0;
                        SoundEngine.PlaySound(swingSound, Projectile.Center);
                    }

                    if (slashProgress > 0.02f && slashProgress < 0.95f && Main.rand.NextBool())
                    {
                        Color randColor = Color.Lerp(HeavenColors.Melee, HeavenColors.MeleeDark, Main.rand.Next(2));
                        Vector2 starPos = Projectile.Center + new Vector2(75, 0).RotatedBy(Projectile.rotation) * visualScale;
                        Particle.NewParticle(Particle.ParticleType<Particles.HeavenSpark>(), starPos, Projectile.rotation.ToRotationVector2().RotatedBy(MathHelper.Pi / 3f * dir) * 3f, randColor, Main.rand.NextFloat(0.3f, 0.6f));
                    }

                    Vector2 bladeEnd = Projectile.Center + new Vector2(80, 0).RotatedBy(Projectile.velocity.ToRotation() + swordRot * 0.5f);
                    hitbox.X = (int)bladeEnd.X - hitbox.Width / 2;
                    hitbox.Y = (int)bladeEnd.Y - hitbox.Height / 2;
                    hitbox.Width = (int)(220 * Projectile.scale);
                    hitbox.Height = (int)(220 * Projectile.scale);

                    break;
            }

            if (Projectile.localAI[0] > 0)
                Projectile.localAI[0]--;

            Projectile.ai[1]++;
            Projectile.rotation = MathHelper.WrapAngle(Projectile.rotation);

            Lighting.AddLight(Projectile.Center + Projectile.velocity, HeavenColors.Melee.ToVector3() * 0.3f);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(allowKill);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            allowKill = reader.ReadBoolean();
        }

        private Rectangle hitbox = new Rectangle(0, 0, 220, 220);

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.ai[0] == 0 && Projectile.ai[1] > 9 * SpeedMod)
                return hitbox.Intersects(targetHitbox);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Projectile proj = Projectile.NewProjectileDirect(Terraria.Entity.InheritSource(Projectile), target.Center, Vector2.Zero, ModContent.ProjectileType<HolyBladeLight>(), (int)(Projectile.damage * 1.1f), Projectile.knockBack, Main.myPlayer, target.whoAmI);
            proj.rotation = Projectile.AngleTo(target.Center);

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
            Asset<Texture2D> trail = ModContent.Request<Texture2D>(Texture + "Trail");
            Vector2 origin = texture.Size() * new Vector2(0.5f - 0.4f * Projectile.spriteDirection * Projectile.direction, 0.9f);
            SpriteEffects spriteDir = Projectile.spriteDirection * Projectile.direction < 0 ? SpriteEffects.FlipHorizontally : 0;
            float rotation = Projectile.rotation - MathHelper.PiOver4 * Projectile.spriteDirection * Projectile.direction + MathHelper.PiOver2;

            lightColor = Color.Lerp(Lighting.GetColor((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16), Color.White, 0.7f);
            Color glowColor = Color.Lerp(Color.GhostWhite, HeavenColors.Melee, 0.3f) * 0.33f;
            glowColor.A = 0;

            switch (Projectile.ai[0])
            {
                case 0:

                    if (Projectile.ai[1] <= 1)
                        return false;

                    visualScale = new Vector2(1f + slashProgress * (1f - slashProgress) * 3f) * Projectile.scale * Utils.GetLerpValue(-0.1f, 0.2f, slashProgress, true);

                    Main.EntitySpriteDraw(shadow.Value, Projectile.Center - Main.screenPosition, null, Color.Black * 0.3f, rotation, origin + new Vector2(14), visualScale, spriteDir, 0);

                    if (Projectile.ai[1] > 6 * SpeedMod)
                    {
                        int trailLength = ProjectileID.Sets.TrailCacheLength[Type];
                        for (int i = 1; i < trailLength - 1; i++)
                        {
                            Color fade = Color.Lerp(HeavenColors.Melee * 0.77f, Color.Black, Utils.GetLerpValue(0, trailLength - 3f, i, true)) * Utils.GetLerpValue(0.1f, 0.5f, slashProgress, true);
                            fade.A = 0;
                            float oldRot = Projectile.oldRot[i] - MathHelper.PiOver4 * Projectile.spriteDirection * Projectile.direction + MathHelper.PiOver2;
                            Main.EntitySpriteDraw(trail.Value, Projectile.Center - Main.screenPosition, null, fade, oldRot, origin, visualScale, spriteDir, 0);

                        }
                    }

                    //Main.EntitySpriteDraw(TextureAssets.BlackTile.Value, hitbox.Center.ToVector2() - Main.screenPosition, hitbox, HeavenColors.Silver * 0.2f, 0, hitbox.Size() / 2f, 1f, 0, 0);

                    Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, null, lightColor, rotation, origin, visualScale, spriteDir, 0);
                    Main.EntitySpriteDraw(bloom.Value, Projectile.Center - Main.screenPosition, null, glowColor, rotation, origin + new Vector2(14), visualScale, spriteDir, 0);

                    DrawSwipe();
                    DrawStar();

                    break;
            }

            return false;
        }

        private void DrawStar()
        {
            Asset<Texture2D> star = TextureAssets.Extra[98];

            Color starColor = HeavenColors.Melee * 0.7f;
            starColor.A = 0;
            Color starGlowColor = Color.GhostWhite * 0.5f;
            starGlowColor.A = 0;
            int dir = Projectile.direction * Projectile.spriteDirection;
            Vector2 starPos = Projectile.Center + new Vector2(60 + 5 * dir, 60 - 5 * dir).RotatedBy(Projectile.rotation - MathHelper.PiOver4) * visualScale;
            float starScale = Projectile.scale * Utils.GetLerpValue(0.98f, 0.94f, slashProgress, true) * Utils.GetLerpValue(0.1f, 0.2f, slashProgress, true);

            Main.EntitySpriteDraw(star.Value, starPos - Main.screenPosition, null, starColor, 0f, star.Size() * 0.5f, new Vector2(1f, 1.7f) * starScale, 0, 0);
            Main.EntitySpriteDraw(star.Value, starPos - Main.screenPosition, null, starColor, MathHelper.PiOver2, star.Size() * 0.5f, new Vector2(1f, 2.5f) * starScale, 0, 0);
            Main.EntitySpriteDraw(star.Value, starPos - Main.screenPosition, null, starGlowColor, 0f, star.Size() * 0.5f, new Vector2(0.6f, 1f) * starScale, 0, 0);
            Main.EntitySpriteDraw(star.Value, starPos - Main.screenPosition, null, starGlowColor, MathHelper.PiOver2, star.Size() * 0.5f, new Vector2(0.6f, 1f) * starScale, 0, 0);
        }

        private void DrawSwipe()
        {
            Asset<Texture2D> swipe = ModContent.Request<Texture2D>($"{nameof(BlockContent)}/Content/Projectiles/Weapons/PieceOfHeaven/HolyBladeSlash");

            Color swipeColor = HeavenColors.MeleeDark;
            swipeColor.A = 13;
            switch (Projectile.ai[0])
            {
                case 0:

                    swipeColor *= Utils.GetLerpValue(0.5f, 0.8f, slashProgress, true) * Utils.GetLerpValue(1.02f, 0.88f, slashProgress, true);
                    SpriteEffects swipeDir = Projectile.direction > 0 ? SpriteEffects.FlipHorizontally : 0;
                    float swipeRot = Projectile.velocity.ToRotation() + MathHelper.PiOver2 - (Projectile.direction * Projectile.spriteDirection > 0 ? MathHelper.ToRadians(40) : -MathHelper.ToRadians(20));
                    Vector2 swipePos = Projectile.Center + new Vector2(0, -10).RotatedBy(swipeRot);
                    float swipeScale = (1f + slashProgress * (1f - slashProgress) * 3f) * Projectile.scale;

                    Main.EntitySpriteDraw(swipe.Value, swipePos - Main.screenPosition, null, swipeColor, swipeRot, swipe.Size() * 0.5f, swipeScale * 0.7f, swipeDir, 0);

                    break;
            }
        }
    }
}
