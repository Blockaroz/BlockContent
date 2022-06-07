using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace BlockContent
{
    public class BlockContent : Mod
    {
        public override void Load()
        {
            ParticleEngine.ParticleLoader.Load();
        }

        public override void Unload()
        {
            ParticleEngine.ParticleLoader.Unload();
        }
    }
}