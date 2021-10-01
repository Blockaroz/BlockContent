using BlockContent.Content.Graphics;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContent
{
	public class BlockContent : Mod
	{
        public override void Load()
        {
            EffectLoader.LoadEffects();
        }
    }
}