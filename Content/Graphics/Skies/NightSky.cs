using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace BlockContent.Content.Skies
{
    public class NightSky : CustomSky
    {
        private Mod Mod
        {
            get { return ModContent.GetInstance<BlockContent>(); }
        }

        private bool _active;

        private float _opacity;

        public override void Activate(Vector2 position, params object[] args)
        {
            _active = true;
            _opacity = 0;
        }

        public override void Deactivate(params object[] args) => _active = false;

        public override void Reset() => _active = false;

        public override bool IsActive() 
        {
            if (!_active)
                return _opacity > 0.001f;
            return true;
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (maxDepth >= float.MaxValue && minDepth < float.MaxValue)
            {
                spriteBatch.Draw(TextureAssets.BlackTile.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black * 0.8f * _opacity);

            }

            float glowOpacity = 0.6f;
            if (!Main.dayTime)
                glowOpacity = 0.3f;

            if (maxDepth >= float.MinValue && minDepth < float.MaxValue)
                spriteBatch.Draw(Mod.Assets.Request<Texture2D>("Assets/Textures/Skies/NightEmpressSky").Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), new Color(128, 128, 128, 50) * glowOpacity * _opacity);

        }

        public override void Update(GameTime gameTime)
        {
            if (_active)
                _opacity = Math.Min(1f, 0.01f + _opacity);
            else
                _opacity = Math.Max(0f, _opacity - 0.01f);
        }
    }
}
