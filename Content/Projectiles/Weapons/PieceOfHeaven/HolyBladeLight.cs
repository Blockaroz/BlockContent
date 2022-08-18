using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleEngine;
using ReLogic.Content;
using System;
using System.Collections.Generic;
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
            DisplayName.SetDefault("Holy Light");
            ProjectileID.Sets.CanDistortWater[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
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

            if (Projectile.timeLeft == 24)
            {
                rotations = new List<float>();
                for (int i = 0; i < Main.rand.Next(4, 5); i++)
                    rotations.Add(Main.rand.NextFloat(MathHelper.TwoPi));

                for (int i = 0; i < 16; i++)
                {
                    Color randColor = Color.Lerp(HeavenColors.Melee, HeavenColors.MeleeDark, Main.rand.Next(2));
                    Particle.NewParticle(Particle.ParticleType<Particles.HeavenSpark>(), Projectile.Center + Main.rand.NextVector2Circular(8, 8), Projectile.rotation.ToRotationVector2().RotatedByRandom(1.5f) * Main.rand.NextFloat(i), randColor, 0.6f * i / 20f * Projectile.scale);
                }

                Particle spark = Particle.NewParticle(Particle.ParticleType<Particles.ImpactSpark>(), Projectile.Center, (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2().RotatedByRandom(1f), HeavenColors.MeleeDark, 1f);
                spark.data = Main.npc[(int)Projectile.ai[0]];
            }
            Projectile.ai[1]++;
        }

        private List<float> rotations;

        public override bool PreDraw(ref Color lightColor) => false;
    }
}
