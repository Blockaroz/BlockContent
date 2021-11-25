using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace BlockContent.Content.Skies
{
    public class NightEmpressSky : CustomSky
    {
        private Mod Mod
        {
            get { return ModContent.GetInstance<BlockContent>(); }
        }

        private bool _active;

        private float _opacity;

        private Asset<Texture2D> sky;

        private Cloud[] _cloud;

        private struct Cloud
        {
            public Asset<Texture2D> texture;

            public Vector2 position;

            public float scale;

            public float depth;

            public Color color;

            public SpriteEffects direction;
        }

        public override void OnLoad()
        {
            sky = Mod.Assets.Request<Texture2D>("Assets/Textures/Skies/NightEmpressSky");
        }

        public override void Activate(Vector2 position, params object[] args)
        {
            _active = true;
            _opacity = 0;
            _cloud = new Cloud[3000];
            int index = 0;
            short[] tex = new short[]
            {
                1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21
            };
            for (int i = 0; i < 300; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    _cloud[index].position.X = ((float)Main.maxTilesX * 16f) * (i / 300f);
                    _cloud[index].position.Y = (((float)(Main.worldSurface * 16f) + 2000f) * (j / 10f)) - 1000f;
                    _cloud[index].texture = Main.Assets.Request<Texture2D>("Images/Cloud_" + Main.rand.Next(tex));//TextureAssets.Cloud[Main.rand.Next(8)];
                    _cloud[index].depth = 1.5f + Main.rand.NextFloat(7f);
                    _cloud[index].scale = 1f + Main.rand.NextFloat(2f);
                    _cloud[index].color = MoreColor.NightSky * 0.1f * Utils.GetLerpValue(8, 0, _cloud[index].depth, true);
                    _cloud[index].position += Main.rand.NextVector2Circular(5, 5);
                    _cloud[index].direction = Main.rand.Next(1) == 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                    index++;
                }
            }
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
                spriteBatch.Draw(TextureAssets.BlackTile.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black * 0.7f * _opacity);
            }

            if (maxDepth >= float.MinValue && minDepth < float.MaxValue)
            {
                spriteBatch.Draw(TextureAssets.BlackTile.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), MoreColor.NightSky * 0.1f * _opacity);
                spriteBatch.Draw(sky.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), new Color(128, 128, 128, 25) * 0.4f * _opacity);
            }

            Rectangle view = new Rectangle(-1000, -1000, 4000, 4000);
            int depth = -1;
            for (int i = 0; i < _cloud.Length; i++)
            {
                if (depth == -1 && _cloud[i].depth < maxDepth)
                {

                }
            }
            for (int j = 0; j < _cloud.Length; j++)
            {
                Vector2 screen = Main.screenPosition + new Vector2(Main.screenWidth >> 1, Main.screenHeight >> 1);
                Vector2 cloudPos = (_cloud[j].position - screen) * new Vector2(1f / _cloud[j].depth, 1.1f / _cloud[j].depth) + screen - Main.screenPosition;
                if (view.Contains(cloudPos.ToPoint()))
                {
                    spriteBatch.Draw(_cloud[j].texture.Value, cloudPos, null, _cloud[j].color * _opacity, 0, _cloud[j].texture.Size() / 2, _cloud[j].scale, _cloud[j].direction, 0);
                }
            }
        }

        public override float GetCloudAlpha()
        {
            return 1f - _opacity;
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
