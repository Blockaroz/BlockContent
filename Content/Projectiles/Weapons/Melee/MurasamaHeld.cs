using BlockContent.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleEngine;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;
using static System.Formats.Asn1.AsnWriter;

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
            Projectile.localNPCHitCooldown = 20;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.hide = true;
            Projectile.extraUpdates = 4;
        }

        private Player Player => Main.player[Projectile.owner];

        public ref float Time => ref Projectile.ai[0];

        public ref float SlashMode => ref Projectile.ai[1];

        public float MaxTime { get; set; }

        private Vector2 newVelocity;

        private bool increment;

        private Vector2[] oldTips;
        private float[] oldRots;
        private float[] oldReach;

        public override void OnSpawn(IEntitySource source)
        {
            oldTips = new Vector2[ProjectileID.Sets.TrailCacheLength[Type]];
            oldRots = new float[ProjectileID.Sets.TrailCacheLength[Type]];
            oldReach = new float[ProjectileID.Sets.TrailCacheLength[Type]];
        }

        private float reach;
        private float roundness;

        private Vector2 Center => Player.RotatedRelativePointOld(Player.MountedCenter);

        public override void AI()
        {
            MaxTime = Player.itemAnimationMax * 4.5f;

            Projectile.velocity = Vector2.Lerp(Projectile.velocity, newVelocity, 0.15f).ToRotation().ToRotationVector2();
            Projectile.direction = (Projectile.velocity.X > 0 ? 1 : -1);

            Player.heldProj = Projectile.whoAmI;
            Player.SetDummyItemTime(2);
            Player.ChangeDir(Projectile.velocity.X > 0 ? 1 : -1);

            Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation * 0.9f * (int)Player.gravDir - MathHelper.PiOver2 - Player.fullRotation);

            roundness = 1f;
            reach = 1f;
            float distance = 150;
            Vector2 tipVelocity = Vector2.Zero;

            switch (SlashMode)
            {
                case 0:

                    roundness = 0.4f;
                    float rotation = MathHelper.Lerp(2f, -1.7f, SwingEase(Time / MaxTime));
                    tipVelocity = new Vector2(distance * reach * Projectile.scale, 0).RotatedBy(rotation * Projectile.direction * (int)Player.gravDir);
                    tipVelocity.Y *= roundness;
                    Projectile.Center = Center + tipVelocity.RotatedBy(Projectile.velocity.ToRotation());

                    Projectile.rotation = Center.AngleTo(Projectile.Center);
                    Projectile.spriteDirection = -Projectile.direction * (int)Player.gravDir;

                    if (Time >= MaxTime)
                        increment = true;

                    break;

                case 1:

                    MaxTime *= 0.7f;
                    reach = MathHelper.Lerp(1f, 1.2f, SwingEase(Time / MaxTime) * (1f - SwingEase(Time / MaxTime)) * 1.5f);
                    roundness = MathHelper.Lerp(0.4f, 0.3f, Utils.GetLerpValue(0f, 0.2f, SwingEase(Time / MaxTime), true));
                    rotation = MathHelper.Lerp(-1.7f, 3f, SwingEase(Time / MaxTime)) + MathHelper.SmoothStep(0, 0.5f, Time / MaxTime);
                    tipVelocity = new Vector2(distance * reach * Projectile.scale, 0).RotatedBy(rotation * Projectile.direction * (int)Player.gravDir);
                    tipVelocity.Y *= roundness;
                    Projectile.Center = Center + tipVelocity.RotatedBy(Projectile.velocity.ToRotation());

                    Projectile.rotation = Center.AngleTo(Projectile.Center);
                    Projectile.spriteDirection = Projectile.direction * (int)Player.gravDir;

                    if (Time >= MaxTime)
                        increment = true;

                    break;

                case 2:

                    MaxTime *= 0.9f;
                    reach = MathHelper.Lerp(1f, 1.6f, SwingEase(Time / MaxTime) * (1f - SwingEase(Time / MaxTime)) * 2f);
                    roundness = MathHelper.SmoothStep(0.3f, 0.6f, Utils.GetLerpValue(0f, 0.2f, SwingEase(Time / MaxTime), true));
                    rotation = MathHelper.Lerp(3.5f, 2.2f, SwingEase(Time / MaxTime)) + SwingEase(Time / MaxTime) * MathHelper.TwoPi;
                    tipVelocity = new Vector2(distance * reach * Projectile.scale, 0).RotatedBy(rotation * Projectile.direction * (int)Player.gravDir);
                    tipVelocity.Y *= roundness;
                    Projectile.Center = Center + tipVelocity.RotatedBy(Projectile.velocity.ToRotation());

                    Projectile.rotation = Center.AngleTo(Projectile.Center);
                    Projectile.spriteDirection = Projectile.direction * (int)Player.gravDir;

                    if (Time >= MaxTime)
                        increment = true;

                    break;

                case 3:

                    MaxTime *= 0.6f;
                    roundness = MathHelper.SmoothStep(0.6f, 0.5f, Utils.GetLerpValue(0.7f, 1f, SwingEase(Time / MaxTime), true));
                    rotation = MathHelper.Lerp(2f, -3f, SwingEase(Time / MaxTime));
                    tipVelocity = new Vector2(distance * reach * Projectile.scale, 0).RotatedBy(rotation * Projectile.direction * (int)Player.gravDir);
                    tipVelocity.Y *= roundness;
                    Projectile.Center = Center + tipVelocity.RotatedBy(Projectile.velocity.ToRotation());

                    Projectile.rotation = Center.AngleTo(Projectile.Center);
                    Projectile.spriteDirection = -Projectile.direction * (int)Player.gravDir;

                    if (Time >= MaxTime)
                        increment = true;

                    break;

                case 4:

                    MaxTime *= 1.3f;
                    reach = MathHelper.Lerp(1f, 1.2f, SwingEase(Time / MaxTime) * (1f - SwingEase(Time / MaxTime)) * 1.5f);
                    roundness = MathHelper.Lerp(0.5f, 0.3f, Utils.GetLerpValue(0f, 0.2f, SwingEase(Time / MaxTime), true));
                    rotation = MathHelper.Lerp(-3f, -2.5f, SwingEase(Time / MaxTime)) - SwingEase(Time / MaxTime) * MathHelper.TwoPi;
                    tipVelocity = new Vector2(distance * reach * Projectile.scale, 0).RotatedBy(rotation * Projectile.direction * (int)Player.gravDir);
                    tipVelocity.Y *= roundness;
                    Projectile.Center = Center + tipVelocity.RotatedBy(Projectile.velocity.ToRotation());

                    Projectile.rotation = Center.AngleTo(Projectile.Center);
                    Projectile.spriteDirection = -Projectile.direction * (int)Player.gravDir;

                    if (Time >= MaxTime)
                        increment = true;

                    break;

                case 5:

                    reach = MathHelper.Lerp(1f, 2f, SwingEase(Time / MaxTime) * (1f - SwingEase(Time / MaxTime)) * 3f);
                    roundness = MathHelper.SmoothStep(0.3f, 0.4f, Utils.GetLerpValue(0f, 0.2f, SwingEase(Time / MaxTime), true));
                    rotation = MathHelper.Lerp(-2.5f, 2f, SwingEase(Time / MaxTime));
                    tipVelocity = new Vector2(distance * reach * Projectile.scale, 0).RotatedBy(rotation * Projectile.direction * (int)Player.gravDir);
                    tipVelocity.Y *= roundness;
                    Projectile.Center = Center + tipVelocity.RotatedBy(Projectile.velocity.ToRotation());

                    Projectile.rotation = Center.AngleTo(Projectile.Center);
                    Projectile.spriteDirection = Projectile.direction * (int)Player.gravDir;

                    if (Time >= MaxTime)
                        increment = true;

                    break;

            }

            for (int i = ProjectileID.Sets.TrailCacheLength[Type] - 1; i > 0; i--)
            {
                oldTips[i] = oldTips[i - 1];
                oldRots[i] = oldRots[i - 1];
                oldReach[i] = oldReach[i - 1];
                if (Projectile.numUpdates == 0 && oldTips[i] != Vector2.Zero)
                    oldTips[i] += Player.position - Player.oldPosition;
            }
            oldRots[0] = Projectile.rotation + MathHelper.PiOver2 - 0.2f * Projectile.spriteDirection;
            oldTips[0] = Projectile.Center - new Vector2(50 * reach, 0).RotatedBy(Projectile.rotation);
            oldReach[0] = reach;

            if (increment)
            {
                if (Player.channel)
                {
                    newVelocity = Projectile.DirectionTo(Main.MouseWorld);
                    SlashMode = (SlashMode + 1) % 6;
                    Time = 0;
                    increment = false;
                }
                else if (Time > MaxTime + 5)
                    Projectile.Kill();

            }

            Time++;

            if (Main.rand.NextBool(6))
            {
                float dustScale = SwingEase(Time / MaxTime) * (1f - SwingEase(Time / MaxTime)) * 5f;
                Vector2 velocity = Projectile.rotation.ToRotationVector2().RotatedBy(MathHelper.Pi / 6f * Projectile.spriteDirection);
                Color redColor = FuryRed;
                Dust sparks = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8, 8), 278, velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(4f), 0, redColor, 1 * dustScale);
                sparks.noGravity = true;
            }
        }

        private float SwingEase(float x)
        {
            float adjX = Utils.GetLerpValue(0f, 0.95f, x, true);

            float[] prog = new float[4];
            prog[0] = 0.05f * (float)Math.Pow(adjX, 0.4f);
            prog[1] = 2f * (float)Math.Pow(adjX - 0.3f, 2f) + 0.1f;
            prog[2] = -2f * (float)Math.Pow(adjX - 0.8f, 2f) + 0.8f;
            prog[3] = 0.4f * adjX + 0.55f;

            if (x <= 0f)
                return 0f;
            if (x < 0.4f)
                return MathHelper.Lerp(prog[0], prog[1], Utils.GetLerpValue(0.25f, 0.4f, adjX, true));
            else if (x < 0.6f)
                return MathHelper.Lerp(prog[1], prog[2], Utils.GetLerpValue(0.4f, 0.6f, adjX, true));
            else if (x < 0.8f)
                return MathHelper.Lerp(prog[2], prog[3], Utils.GetLerpValue(0.7f, 0.8f, adjX, true));
            else
                return MathHelper.Lerp(prog[3], 1f, Utils.GetLerpValue(0.8f, 1f, adjX, true));

        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            int width = 200;
            int height = 200;

            hitbox = new Rectangle((int)Projectile.Center.X - width / 2, (int)Projectile.Center.Y - height / 2, width, height);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return projHitbox.Intersects(targetHitbox) && Time < MaxTime * 0.9f;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(newVelocity);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            newVelocity = reader.ReadVector2();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            SpriteEffects dir = Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float rotation = Projectile.rotation - 1.5f + MathHelper.PiOver2 * Projectile.spriteDirection;
            Vector2 scale = new Vector2(Center.Distance(Projectile.Center) / 150f, 1f) * 4.2f * Projectile.scale;

            Effect spriteEffect = ModContent.Request<Effect>($"{nameof(BlockContent)}/Assets/Effects/SwingSprite", AssetRequestMode.ImmediateLoad).Value;
            spriteEffect.Parameters["uTexture"].SetValue(ModContent.Request<Texture2D>(Texture).Value);
            spriteEffect.Parameters["uRotation"].SetValue(MathHelper.PiOver4);
            spriteEffect.Parameters["lengthPercent"].SetValue(0.04f);

            spriteEffect.Parameters["uColor"].SetValue(Vector4.One);
            spriteEffect.CurrentTechnique.Passes[0].Apply();

            Main.EntitySpriteDraw(texture.Value, Player.MountedCenter - Main.screenPosition, null, Color.White, rotation, texture.Size() * 0.5f, scale, dir, 0);

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
            {
                Vector2 oldScale = new Vector2(Center.Distance(Projectile.oldPos[i] + Projectile.Size / 2f) / 150f, 1f) * 4f * Projectile.scale;
                float oldRotation = Projectile.oldRot[i] - 1.5f + MathHelper.PiOver2 * Projectile.spriteDirection;
                Color trailColor = Color.Lerp(FuryRed * 0.1f, Color.Transparent, (float)i / ProjectileID.Sets.TrailCacheLength[Type]);
                trailColor.A = 0;
                spriteEffect.Parameters["uColor"].SetValue(trailColor.ToVector4());
                spriteEffect.CurrentTechnique.Passes[0].Apply();

                Main.EntitySpriteDraw(texture.Value, Player.MountedCenter - Main.screenPosition, null, trailColor, oldRotation, texture.Size() * 0.5f, oldScale, dir, 0);
            }

            Effect trailEffect = ModContent.Request<Effect>($"{nameof(BlockContent)}/Assets/Effects/VertexShader", AssetRequestMode.ImmediateLoad).Value;
            trailEffect.Parameters["uTexture"].SetValue(TextureAssets.Extra[201].Value);
            trailEffect.Parameters["uColor"].SetValue(Color.White.ToVector3());
            trailEffect.Parameters["uProgress"].SetValue(0);
            trailEffect.Parameters["uTransformMatrix"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);
            trailEffect.CurrentTechnique.Passes[0].Apply();

            VertexStrip strip = new VertexStrip();
            strip.PrepareStrip(oldTips, oldRots, StripColor, StripWidth, -Main.screenPosition + Projectile.Size / 2f, oldTips.Length, true);
            strip.DrawTrail();

            Main.pixelShader.CurrentTechnique.Passes[0].Apply();

            return false;
        }

        private Color StripColor(float progressOnStrip)
        {
            return FuryRed * (float)Math.Sqrt(Utils.GetLerpValue(0.1f, 0.3f, progressOnStrip, true)) * Utils.GetLerpValue(0.1f, 0.2f, SwingEase(Time / MaxTime), true) * Utils.GetLerpValue(1f, 0.9f, SwingEase(Time / MaxTime), true);
        }        
        
        private float StripWidth(float progressOnStrip)
        {
            return 80f * oldReach[(int)(progressOnStrip * (ProjectileID.Sets.TrailCacheLength[Type] - 1))];
        }

        private static readonly Color FuryRed = Color.Lerp(Color.Red, Color.Orchid, 0.12f);
    }
}
