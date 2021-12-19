using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace BlockContent.Content.NPCs
{
    public class ShaderTest : ModNPC
    {
        public override void SetDefaults()
        {
            NPC.width = NPC.height = 10;
            NPC.friendly = true;
            NPC.lifeMax = 20;
            NPC.noGravity = true;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => false;

        public override string Texture => $"{nameof(BlockContent)}/Assets/Textures/Extra/Glowball_" + (short)3;

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Asset<Texture2D> tex = Mod.Assets.Request<Texture2D>("Assets/Textures/Extra/Glowball_" + (short)3);

            ExtraUtils.ResetSpritebatch(true);

            DrawData data = new DrawData(tex.Value, NPC.Center - screenPos, null, Color.White, 0, tex.Size() / 2, 1f, SpriteEffects.None, 0);
            GameShaders.Misc["Blockaroz:NightEmpress"].Apply(data);
            data.Draw(spriteBatch);
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();

            ExtraUtils.ResetSpritebatch(false);

            return false;
        }
    }
}
