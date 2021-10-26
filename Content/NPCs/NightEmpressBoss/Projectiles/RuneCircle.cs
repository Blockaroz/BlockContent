using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.GameContent;
using Terraria.ID;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;

namespace BlockContent.Content.NPCs.NightEmpressBoss.Projectiles
{
    public class RuneCircle : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rune Circle");
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
            Projectile.timeLeft = _totalTime;
        }

        private ref float _safeRadius => ref Projectile.ai[0];
        private const int _explodeTime = 30;
        private const int _totalTime = 300;

        public override void AI()
        {
            NPC owner = Main.npc[(int)Projectile.ai[1]];
            Projectile.Center = owner.Center;
            if (Projectile.timeLeft > _explodeTime && Projectile.timeLeft <= _explodeTime + 90)
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector2 vector2 = Main.rand.NextVector2CircularEdge(_safeRadius, _safeRadius);
                    Vector2 velocity = Main.rand.NextVector2Circular(10, 10);
                    Color color = NightEmpress.NightColor(0);
                    color.A /= 5;
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + vector2, NightEmpress.GlowDustID, velocity, 0, color, 1f); 
                    dust.noGravity = true;
                }
            }

            if (Projectile.timeLeft == _explodeTime + 90)
                SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Assets/Sounds/NightEmpress/explod"), Projectile.Center);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float distance = targetHitbox.Distance(Projectile.Center);
            if (distance > _safeRadius && 
                Projectile.timeLeft <= _explodeTime + 90)
                return true;

            return false;
        }

        public override bool PreDraw(ref Color lightColor) => false; //The circle's drawing effects will be handled by the empress herself
    }
}
