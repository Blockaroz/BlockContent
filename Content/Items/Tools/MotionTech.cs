using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContent.Content.Items.Tools
{
    public class MotionTech : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Motion Tech");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 20;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.knockBack = 7f;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.rare = ItemRarityID.Purple;
            Item.shoot = ModContent.ProjectileType<MotionTechHook>();
            Item.shootSpeed = 30f;
            Item.value = Item.buyPrice(0, 5, 50, 0);
        }
    }

    public class MotionTechHook : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Motion Tech");
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft *= 10;
            Projectile.aiStyle = 7;
        }

        public override bool? SingleGrappleHook(Player player) => true;

        public override float GrappleRange() => 1100f;

        public override void NumGrappleHooks(Player player, ref int numHooks) => numHooks = 1;

        public override void GrappleRetreatSpeed(Player player, ref float speed) => speed = 30f;

        private bool canPull;
        private float lengthVelocity;
        private float curDistance;

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(canPull);
            writer.Write(curDistance);
            writer.Write(lengthVelocity);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            canPull = reader.ReadBoolean();
            curDistance = reader.Read();
            lengthVelocity = reader.Read();
        }

        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner];

            if (player.dead || player.stoned || player.webbed || player.frozen)
                Projectile.Kill();

            if (!player.controlHook)
                canPull = false;

            ResetPlayerEffects(player);

            bool hooked = Projectile.ai[0] == 2f;

            Vector2 angle = (player.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
            Vector2 difference = Projectile.Center - player.Center;
            float distance = Vector2.Distance(player.Center, Projectile.Center);
            float maxDistance = 1000f;

            if (hooked)
            {
                Projectile.extraUpdates = 0;
                Projectile.rotation = Projectile.AngleTo(player.Center) - MathHelper.PiOver2;
                player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, player.AngleTo(Projectile.Center) - MathHelper.PiOver2);

                bool shoot = canPull && player.controlHook;
                player.GoingDownWithGrapple = shoot && angle.Y < 0;

                float pullDir = player.controlDown ? 1f : -1f;
                bool descend = player.controlDown && curDistance < maxDistance;

                if (shoot || (descend || player.controlUp) && descend != player.controlUp)
                    lengthVelocity += shoot ? -3f : (1f * pullDir);

                curDistance += lengthVelocity;
                curDistance = MathHelper.Clamp(curDistance, 10f, maxDistance);
                distance = curDistance;
                lengthVelocity *= 0.9f;

                if ((player.controlLeft || player.controlRight) && (!player.controlLeft || !player.controlRight) && player.velocity.Y != 0)
                    player.velocity.X *= 1.015f;
                else
                    player.velocity.X *= 1.005f;

                float maxSpeed = Math.Max(distance / 10f, 15f);
                player.velocity = Vector2.Clamp(player.velocity, Vector2.One * -maxSpeed, Vector2.One * maxSpeed);

                float force = player.gravity + (Vector2.Distance(player.Center + player.velocity, Projectile.Center) - distance);
                difference *= (difference.Length() > force ? (force / difference.Length()) : 1f);

                if (distance >= curDistance)
                {
                    player.velocity += difference;
                    player.runAcceleration *= 3f;
                    player.maxRunSpeed = 15f;
                }
                else
                {
                    player.runAcceleration = 0f;
                    player.moveSpeed = 0f;
                }
            }
            else
            {
                Projectile.extraUpdates = 2;
                curDistance = distance;
            }
            player.gravity *= 2f;
            player.wingsLogic = -1;
            player.wingTime = 0;

            return !hooked;
        }

        private void ResetPlayerEffects(Player player)
        {
            if (player.mount.Active)
                player.mount.Dismount(player);

            player.RefreshMovementAbilities();

            player.rocketFrame = false;
            player.canRocket = false;
            player.rocketRelease = false;
            player.sandStorm = false;
            player.fallStart = (int)(player.Center.Y / 16);

            if (player.controlJump)
            {
                player.velocity.Y = Math.Min(player.velocity.Y, -Player.jumpSpeed);
                player.jump = 0;
                player.RefreshMovementAbilities();
                player.RemoveAllGrapplingHooks();
                Projectile.Kill();
            }
        }

        public override bool PreDrawExtras() => false;

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}