using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace BlockContent.Core
{
    public abstract class Particle : ModType
    {
        public Vector2 position;

        public Vector2 velocity;

        public float rotation;

        public float scale;

        public Color color;

        public bool emit;

        public object data;

        public ArmorShaderData shader;

        public bool Active { get; set; }

        public int Type { get; private set; }

        public virtual void Update() { }

        public virtual void Draw(SpriteBatch spriteBatch) { }

        public virtual void OnSpawn() { }

        protected sealed override void Register() 
        {
            ModTypeLookup<Particle>.Register(this);
            Type = ParticleSystem.ReserveParticleID();
            ParticleSystem.particleTypes.Add(this);
        }

        public sealed override void SetupContent() => SetStaticDefaults();

        public static int ParticleType<T>() where T : Particle => ModContent.GetInstance<T>()?.Type ?? -1;

        public static Particle NewParticle(int type, Vector2 position, Vector2 velocity, Color color, float scale = 1f)
        {
            if (!Main.gamePaused)
            {
                Particle p = (Particle)ParticleSystem.GetParticle(type).MemberwiseClone();
                p.position = position;
                p.velocity = velocity;
                p.color = color;
                p.scale = scale;
                p.rotation = velocity.ToRotation() + (Main.rand.NextFloat(-0.2f, 0.2f) * MathHelper.TwoPi);
                p.Active = true;
                p.Type = type;
                p.OnSpawn();
                ParticleSystem.particle.Add(p);
                return p;
            }
            return null;
        }
    }
}
