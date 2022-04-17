using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace BlockContent.Content.Projectiles.NegastaffMinions
{
    public partial class NegastaffMinion : ModProjectile
    {
        public void AI_Seeksery()
        {
            Player player = Main.player[Projectile.owner];

            if (FindTarget_NPC(player, out int id, 1000))
            {
                
            }
            else
                Float(player);
        }

        private void Float(Player player)
        {
            Projectile.spriteDirection = player.direction;
            Vector2 floaty = new Vector2((float)Math.Sin(Projectile.localAI[0] / 20f % MathHelper.TwoPi), (float)Math.Cos(Projectile.localAI[0] / 20f % MathHelper.TwoPi) * 0.4f);
            if (Projectile.Distance(idlePos) > 50)
                floaty /= Projectile.Distance(idlePos) * 0.01f;
            Projectile.Center = Vector2.Lerp(Projectile.Center, idlePos, 0.1f) + floaty;
        }

        public void Draw_Seeksery()
        {
            Asset<Texture2D> snail = Mod.Assets.Request<Texture2D>("Content/Projectiles/NegastaffMinions/Seeksery/Seeksery");
            Rectangle baseFrame = snail.Frame(1, 2, 0, 0);
            Rectangle glowFrame = snail.Frame(1, 2, 0, 1);

            for (int i = 0; i < 9; i++)
            {

            }

            Main.EntitySpriteDraw(snail.Value, Projectile.Center - Main.screenPosition, baseFrame, Color.White, Projectile.rotation, snail.Size() * 0.5f, Projectile.scale, GetSpriteEffects(), 0);
        }
    }
}
