using BlockContent.Content.Graphics;
using BlockContent.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContent.Content.Projectiles.Weapons.Holy
{
    public class PaleBladeHold : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Blade");
            ProjectileID.Sets.TrailCacheLength[Type] = 4;
            ProjectileID.Sets.TrailingMode[Type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
            Projectile.manualDirectionChange = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
            Projectile.penetrate = -1;
            Projectile.hide = true;
        }

        private ref float Time => ref Projectile.localAI[0];

        private ref float Angle => ref Projectile.ai[0];
        private ref float NewAngle => ref Projectile.ai[1];

        public override void AI()
        {
            //Projectile.damage = -1;
            Player player = Main.player[Projectile.owner];
            player.heldProj = Projectile.whoAmI;
            Projectile.spriteDirection = Projectile.velocity.X < 0 ? -1 : 1;

            if (Time == 5)
                SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Assets/Sounds/Items/PaleSlash").WithPitchVariance(0.33f), Projectile.Center);

            Projectile.timeLeft = 3;
            player.SetDummyItemTime(3);
            player.ChangeDir(Projectile.spriteDirection);

            if ((!player.channel || player.HeldItem.type != ModContent.ItemType<Items.Weapons.Holy.PaleBlade>() || player.noItems || player.whoAmI != Projectile.owner) && Time % 30 <= 0)
                Projectile.Kill();

            if (Time > 30)
            {
                Time = 0;
                Projectile.velocity = GetTargetDistance(player).SafeNormalize(Vector2.Zero);
                Projectile.direction *= -1;
            }
            if (Time < 1)
            {
                NewAngle = Angle;
                Angle = MathHelper.ToRadians(Main.rand.Next(90, 150));
                //Projectile p = Projectile.NewProjectileDirect(Projectile.GetProjectileSource_FromThis(), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<PaleBladeProjectile>(), player.HeldItem.damage, Projectile.knockBack, Projectile.owner, Angle, GetTargetDistance(player).Length());
                //p.direction = Projectile.direction;
            }

            Time++;

            float rotation = (MathHelper.SmoothStep(-NewAngle, Angle, Utils.GetLerpValue(-3, 30, Time, true)) * Projectile.direction);
            Projectile.rotation = Projectile.velocity.ToRotation() + rotation;

            float handRot = Projectile.velocity.ToRotation() - MathHelper.PiOver2 + (rotation * 0.33f);
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, handRot);
            player.itemLocation = player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Quarter, handRot);
            Projectile.Center = player.MountedCenter + new Vector2(60, 0).RotatedBy(Projectile.rotation);
        }

        public Vector2 GetTargetDistance(Player player)
        {
            Vector2 reachPoint = Main.MouseWorld;
            player.LimitPointToPlayerReachableArea(ref reachPoint);
            Vector2 distance = reachPoint - player.MountedCenter;

            if (Time < 3)
            {
                int index;
                bool targetAcquired = ExtraUtils.NPCInRange(player, reachPoint, 500, out index);

                if (targetAcquired)
                    distance = Main.npc[index].Center - player.MountedCenter;

                bool isTwo = (Time == 2);
                if (!isTwo && !targetAcquired)
                    isTwo = true;

                if (isTwo)
                    distance += Main.rand.NextVector2Circular(20, 20);
            }
            return distance;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawSword();
            return false;
        }

        public void DrawSword()
        {
            Player player = Main.player[Projectile.owner];

            Asset<Texture2D> sword = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Holy/PaleBlade");
            Asset<Texture2D> swordGlow = Mod.Assets.Request<Texture2D>("Content/Projectiles/Weapons/Holy/PaleBladeHold");

            SpriteEffects direction = Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 origin = new Vector2(Projectile.spriteDirection < 0 ? sword.Width() - 3 : 3, sword.Height() - 3);
            float diagonal = (Projectile.spriteDirection < 0 ? MathHelper.Pi - MathHelper.PiOver4 : MathHelper.PiOver4);

            Main.EntitySpriteDraw(sword.Value, player.itemLocation - Main.screenPosition, null, Color.White, Projectile.rotation + diagonal, origin, Projectile.scale, direction, 0);

            Color glowColor = Color2.HolyMelee;
            glowColor.A /= 3;

            //Main.EntitySpriteDraw(TextureAssets.BlackTile.Value, Projectile.position - Main.screenPosition, new Rectangle(0, 0, Projectile.width, Projectile.height), Color.Black * 0.5f, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);

            for (int i = 0; i < 4; i++)
            {
                Vector2 offset = new Vector2(2, 0).RotatedBy((MathHelper.TwoPi / 4 * i) + Projectile.velocity.ToRotation() + MathHelper.PiOver4);
                Main.EntitySpriteDraw(swordGlow.Value, player.itemLocation + offset - Main.screenPosition, null, glowColor * 0.2f, Projectile.rotation + diagonal, origin, Projectile.scale, direction, 0);
            }

            float sparkleScale = ExtraUtils.DualLerp(8f, 15f, 22f, Time, true);
            ExtraUtils.DrawSparkle(TextureAssets.Extra[98], SpriteEffects.None,
                player.itemLocation + new Vector2(75, 0).RotatedBy(Projectile.rotation) - Main.screenPosition,
                TextureAssets.Extra[98].Size() / 2, 0.3f + sparkleScale, 0.2f, 1.4f, 3f, 0f,
                Color2.HolyMelee, Color2.PaleGray, sparkleScale);
            
        }
    }
}
