using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ID;

namespace BlockContent.Content.Graphics
{
    public struct BlessingChad
    {
        private Mod Mod
        {
            get { return ModContent.GetInstance<BlockContent>(); }
        }

        public void Draw(Player player, float time)
        {
            Asset<Texture2D> chadHead = Mod.Assets.Request<Texture2D>("Assets/Textures/BlessingChad/BlessingChad_Head");
            Asset<Texture2D> chadBody = Mod.Assets.Request<Texture2D>("Assets/Textures/BlessingChad/BlessingChad");
            Asset<Texture2D> chadArm1 = Mod.Assets.Request<Texture2D>("Assets/Textures/BlessingChad/BlessingChad_Arm1");
            Asset<Texture2D> chadArm2 = Mod.Assets.Request<Texture2D>("Assets/Textures/BlessingChad/BlessingChad_Arm2");

            SpriteEffects effects = player.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float opacity = Utils.GetLerpValue(0, 10, time, true) * Utils.GetLerpValue(100, 90, time, true);
            float startToFinishLerp = Utils.GetLerpValue(0, 100, time, true);

            Vector2 pos = player.Center + new Vector2(0, MathHelper.SmoothStep(-90, -120, startToFinishLerp));
            pos.Y -= 0.5f;
            float scale = MathHelper.SmoothStep(0.8f, 1f, startToFinishLerp);

            float armRotLerp = Utils.GetLerpValue(0, 70, time, true);
            float armRot = MathHelper.Lerp(MathHelper.ToRadians(-5), MathHelper.ToRadians(8), armRotLerp) * player.direction;

            float armXOffset = MathHelper.SmoothStep(5, 20, armRotLerp) * player.direction;
            float armYOffset = MathHelper.Lerp(10, -20, armRotLerp);
            Vector2 armPos1 = pos + new Vector2(-265 * player.direction, -108) + new Vector2(armXOffset, armYOffset);
            Vector2 armPos2 = pos + new Vector2(230 * player.direction, -90) + new Vector2(-armXOffset, armYOffset);

            Vector2 headPos = pos + new Vector2(MathHelper.Lerp(-2, -15, startToFinishLerp) * player.direction, MathHelper.Lerp(-170, -185, startToFinishLerp));

            Color alpha = Color.White;
            alpha.A = 0;

            for (int i = 0; i < 4; i++)
            {
                Vector2 rotOffset = new Vector2(0, 5f).RotatedBy(MathHelper.PiOver2 * i);
                Main.EntitySpriteDraw(chadBody.Value, pos + rotOffset - Main.screenPosition, null, alpha * 0.1f * opacity, 0, chadBody.Size() / 2, 1f, effects, 0);
                Main.EntitySpriteDraw(chadHead.Value, headPos + rotOffset - Main.screenPosition, null, alpha * 0.1f * opacity, 0, chadHead.Size() / 2, 1f, effects, 0);
                Main.EntitySpriteDraw(chadArm1.Value, armPos1 + rotOffset - Main.screenPosition, null, alpha * 0.1f * opacity, armRot, chadArm1.Size() / 2, 1f, effects, 0);
                Main.EntitySpriteDraw(chadArm2.Value, armPos2 + rotOffset - Main.screenPosition, null, alpha * 0.1f * opacity, -armRot, chadArm2.Size() / 2, 1f, effects, 0);
            }

            float glowLerp = Utils.GetLerpValue(70, 100, time, true) * Utils.GetLerpValue(130, 80, time, true);
            for (int i = 0; i < 5; i++)
            {
                Vector2 offsetSize = new Vector2((i - 2f) * 50, 0);
                Vector2 glowOffset = Vector2.SmoothStep(Vector2.Zero, offsetSize, glowLerp);
                Main.EntitySpriteDraw(chadBody.Value, pos + glowOffset - Main.screenPosition, null, alpha * 0.2f * opacity, 0, chadBody.Size() / 2, 1f, effects, 0);
                Main.EntitySpriteDraw(chadHead.Value, headPos + glowOffset - Main.screenPosition, null, alpha * 0.2f * opacity, 0, chadHead.Size() / 2, 1f, effects, 0);
                Main.EntitySpriteDraw(chadArm1.Value, armPos1 + glowOffset - Main.screenPosition, null, alpha * 0.2f * opacity, armRot, chadArm1.Size() / 2, 1f, effects, 0);
                Main.EntitySpriteDraw(chadArm2.Value, armPos2 + glowOffset - Main.screenPosition, null, alpha * 0.2f * opacity, -armRot, chadArm2.Size() / 2, 1f, effects, 0);
            }

            Main.EntitySpriteDraw(chadBody.Value, pos - Main.screenPosition, null, alpha * opacity, 0, chadBody.Size() / 2, 1f, effects, 0);
            Main.EntitySpriteDraw(chadArm1.Value, armPos1 - Main.screenPosition, null, alpha * opacity, armRot, chadArm1.Size() / 2, 1f, effects, 0);
            Main.EntitySpriteDraw(chadArm2.Value, armPos2 - Main.screenPosition, null, alpha * opacity, -armRot, chadArm2.Size() / 2, 1f, effects, 0);

            Dust dust = Dust.NewDustDirect(pos - new Vector2(377, 284), 754, 588, DustID.AncientLight, 0, -10, 0, Color.Wheat, Main.rand.NextFloat(1, 2.5f));
            dust.noGravity = true;
            dust.velocity *= 0.8f;
        }
    }
}
