using BlockContent.Content.NPCs.NightEmpressBoss;
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
        private Vector2 _modifiedCenter;

        public override void AI()
        {
            if (Main.npc.IndexInRange((int)Projectile.ai[1]))
            {
                Projectile.localAI[0]++;
                NPC owner = Main.npc[(int)Projectile.ai[1]];
                Projectile.Center = owner.Center;
                Projectile.scale = MoreUtils.DualLerp(0, 20, _maxTime - 60, _maxTime, Projectile.localAI[0], true);
                float rotation = Utils.GetLerpValue(_maxTime, 15, Projectile.localAI[0], true) + (Utils.GetLerpValue(30, 0, Projectile.localAI[0], true) * 0.5f);
                Projectile.rotation = Projectile.ai[0] - (rotation * MathHelper.ToRadians(45));

                float centerLerp = MathHelper.SmoothStep(0, 1, MoreUtils.DualLerp(45, 60, _maxTime - 90, _maxTime - 45, Projectile.localAI[0], true));
                _modifiedCenter = Projectile.Center + new Vector2(centerLerp * 340, 0).RotatedBy(Projectile.rotation);
            }
            else
                Projectile.Kill();
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0;
            float modif = Projectile.scale * 0.85f;
            Vector2 outward = Projectile.rotation.ToRotationVector2() * modif;
            if (Collision.CheckAABBvLineCollision(targetHitbox.Center(), targetHitbox.Size(), _modifiedCenter, _modifiedCenter + (outward * 170), modif * 70, ref collisionPoint))
                return true;
            if (Collision.CheckAABBvLineCollision(targetHitbox.Center(), targetHitbox.Size(), _modifiedCenter, _modifiedCenter + (outward * 340), modif * 40, ref collisionPoint))
                return true;
            if (Collision.CheckAABBvLineCollision(targetHitbox.Center(), targetHitbox.Size(), _modifiedCenter, _modifiedCenter + (outward * 420), modif * 20, ref collisionPoint))
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
            Vector2 origin = baseFrame.Size() * new Vector2(0.08f, 0.5f);
            Color dark = NightEmpress.SpecialColor(1);
            dark.A = 25;
            Color light = NightEmpress.SpecialColor(0);
            light.A = 25;
            Color night = NightEmpress.SpecialColor(0, true);
            night.A = 25;

            Vector2 scale = new Vector2(0.9f, 0.7f) * Projectile.scale;

            Main.EntitySpriteDraw(texture.Value, _modifiedCenter - Main.screenPosition, baseFrame, Color.Black * 0.15f, Projectile.rotation, origin, scale * 1.1f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture.Value, _modifiedCenter - Main.screenPosition, baseFrame, dark, Projectile.rotation, origin, scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture.Value, _modifiedCenter - Main.screenPosition, baseFrame, light * 0.4f, Projectile.rotation, origin, scale * 0.94f, SpriteEffects.None, 0);
            //overlay
            Main.EntitySpriteDraw(texture.Value, _modifiedCenter - Main.screenPosition, overlayFrame, night, Projectile.rotation, origin, scale * 0.9f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture.Value, _modifiedCenter - Main.screenPosition, overlayFrame, MoreColor.NightSky, Projectile.rotation, origin, scale * 0.75f, SpriteEffects.None, 0);
            
            return false;
        }
    }
}
