using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleEngine;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContent.Content.Projectiles.Weapons.PieceOfHeaven
{
    public class HolyBladeLight : ModProjectile
    {
        public override string Texture => $"{nameof(BlockContent)}/Assets/Textures/GlowStar";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Blade");
            ProjectileID.Sets.CanDistortWater[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.manualDirectionChange = true;
            Projectile.noEnchantmentVisuals = true;

            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.timeLeft = 24;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
        }

        public override void AI()
        {
            Projectile.Center = Main.npc[(int)Projectile.ai[0]].Center;

            if (Projectile.ai[1] == 0)
            {
                for (int i = 0; i < 20; i++)
                {
                    Particle star = Particle.NewParticle(Particle.ParticleType<Particles.HeavenSpark>(), Projectile.Center + Main.rand.NextVector2Circular(8, 8), Projectile.rotation.ToRotationVector2().RotatedByRandom(0.4f) * Main.rand.NextFloat(i), HeavenColors.Melee, 0.8f * i / 20f * Projectile.scale);
                    star.data = Main.npc[(int)Projectile.ai[0]];
                }
                SoundStyle hitSound = SoundID.DD2_BetsyFireballImpact;// new SoundStyle($"{nameof(BlockContent)}/Assets/Sounds/Items/HolyBladeHit");
                hitSound.MaxInstances = 0;
                hitSound.Pitch = 0.5f;
                hitSound.PitchVariance = 0.3f;
                hitSound.Volume *= 1.2f;
                SoundEngine.PlaySound(hitSound, Projectile.Center);

                Projectile.ai[1]++;
            }
        }

        public override bool PreDraw(ref Color lightColor) => false;
    }
}
