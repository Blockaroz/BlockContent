using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using BlockContent.Core;

namespace BlockContent.Content.Projectiles.NegastaffMinions
{
    public partial class NegastaffMinion : ModProjectile
    {
        public void AI_Seeksery()
        {
            Player player = Main.player[Projectile.owner];

            if (FindTarget_NPC(player, out int id, 1800))
            {
                NPC target = Main.npc[id];
                Projectile.spriteDirection = Projectile.Center.X > target.Center.X ? -1 : 1;

                Projectile.ai[1]++;
                if (Projectile.ai[1] == 100)
                {
                    Vector2 megidoPos = SuitableMegidoLocation(Projectile.Center);
                    Projectile megido = Projectile.NewProjectileDirect(Projectile.GetProjectileSource_FromThis(), megidoPos, megidoPos.DirectionTo(target.Center) * 25f, ModContent.ProjectileType<Seeksery.MegidoMeteor>(), (int)(Projectile.damage * 1.3f), 5f, Projectile.owner);
                    megido.ai[0] = target.whoAmI;
                    
                }
                else if (Projectile.ai[1] > 120)
                    Projectile.ai[1] = 0;
            }
            else
            {
                Projectile.spriteDirection = player.direction;
                Projectile.ai[1] = 0;
            }
            Float(player);

            //if (Main.rand.Next(10) == 0)
            //    Particle.NewParticle(Particle.ParticleType<Particles.LargeExplosion>(), Projectile.Center + Main.rand.NextVector2Circular(40, 40), Vector2.Zero, new Color(255, 255, 255, 0));
        }

        private Vector2 SuitableMegidoLocation(Vector2 from)
        {
            float maxHeight = 0;
            float rand = Main.rand.NextFloat(0.1f) * -Projectile.spriteDirection;
            for (int i = 0; i < 200; i++)
            {
                Vector2 height = from + new Vector2(0, -maxHeight).RotatedBy(rand);
                if (Collision.SolidTiles(height, 20, 20))
                    continue;
                maxHeight += 11;
            }
            return from + new Vector2(0, -maxHeight).RotatedBy(rand);
        }

        private void Float(Player player)
        {
            float counter = (Main.GlobalTimeWrappedHourly + (Projectile.ai[0] * 0.33f)) % 60;
            Vector2 floaty = new Vector2((float)Math.Sin(counter * 3f % MathHelper.TwoPi), (float)Math.Cos(counter * 3f % MathHelper.TwoPi) * 0.4f);
            if (Projectile.Distance(idlePos) > 50)
                floaty /= Projectile.Distance(idlePos) * 0.01f;
            if (Projectile.ai[0] > 0)
                floaty *= 0.1f;
            Projectile.Center = Vector2.Lerp(Projectile.Center, idlePos, 0.4f);
            Projectile.Center += floaty;
        }

        private void Draw_SeekseryRuneCircle(float progress)
        {

        }

        public void Draw_Seeksery()
        {
            Asset<Texture2D> snail = Mod.Assets.Request<Texture2D>("Content/Projectiles/NegastaffMinions/Seeksery/Seeksery");
            Rectangle baseFrame = snail.Frame(1, 2, 0, 0);
            Rectangle glowFrame = snail.Frame(1, 2, 0, 1);

            if (Projectile.ai[1] > 0)
                Draw_SeekseryRuneCircle(Utils.GetLerpValue(5, 70, Projectile.ai[1], true) * Utils.GetLerpValue(120, 100, Projectile.ai[1], true));

            for (int i = 0; i < 4; i++)
            {
                Vector2 posOffset = new Vector2(1, 1).RotatedBy(MathHelper.TwoPi / 4f * i);
                Main.EntitySpriteDraw(snail.Value, Projectile.Center + posOffset - Main.screenPosition, glowFrame, Color.White, Projectile.rotation, glowFrame.Size() * 0.5f, Projectile.scale, GetSpriteEffects(), 0);
            }

            Main.EntitySpriteDraw(snail.Value, Projectile.Center - Main.screenPosition, baseFrame, Lighting.GetColor(Projectile.Center.ToTileCoordinates()), Projectile.rotation, baseFrame.Size() * 0.5f, Projectile.scale, GetSpriteEffects(), 0);
        }
    }
}
