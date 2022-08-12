using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
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
    public class SaboteurHeld : ModProjectile
    {
        public override string Texture => $"{nameof(BlockContent)}/Content/Items/Weapons/Red/Saboteur";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Saboteur");
        }

        public override void SetDefaults()
        {
            Projectile.width = 66;
            Projectile.height = 20;
            Projectile.aiStyle = -1;
            Projectile.hide = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.netImportant = true;
        }

        private Player Player => Main.player[Projectile.owner];

        public ref float ShootTime => ref Projectile.ai[0];
        public float ChargeTime;
        public int ShotCount;
        public ref float Special => ref Projectile.ai[1];

        public ref float BounceTime => ref Projectile.localAI[0];
        public ref float SpecialFX => ref Projectile.localAI[1];

        public override void AI()
        {
            Player.SetDummyItemTime(3);
            Player.itemAnimation = 3;
            Player.heldProj = Projectile.whoAmI;
            Player.ChangeDir(Projectile.direction);

            float armRot = Projectile.velocity.ToRotation() * Player.gravDir - MathHelper.PiOver2 - Player.fullRotation;
            Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.None, armRot);
            Player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, armRot);

            Projectile.Center = Player.RotatedRelativePointOld(Player.GetFrontHandPosition(Player.CompositeArmStretchAmount.None, Projectile.velocity.ToRotation() - MathHelper.PiOver2 - Player.fullRotation));
            Projectile.direction = Math.Sign(Projectile.velocity.X);

            bool shoot = false;

            float speed;

            if (ChargeTime < 20)
                speed = Player.itemAnimationMax * 3f;
            else if (ChargeTime < 50)
                speed = Player.itemAnimationMax * 2.33f;            
            else if (ChargeTime < 80)
                speed = Player.itemAnimationMax * 1.66f;
            else
                speed = Player.itemAnimationMax;

            if (ShootTime <= 0)
            {
                Projectile.velocity = Projectile.DirectionTo(Main.MouseWorld).RotatedByRandom(0.01f) * 5f;
                shoot = true;
                ShootTime = (int)speed;
            }

            if (shoot && Player.HasAmmo(Player.HeldItem) && !Player.CCed && !Player.noItems)
            {
                BounceTime++;

                if (ChargeTime > 80)
                {
                    ShotCount++;
                    if (ShotCount >= 7)
                    {
                        Special++;
                        SpecialFX++;
                        ShotCount = 0;
                        Main.NewText(Special, new Color(255, 33, 33));
                    }
                }

                Vector2 muzzlePos = new Vector2(35, -7 * Projectile.direction).RotatedBy(Projectile.rotation);

                Player.PickAmmo(Player.HeldItem, out int projType, out float projSpeed, out int damage, out float knockBack, out int ammoType, Main.rand.NextBool());
                IEntitySource source = Player.GetSource_ItemUse_WithPotentialAmmo(Player.HeldItem, ammoType);
                Vector2 bulletVelocity = Vector2.Lerp(Projectile.rotation.ToRotationVector2(), Projectile.velocity, 0.4f).SafeNormalize(Vector2.Zero);
                Projectile.NewProjectileDirect(source, Projectile.Center + muzzlePos, bulletVelocity * projSpeed * 15f, projType, damage, knockBack, Projectile.owner);

                SoundStyle shootSound = new SoundStyle($"{nameof(BlockContent)}/Assets/Sounds/Items/SaboteurShoot");
                shootSound.MaxInstances = 0;
                shootSound.PitchVariance = 0.1f;
                shootSound.Pitch = 0.2f - speed * 0.01f;
                shootSound.Volume = 0.6f;
                SoundEngine.PlaySound(shootSound, Projectile.Center);
            }

            if (!Player.channel || Player.dead || Player.CCed || Player.noItems || Player.whoAmI != Projectile.owner)
                ChargeTime = (int)(ChargeTime * 0.75f);

            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.ToRadians(4) * Projectile.direction * BounceTime;

            BounceTime = MathHelper.Lerp(BounceTime, 0, 0.4f);
            if (BounceTime < 0.01f)
                BounceTime = 0;

            SpecialFX = MathHelper.Lerp(BounceTime, 0, 0.1f);
            if (SpecialFX < 0.01f)
                SpecialFX = 0;

            if (Player.channel && ChargeTime < 120)
                ChargeTime++;
            if (ChargeTime <= 0 && ShootTime < 5 && ShootTime > 2)
                Projectile.Kill();

            ShootTime--;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(ShotCount);
            writer.Write(ChargeTime);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            ShotCount = reader.Read();
            ChargeTime = reader.ReadSingle();
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => false;

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Asset<Texture2D> glow = ModContent.Request<Texture2D>(Texture + "Glow");
            Asset<Texture2D> flash = ModContent.Request<Texture2D>($"{nameof(BlockContent)}/Assets/Textures/Item/MuzzleFlash");
            int spriteDir = Projectile.direction * (int)Player.gravDir;
            Vector2 origin = texture.Size() * new Vector2(0.22f, 0.5f + 0.15f * spriteDir);
            SpriteEffects dir = spriteDir > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            Color drawColor = Color.White;

            Vector2 handleOffset = new Vector2(-15, 0).RotatedBy(Projectile.velocity.ToRotation());
            Main.EntitySpriteDraw(texture.Value, Projectile.Center + handleOffset - Main.screenPosition, null, lightColor.MultiplyRGBA(drawColor), Projectile.rotation, origin, Projectile.scale, dir, 0);
            Main.EntitySpriteDraw(glow.Value, Projectile.Center + handleOffset - Main.screenPosition, null, Color.White.MultiplyRGB(drawColor), Projectile.rotation, origin, Projectile.scale, dir, 0);

            if (ShootTime < 2)
            {
                Vector2 flashOffset = new Vector2(40, -7 * spriteDir).RotatedBy(Projectile.rotation);
                Main.EntitySpriteDraw(flash.Value, Projectile.Center + flashOffset - Main.screenPosition, null, new Color(255, 33, 33, 128).MultiplyRGBA(drawColor), Projectile.rotation, flash.Size() * new Vector2(0.2f, 0.5f), Projectile.scale, 0, 0);
                Main.EntitySpriteDraw(TextureAssets.Extra[98].Value, Projectile.Center + flashOffset - Main.screenPosition, null, new Color(255, 33, 33, 50), Projectile.rotation + MathHelper.PiOver2, TextureAssets.Extra[98].Size() * new Vector2(0.5f, 0.6f), Projectile.scale * 0.6f, 0, 0);
                Main.EntitySpriteDraw(flash.Value, Projectile.Center + flashOffset - Main.screenPosition, null, new Color(255, 255, 255, 0), Projectile.rotation, flash.Size() * new Vector2(0.2f, 0.5f), Projectile.scale * 0.5f, 0, 0);
            }

            SaboteurEffects effects = default(SaboteurEffects);

            return false;
        }

        private static readonly Color Sanguine = new Color(255, 33, 33);
    }
}
