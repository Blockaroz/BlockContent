using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
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

        private float lengthVelocity;
        private float curDistance;

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(curDistance);
            writer.Write(lengthVelocity);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            curDistance = reader.Read();
            lengthVelocity = reader.Read();
        }

        public override bool PreAI() => Projectile.ai[0] != 2f;

        public override void PostAI()
        {
            Player player = Main.player[Projectile.owner];
            ResetPlayerEffects(player);

            Vector2 difference = Projectile.Center - player.Center;
            float distance = Vector2.Distance(player.Center, Projectile.Center);
            float maxDistance = 1000f;
            Projectile.rotation = Projectile.AngleTo(player.Center) - MathHelper.PiOver2;

            if (Projectile.ai[0] == 2f)
            {
                Projectile.extraUpdates = 0;
                player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, player.AngleTo(Projectile.Center) - MathHelper.PiOver2);

                //player.GoingDownWithGrapple = player.controlHook && angle.Y < 0;

                float pullDir = player.controlDown ? 1f : -1f;
                bool descend = player.controlDown && curDistance < maxDistance;

                if (player.controlHook || ((descend || player.controlUp) && descend != player.controlUp))
                    lengthVelocity += 0.7f * pullDir;

                curDistance += lengthVelocity;
                curDistance = MathHelper.Clamp(curDistance, 24f, maxDistance);
                distance = curDistance;
                lengthVelocity *= 0.85f;

                if ((player.controlLeft || player.controlRight) && (!player.controlLeft || !player.controlRight) && player.velocity.Y != 0)
                    player.velocity.X *= 1.015f;
                else
                    player.velocity.X *= 1.005f;

                float force = player.gravity + (Vector2.Distance(player.Center + player.velocity, Projectile.Center) - distance);
                difference *= (difference.Length() > force ? (force / difference.Length()) : 1f);

                if (curDistance >= distance)
                    player.velocity += difference;

                float maxSpeed = Math.Max(distance / 10f, 15f);
                player.velocity = Vector2.Clamp(player.velocity, Vector2.One * -maxSpeed, Vector2.One * maxSpeed);
            }
            else
            {
                Projectile.extraUpdates = 2;
                curDistance = distance;
            }
        }

        private void ResetPlayerEffects(Player player)
        {
            if (player.dead || player.stoned || player.webbed || player.frozen)
                Projectile.Kill();

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
                player.velocity.Y = Math.Min(player.velocity.Y, -Player.jumpSpeed * 1.5f);
                player.jump = 0;
                player.RefreshMovementAbilities();
                player.RemoveAllGrapplingHooks();
                Projectile.Kill();
            }
        }

        public override bool PreDrawExtras() => false;

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            TempDraw(player);
            return false;
        }

        private void TempDraw(Player player)
        {
            Asset<Texture2D> tex = TextureAssets.FishingLine;

            Color col = Color2.IndigoFlux;
            col.A /= 2;

            float stringLength = Projectile.Distance(player.Center) / tex.Height();
            Main.EntitySpriteDraw(tex.Value, Projectile.Center - Main.screenPosition, null, col, Projectile.rotation, new Vector2(tex.Width() * 0.5f, 0f), new Vector2(2f, stringLength), SpriteEffects.None, 0);
            Main.EntitySpriteDraw(tex.Value, Projectile.Center - Main.screenPosition, null, new(255, 200, 255, 0), Projectile.rotation, new Vector2(tex.Width() * 0.5f, 0f), new Vector2(0.75f, stringLength), SpriteEffects.None, 0);
        }
    }
}