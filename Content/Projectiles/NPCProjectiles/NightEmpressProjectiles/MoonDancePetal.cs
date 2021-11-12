﻿using BlockContent.Content.NPCs.NightEmpressBoss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContent.Content.Projectiles.NPCProjectiles.NightEmpressProjectiles
{
    public class MoonDancePetal : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Moon Dance");
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = _maxTime;
            Projectile.hide = true;
        }

        private const int _maxTime = 190;

        public override void AI()
        {
            if (Main.npc.IndexInRange((int)Projectile.ai[1]))
            {
                Projectile.localAI[0]++;
                NPC owner = Main.npc[(int)Projectile.ai[1]];
                Projectile.Center = owner.Center;
                Projectile.scale = MoreUtils.DualLerp(0, 20, _maxTime - 60, _maxTime, Projectile.localAI[0], true);
                float rotation = Utils.GetLerpValue(_maxTime, 15, Projectile.localAI[0], true);
                Projectile.rotation = Projectile.ai[0] - (rotation * MathHelper.ToRadians(45));
            }
            else
                Projectile.Kill();
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0;
            float modif = Projectile.scale * 0.85f;
            Vector2 outward = Projectile.rotation.ToRotationVector2() * modif;
            if (Collision.CheckAABBvLineCollision(targetHitbox.Center(), targetHitbox.Size(), Projectile.Center, Projectile.Center + (outward * 170), modif * 70, ref collisionPoint))
                return true;
            if (Collision.CheckAABBvLineCollision(targetHitbox.Center(), targetHitbox.Size(), Projectile.Center, Projectile.Center + (outward * 350), modif * 40, ref collisionPoint))
                return true;
            if (Collision.CheckAABBvLineCollision(targetHitbox.Center(), targetHitbox.Size(), Projectile.Center, Projectile.Center + (outward * 420), modif * 20, ref collisionPoint))
                return true;
            return false;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> texture = Mod.Assets.Request<Texture2D>("Content/Projectiles/NPCProjectiles/NightEmpressProjectiles/MoonDancePetal");
            Rectangle baseFrame = texture.Frame(1, 2, 0, 0);
            Rectangle overlayFrame = texture.Frame(1, 2, 0, 1);
            Vector2 origin = baseFrame.Size() * new Vector2(0.04f, 0.5f);
            Color light = NightEmpress.GlowColor(0.66f);
            light.A = 25;
            Color night = NightEmpress.GlowColor(0, true);
            night.A = 25;

            Vector2 scale = new Vector2(0.8f, 0.66f) * Projectile.scale;

            Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, baseFrame, Color.Black * 0.15f, Projectile.rotation, origin, scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, baseFrame, light, Projectile.rotation, origin, scale * 0.95f, SpriteEffects.None, 0);
            //overlay
            Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, overlayFrame, night, Projectile.rotation, origin, scale * 0.92f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, overlayFrame, MoreColor.NightSky, Projectile.rotation, origin, scale * 0.75f, SpriteEffects.None, 0);
            
            return false;
        }
    }
}
