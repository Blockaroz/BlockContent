using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.GameContent.NetModules;
using Terraria.Graphics.Renderers;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace BlockContent.Content.Graphics
{
    public class ParticleEffects
    {
        public static void CreatePaleSparkles(ParticleOrchestraSettings settings, Color color)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                PrettySparkleParticle particle = new PrettySparkleParticle();
                float scale = Main.rand.NextFloat() * 0.75f;
                particle.Velocity = (settings.MovementVector * 0.1f) + Main.rand.NextVector2Circular(2, 2);
                particle.Scale = new Vector2(scale);
                particle.ColorTint = color;
                particle.ColorTint.A /= 2;
                particle.Opacity = Main.rand.NextFloat() + 0.2f;
                particle.LocalPosition = settings.PositionInWorld + Main.rand.NextVector2Circular(7, 7);
                Main.ParticleSystem_World_OverPlayers.Add(particle);
            }
        }

        public static void CreateNightMagic(ParticleOrchestraSettings settings, Color color)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                PrettySparkleParticle particle = new PrettySparkleParticle();
                particle.Velocity = settings.MovementVector * 0.2f;
                particle.Scale = new Vector2(Main.rand.NextFloat(), Main.rand.NextFloat());
                particle.Rotation = MathHelper.PiOver2;
                particle.ColorTint = color;
                particle.ColorTint.A /= 2;
                particle.Opacity = 1;
                particle.LocalPosition = settings.PositionInWorld + Main.rand.NextVector2Circular(0.2f, 0.2f);
                Main.ParticleSystem_World_OverPlayers.Add(particle);
            }
        }
    }
}
