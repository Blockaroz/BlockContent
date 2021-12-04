using Terraria.Graphics.Renderers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace BlockContent.Content.Graphics.Particles
{
    public class StarParticle : ABasicParticle
    {
        public Color ColorTint = Color.White;

        public int Direction = 1;

        private float _timeLeft;

        private float _timeAlive = 0f;

        public void SetTypeInfo(float timeLeft)
        {
            _timeLeft = Math.Max(10f, timeLeft);
        }

        public override void Update(ref ParticleRendererSettings settings)
        {
            base.Update(ref settings);
            _timeAlive++;
            if (_timeAlive > _timeLeft)
                ShouldBeRemovedFromRenderer = true;
        }

        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch)
        {
            Vector2 scale = Scale * Utils.GetLerpValue(0f, 10f, _timeAlive, true) * Utils.GetLerpValue(_timeLeft, _timeLeft - 5f, _timeAlive, true);
            spritebatch.Draw(_texture.Value, settings.AnchorPosition + LocalPosition, _frame, ColorTint, Rotation, _origin, scale, SpriteEffects.None, 0);
            spritebatch.Draw(_texture.Value, settings.AnchorPosition + LocalPosition, _frame, new Color(255, 255, 255, 0), Rotation, _origin, scale * 0.5f, SpriteEffects.None, 0);
        }
    }
}
