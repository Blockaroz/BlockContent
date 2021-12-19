using BlockContent.Content.Graphics;
using Microsoft.Xna.Framework.Graphics;
using BlockContent.Core;
using Terraria;
using Terraria.ModLoader;

namespace BlockContent
{
    public class BlockContent : Mod
    {
        public override void Load()
        {
            EffectLoader.LoadEffects();

            if (!Main.dedServ)
            {
                On.Terraria.Main.DrawDust += DrawParticles;
                On.Terraria.Main.UpdateParticleSystems += UpdateParticles;
            }
        }

        private void DrawParticles(On.Terraria.Main.orig_DrawDust orig, Main self)
        {
            orig(self);
            Main.spriteBatch.Begin(default, default, SamplerState.PointWrap, default, default, null, Main.GameViewMatrix.TransformationMatrix);
            ParticlePool.DrawParticles(Main.spriteBatch);
            Main.spriteBatch.End();
        }

        private void UpdateParticles(On.Terraria.Main.orig_UpdateParticleSystems orig, Main self)
        {
            orig(self);
            ParticlePool.UpdateParticles();
        }
    }
}