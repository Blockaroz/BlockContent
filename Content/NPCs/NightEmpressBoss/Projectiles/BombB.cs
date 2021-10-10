using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.GameContent;
using Terraria.ID;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;

namespace BlockContent.Content.NPCs.NightEmpressBoss.Projectiles
{
    public class BombB : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("UnFini");
            ProjectileID.Sets.TrailCacheLength[Type] = 130;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 280;
        }

        public override void AI()
        {
            const int explodeTime = 30;

            int targetIndex = (int)Projectile.ai[0];
            Player target = Main.player[targetIndex];
            if (Projectile.timeLeft > 240)
                Projectile.velocity *= 0.97f;

            if (Projectile.timeLeft < 240 && Projectile.timeLeft > 90)
            {
                Vector2 newVelocity = Projectile.velocity;

                if (Main.player.IndexInRange(targetIndex))
                    newVelocity = Projectile.DirectionTo(target.Center) * 32;

                float lerpValue = MathHelper.Lerp(0.05f, 0.1f, Utils.GetLerpValue(190, 40, Projectile.timeLeft, true));
                Projectile.velocity = Vector2.SmoothStep(Projectile.velocity, newVelocity, lerpValue);


                if (Projectile.Distance(target.Center) < 80)
                {
                    Projectile.timeLeft = explodeTime;
                    Projectile.ai[1] = 0;
                }
            }

            if (Projectile.timeLeft <= explodeTime)
            {
                Projectile.velocity = Vector2.Zero;
                if (Projectile.timeLeft < 20)
                    Projectile.Resize(80, 80);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> ball = Mod.Assets.Request<Texture>("Content/NPCs/NightEmpressBoss/Projectiles/BombB");
            return false;
        }
    }
}
