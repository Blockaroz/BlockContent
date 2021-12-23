using BlockContent.Content.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using BlockContent.Core;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContent.Content.Projectiles.Weapons.Red
{
    public class SanctuaryProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sanctuary");
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.damage = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.hide = true;
        }

        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner];

            if (player.channel)
            {
                Projectile.ai[0] += 1;
                coolDown += 1;
                if (coolDown > 40)
                    coolDown = 40;
            }
            int extraFrameSpeed = 0;
            if (Projectile.ai[0] > 40)
                extraFrameSpeed++;
            if (Projectile.ai[0] > 100)
                extraFrameSpeed++;
            int resetAI1 = 15 - (5 * extraFrameSpeed);
            Projectile.ai[1] -= 1;
            int missile = -1;
            bool tryShoot = false;
            if (Projectile.ai[1] <= 0)
            {
                Projectile.ai[1] = resetAI1;
                tryShoot = true;
                if (player.channel)
                    SoundEngine.PlaySound(SoundID.Item40.WithPitchVariance(0.1f), Projectile.Center);
            }
            if (Projectile.ai[0] % 50 <= 10 && Projectile.ai[0] > 90)
                missile = 0;

            Projectile.frameCounter += 1 + extraFrameSpeed;
            if (Projectile.frameCounter > 2)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= 6)
                    Projectile.frame = 0;
            }

            float randomRotation = MathHelper.ToRadians(Main.rand.NextFloat(-0.75f, 0.75f));
            Item selection = player.inventory[player.selectedItem];
            bool canShoot = player.channel && player.HasAmmo(selection, true) && !player.noItems && !player.CCed;

            Vector2 spinPoint = player.RotatedRelativePoint(player.MountedCenter);
            Vector2 shootDir0 = Main.screenPosition + Main.MouseScreen - spinPoint;
            if (player.gravDir == -1)
                shootDir0.Y = -shootDir0.Y;
            Vector2 shootDir1 = shootDir0.SafeNormalize(Vector2.Zero);
            shootDir1 = shootDir1.RotatedBy(randomRotation);
            if (shootDir1.X != Projectile.velocity.X || shootDir1.Y != Projectile.velocity.Y)
                Projectile.netUpdate = true;
            Projectile.velocity = shootDir1.SafeNormalize(Vector2.Zero) * 15f;
            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter - new Vector2(0, 2), false, false);
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.spriteDirection = Projectile.direction;
            Projectile.timeLeft = 3;
            player.SetDummyItemTime(3);
            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            player.itemRotation = MathHelper.WrapAngle((float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction) + randomRotation);
            Projectile.position.Y += player.gravDir * 2f;

            if (tryShoot && Main.myPlayer == Projectile.owner && player.channel)
            {
                int bulletType = ProjectileID.Bullet;
                float bulletSpeed = 36f;
                int bulletDamage = player.GetWeaponDamage(selection);
                float bulletKnockBack = selection.knockBack;
                if (canShoot)
                {
                    player.PickAmmo(selection, ref bulletType, ref bulletSpeed, ref canShoot, ref bulletDamage, ref bulletKnockBack, out int usedAmmoItemID);
                    IProjectileSource projSource = player.GetProjectileSource_Item_WithPotentialAmmo(player.HeldItem, usedAmmoItemID);

                    for (int i = 0; i < 1; i++)
                    {
                        Vector2 bulletDir = Vector2.Normalize(Projectile.velocity) * bulletSpeed;
                        bulletDir = bulletDir.RotatedBy(randomRotation);
                        if (bulletDir.HasNaNs())
                            bulletDir = -Vector2.UnitY;

                        if (bulletType == ProjectileID.Bullet)
                            bulletType = ModContent.ProjectileType<SanctuaryBeam>();

                        Projectile bullet = Projectile.NewProjectileDirect(projSource, spinPoint, bulletDir, bulletType, bulletDamage, bulletKnockBack, Projectile.owner);
                        bullet.rotation = bulletDir.ToRotation();

                        if (Projectile.ai[0] > 70 && missile != 0)
                        {
                            for (int j = 0; j < Main.rand.Next(0, 5); j++)
                            {
                                Vector2 offsetVector = new Vector2(40, 0).RotatedBy(Projectile.rotation) + Main.rand.NextVector2CircularEdge(20, 30).RotatedBy(Projectile.rotation);
                                bullet = Projectile.NewProjectileDirect(projSource, spinPoint + offsetVector, bulletDir, bulletType, bulletDamage, bulletKnockBack, Projectile.owner);
                                bullet.rotation = bulletDir.ToRotation();
                            }
                        }
                    }

                    if (missile == 0)
                    {
                        bulletType = ModContent.ProjectileType<SanctuaryMissile>();
                        bulletSpeed = 13f;
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
            if (!player.channel)
            {
                coolDown -= 1;
                Projectile.ai[0] = MathHelper.Lerp(Projectile.ai[0], 0, Utils.GetLerpValue(30, 0, coolDown, true));
                if (coolDown <= 0)
                    Projectile.Kill();
            }
            if (player.dead)
                Projectile.Kill();

            return true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(coolDown);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            coolDown = reader.Read();
        }

        public static int coolDown;

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            Projectile.localAI[0] += MathHelper.SmoothStep(0, 1, Utils.GetLerpValue(10, 100, Projectile.ai[0], true));
            SanctuaryHelper sanctuaryHelper = new SanctuaryHelper();
            sanctuaryHelper.Draw(Projectile);
            DrawItem(player);
            DrawShootEffects(player);

            return false;
        }

        public void DrawItem(Player player)
        {
            Color baseColor = Color.White;
            Color glowColor = Color2.Sanguine;
            glowColor.A = 50;
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
            Asset<Texture2D> glowTexture = Mod.Assets.Request<Texture2D>("Content/Projectiles/Weapons/Red/SanctuaryProjectile");
            Rectangle glowFrame = glowTexture.Frame(1, 6, 0, Projectile.frame);

            for (int i = 0; i < 4; i++)
            {
                Vector2 offset = new Vector2(2.5f, 0).RotatedBy((MathHelper.TwoPi / 4 * i) + Projectile.rotation + MathHelper.PiOver4);
                Main.EntitySpriteDraw(baseTexture.Value, drawPos + offset - Main.screenPosition, null, glowColor * 0.7f, Projectile.rotation, baseTexture.Size() / 2, Projectile.scale, GetSpriteEffects(Projectile), 0);
                Main.EntitySpriteDraw(glowTexture.Value, drawPos + offset - Main.screenPosition, glowFrame, glowColor * 0.7f, Projectile.rotation, glowFrame.Size() / 2, Projectile.scale * 1.1f, GetSpriteEffects(Projectile), 0);
            }

            Main.EntitySpriteDraw(baseTexture.Value, drawPos - Main.screenPosition, null, baseColor, Projectile.rotation, baseTexture.Size() / 2, Projectile.scale, GetSpriteEffects(Projectile), 0);
            Main.EntitySpriteDraw(glowTexture.Value, drawPos - Main.screenPosition, glowFrame, glowColor, Projectile.rotation, glowFrame.Size() / 2, Projectile.scale, GetSpriteEffects(Projectile), 0);
        }

        public void DrawShootEffects(Player player)
        {
            Asset<Texture2D> flashTexture = TextureAssets.Extra[98];
            Vector2 flashPos = Projectile.Center + new Vector2(40, -5 * Projectile.spriteDirection).RotatedBy(Projectile.rotation);

            if (Projectile.frame <= 1)
                ExtraUtils.DrawSparkle(flashTexture, GetSpriteEffects(Projectile), flashPos - Main.screenPosition, flashTexture.Size() / 2, Projectile.scale, 0.7f, 0.3f, 0.4f, Projectile.velocity.ToRotation() - MathHelper.Pi, Color2.Sanguine, Color.White, alpha: 25);

            //if (Projectile.frame == 1)
            //{
            //    Particle particle = ParticlePool.NewParticle(new Particles.SanctuaryEmber(), flashPos, Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(2), Color2.Sanguine, Projectile.rotation, 1f);
            //    particle.position += player.velocity;
            //}
        }

        private static SpriteEffects GetSpriteEffects(Projectile proj)
        {
            SpriteEffects spriteEffects = proj.spriteDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None;
            return spriteEffects;
        }
    }
}
