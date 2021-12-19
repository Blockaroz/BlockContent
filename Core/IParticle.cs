using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace BlockContent.Core
{
    public interface IParticle
    {
        public void SetStaticDefaults();

        public void Update();

        public void Draw(SpriteBatch spriteBatch);

        public bool Active { get; set; }
    }
}
