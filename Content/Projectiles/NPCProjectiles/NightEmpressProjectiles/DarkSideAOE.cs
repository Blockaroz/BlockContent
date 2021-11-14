using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.GameContent;
using Terraria.ID;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using BlockContent.Content.NPCs.NightEmpressBoss;

namespace BlockContent.Content.Projectiles.NPCProjectiles.NightEmpressProjectiles
{
    public class DarkSideAOE : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dark Side");
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
        private const int _totalTime = 150;

        public override void AI()
        {
            if (Main.npc.IndexInRange((int)Projectile.ai[1]))
            {
                NPC owner = Main.npc[(int)Projectile.ai[1]];
                Projectile.Center = owner.Center;
            }
            else
                Projectile.Kill();
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (targetHitbox.Distance(Projectile.Center) <= _safeRadius)
                return false;
            if (targetHitbox.Distance(Projectile.Center) > 7200f)
                return false;

            return true;
        }

        public override bool PreDraw(ref Color lightColor) => false;
    }
}
