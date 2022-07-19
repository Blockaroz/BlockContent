using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleEngine;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace BlockContent.Nihilus.Content.Particles
{
    public class AbyssalFlame : Particle
    {
        //THIS SHOULD NOT DRAW
        public override void OnSpawn()
        {
            life = 0;
            frame = Main.rand.Next(7);
            rotation = Main.rand.NextFloat(0.1f, 1f) * Main.rand.NextFloatDirection();
        }

        public int life;
        public int frame;

        public override void Update()
        {
            scale *= 0.95f;
            if (life > 25)
            {
                scale *= 0.92f;
                velocity *= 0.92f;
            }
            if (scale < 0.05f)
                Active = false;
            velocity.Y -= 0.003f;
            rotation *= 1.02f;

            life++;
        }
    }
}
