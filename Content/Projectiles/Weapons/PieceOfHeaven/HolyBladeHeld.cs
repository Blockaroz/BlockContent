using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
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
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 5;
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

        private float SpeedMod { get => Player.itemAnimationMax / 18f; }

        private float slashEase(float x) => x < 0.5f ? (1f - (float)Math.Sqrt(1f - Math.Pow(2 * x, 3f))) / 2f : 1f - (float)Math.Pow(1.4f, -20 * x + 10) / 2f;

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
                    Projectile.ai[1] = -4;
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

                    slashProgress = slashEase(Utils.GetLerpValue(0, Player.itemAnimationMax * 1.05f, Projectile.ai[1], true));
                    float swordRot = (slashProgress - (Projectile.direction > 0 ? 0.7f : 0.5f)) * MathHelper.ToRadians(240) * Projectile.spriteDirection * Projectile.direction;
                    Projectile.rotation = Projectile.velocity.ToRotation() + swordRot;

                    Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.velocity.ToRotation() + swordRot * 0.8f - MathHelper.PiOver2 - Player.fullRotation);
                    Projectile.Center = Player.RotatedRelativePointOld(Player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2 - Player.fullRotation), true);
                    if (Projectile.ai[1] > Player.itemAnimationMax)
                        allowKill = true;


                    if (Projectile.ai[1] == (int)(5 * SpeedMod))
                    {
                        SoundStyle swingSound = SoundID.DD2_MonkStaffSwing;
                        swingSound.Pitch = (-SpeedMod + 0.8f) * 0.5f;
                        swingSound.PitchVariance = 0.3f;
                        swingSound.MaxInstances = 0;
                        SoundEngine.PlaySound(swingSound, Projectile.Center);
                    }

                    break;
            }

            Projectile.ai[1]++;
            Projectile.rotation = MathHelper.WrapAngle(Projectile.rotation);
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

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            SoundStyle hitNoise = SoundID.DD2_MonkStaffGroundMiss;
            hitNoise.MaxInstances = 0;
            hitNoise.PitchVariance = 0.5f;
            SoundEngine.PlaySound(hitNoise.WithVolumeScale(1.33f), Projectile.Center);
        }

        public override bool? CanCutTiles() => Projectile.ai[0] == 0 ? slashProgress > 0.18f : true;

        public override string Texture => $"{nameof(BlockContent)}/Content/Items/Weapons/PieceOfHeaven/HolyBlade";

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Asset<Texture2D> bloom = ModContent.Request<Texture2D>(Texture + "Bloom");
            Asset<Texture2D> trail = ModContent.Request<Texture2D>(Texture + "Trail");
            Vector2 scale = Vector2.One * Projectile.scale;
            Vector2 origin = texture.Size() * new Vector2(0.5f - 0.4f * Projectile.spriteDirection * Projectile.direction, 0.9f);
            SpriteEffects spriteDir = Projectile.spriteDirection * Projectile.direction < 0 ? SpriteEffects.FlipHorizontally : 0;
            float rotation = Projectile.rotation - MathHelper.PiOver4 * Projectile.spriteDirection * Projectile.direction + MathHelper.PiOver2;

            lightColor = Color.Lerp(Lighting.GetColor((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16), Color.White, 0.7f);
            Color glowColor = Color.Lerp(Color.GhostWhite, Color.MediumAquamarine, 0.5f) * 0.5f;
            glowColor.A = 0;

            switch (Projectile.ai[0])
            {
                case 0:

                    if (Projectile.ai[1] <= 0)
                        return false;

                    scale = new Vector2(1.1f + slashProgress * (1f - slashProgress) * 2f) * Projectile.scale;
                    
                    if (Projectile.ai[0] == 0)
                    {
                        int trailLength = ProjectileID.Sets.TrailCacheLength[Type];
                        for (int i = 1; i < trailLength; i++)
                        {
                            float oldRot = MathHelper.Lerp(Projectile.oldRot[i], Projectile.oldRot[i - 1], 0.6f) - MathHelper.PiOver4 * Projectile.spriteDirection * Projectile.direction + MathHelper.PiOver2;
                            Color fade = Color.Lerp(glowColor, Color.MediumAquamarine * 0.5f, (float)i / trailLength) * 0.66f * slashProgress;
                            fade.A = 0;
                            Rectangle trailFrame = trail.Frame(1, 4, 0, (int)((float)Math.Sqrt((float)i / trailLength) * 4f));
                            Main.EntitySpriteDraw(trail.Value, Projectile.Center - Main.screenPosition, trailFrame, fade, oldRot, origin, Projectile.scale * 1.2f, spriteDir, 0);
                        }
                    }

                    break;
            }                    
            
            //dark spot

            Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, null, lightColor, rotation, origin, scale, spriteDir, 0);
            Main.EntitySpriteDraw(bloom.Value, Projectile.Center - Main.screenPosition, null, glowColor, rotation, origin + new Vector2(14), scale, spriteDir, 0);

            DrawSwipe();

            return false;
        }

        private void DrawStar()
        {
            Asset<Texture2D> star = TextureAssets.Extra[98];
        }

        private void DrawSwipe()
        {
            Asset<Texture2D> swipe = ModContent.Request<Texture2D>($"{nameof(BlockContent)}/Assets/Textures/SwordSlash0");

            Color swipeColor = Color.MediumAquamarine * 0.7f;
            swipeColor.A = 50;

            SpriteEffects swipeDir = Projectile.direction > 0 ? SpriteEffects.FlipHorizontally : 0;

            switch (Projectile.ai[0])
            {
                case 0:

                    swipeColor *= Utils.GetLerpValue(0.5f, 0.8f, slashProgress, true) * Utils.GetLerpValue(1.02f, 0.8f, slashProgress, true) * 0.3f;

                    float swipeRot = Projectile.velocity.ToRotation() + MathHelper.PiOver2 - (slashProgress - (Projectile.direction > 0 ? 1.3f : 0.7f)) * -MathHelper.ToRadians(120) * Projectile.direction * Projectile.spriteDirection;
                    Vector2 swipePos = Projectile.Center + new Vector2(0, -10).RotatedBy(swipeRot);
                    Main.EntitySpriteDraw(swipe.Value, swipePos - Main.screenPosition, null, swipeColor, swipeRot, swipe.Size() * 0.5f, Projectile.scale * 0.9f, swipeDir, 0);

                    break;
            }
        }
    }
}
