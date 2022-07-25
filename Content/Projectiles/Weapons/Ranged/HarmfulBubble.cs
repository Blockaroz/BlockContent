using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleEngine;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContent.Content.Projectiles.Weapons.Ranged
{
    public class HarmfulBubble : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Harmful Bubble");
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 240;
            Projectile.tileCollide = true;
        }

        public override void AI()
        {
            //the worst homing you will ever know
            if (Projectile.ai[0] > 12)
            {
                int search = Projectile.FindTargetWithLineOfSight(600);
                if (search >= 0)
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Main.npc[search].Center).RotatedByRandom(1f), 0.01f);

                Projectile.velocity *= 0.94f;
            }

            if (Main.rand.NextBool())
                Projectile.velocity += Main.rand.NextVector2Circular(1, 1) * 0.1f;

            //coalesce
            //int follow = -1;
            //foreach (Projectile other in Main.projectile)
            //{
            //    if (other.type == Type && other.active && other.owner == Projectile.owner && other.whoAmI != Projectile.whoAmI && Projectile.Distance(other.Center) < 700)
            //    {
            //        follow = other.whoAmI;
            //        break;
            //    }
            //}
            //if (follow >= 0)
            //{
            //    if (Main.projectile[follow].Center.Distance(Projectile.Center) > 54)
            //        Projectile.velocity += Projectile.DirectionTo(Main.projectile[follow].Center).RotatedByRandom(0.5f) * 0.01f;
            //}

            Vector2 pos = Main.rand.NextVector2CircularEdge(14, 14) * Projectile.scale;
            Vector2 outwardVel = pos.DirectionFrom(Vector2.Zero) * Main.rand.NextFloat();
            Color lightColor = Lighting.GetColor((int)(Projectile.Center.X / 16f), (int)(Projectile.Center.Y / 16)) * 0.4f;
            Dust.NewDustPerfect(Projectile.Center + pos, DustID.BubbleBlock, outwardVel, 180, lightColor, 0.5f + Main.rand.NextFloat()).noGravity = true;

            Projectile.ai[1] = Utils.GetLerpValue(-1, 11, Projectile.ai[0], true);
            Projectile.ai[0]++;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > 0)
                Projectile.velocity.X = -oldVelocity.X;            
            if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > 0)
                Projectile.velocity.Y = -oldVelocity.Y;

            if (Main.rand.NextBool(10))
                Projectile.Kill(); 
            
            return false;
        }

        public override void Kill(int timeLeft)
        {
            SoundStyle bubbleNoise = SoundID.Item54;
            bubbleNoise.MaxInstances = 0;
            bubbleNoise.Volume = 2f;
            bubbleNoise.PitchVariance = 0.4f;
            SoundEngine.PlaySound(bubbleNoise, Projectile.Center);

            for (int i = 0; i < 25; i++)
            {
                Vector2 pos = Main.rand.NextVector2CircularEdge(14, 14) * Projectile.scale;
                Vector2 outwardVel = pos.DirectionFrom(Vector2.Zero) * Main.rand.NextFloat(4f);
                Color lightColor = Lighting.GetColor((int)(Projectile.Center.X / 16f), (int)(Projectile.Center.Y / 16)) * 0.4f;
                Dust.NewDustPerfect(Projectile.Center + pos, DustID.BubbleBlock, outwardVel, 80, lightColor, 0.5f + Main.rand.NextFloat()).noGravity = true;

            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            lightColor.A = 150;
            lightColor *= Utils.GetLerpValue(0.2f, 1f, Projectile.ai[1], true);
            float fadeIn = Projectile.ai[1] * 0.7f + 0.3f;
            Vector2 stretch = new Vector2(0.92f + (float)Math.Sin(Projectile.ai[0] * 0.08f) * 0.08f, 0.92f + (float)Math.Cos(Projectile.ai[0] * 0.08f) * 0.08f) * Projectile.scale * fadeIn;
            Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, texture.Size() * 0.5f, stretch, 0, 0);
            return false;
        }
    }
}
