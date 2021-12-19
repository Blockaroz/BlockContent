using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace BlockContent.Core
{
    public class Particle : IParticle
    {
        public Vector2 position;

        public Vector2 velocity;

        public float rotation;

        public float scale;

        public Color color;

        public bool Active { get; set; }

        public float misc;

        public Mod Mod
        {
            get => ModContent.GetInstance<BlockContent>();
        }

        public virtual void SetStaticDefaults() { }

        public virtual void Update() { }

        public virtual void Draw(SpriteBatch spriteBatch) { }
    }
}
