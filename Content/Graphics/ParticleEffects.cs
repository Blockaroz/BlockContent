using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.GameContent.NetModules;
using Terraria.Graphics.Renderers;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContent.Content.Graphics
{
    public class ParticleEffects
    {
        private static Mod Mod
        {
            get { return ModContent.GetInstance<BlockContent>(); }
        }

        public static void CreateSanctuaryRipples(ParticleOrchestraSettings settings)
        {
            if (Main.netMode != NetmodeID.Server && !Main.gamePaused)
            {
                FadingParticle particle = new FadingParticle();
                particle.SetBasicInfo(Mod.Assets.Request<Texture2D>("Assets/Textures/Ring_" + (short)0), null, Vector2.Zero, Vector2.Zero);
                particle.SetTypeInfo(30);
                particle.ColorTint = MoreColor.Sanguine;
                particle.ColorTint.A = 25;
                particle.LocalPosition = settings.PositionInWorld;
                particle.Velocity = settings.MovementVector * 0.9f;
                particle.AccelerationPerFrame *= 0.9f;
                particle.Rotation = Main.rand.NextFloat() * MathHelper.TwoPi;
                particle.Scale = new Vector2(0.2f);
                particle.ScaleVelocity = Vector2.One * 0.007f;
                particle.ScaleAcceleration = Vector2.One * 0.002f;
                particle.FadeInNormalizedTime = 0.001f;
                particle.FadeOutNormalizedTime = 0.001f;
                Main.ParticleSystem_World_OverPlayers.Add(particle);

                if (Main.rand.Next(2) == 0)
                {
                    for (int i = 0; i < Main.rand.Next(2, 5); i++)
                    {
                        Dust dust = Dust.NewDustDirect(settings.PositionInWorld - new Vector2(2), 4, 4, 278, 0, 0, 50, MoreColor.Sanguine, 1f);
                        dust.noGravity = true;
                        dust.color.A = 50;
                        dust.velocity += settings.MovementVector;
                        dust.scale = (Main.rand.NextFloat() * 0.5f) + 0.5f;
                    }
                }
            }
        }

        public static void CreatePaleSpeckles(ParticleOrchestraSettings settings)
        {
            if (Main.netMode != NetmodeID.Server && !Main.gamePaused)
            {
                FadingParticle speckle = new FadingParticle();
                speckle.SetBasicInfo(Mod.Assets.Request<Texture2D>("Assets/Textures/Glowball_" + (short)2), null, Vector2.Zero, Vector2.Zero);
                speckle.SetTypeInfo(40);
                speckle.Velocity = (settings.MovementVector * 0.1f) + Main.rand.NextVector2Circular(2, 2);
                speckle.Scale = Vector2.One * Main.rand.NextFloat(0.1f, 0.7f);
                speckle.ScaleVelocity = Vector2.One * -0.01f;
                speckle.LocalPosition = settings.PositionInWorld + Main.rand.NextVector2Circular(5, 5);
                speckle.ColorTint = Color.Lerp(MoreColor.PaleGray, Color.Gainsboro, Main.rand.NextFloat());
                speckle.ColorTint.A = 0;
                speckle.FadeInNormalizedTime = 0.02f;
                speckle.FadeOutNormalizedTime = 0.01f;
                if (Main.rand.Next(2) == 0)
                    Main.ParticleSystem_World_OverPlayers.Add(speckle);

                PrettySparkleParticle sparkle = new PrettySparkleParticle();
                sparkle.Velocity = (settings.MovementVector * 0.1f) + Main.rand.NextVector2Circular(2, 2);
                sparkle.Scale = Vector2.One * Main.rand.NextFloat(0.1f, 0.7f);
                sparkle.ColorTint = Color.Lerp(MoreColor.PaleGray, Color.Gainsboro, Main.rand.NextFloat());
                sparkle.ColorTint.A = 0;
                sparkle.LocalPosition = settings.PositionInWorld + Main.rand.NextVector2Circular(5, 5);
                sparkle.AccelerationPerFrame *= 0.7f;
                sparkle.ScaleVelocity = Vector2.One * -0.01f;
                if (Main.rand.Next(3) == 0)
                    Main.ParticleSystem_World_OverPlayers.Add(sparkle);
            }
        }

        public static void CreateNightMagic(ParticleOrchestraSettings settings, Color color)
        {
            if (Main.netMode != NetmodeID.Server && !Main.gamePaused)
            {
                PrettySparkleParticle particle = new PrettySparkleParticle();
                particle.Velocity = settings.MovementVector * 0.2f;
                particle.Scale = new Vector2(Main.rand.NextFloat()) * 0.6f;
                particle.Rotation = MathHelper.PiOver2 + Main.rand.NextFloat(-0.1f, 0.1f);
                particle.ColorTint = color;
                particle.ColorTint.A = 25;
                particle.Opacity = 1;
                particle.LocalPosition = settings.PositionInWorld + Main.rand.NextVector2Circular(0.2f, 0.2f);
                Main.ParticleSystem_World_OverPlayers.Add(particle);
            }
        }
    }
}
