using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContent.Content.Graphics
{
    public struct SanctuaryHelper
    {
        private Mod Mod
        {
            get { return ModContent.GetInstance<BlockContent>(); }
        }

        private static int _idOffset;
        public void Draw(Projectile proj)
        {
            int[] index = new int[]
            {
                ItemID.QuadBarrelShotgun,
                ItemID.Boomstick,
                ItemID.PhoenixBlaster,
                ItemID.ClockworkAssaultRifle,
                ItemID.OnyxBlaster,
                ItemID.Flamethrower,
                ItemID.VenusMagnum,
                ItemID.SniperRifle,
                ItemID.ChainGun,
                ItemID.Xenopopper,
                ItemID.SDMG,
                ItemID.Celeb2
            };
            if (proj.ai[0] == 0)
                _idOffset = Main.rand.Next(index.Length - 7);

            for (int i = 0; i < 7; i++)
            {
                Asset<Texture2D> gunTexture = Main.Assets.Request<Texture2D>("Images/Item_" + index[i + _idOffset]);
                float rotation = proj.localAI[0] * 0.2f * proj.spriteDirection;
                float progress = Utils.GetLerpValue(20, 70, proj.ai[0], true);

                const int ovalWidth = 60;
                Vector2 projCenter = proj.Center + new Vector2(32, 0).RotatedBy(proj.rotation);
                Vector2 oval = new Vector2(ovalWidth * progress, 0).RotatedBy((MathHelper.TwoPi / 7 * i) + rotation);
                oval.Y *= 0.7f;
                Vector2 gunPosition = projCenter + oval.RotatedBy(proj.rotation);
                Vector2 gunOrigin = gunTexture.Size() * new Vector2(0.2f, 0.5f);
                SpriteEffects effects = proj.spriteDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None;

                Color gradient = new GradientColor(new Color[]
                {
                    MoreColor.Sanguine,
                    new Color(233, 0, 50),
                    new Color(233, 50, 0),
                    new Color(250, 33, 33)
                },
                2f, 1.8f).Value;
                Color glowColor = Color.Lerp(Color.DarkRed, gradient, Utils.GetLerpValue(-25 * progress, 5 * progress, oval.X, true));
                glowColor.A = 0;
                Color darkColor = Color.Lerp(Color.Black * 0.5f, Color.DarkRed, Utils.GetLerpValue(-25 * progress, 5 * progress, oval.X, true));
                darkColor.A /= 2;

                //Main.spriteBatch.End();
                //Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                DrawData gunData = new DrawData(gunTexture.Value, gunPosition - Main.screenPosition, null, glowColor * progress, proj.rotation, gunOrigin, 1f, effects, 0);
                DrawData gunBackData = new DrawData(gunTexture.Value, gunPosition - Main.screenPosition, null, darkColor * progress, proj.rotation, gunOrigin, 1f, effects, 0);
                //GameShaders.Misc["BlockContent:Grayscale"].Apply(gunData);
                Main.EntitySpriteDraw(gunBackData);
                Main.EntitySpriteDraw(gunData);
                //Main.spriteBatch.End();
                //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            }
        }
    }
}
