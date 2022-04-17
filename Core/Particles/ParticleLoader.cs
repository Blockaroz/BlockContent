using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace BlockContent.Core
{
    public class ParticleLoader : ILoadable
    {
        public void Load(Mod mod)
        {
            if (!Main.dedServ)
            {
                On.Terraria.Main.DrawDust += DrawParticles;
                On.Terraria.Main.UpdateParticleSystems += UpdateParticles;
            }
        }

        public void Unload()
        {
            ParticleSystem.Unload();
        }

        private void DrawParticles(On.Terraria.Main.orig_DrawDust orig, Main self)
        {
            orig(self);
            Main.spriteBatch.Begin(default, default, SamplerState.PointWrap, default, default, null, Main.GameViewMatrix.TransformationMatrix);
            ParticleSystem.DrawParticles(Main.spriteBatch);
            Main.spriteBatch.End();
        }

        private void UpdateParticles(On.Terraria.Main.orig_UpdateParticleSystems orig, Main self)
        {
            orig(self);
            ParticleSystem.UpdateParticles();
        }
    }
}
