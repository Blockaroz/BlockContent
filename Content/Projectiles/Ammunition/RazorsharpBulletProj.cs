using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContent.Content.Projectiles.Ammunition
{
    public class RazorsharpBulletProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Example Bullet");
			ProjectileID.Sets.CanDistortWater[Type] = true;
		}

		public override void SetDefaults()
		{
			Projectile.width = 8;
			Projectile.height = 8;
			Projectile.aiStyle = 1;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.penetrate = 5;
			Projectile.timeLeft = 600;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = true;
			Projectile.extraUpdates = 1;
			Projectile.alpha = 255;

			AIType = ProjectileID.Bullet;
		}

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			Projectile.damage = (int)(Projectile.damage * 0.9f);
			target.AddBuff(BuffID.Bleeding, 120, true);
        }

        public override void AI()
        {
			if (Projectile.alpha > 0)
				Projectile.alpha--;

			//Lighting.AddLight(Projectile.Center, Color.DarkSlateGray.ToVector3() * 0.5f);
        }

		public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 128).MultiplyRGBA(Color.Lerp(Color.DimGray, lightColor, 0.5f));

        public override bool PreDraw(ref Color lightColor)
        {
			Asset<Texture2D> bullet = ModContent.Request<Texture2D>(Texture);
			float fadeIn = (255 - Projectile.alpha) / 255f;
			Main.EntitySpriteDraw(bullet.Value, Projectile.Center - Main.screenPosition, null, GetAlpha(lightColor).Value * fadeIn, Projectile.rotation, bullet.Size() * new Vector2(0.5f, 0.1f), Projectile.scale, SpriteEffects.None, 0);

			return false;
        }
    }
}
