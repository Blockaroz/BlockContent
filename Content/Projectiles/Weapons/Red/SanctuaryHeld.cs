using BlockContent.Common;
using BlockContent.Content.Graphics;
using BlockContent.Content.Particles;
using BlockContent.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContent.Content.Projectiles.Weapons.Red
{
    public class SanctuaryHeld : ModProjectile
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
            Projectile.damage = -1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.hide = true;
        }

        public override void SendExtraAI(BinaryWriter writer) => writer.Write(Mode);

        public override void ReceiveExtraAI(BinaryReader reader) => Mode = reader.Read();

        private int Mode 
        { 
            get => Main.player[Projectile.owner].GetModPlayer<SpecialWeaponPlayer>().sanctuaryMode; 
            set => Main.player[Projectile.owner].GetModPlayer<SpecialWeaponPlayer>().sanctuaryMode = value; 
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Item selection = player.inventory[player.selectedItem];
            bool tryShoot = (player.channel || player.altFunctionUse == 2) && player.HasAmmo(selection, true) && !player.noItems && !player.CCed;
            bool isShooting = false;
            int useTime = Mode == 1 ? 70 : 35;

            if (tryShoot)
            {
                Projectile.ai[1]++;
            }
            else
            {
                Projectile.ai[1]--;
            }

            if (Projectile.ai[0] - 1 % useTime == 0)
            {
                isShooting = true;
                SoundEngine.PlaySound(SoundID.Item40.WithPitchVariance(0.15f), Projectile.Center);
            }

            if (isShooting)
            {
            }

            SetOrientation(player);

            if (player.dead || !player.active || Projectile.ai[1] < 0)
                Projectile.Kill();   
        }

        private void SetOrientation(Player player)
        {
            Projectile.rotation = MathHelper.WrapAngle(MathHelper.Lerp(Projectile.rotation, Projectile.velocity.ToRotation(), 0.1f));
            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter + new Vector2(15f, 0).RotatedBy(Projectile.velocity.ToRotation()));
            Projectile.position.Y += player.gravDir * 2f;

            Projectile.spriteDirection = player.direction;
            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            player.itemRotation = MathHelper.WrapAngle((float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction));
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.ThreeQuarters, player.itemRotation);

            Projectile.timeLeft = 5;
            player.SetDummyItemTime(5);

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            Asset<Texture2D> gunTex = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Red/Sanctuary");

            Main.EntitySpriteDraw(gunTex.Value, Projectile.Center - Main.screenPosition, null, GunColor(player), Projectile.rotation, gunTex.Size() * 0.5f, Projectile.scale, GunSpriteEffects(), 0);
            return false;
        }

        private Color GunColor(Player player)
        {
            Color baseColor = Color.White;

            if (player.shroomiteStealth && player.inventory[player.selectedItem].DamageType == DamageClass.Ranged)
                baseColor *= MathHelper.Min(player.stealth, 0.03f);

            if (player.shroomiteStealth && player.inventory[player.selectedItem].DamageType == DamageClass.Ranged)
                baseColor = baseColor.MultiplyRGBA(new Color(Vector4.Lerp(Vector4.One, new(0f, 0.12f, 0.16f, 0f), 1f - MathHelper.Min(player.stealth, 0.03f))));

            return baseColor;
        }

        private SpriteEffects GunSpriteEffects()
        {
            SpriteEffects effects = SpriteEffects.None;
            effects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None;
            return effects;
        }
    }
}