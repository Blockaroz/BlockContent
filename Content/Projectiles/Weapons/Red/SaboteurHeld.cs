using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContent.Content.Projectiles.Weapons.Red
{
    public class SaboteurHeld : ModProjectile
    {
        public override string Texture => $"{nameof(BlockContent)}/Content/Items/Weapons/Red/Saboteur";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Saboteur");
        }

        public override void SetDefaults()
        {
            Projectile.width = 66;
            Projectile.height = 20;
            Projectile.aiStyle = -1;
            Projectile.hide = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
        }

        private Player Player => Main.player[Projectile.owner];

        public ref float Time => ref Projectile.ai[0];

        public ref float BounceTime => ref Projectile.localAI[0];

        public override void AI()
        {
            Player.SetDummyItemTime(3);
            Player.itemAnimation = 3;
            Player.heldProj = Projectile.whoAmI;
            Player.ChangeDir(Projectile.direction);

            Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.velocity.ToRotation() - MathHelper.PiOver2 - Player.fullRotation);
            Player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, Projectile.velocity.ToRotation() - MathHelper.PiOver2 - Player.fullRotation);

            Projectile.Center = Player.RotatedRelativePointOld(Player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.velocity.ToRotation() - MathHelper.PiOver2 - Player.fullRotation));
            Projectile.direction = Math.Sign(Projectile.velocity.X);

            bool shoot = false;

            float speed;

            if (Projectile.ai[1] < 10)
                speed = Player.itemAnimationMax * 3.5f;
            else if (Projectile.ai[1] < 30)
                speed = Player.itemAnimationMax * 2.78f;            
            else if (Projectile.ai[1] < 60)
                speed = Player.itemAnimationMax * 1.5f;
            else
                speed = Player.itemAnimationMax;

            if (Time <= 0)
            {
                Projectile.velocity = Projectile.DirectionTo(Main.MouseWorld).RotatedByRandom(0.07f) * 5f;
                Time = speed;
                shoot = true;
            }
            if (!Player.channel || Player.dead || Player.stoned || Player.webbed || Player.noItems)
                Projectile.ai[1] *= 0.5f;

            if (Projectile.ai[1] < 0 && Time <= 1)
                Projectile.Kill();

            if (shoot)
            {
                BounceTime++;

                Vector2 muzzlePos = new Vector2(41, -11 * Projectile.direction).RotatedBy(Projectile.rotation);

                //bullet logic

                Dust dust = Dust.NewDustPerfect(Projectile.Center + muzzlePos, 267, Vector2.UnitX.RotatedBy(Projectile.rotation) * Main.rand.NextFloat(1.5f, 3f), 0, Sanguine, 1f);
                dust.noLightEmittence = true;
                dust.noGravity = true;

                SoundStyle shootSound = SoundID.Item40;//new SoundStyle($"{nameof(BlockContent)}/Assets/Sounds/Items/SaboteurShoot");
                shootSound.MaxInstances = 0;
                shootSound.PitchVariance = 0.1f;
                shootSound.Pitch = 0.1f;
                shootSound.Volume = 0.8f;
                SoundEngine.PlaySound(shootSound, Projectile.Center);

                shoot = false;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.ToRadians(10) * Projectile.direction * BounceTime;

            BounceTime = MathHelper.Lerp(BounceTime, 0, 0.3f);
            if (BounceTime < 0.001f)
                BounceTime = 0;

            if (Player.channel && Projectile.ai[0] <= 60)
                Projectile.ai[1]++;
            else if (Projectile.ai[1] >= 0)
                Projectile.ai[1]--;

            Time--;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => false;

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Asset<Texture2D> glow = ModContent.Request<Texture2D>(Texture + "Glow");
            int spriteDir = Projectile.direction;
            Vector2 origin = texture.Size() * new Vector2(0.2f, 0.5f + 0.16f * spriteDir);
            SpriteEffects dir = spriteDir > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            Color drawColor = Color.White;

            Vector2 handleOffset = new Vector2(-14, 0).RotatedBy(Projectile.rotation);
            Main.EntitySpriteDraw(texture.Value, Projectile.Center + handleOffset - Main.screenPosition, null, lightColor.MultiplyRGBA(drawColor), Projectile.rotation, origin, Projectile.scale, dir, 0);
            Main.EntitySpriteDraw(glow.Value, Projectile.Center + handleOffset - Main.screenPosition, null, new Color(255, 33, 33, 128).MultiplyRGBA(drawColor), Projectile.rotation, origin, Projectile.scale, dir, 0);
            return false;
        }

        private static readonly Color Sanguine = new Color(255, 33, 33);
    }
}
