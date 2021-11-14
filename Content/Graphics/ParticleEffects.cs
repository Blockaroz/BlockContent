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

        public static void CreatePaleSparkles(ParticleOrchestraSettings settings, Color color)
        {
            if (Main.netMode != NetmodeID.Server && !Main.gamePaused)
            {
                FadingParticle particle = new FadingParticle();
                particle.SetBasicInfo(Mod.Assets.Request<Texture2D>("Assets/Textures/Glowball_" + (short)2), null, Vector2.Zero, Vector2.Zero);
                particle.SetTypeInfo(28);
                particle.Velocity = (settings.MovementVector * 0.1f) + Main.rand.NextVector2Circular(2, 2);
                particle.Scale = Vector2.One * Main.rand.NextFloat(0.1f, 0.7f);
                particle.ColorTint = color;
                particle.ColorTint.A = 0;
                particle.LocalPosition = settings.PositionInWorld + Main.rand.NextVector2Circular(7, 7);
                particle.FadeInNormalizedTime = 0.1f;
                particle.FadeOutNormalizedTime = 0.3f;
                particle.AccelerationPerFrame *= 0.7f;
                Main.ParticleSystem_World_OverPlayers.Add(particle);
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
