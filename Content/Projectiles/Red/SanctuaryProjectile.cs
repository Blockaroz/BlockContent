using BlockContent.Content.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContent.Content.Projectiles.Red
{
    public class SanctuaryProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sanctuary");
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.hide = true;
        }

        public override void AI()
        {
            DoHoldLogic();
        }

        public void DoHoldLogic()
        {
            Player player = Main.player[Projectile.owner];
            float pointAngle = 0;
            if (Projectile.spriteDirection == -1)
                pointAngle = MathHelper.Pi;

            Projectile.ai[0] += 1;
            int extraFrameSpeed = 0;
            if (Projectile.ai[0] > 40)
                extraFrameSpeed++;
            if (Projectile.ai[0] > 100)
                extraFrameSpeed++;
            int resetAI1 = 15 - 5 * extraFrameSpeed;
            Projectile.ai[1] -= 1;
            int missile = -1;
            bool tryShoot = false;
            if (Projectile.ai[1] <= 0)
            {
                Projectile.ai[1] = resetAI1;
                SoundEngine.PlaySound(SoundID.Item40.WithPitchVariance(0.1f), Projectile.Center);
                tryShoot = true;
                if (Projectile.ai[0] % 7 == 0 && Projectile.ai[0] > 70)
                    missile = 0;
            }
            Projectile.frameCounter += 1 + extraFrameSpeed;
            if (Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= 6)
                    Projectile.frame = 0;
            }
            float randomRotation = MathHelper.ToRadians(Main.rand.NextFloat(-0.66f, 0.66f));
            if (tryShoot && Main.myPlayer == Projectile.owner)
            {
                Item selection = player.inventory[player.selectedItem];
                bool canShoot = player.channel && player.HasAmmo(selection, true) && !player.noItems && !player.CCed;

                int bulletType = ProjectileID.Bullet;
                float bulletSpeed = 36f;
                int bulletDamage = player.GetWeaponDamage(selection);
                float bulletKnockBack = selection.knockBack;
                if (canShoot)
                {
                    player.PickAmmo(selection, ref bulletType, ref bulletSpeed, ref canShoot, ref bulletDamage, ref bulletKnockBack, out int usedAmmoItemID);
                    IProjectileSource projSource = player.GetProjectileSource_Item_WithPotentialAmmo(player.HeldItem, usedAmmoItemID);
                    float bulletShootSpeed = selection.shootSpeed * Projectile.scale;
                    Vector2 spinPoint = player.RotatedRelativePoint(player.MountedCenter);
                    Vector2 shootDir0 = Main.screenPosition + Main.MouseScreen - spinPoint;
                    if (player.gravDir == -1)
                        shootDir0.Y = -shootDir0.Y;
                    Vector2 shootDir1 = Vector2.Normalize(shootDir0);
                    if (shootDir1.HasNaNs())
                        shootDir1 = -Vector2.UnitY;
                    shootDir1 *= bulletShootSpeed;
                    shootDir1 = shootDir1.RotatedBy(randomRotation);
                    if (shootDir1.X != Projectile.velocity.X || shootDir1.Y != Projectile.velocity.Y)
                        Projectile.netUpdate = true;
                    Projectile.velocity = shootDir1;
                    
                    for (int n = 0; n < 1; n++)
                    {
                        Vector2 bulletDir = Vector2.Normalize(Projectile.velocity) * bulletSpeed;
                        bulletDir = bulletDir.RotatedBy(randomRotation);
                        if (bulletDir.HasNaNs())
                            bulletDir = -Vector2.UnitY;

                        Projectile.NewProjectileDirect(projSource, spinPoint, bulletDir, bulletType, bulletDamage, bulletKnockBack, Projectile.owner);

                        if (Projectile.ai[0] > 70 && missile != 0)
                        {
                            for (int i = 0; i < Main.rand.Next(0, 3); i++)
                            {
                                Vector2 offsetVector = new Vector2(20 * Projectile.spriteDirection, 0).RotatedBy(Projectile.rotation) + Main.rand.NextVector2Circular(28, 28);
                                Projectile.NewProjectileDirect(projSource, spinPoint + offsetVector, bulletDir, bulletType, bulletDamage, bulletKnockBack, Projectile.owner);
                            }
                        }
                    }

                    if (missile == 0)
                    {
                        bulletType = ModContent.ProjectileType<SanctuaryMissile>();
                        bulletSpeed = 10f;
                        for (int n = 0; n < 1; n++)
                        {
                            Vector2 missileDir = Vector2.Normalize(Projectile.velocity) * bulletSpeed;
                            missileDir = missileDir.RotatedBy(randomRotation);
                            if (missileDir.HasNaNs())
                                missileDir = -Vector2.UnitY;

                            Projectile.NewProjectileDirect(projSource, spinPoint, missileDir, bulletType, bulletDamage, bulletKnockBack, Projectile.owner);
                        }
                    }
                }
            }
            else if (!player.channel)
                Projectile.Kill();

            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter, false, false);
            Projectile.rotation = Projectile.velocity.ToRotation() + pointAngle;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.timeLeft = 2;
            player.SetDummyItemTime(2);
            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            player.itemRotation = MathHelper.WrapAngle((float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction) + randomRotation);
            Projectile.position.Y += player.gravDir * 2f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawItem();

            DrawMuzzleFlash();

            return false;
        }

        public void DrawItem()
        {
            Player player = Main.player[Projectile.owner];
            Color baseColor = Color.White;
            Color glowColor = MoreColor.Sanguine;
            glowColor.A /= 4;
            if (player.shroomiteStealth && player.inventory[player.selectedItem].DamageType == DamageClass.Ranged)
            {
                float stealthValue = player.stealth;
                if (stealthValue < 0.03f)
                    stealthValue = 0.03f;
                baseColor *= stealthValue;
                glowColor *= stealthValue;
            }
            if (player.shroomiteStealth && player.inventory[player.selectedItem].DamageType == DamageClass.Ranged)
            {
                float stealthValue = player.stealth;
                if (stealthValue < 0.03f)
                    stealthValue = 0.03f;
                baseColor = baseColor.MultiplyRGBA(new Color(Vector4.Lerp(Vector4.One, new(0f, 0.12f, 0.16f, 0f), 1f - stealthValue)));
                glowColor = glowColor.MultiplyRGBA(new Color(Vector4.Lerp(Vector4.One, new(0f, 0.12f, 0.16f, 0f), 1f - stealthValue)));
            }
            Vector2 drawPos = Projectile.Center + new Vector2(0, Projectile.gfxOffY);
            Asset<Texture2D> baseTexture = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Red/Sanctuary");
            Asset<Texture2D> glowTexture = Mod.Assets.Request<Texture2D>("Content/Projectiles/Red/SanctuaryProjectile");
            Rectangle glowFrame = glowTexture.Frame(1, 6, 0, Projectile.frame);

            for (int i = 0; i < 4; i++)
            {
                Vector2 offset = new Vector2(2, 0).RotatedBy((MathHelper.TwoPi / 4 * i) + Projectile.rotation + MathHelper.PiOver4);
                Main.EntitySpriteDraw(baseTexture.Value, drawPos + offset - Main.screenPosition, null, glowColor * 0.8f, Projectile.rotation, baseTexture.Size() / 2, Projectile.scale, GetSpriteEffects(Projectile), 0);
            }

            Main.EntitySpriteDraw(baseTexture.Value, drawPos - Main.screenPosition, null, baseColor, Projectile.rotation, baseTexture.Size() / 2, Projectile.scale, GetSpriteEffects(Projectile), 0);
            Main.EntitySpriteDraw(glowTexture.Value, drawPos - Main.screenPosition, glowFrame, glowColor, Projectile.rotation, glowFrame.Size() / 2, Projectile.scale, GetSpriteEffects(Projectile), 0);
        }

        public void DrawMuzzleFlash()
        {
            Player player = Main.player[Projectile.owner];
            Asset<Texture2D> flashTexture = TextureAssets.Extra[98];
            Vector2 drawPos = Projectile.Center + new Vector2(40 * Projectile.spriteDirection, -5 * player.gravDir).RotatedBy(Projectile.rotation);
            if (Projectile.frame <= 1)
                MoreUtils.DrawSparkle(flashTexture, GetSpriteEffects(Projectile), drawPos - Main.screenPosition, flashTexture.Size() / 2, Projectile.scale, 0.5f, 0.5f, 0.8f, Projectile.rotation, MoreColor.Sanguine, Color.White);
        }

        private static SpriteEffects GetSpriteEffects(Projectile proj)
        {
            Player player = Main.player[proj.owner];
            SpriteEffects spriteEffects = proj.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            if (player.gravDir == -1f)
            {
                if (player.direction == 1)
                    spriteEffects = SpriteEffects.FlipVertically;
                if (player.direction == -1)
                    spriteEffects = SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically;
            }
            return spriteEffects;
        }
    }
}
