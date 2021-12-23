using BlockContent.Content.Graphics;
using BlockContent.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContent.Content.Projectiles.Weapons.Holy
{
    public class PaleBladeProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Blade");
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
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 2;
            Projectile.penetrate = -1;
        }

        private ref float Time => ref Projectile.localAI[0];

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (Time > 50)
                Projectile.Kill();

            float rotation = (MathHelper.SmoothStep(-Projectile.ai[0], Projectile.ai[0], Utils.GetLerpValue(0f, 20f, Time, true))) * Projectile.direction;
            float length = MathHelper.SmoothStep(70f, MathHelper.Clamp(Projectile.ai[1], 70f, 700f), ExtraUtils.DualLerp(5f, 15f, 20f, Time, true));
            Vector2 slash = Vector2.SmoothStep(Vector2.Zero, new Vector2(MathHelper.Max(70f, length), 0), ExtraUtils.DualLerp(0f, 10f, 20f, 30f, Time, true));
            Vector2 endPoint = player.MountedCenter + slash.RotatedBy(Projectile.velocity.ToRotation());

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 + rotation;
            Projectile.Center = endPoint;

            Time++;

        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (targetHitbox.Distance(Projectile.Center) < 170)
                return true;

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            default(HolyBladeDrawer).Draw(Projectile);

            return false;
        }
    }
}
