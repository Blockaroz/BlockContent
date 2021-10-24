﻿using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.GameContent;
using Terraria.ID;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;

namespace BlockContent.Content.NPCs.NightEmpressBoss.Projectiles
{
    public class RuneCircleBomb : ModProjectile
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
        private const int _explodeTime = 60;
        private const int _totalTime = 300;

        public override void AI()
        {
            NPC owner = Main.npc[(int)Projectile.ai[1]];
            Projectile.Center = owner.Center;
            if (Projectile.timeLeft >= _explodeTime)
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector2 vector2 = Main.rand.NextVector2CircularEdge(_safeRadius, _safeRadius);
                    Vector2 velocity = Main.rand.NextVector2Circular(10, 10);
                    Color color = NightEmpress.NightColor(0);
                    color.A /= 5;
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + vector2, NightEmpress.GlowDustID, velocity, 0, color, 1.5f); 
                    dust.noGravity = true;
                }
            }

            if (Projectile.timeLeft == _explodeTime + 60)
                SoundEngine.PlaySound(Mod.GetLegacySoundSlot(Terraria.ModLoader.SoundType.Item, "Assets/Sounds/Item/NightEmpress/explod"), Projectile.Center);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float distance = targetHitbox.Distance(Projectile.Center);
            if (distance > _safeRadius && 
                Projectile.timeLeft <= _explodeTime + 60)
                return true;

            return false;
        }

        public override bool PreDraw(ref Color lightColor) => false; //The circle's drawing effects will be handled by the empress herself
    }
}
