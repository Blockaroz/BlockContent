using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace BlockContent.Core
{
    public abstract class Particle 
    {
        public Vector2 position;

        public Vector2 velocity;

        public float rotation;

        public float scale;

        public Color color;

        public float misc;

        public bool Active { get; set; }

        public static Mod Mod
        {
            get => ModContent.GetInstance<BlockContent>();
        }

        public virtual void SetStaticDefaults() { }

        public virtual void Update() { }

        public virtual void Draw(SpriteBatch spriteBatch) { }
    }
}
