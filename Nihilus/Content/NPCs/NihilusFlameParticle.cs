using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleEngine;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace BlockContent.Nihilus.Content.NPCs
{
    public class NihilusFlameParticle : Particle
    {
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
            velocity *= 0.95f;
            scale *= 0.95f;
            if (life > 25)
                velocity *= 0.9f;
            if (scale < 0.05f)
                Active = false;
            velocity.X *= 1.02f;
            velocity.Y *= 1.01f;
            velocity.Y -= 0.002f;
            rotation *= 1.02f;

            life++;
        }
    }
}
