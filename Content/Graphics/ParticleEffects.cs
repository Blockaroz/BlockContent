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
                particle.SetTypeInfo(25);
                particle.ColorTint = MoreColor.Sanguine;
                particle.ColorTint.A = 50;
                particle.LocalPosition = settings.PositionInWorld + Main.rand.NextVector2Circular(8, 8);
                particle.Velocity = settings.MovementVector * 0.5f;
                particle.Rotation = Main.rand.NextFloat() * MathHelper.TwoPi;
                particle.Scale = new Vector2(0.2f);
                particle.ScaleVelocity = Vector2.One * 0.006f;
                particle.ScaleAcceleration = Vector2.One * 0.002f;
                particle.FadeInNormalizedTime = 0.001f;
                particle.FadeOutNormalizedTime = 0.001f;
                Main.ParticleSystem_World_OverPlayers.Add(particle);

                if (Main.rand.Next() == 0)
                {
                    Dust dust = Dust.NewDustDirect(settings.PositionInWorld - new Vector2(2), 4, 4, 278, 0, 0, 0, MoreColor.Sanguine, 1f);
                    dust.noGravity = true;
                    dust.velocity += settings.MovementVector;
                    dust.scale = (Main.rand.NextFloat() * 0.5f) + 0.5f;
                }
            }
        }

        public static void CreatePaleSpeckles(ParticleOrchestraSettings settings)
        {
            if (Main.netMode != NetmodeID.Server && !Main.gamePaused)
            {
                FadingParticle particle = new FadingParticle();
                particle.SetBasicInfo(Mod.Assets.Request<Texture2D>("Assets/Textures/Glowball_" + (short)2), null, Vector2.Zero, Vector2.Zero);
                particle.SetTypeInfo(30);
                particle.Velocity = (settings.MovementVector * 0.1f) + Main.rand.NextVector2Circular(2, 2);
                particle.Scale = Vector2.One * Main.rand.NextFloat(0.1f, 0.7f);
                particle.ColorTint = Color.Gainsboro;
                particle.ColorTint.A = 0;
                particle.LocalPosition = settings.PositionInWorld + Main.rand.NextVector2Circular(5, 5);
                particle.FadeInNormalizedTime = 0.02f;
                particle.FadeOutNormalizedTime = 0.1f;
                particle.AccelerationPerFrame *= 0.7f;
                Main.ParticleSystem_World_OverPlayers.Add(particle);

                if (Main.rand.Next(3) == 0)
                {
                    Color dustColor = Color.Lerp(MoreColor.PaleGray, Color.DimGray, Main.rand.NextFloat(0.5f));
                    dustColor.A = 20;
                    Dust dust = Dust.NewDustPerfect(settings.PositionInWorld + Main.rand.NextVector2Circular(2, 2), ModContent.DustType<Dusts.GlowballDust>(), null, 100, dustColor, 1.33f);
                    dust.fadeIn = 1f + (Main.rand.NextFloat() * 0.7f);
                    dust.noGravity = true;
                    dust.velocity += Main.rand.NextVector2Circular(2, 2);
                    dust.noLightEmittence = true;
                }
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
