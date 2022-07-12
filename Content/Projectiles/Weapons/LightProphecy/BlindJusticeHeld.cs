using BlockContent.Content.Items.Weapons.LightProphecy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContent.Content.Projectiles.Weapons.LightProphecy
{
    public class BlindJusticeHeld : ModProjectile
    {
        public override string Texture => $"{nameof(BlockContent)}/Content/Items/Weapons/LightProphecy/BlindJusticeHeld";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blind Justice");
            ProjectileID.Sets.TrailingMode[Type] = 4;
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
            ProjectileID.Sets.CanDistortWater[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.aiStyle = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
            Projectile.tileCollide = false;
            Projectile.manualDirectionChange = true;
            Projectile.noEnchantmentVisuals = true;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.hide = true;
        }

        private Player Player { get => Main.player[Projectile.owner]; }
        private bool allowKill = false;
        private float slashProgress;

        private float slashEase(float x, float c) => x < 0.5f ? (1f - (float)Math.Sqrt(1f - Math.Pow(2 * x, 2.5f))) / 2f : 1f - (float)Math.Pow(1.4f, -20 * x + 10) / 2f;

        private float SpeedMod { get => Player.itemAnimationMax / 17f; }

        public override void AI()
        {
            Projectile.timeLeft = 3;
            Player.SetDummyItemTime(3);
            Player.ChangeDir(Projectile.spriteDirection);
            Player.heldProj = Projectile.whoAmI;

            if (allowKill)
            {
                if (!Player.channel)
                    Projectile.Kill();
                else
                {
                    Projectile.ai[1] = -3;
                    allowKill = false;
                }
            }

            if (Projectile.ai[1] == 0)
            {
                Projectile.direction *= -1;
                Projectile.velocity = Projectile.DirectionTo(Main.MouseWorld).RotatedByRandom(0.3f) * 5;
                Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
                Projectile.idStaticNPCHitCooldown = (int)(10f * SpeedMod);
            }

            switch (Projectile.ai[0])
            {
                case 0:

                    slashProgress = slashEase(Utils.GetLerpValue(0, Player.itemAnimationMax * 1.1f, Projectile.ai[1], true), 1f);
                    float swordRot = (slashProgress - 0.6f) * MathHelper.ToRadians(270) * Projectile.spriteDirection * Projectile.direction;
                    Projectile.rotation = Projectile.velocity.ToRotation() + swordRot;

                    Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.velocity.ToRotation() + swordRot * Player.gravDir - MathHelper.PiOver2 - Player.fullRotation);
                    Projectile.Center = Player.RotatedRelativePointOld(Player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2 - Player.fullRotation), true);
                    if (Projectile.ai[1] > Player.itemAnimationMax)
                        allowKill = true;


                    if (Projectile.ai[1] == (int)(7 * SpeedMod))
                    {
                        SoundStyle swingSound = SoundID.DD2_MonkStaffSwing;
                        swingSound.Pitch = 0.2f;
                        swingSound.PitchVariance = 0.7f;
                        swingSound.MaxInstances = 0;
                        SoundEngine.PlaySound(swingSound, Projectile.Center);
                    }
                    break;

                case 1:

                    Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Player.AngleTo(Projectile.Center + Player.velocity) * Player.gravDir - MathHelper.PiOver2);
                    Vector2 handPos = Player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);

                    if (Projectile.ai[1] == 8)
                    {
                        Projectile.velocity = Projectile.DirectionTo(Main.MouseWorld).RotatedByRandom(0.2f) * Math.Max(20, Projectile.Distance(Main.MouseWorld) * 0.052f);
                    }

                    if (Projectile.ai[1] < 8)
                    {
                        slashProgress = slashEase(Projectile.ai[1] / 12f, 1.5f) * Projectile.spriteDirection;
                        Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.Pi / 1.5f * Projectile.spriteDirection;
                        Projectile.Center = handPos;
                    }
                    else if (Projectile.ai[1] < 40 * SpeedMod)
                    {
                        Projectile.rotation += 0.6f / SpeedMod * Projectile.spriteDirection;
                        if (Projectile.ai[1] > 20 * SpeedMod)
                            Projectile.velocity *= Utils.GetLerpValue(40 * SpeedMod, 30 * SpeedMod, Projectile.ai[1], true) * 0.99f;
                    }
                    else
                    {
                        Projectile.Center = Vector2.Lerp(Projectile.Center, handPos, (float)Math.Pow(Utils.GetLerpValue(38 * SpeedMod, 55 * SpeedMod, Projectile.ai[1], true), 4));
                        if (Projectile.ai[1] > 54 * SpeedMod)
                        {
                            allowKill = true;
                            Projectile.rotation = Projectile.AngleTo(Main.MouseWorld);
                        }
                        else
                            Projectile.rotation = MathHelper.Lerp(Projectile.rotation, Player.MountedCenter.AngleTo(Projectile.Center), 0.33f);

                    }

                    break;
            }

            Projectile.ai[1]++;

            Projectile.rotation = MathHelper.WrapAngle(Projectile.rotation);

            Lighting.AddLight(Projectile.Center + Projectile.velocity, Color.SeaGreen.ToVector3() * 0.5f);

        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {

        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {

        }

        private Rectangle hitbox = new Rectangle(0, 0, 100, 100);

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 bladeEnd = Projectile.Center + new Vector2(80, 0).RotatedBy(Projectile.rotation);
            hitbox.X = (int)bladeEnd.X - hitbox.Width / 2;
            hitbox.Y = (int)bladeEnd.Y - hitbox.Height / 2;
            if (Projectile.ai[0] == 0 && slashProgress > 0.15f)
                return hitbox.Intersects(targetHitbox);
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> sword = ModContent.Request<Texture2D>(Texture);
            Asset<Texture2D> swordGlow = ModContent.Request<Texture2D>($"{nameof(BlockContent)}/Content/Items/Weapons/LightProphecy/BlindJusticeGlow");
            Asset<Texture2D> swordBloom = ModContent.Request<Texture2D>($"{nameof(BlockContent)}/Content/Items/Weapons/LightProphecy/BlindJusticeBloom");

            lightColor = Color.Lerp(Color.White, Lighting.GetColor((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16), 0.7f);

            Vector2 scale = Vector2.One * Projectile.scale;
            Vector2 originOff = new Vector2(0.5f - 0.35f * Projectile.spriteDirection, 0.85f);
            SpriteEffects spriteDir = Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : 0;
            float rotation = Projectile.rotation - MathHelper.PiOver4 * Projectile.spriteDirection + MathHelper.PiOver2;

            switch (Projectile.ai[0])
            {
                case 0:

                    scale *= 1f + (slashProgress * (1f - slashProgress) * 2f);

                    break;

                case 1:

                    float throwOut = Utils.GetLerpValue(5, 10, Projectile.ai[1], true) * Utils.GetLerpValue(55 * SpeedMod, 40 * SpeedMod, Projectile.ai[1], true);
                    originOff = Vector2.Lerp(new Vector2(0.5f - 0.4f * Projectile.spriteDirection, 0.9f), new Vector2(0.5f), throwOut);

                    break;
            }
            if (Projectile.ai[1] <= 0)
                return false;

            //dark spot
            //Main.EntitySpriteDraw(TextureAssets.Extra[98].Value, Projectile.Center - Main.screenPosition, null, Color.Black * 0.4f, Projectile.rotation + MathHelper.PiOver2 * Projectile.spriteDirection, TextureAssets.Extra[98].Size() * new Vector2(0.5f, 1f - originOff.X), scale * new Vector2(0.85f, 1.85f), 0, 0);

            Color glowColor = BlindJustice.PulseColor(12f);
            glowColor.A /= 2;
            Color bloomColor = BlindJustice.QuasarColor(12f);
            bloomColor.A = 0;

            Main.EntitySpriteDraw(sword.Value, Projectile.Center - Main.screenPosition, null, lightColor, rotation, sword.Size() * originOff, scale, spriteDir, 0);
            Main.EntitySpriteDraw(swordGlow.Value, Projectile.Center - Main.screenPosition, null, glowColor, rotation, sword.Size() * originOff, scale, spriteDir, 0);
            Main.EntitySpriteDraw(swordBloom.Value, Projectile.Center - Main.screenPosition, null, bloomColor, rotation, sword.Size() * originOff + new Vector2(8), scale, spriteDir, 0);

            return false;
        }
    }
}
