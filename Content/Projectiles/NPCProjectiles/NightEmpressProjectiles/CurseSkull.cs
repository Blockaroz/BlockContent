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
    public class CurseSkull : ModProjectile
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
            Projectile.timeLeft = 700;
            Projectile.hide = true;
        }

        public override void AI()
        {
            Projectile.ai[0]++;
            if (Projectile.ai[0] <= 165)
            {
                if (Main.npc.IndexInRange((int)Projectile.ai[1]))
                {
                    NPC owner = Main.npc[(int)Projectile.ai[1]];
                    NPCAimedTarget target = owner.GetTargetData();
                    Vector2 targetPos = target.Invalid ? Projectile.Center : target.Center;

                    Vector2 aim = Projectile.DirectionTo(targetPos).SafeNormalize(Vector2.Zero);
                    aim = Vector2.Normalize(Vector2.Lerp(Projectile.velocity.SafeNormalize(Vector2.Zero), aim, 0.05f));
                    if (aim.HasNaNs())
                        aim = -Vector2.UnitY;
                    Projectile.velocity = aim;
                    float distance = MathHelper.SmoothStep(10, 220, Utils.GetLerpValue(0, 100, Projectile.ai[0], true));
                    Projectile.Center = owner.Center + new Vector2(distance, 0).RotatedBy(Projectile.velocity.ToRotation());
                }
            }
            else if (Projectile.ai[0] <= 200)
            {
                PunchCameraModifier punch = new PunchCameraModifier(Projectile.Center, (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2(), 18, 8, 40, 7000f, "NightEmpress");
                Main.instance.CameraModifiers.Add(punch);
            }

            Projectile.spriteDirection = Projectile.direction;
            float flip = Projectile.spriteDirection == -1 ? MathHelper.Pi : 0;
            Projectile.rotation = Projectile.velocity.ToRotation() + flip;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (targetHitbox.Distance(Projectile.Center) <= 240)
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

        public void DrawCurseCharge()
        {

        }

        public void DrawCurseSkull()
        {
            Asset<Texture2D>[] skull = new Asset<Texture2D>[]
            {
                Mod.Assets.Request<Texture2D>("Assets/Textures/NightEmpress/Skull_" + (short)0),
                Mod.Assets.Request<Texture2D>("Assets/Textures/NightEmpress/Skull_" + (short)1)
            };

            float skullScale = MathHelper.SmoothStep(0, 1, Utils.GetLerpValue(60, 140, Projectile.ai[0], true));

            SpriteEffects effects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 skullOrigin = skull[0].Size() / 2 + new Vector2(0, 78);
            float skullRotation = MathHelper.SmoothStep(0, MathHelper.ToRadians(-10), Utils.GetLerpValue(125, 205, Projectile.ai[0], true)) * Projectile.spriteDirection;
            Vector2 skullOffset = new Vector2(0, 78).RotatedBy(Projectile.rotation) * skullScale;

            Vector2 jawOrigin = skull[1].Size() / 2 + new Vector2(-64 * Projectile.spriteDirection, -36);
            float jawRotation = skullRotation + MathHelper.SmoothStep(0, MathHelper.ToRadians(30), Utils.GetLerpValue(120, 200, Projectile.ai[0], true)) * Projectile.spriteDirection;
            Vector2 jawOffset = new Vector2(-24 * Projectile.spriteDirection, 64).RotatedBy(Projectile.rotation) * skullScale;

            Color glowColor = NightEmpress.SpecialColor(0);
            glowColor.A = 25;
            Color drawColor = Color.Lerp(MoreColor.NightSky, glowColor, Utils.GetLerpValue(60, 0, Projectile.timeLeft, true));

            //draw borders
            for (int i = 0; i < 7; i++)
            {

            }

            //draw final skull
            Main.EntitySpriteDraw(skull[1].Value, Projectile.Center + jawOffset - Main.screenPosition, null, drawColor, Projectile.rotation + jawRotation, jawOrigin, skullScale, effects, 0);
            Main.EntitySpriteDraw(skull[0].Value, Projectile.Center + skullOffset - Main.screenPosition, null, drawColor, Projectile.rotation + skullRotation, skullOrigin, skullScale, effects, 0);

        }
    }
}
