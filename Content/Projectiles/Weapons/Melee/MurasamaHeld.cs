using BlockContent.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleEngine;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContent.Content.Projectiles.Weapons.Melee
{
    public class MurasamaHeld : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CanDistortWater[Type] = true;
            ProjectileID.Sets.NoMeleeSpeedVelocityScaling[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.aiStyle = -1;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.noEnchantmentVisuals = true;
            Projectile.manualDirectionChange = true;
            Projectile.localNPCHitCooldown = 5;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        private Player Player => Main.player[Projectile.owner];

        public ref float Time => ref Projectile.ai[0];

        public ref float SlashMode => ref Projectile.ai[1];

        private Vector2 swordTip;

        public override void AI()
        {
            Player.heldProj = Projectile.whoAmI;
            Player.SetDummyItemTime(2);
            Player.ChangeDir(Projectile.direction);

            if (Player.channel)
                Projectile.timeLeft = 12;

            Projectile.Center = Player.MountedCenter;

            switch (SlashMode)
            {
                case 0:

                    Projectile.rotation = Projectile.AngleTo(swordTip);

                    break;
            }

            Particle.NewParticle(Particle.ParticleType<Ember>(), swordTip, Vector2.Zero, Color.Red, 1f);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(swordTip);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            swordTip = reader.ReadVector2();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);

            Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation + 0.9f, new Vector2(0, texture.Height()), Projectile.scale, 0, 0);
            return false;
        }
    }
}
