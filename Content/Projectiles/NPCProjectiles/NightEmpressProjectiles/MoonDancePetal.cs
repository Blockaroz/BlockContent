using Terraria;
using Terraria.ModLoader;

namespace BlockContent.Content.Projectiles.NPCProjectiles.NightEmpressProjectiles
{
    public class MoonDancePetal : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Moon Dance");
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
            Projectile.timeLeft = 180;
        }
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
    }
}
