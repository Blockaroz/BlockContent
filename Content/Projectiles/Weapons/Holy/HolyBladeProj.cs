using BlockContent.Content.Graphics;
using BlockContent.Content.Particles;
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
    public class HolyBladeProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Blade");
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 128;
            Projectile.height = 128;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
            Projectile.manualDirectionChange = true;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 4;
            Projectile.penetrate = -1;
            Projectile.hide = true;
            Projectile.noEnchantmentVisuals = true;
        }

        private ref float Time => ref Projectile.localAI[0];
        private ref float TimeMax => ref Projectile.localAI[1];

        private float angle = MathHelper.ToRadians(120);

        public override void AI()
        {
            //Projectile.damage = -1;
            Player player = Main.player[Projectile.owner];
            player.heldProj = Projectile.whoAmI;
            TimeMax = player.itemAnimationMax;
            Projectile.spriteDirection = Projectile.velocity.X < 0 ? -1 : 1;

            Time++;

            if (Time == 1)
                SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Assets/Sounds/Items/PaleSlash").WithPitchVariance(0.33f), player.MountedCenter);

            Projectile.timeLeft = 3;
            player.SetDummyItemTime(3);
            player.ChangeDir(Projectile.spriteDirection);

            if ((!player.channel || player.HeldItem.type != ModContent.ItemType<Items.Weapons.Holy.HolyBlade>() || player.noItems || player.whoAmI != Projectile.owner) && Time % TimeMax <= 0)
                Projectile.Kill();

            if (Time > TimeMax)
            {
                Time = 0;
                Projectile.direction *= -1;
                for (int i = 0; i < Projectile.oldPos.Length; i++)
                {
                    Projectile.oldPos[i] = Projectile.position;
                    Projectile.oldRot[i] = Projectile.rotation;
                }
            }

            Projectile.velocity = Vector2.Lerp(Projectile.velocity, GetTargetDistance(player).SafeNormalize(Vector2.Zero), 0.08f);

            float rotation = (MathHelper.SmoothStep(-angle, angle, Utils.GetLerpValue(TimeMax * 0.15f, TimeMax * 0.85f, Time, true)) * Projectile.direction);
            Projectile.rotation = Projectile.velocity.ToRotation() + rotation;

            float handRot = Projectile.velocity.ToRotation() - MathHelper.PiOver2 + rotation - player.fullRotation;
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, handRot);
            player.itemLocation = player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Quarter, handRot);

            Projectile.Center = player.MountedCenter + new Vector2(Vector2.Distance(player.Center, player.itemLocation), 0).RotatedBy(Projectile.rotation);
             
            if (Main.rand.Next(2) == 0)
                Particle.NewParticle(Particle.ParticleType<Ember>(), player.itemLocation + new Vector2(95, 0).RotatedBy(Projectile.rotation), Projectile.velocity * Main.rand.NextFloat(), Color2.HolyMelee, Main.rand.NextFloat());

            //for (int i = 0; i < 3; i++)
            //    Particle.NewParticle(Particle.ParticleType<GenericFlame>(), 
            //        player.itemLocation + new Vector2(48 + (i * 15), 0).RotatedBy(Projectile.rotation), 
            //        player.velocity + new Vector2(6 - i, 0).RotatedBy(Projectile.rotation), 
            //        Color2.HolyMelee, 0.7f);

        }

        public Vector2 GetTargetDistance(Player player)
        {
            Vector2 reachPoint = Main.MouseWorld;
            player.LimitPointToPlayerReachableArea(ref reachPoint);
            Vector2 distance = reachPoint - player.MountedCenter;

            if (Time < 3)
            {
                int index;
                bool targetAcquired = ExtraUtils.NPCInRange(player, reachPoint, 200, out index);

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

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Player player = Main.player[Projectile.owner];

            bool nearMouse = (player.MountedCenter + GetTargetDistance(player)).Distance(targetHitbox.Center.ToVector2()) < 150;
            bool swordMelee = projHitbox.Intersects(targetHitbox);

            return swordMelee || nearMouse;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            DrawSword(player);

            float sparkleScale = ExtraUtils.DualLerp(0, TimeMax * 0.3f, TimeMax * 0.8f, TimeMax, Time, true);
            ExtraUtils.DrawSparkle(TextureAssets.Extra[98], SpriteEffects.None,
            player.MountedCenter + new Vector2(70 + (sparkleScale * 10), 0).RotatedBy(Projectile.rotation) - Main.screenPosition,
            TextureAssets.Extra[98].Size() / 2, 0.3f + sparkleScale, 0.2f, 1.4f, 3f, 0f,
            Color2.HolyMelee, Color2.PaleGray, sparkleScale);

            return false;
        }

        public void DrawSword(Player player)
        {
            Asset<Texture2D> sword = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Holy/HolyBlade");
            Asset<Texture2D> swordGlow = Mod.Assets.Request<Texture2D>("Content/Projectiles/Weapons/Holy/HolyBladeProj");

            SpriteEffects direction = Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 origin = new Vector2(Projectile.spriteDirection < 0 ? sword.Width() - 3 : 3, sword.Height() - 3);
            float diagonal = (Projectile.spriteDirection < 0 ? MathHelper.Pi - MathHelper.PiOver4 : MathHelper.PiOver4);

            Main.EntitySpriteDraw(sword.Value, player.itemLocation - Main.screenPosition, null, Color.White, Projectile.rotation + diagonal, origin, Projectile.scale, direction, 0);

            Color glowColor = Color2.HolyMelee;
            glowColor.A = 40;

            //hitbox
            //Main.EntitySpriteDraw(TextureAssets.BlackTile.Value, Projectile.position - Main.screenPosition, new Rectangle(0, 0, Projectile.width, Projectile.height), Color.Black * 0.5f, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);

            for (int i = 0; i < 4; i++)
            {
                Vector2 offset = new Vector2(2, 0).RotatedBy((MathHelper.TwoPi / 4 * i) + Projectile.velocity.ToRotation() + MathHelper.PiOver4);
                Main.EntitySpriteDraw(swordGlow.Value, player.itemLocation + offset - Main.screenPosition, null, glowColor * 0.3f, Projectile.rotation + diagonal, origin, Projectile.scale, direction, 0);
            }
        }
    }
}
