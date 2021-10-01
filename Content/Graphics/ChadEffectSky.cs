using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace BlockContent.Content.Graphics
{
    public class ChadEffectSky : CustomSky
    {
        private float _opacity;
        private float _scale;
        private bool _active;

        private struct Star
        {
            public Vector2 center;
            public float depth;
            public float scale;
            public int texture;
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            spriteBatch.Draw(TextureAssets.BlackTile.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black * _opacity);
        }

        public override bool IsActive()
        {
            return _active;
        }

        public override void Activate(Vector2 position, params object[] args)
        {
            _active = true;
        }

        public override void Deactivate(params object[] args)
        {
            _active = false;
        }

        public override void Reset()
        {
            _active = false;
        }

        public override void Update(GameTime gameTime)
        {
            if (_active)
            {
                _opacity = Math.Min(1f, 0.02f + _opacity);
                _scale = Math.Min(1f, 0.02f + _scale);
            }
            else
            {
                _opacity = Math.Max(0f, _opacity - 0.02f);
                _scale = Math.Max(0f, _scale - 0.02f);
            }
        }

        public override float GetCloudAlpha() => 0f;
    }
}
