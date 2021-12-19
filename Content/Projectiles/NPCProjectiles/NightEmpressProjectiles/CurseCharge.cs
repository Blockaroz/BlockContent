using BlockContent.Content.Graphics;
using BlockContent.Content.NPCs.NightEmpressBoss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContent.Content.Projectiles.NPCProjectiles.NightEmpressProjectiles
{
    public class CurseCharge : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Curse");
            ProjectileID.Sets.TrailingMode[Type] = 3;
            ProjectileID.Sets.TrailCacheLength[Type] = 30;
        }
        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 200;
            Projectile.hide = true;
        }

        public override void AI()
        {
            Projectile.ai[0]++;

            if (Projectile.ai[0] == 220)
            {
                PunchCameraModifier punch = new PunchCameraModifier(Projectile.Center, (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2(), 18, 8, 40, 7000f, "NightEmpress");
                Main.instance.CameraModifiers.Add(punch);
            }
            if (Projectile.timeLeft > 180)
                Projectile.velocity *= 1.01f;
            NPC owner = Main.npc[(int)Projectile.ai[1]];
            if (owner.active && owner.ModNPC is NightEmpress)
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(owner.GetTargetData().Center).SafeNormalize(Vector2.Zero) * 10f, 0.01f);
            else
                Projectile.Kill();

            Projectile.spriteDirection = Projectile.direction;
            float flip = Projectile.spriteDirection == -1 ? MathHelper.Pi : 0;
            Projectile.rotation = Projectile.velocity.ToRotation() + flip;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (targetHitbox.Distance(Projectile.Center) <= 240 && Projectile.ai[0] > 120)
                return true;

            return false;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawCurseSkull();

            return false;
        }

        public void DrawCurseSkull()
        {
            Asset<Texture2D>[] skull = new Asset<Texture2D>[]
            {
                Mod.Assets.Request<Texture2D>("Assets/Textures/NightEmpress/Skull_" + (short)0),
                Mod.Assets.Request<Texture2D>("Assets/Textures/NightEmpress/Skull_" + (short)1)
            };

            float skullScale = MathHelper.SmoothStep(0.3f, 0.7f, Utils.GetLerpValue(0, 40, Projectile.ai[0], true));

            SpriteEffects effects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 skullOrigin = (skull[0].Size() / 2) + new Vector2(0, 78);
            float skullRotation = MathHelper.SmoothStep(0, MathHelper.ToRadians(-10), Utils.GetLerpValue(125, 205, Projectile.ai[0], true)) * Projectile.spriteDirection;
            Vector2 skullOffset = new Vector2(0, 78).RotatedBy(Projectile.rotation) * skullScale;

            Vector2 jawOrigin = skull[1].Size() / 2 + new Vector2(-64 * Projectile.spriteDirection, -36);
            float jawRotation = skullRotation + MathHelper.SmoothStep(0, MathHelper.ToRadians(30), Utils.GetLerpValue(30, 60, Projectile.ai[0], true)) * Projectile.spriteDirection;
            Vector2 jawOffset = new Vector2(-24 * Projectile.spriteDirection, 64).RotatedBy(Projectile.rotation) * skullScale;

            float opacity = Utils.GetLerpValue(0, 30, Projectile.timeLeft, true);
            Color glowColor = NightEmpress.SpecialColor(1);
            glowColor.A = 50;
            glowColor *= opacity;
            Color drawColor = Color.Lerp(Color2.NightSky, new Color(255, 255, 255, 0), Utils.GetLerpValue(60, 10, Projectile.timeLeft, true));

            //draw borders
            for (int i = 0; i < 3; i++)
            {
                Vector2 offset = new Vector2(3, 0).RotatedBy((MathHelper.TwoPi / 7 * i) + (Projectile.ai[0] / 60 * Projectile.spriteDirection));
                Main.EntitySpriteDraw(skull[1].Value, Projectile.Center + offset + jawOffset - Main.screenPosition, null, glowColor, Projectile.rotation + jawRotation, jawOrigin, skullScale, effects, 0);
                Main.EntitySpriteDraw(skull[0].Value, Projectile.Center + offset + skullOffset - Main.screenPosition, null, glowColor, Projectile.rotation + skullRotation, skullOrigin, skullScale, effects, 0);
            }

            //draw final skull
            Main.EntitySpriteDraw(skull[1].Value, Projectile.Center + jawOffset - Main.screenPosition, null, drawColor, Projectile.rotation + jawRotation, jawOrigin, skullScale, effects, 0);
            Main.EntitySpriteDraw(skull[0].Value, Projectile.Center + skullOffset - Main.screenPosition, null, drawColor, Projectile.rotation + skullRotation, skullOrigin, skullScale, effects, 0);
        }
    }
}
