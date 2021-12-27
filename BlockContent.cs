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
            LoadParticles();
        }

        private void DrawParticles(On.Terraria.Main.orig_DrawGore orig, Main self)
        {
            orig(self);
            //Main.spriteBatch.Begin(default, default, SamplerState.PointWrap, default, default, null, Main.GameViewMatrix.TransformationMatrix);
            ParticleSystem.DrawParticles(Main.spriteBatch);
            //Main.spriteBatch.End();
        }

        private void DrawParticlesBehind(On.Terraria.Main.orig_DrawGoreBehind orig, Main self)
        {
            orig(self);
            //Main.spriteBatch.Begin(default, default, SamplerState.PointWrap, default, default, null, Main.GameViewMatrix.TransformationMatrix);
            ParticleSystem.DrawParticlesBehind(Main.spriteBatch);
            //Main.spriteBatch.End();
        }

        private void UpdateParticles(On.Terraria.Main.orig_UpdateParticleSystems orig, Main self)
        {
            orig(self);
            ParticleSystem.UpdateParticles();
        }

        public void LoadParticles()
        {
            if (!Main.dedServ)
            {
                On.Terraria.Main.DrawGoreBehind += DrawParticlesBehind;
                On.Terraria.Main.DrawGore += DrawParticles;
                On.Terraria.Main.UpdateParticleSystems += UpdateParticles;
            }
        }
    }
}