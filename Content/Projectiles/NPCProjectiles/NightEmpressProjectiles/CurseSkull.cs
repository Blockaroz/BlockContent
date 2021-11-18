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
                    Projectile.Center = owner.Center + new Vector2(80, 0).RotatedBy(Projectile.velocity.ToRotation());
                }
            }
            Projectile.spriteDirection = Projectile.direction;
            Projectile.rotation = 0;//Projectile.velocity.ToRotation();
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

            SpriteEffects effects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 jawOrigin = new Vector2(12, 34);
            float jawRotation = MathHelper.SmoothStep(0, MathHelper.ToRadians(30), Utils.GetLerpValue(120, 200, Projectile.ai[0], true)) * Projectile.spriteDirection;
            Vector2 jawOffset = new Vector2(-24 * Projectile.spriteDirection, 64).RotatedBy(Projectile.rotation) * Projectile.scale;

            Color glowColor = NightEmpress.SpecialColor(0);
            glowColor.A = 0;
            Color drawColor = Color.Lerp(MoreColor.NightSky, glowColor, Utils.GetLerpValue(30, 0, Projectile.timeLeft, true));

            //draw borders
            for (int i = 0; i < 7; i++)
            {

            }

            //draw final skull
            Main.EntitySpriteDraw(skull[1].Value, Projectile.Center + jawOffset - Main.screenPosition, null, drawColor, Projectile.rotation + jawRotation, jawOrigin, Projectile.scale, effects, 0);
            Main.EntitySpriteDraw(skull[0].Value, Projectile.Center - Main.screenPosition, null, drawColor, Projectile.rotation, skull[0].Size() / 2, Projectile.scale, effects, 0);

        }
    }
}
