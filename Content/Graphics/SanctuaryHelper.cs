using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContent.Content.Graphics
{
    public struct SanctuaryHelper
    {
        private static Mod Mod
        {
            get { return ModContent.GetInstance<BlockContent>(); }
        }

        private struct Gun
        {
            public int index;

            public Vector2 position;

            public float rotation;

            public Color color;

            public Color edgeColor;

            public SpriteEffects direction;

            public void DrawGun()
            {
                Asset<Texture2D> texture = Main.Assets.Request<Texture2D>("Images/Item_" + index);
                Effect shader = new MiscShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Assets/Effects/Color", AssetRequestMode.ImmediateLoad).Value), "Color").Shader;
                shader.Parameters["uColor0"].SetValue(Color2.Sanguine.ToVector3());
                shader.Parameters["uColor1"].SetValue(Color.Cyan.ToVector3());
                shader.Parameters["uImageSize"].SetValue(texture.Size());

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, default, default, default, default, shader, Main.GameViewMatrix.TransformationMatrix);

                for (int i = 0; i < 4; i++)
                {
                    Vector2 offset = new Vector2(2f, 0).RotatedBy((MathHelper.TwoPi / 4 * i) + rotation + MathHelper.PiOver4);
                    Main.EntitySpriteDraw(texture.Value, position + offset - Main.screenPosition, null, edgeColor, rotation, texture.Size() * new Vector2(0.2f, 0.5f), 1f, direction, 0);
                }
                Main.EntitySpriteDraw(texture.Value, position - Main.screenPosition, null, color, rotation, texture.Size() * new Vector2(0.2f, 0.5f), 1f, direction, 0);
                
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, default, default, default, default, null, Main.GameViewMatrix.TransformationMatrix);
            }
        }

        public void Draw(Projectile proj)
        {
            int[] possibleIndexes = new int[]
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

            for (int i = 0; i < 7; i++)
            {
                Gun gun = new Gun();
                float rotation = proj.localAI[0] * 0.2f * proj.spriteDirection;
                float progress = Utils.GetLerpValue(20, 70, proj.ai[0], true);
                Vector2 projCenter = proj.Center + new Vector2(34, 0).RotatedBy(proj.rotation);
                Vector2 oval = new Vector2(60f * progress, 0).RotatedBy((MathHelper.TwoPi / 7 * i) + rotation);
                oval.Y *= 0.7f;
                gun.index = Main.rand.Next(possibleIndexes);
                gun.position = projCenter + oval.RotatedBy(proj.rotation);
                gun.rotation = proj.rotation;
                gun.direction = proj.spriteDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None;

                Color drawColor = Color.Lerp(Color.DarkRed, Color2.Sanguine, Utils.GetLerpValue(-40f * progress, 15f * progress, oval.X, true));
                drawColor.A = 0;
                Color edgeColor = Color.Lerp(new Color(20, 0, 0, 0), Color.Black, Utils.GetLerpValue(-40f * progress, 15f * progress, oval.X, true));
                gun.color = drawColor * progress;
                gun.edgeColor = edgeColor * 0.3f * progress;
                gun.DrawGun();
            }
        }
    }
}
