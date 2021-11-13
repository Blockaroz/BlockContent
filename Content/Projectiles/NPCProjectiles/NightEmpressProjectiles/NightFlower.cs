using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.GameContent;
using Terraria.ID;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;
using BlockContent.Content.NPCs.NightEmpressBoss;

namespace BlockContent.Content.Projectiles.NPCProjectiles.NightEmpressProjectiles
{
    public class NightFlower : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flowering Night");
            ProjectileID.Sets.TrailCacheLength[Type] = 110;
            ProjectileID.Sets.TrailingMode[Type] = 3;
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 800;
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
            Projectile.timeLeft = 700;
        }

        public override void AI()
        {
            const float twoDegrees = MathHelper.Pi / 360f;
            const float value = 21;//Lower is stronger
            float rot = Projectile.ai[0] * Projectile.ai[1];

            Projectile.velocity = Projectile.velocity.RotatedBy(rot);
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Projectile.ai[0] < twoDegrees)
                Projectile.ai[0] += twoDegrees / value;

            if (Main.getGoodWorld)
                Projectile.extraUpdates = 1;

        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type] - 2; i++)
            {
                if (targetHitbox.Contains(Projectile.oldPos[i].ToPoint()))
                    return true;
            }
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float opacity = Utils.GetLerpValue(720, 690, Projectile.timeLeft, true) * Utils.GetLerpValue(0, 60, Projectile.timeLeft, true);

            Asset<Texture2D> baseTexture = Mod.Assets.Request<Texture2D>("Content/Projectiles/NPCProjectiles/NightEmpressProjectiles/NightFlower");

            Color nightShade = NightEmpress.SpecialColor(0, true);
            nightShade.A = 25;
            Color darkShade = NightEmpress.SpecialColor(1);
            darkShade.A = 0;

            Vector2 origin = new Vector2(29, 27);

            Rectangle mainFrame = baseTexture.Frame(2, 1, 0, 0);
            Rectangle trailFrame = baseTexture.Frame(2, 1, 1, 0);

            for (int i = 1; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
            {
                float trailLerp = 0.3f + (Utils.GetLerpValue(ProjectileID.Sets.TrailCacheLength[Type], 0, i, true) * 0.7f);
                Color trailColor = NightEmpress.SpecialColor(Utils.GetLerpValue(5, ProjectileID.Sets.TrailCacheLength[Type], i, true));
                trailColor.A = 0;
                Vector2 oldposition = Projectile.oldPos[i] + (Projectile.Size / 2);

                Main.EntitySpriteDraw(baseTexture.Value, oldposition - Main.screenPosition, trailFrame, MoreColor.NightSky * 0.05f * opacity, Projectile.oldRot[i] + MathHelper.PiOver2, origin, Projectile.scale * 1.05f * trailLerp, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(baseTexture.Value, oldposition - Main.screenPosition, trailFrame, trailColor * 0.5f * opacity, Projectile.oldRot[i] + MathHelper.PiOver2, origin, Projectile.scale * trailLerp, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(baseTexture.Value, Projectile.Center - Main.screenPosition, mainFrame, darkShade * opacity, Projectile.rotation + MathHelper.PiOver2, origin, Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(baseTexture.Value, Projectile.Center - Main.screenPosition, mainFrame, nightShade * opacity, Projectile.rotation + MathHelper.PiOver2, origin, Projectile.scale * 0.7f, SpriteEffects.None, 0);

            return false;
        }
    }
}
