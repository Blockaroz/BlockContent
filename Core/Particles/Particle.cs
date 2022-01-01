﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
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

        public float misc;

        public bool active;

        public int Type { get; private set; }

        public int Index { get; private set; }

        public virtual void Update() { }

        public virtual void Draw(SpriteBatch spriteBatch) { }

        public virtual void OnSpawn() { }

        protected sealed override void Register() 
        {
            ModTypeLookup<Particle>.Register(this);
            Type = ParticleLoader.ReserveParticleID();
            ParticleLoader.particleTypes.Add(this);
        }

        public sealed override void SetupContent()
        {
            SetStaticDefaults();
        }

        public static int ParticleType<T>() where T : Particle
        {
            return ModContent.GetInstance<T>()?.Type ?? -1;
        }

        public virtual Particle NewInstance() => (Particle)MemberwiseClone();

        public static int NewParticle(int type, Vector2 position, Vector2 velocity, Color color, float scale = 1f)
        {
            if (!Main.gamePaused)
            {
                Particle particle = ParticleLoader.GetParticle(type).NewInstance();
                particle.OnSpawn();
                particle.position = position;
                particle.velocity = velocity;
                particle.color = color;
                particle.scale = scale;
                particle.rotation = velocity.ToRotation() + (Main.rand.NextFloat(-0.2f, 0.2f) * MathHelper.TwoPi);
                particle.active = true;
                particle.Index = ParticleLoader.particlesInGame.Count;
                particle.Type = type;
                ParticleLoader.particlesInGame.Add(particle);
                return particle.Index;
            }
               
            return -1;
        }
    }
}
