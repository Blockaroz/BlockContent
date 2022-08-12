using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;
using ReLogic.Content;
using Terraria.Graphics.Shaders;
using Terraria.Audio;

namespace BlockContent.Content.Mounts.SteampunkJetwing
{
    public class SteampunkJetwing : ModMount
    {
        public override void SetStaticDefaults()
        {
            MountData.blockExtraJumps = true;
            MountData.constantJump = false;
            MountData.heightBoost = 2;
            MountData.fallDamage = 0f;
            MountData.spawnDust = DustID.Torch;
            MountData.buff = ModContent.BuffType<SteampunkJetwingBuff>();

            MountData.totalFrames = 1;
            MountData.xOffset = 2;
            MountData.yOffset = 3; 
            MountData.playerYOffsets = Enumerable.Repeat(2, MountData.totalFrames).ToArray();
            MountData.bodyFrame = 6;

            MountData.acceleration = 0f;
            MountData.runSpeed = 0f;
            MountData.dashSpeed = 0f;
            MountData.jumpHeight = 0;
            MountData.jumpSpeed = 0;
            MountData.flightTimeMax = int.MaxValue - 1;
            MountData.fatigueMax = int.MaxValue - 1;
            MountData.usesHover = true;

            if (!Main.dedServ)
            {
                MountData.textureWidth = MountData.backTexture.Width();
                MountData.textureHeight = MountData.backTexture.Height();
            }
        }

        public struct JetwingData
        {
            public JetwingData()
            {
                wingRot = 0;
                boostTimeMax = 60;
                boostTime = 0;
                boostSpeed = 2.5f;
            }

            public int boostTimeMax;
            public int boostTime;
            public float boostSpeed;

            public float wingRot;
        }

        public override void SetMount(Player player, ref bool skipDust)
        {
            player.mount._mountSpecificData = new JetwingData();
            skipDust = true;
            if (!Main.dedServ)
            {
                for (int i = 0; i < 20; i++)
                {
                    Dust.NewDustPerfect(player.Center, MountData.spawnDust, Main.rand.NextVector2Circular(10, 10), 0, Color.White, 2).noGravity = true;
                    Dust.NewDustPerfect(player.Center, DustID.Smoke, Main.rand.NextVector2Circular(10, 10), 128, Color.Gray, 3).noGravity = true;
                }
            }
        }

        public override void Dismount(Player player, ref bool skipDust)
        {
            skipDust = true;
            if (!Main.dedServ)
            {
                for (int i = 0; i < 20; i++)
                {
                    Dust.NewDustPerfect(player.Center, MountData.spawnDust, Main.rand.NextVector2Circular(10, 10), 0, Color.White, 2).noGravity = true;
                    Dust.NewDustPerfect(player.Center, DustID.Smoke, Main.rand.NextVector2Circular(10, 10), 128, Color.Gray, 3).noGravity = true;
                }
            }
        }

        public override void JumpSpeed(Player mountedPlayer, ref float jumpSeed, float xVelocity)
        {
            jumpSeed = 0;
        }

        public override void JumpHeight(Player mountedPlayer, ref int jumpHeight, float xVelocity)
        {
            jumpHeight = 0;
        }

        public override void UpdateEffects(Player player)
        {
            var data = (JetwingData)player.mount._mountSpecificData;
            if (player.controlJump && player.releaseJump && data.boostTime <= 0)
                data.boostTime = data.boostTimeMax;

            if (data.boostTime > 0)
                data.boostTime--;

            bool boosting = data.boostTime > data.boostTimeMax * 0.33f; 
            float speedMult = 1f;
            if (boosting)
                speedMult = Utils.GetLerpValue(data.boostTimeMax, data.boostTimeMax * 0.5f, data.boostTime, true) * data.boostSpeed;

            Vector2 targetVelocity = Vector2.Zero;
            if (player.controlLeft)
                targetVelocity.X = -10 * speedMult;
            if (player.controlRight)
                targetVelocity.X = 10f * speedMult;
                        
            if (player.controlUp)
                targetVelocity.Y = -10f * speedMult;
            if (player.controlDown)
                targetVelocity.Y = 10f * speedMult;

            if (targetVelocity.Length() > 10 * speedMult)
                targetVelocity = Vector2.Lerp(targetVelocity, targetVelocity.SafeNormalize(Vector2.Zero) * 10 * speedMult, 0.1f);

            player.velocity = Vector2.Lerp(player.velocity, targetVelocity, 0.08f);
            player.maxFallSpeed = 100f;

            player.fullRotationOrigin = player.Size * 0.5f;

            float targetRot = 0;
            if (player.velocity.Length() > 2)
            {
                player.velocity.Y -= (float)Math.Sin(player.miscCounterNormalized * MathHelper.TwoPi * 3f) * 0.1f;
                if (player.velocity.X > 0)
                    player.ChangeDir(1);
                if (player.velocity.X < 0)
                    player.ChangeDir(-1);
                targetRot = player.velocity.ToRotation() + MathHelper.PiOver2;
            }
            else
                player.velocity += Main.rand.NextVector2Circular(0.2f, 0.1f);
            player.fullRotation = player.fullRotation.AngleLerp(targetRot, 0.15f);

            data.wingRot = MathHelper.Lerp(0, -player.velocity.ToRotation(), player.velocity.Length() * 0.2f);

            AllDust(player);
        }

        private void AllDust(Player player)
        {
            var data = (JetwingData)player.mount._mountSpecificData;

            Vector2 flameOffset = new Vector2(-17 * player.direction, 4).RotatedBy(player.fullRotation);
            Vector2 flameVel = new Vector2(0, Main.rand.NextFloat(1f)).RotatedBy(player.fullRotation).RotatedByRandom(0.3f);

            if (Main.rand.NextBool())
            {
                if (player.wet)
                {
                    Dust bubble = Dust.NewDustPerfect(player.Center + flameOffset, DustID.BubbleBurst_White, flameVel, 0, Color.White, Main.rand.NextFloat(2f));
                    bubble.noGravity = true;
                    bubble.fadeIn = 1.5f;
                }
                else
                {
                    Dust smoke = Dust.NewDustPerfect(player.Center + flameOffset, DustID.Smoke, flameVel * 2, 128, Color.Black, 1f + Main.rand.NextFloat(2f));
                    smoke.noGravity = true;
                    smoke.fadeIn = 2f;
                    smoke.shader = GameShaders.Armor.GetSecondaryShader(player.cMount, player);

                    Dust flame = Dust.NewDustPerfect(player.Center + flameOffset, MountData.spawnDust, flameVel, 0, Color.White, 1f + Main.rand.NextFloat(2f));
                    flame.noGravity = true;
                    flame.shader = GameShaders.Armor.GetSecondaryShader(player.cMount, player);
                }
            }

            if (data.boostTime > data.boostTimeMax * 0.16f && data.boostTime < data.boostTimeMax * 0.83f)
            {
                Dust extraSmoke = Dust.NewDustPerfect(player.Center + flameOffset, DustID.Smoke, Main.rand.NextVector2Circular(3, 3), 128, Color.Black, 2f + Main.rand.NextFloat(2f));
                extraSmoke.noGravity = true;
                extraSmoke.fadeIn = 2f;
                extraSmoke.shader = GameShaders.Armor.GetSecondaryShader(player.cMount, player);
            }
            if (data.boostTime > data.boostTimeMax * 0.83f && data.boostTime < data.boostTimeMax && data.boostTime % 3 == 0)
            {
                SoundEngine.PlaySound(SoundID.Item92, player.Center);
                for (int i = 0; i < 30; i++)
                {
                    Vector2 circle = Main.rand.NextVector2CircularEdge(5, 3).RotatedBy(player.fullRotation).RotatedByRandom(0.2f);
                    Dust boostFlame = Dust.NewDustPerfect(player.Center + flameOffset, MountData.spawnDust, circle, 0, Color.White, 2f);
                    boostFlame.noGravity = true;
                    boostFlame.shader = GameShaders.Armor.GetSecondaryShader(player.cMount, player);
                }
            }
        }

        public override bool Draw(List<DrawData> playerDrawData, int drawType, Player drawPlayer, ref Texture2D texture, ref Texture2D glowTexture, ref Vector2 drawPosition, ref Rectangle frame, ref Color drawColor, ref Color glowColor, ref float rotation, ref SpriteEffects spriteEffects, ref Vector2 drawOrigin, ref float drawScale, float shadow)
        {
            var data = (JetwingData)drawPlayer.mount._mountSpecificData;

            if (drawType == 0)
            {
                Asset<Texture2D> jetTexture = ModContent.Request<Texture2D>($"{nameof(BlockContent)}/Content/Mounts/SteampunkJetwing/SteampunkJetwingJet");
                Asset<Texture2D> wingTexture = ModContent.Request<Texture2D>($"{nameof(BlockContent)}/Content/Mounts/SteampunkJetwing/SteampunkJetwingWing");
                
                SpriteEffects dir = drawPlayer.direction < 0 ? SpriteEffects.FlipHorizontally : 0;
                SpriteEffects otherDir = drawPlayer.direction > 0 ? SpriteEffects.FlipHorizontally : 0;

                Rectangle frontFrame = wingTexture.Frame(1, 2, 0, 0);
                Rectangle backFrame = wingTexture.Frame(1, 2, 0, 1);
                Vector2 wingOrigin = frontFrame.Size() * new Vector2(0.5f + 0.4f * drawPlayer.direction, 0.5f);
               
                float frontWingTopRot = (data.wingRot + 0.1f) * drawPlayer.direction;
                float frontWingBotRot = (data.wingRot - 0.3f) * drawPlayer.direction;
                DrawData frontWingTop = new DrawData(wingTexture.Value, drawPosition - new Vector2(22 * drawPlayer.direction, 18).RotatedBy(rotation), frontFrame, drawColor, frontWingTopRot, wingOrigin, drawScale, dir, 0);
                DrawData frontWingBot = new DrawData(wingTexture.Value, drawPosition - new Vector2(22 * drawPlayer.direction, 8).RotatedBy(rotation), backFrame, drawColor, frontWingBotRot, wingOrigin, drawScale, dir, 0);
               
                float backWingTopRot = (data.wingRot + 0.2f) * drawPlayer.direction;
                float backWingBotRot = (data.wingRot - 0.1f) * drawPlayer.direction;
                DrawData backWingTop = new DrawData(wingTexture.Value, drawPosition - new Vector2(12 * drawPlayer.direction, 18).RotatedBy(rotation), frontFrame, drawColor * 0.6f, backWingTopRot, wingOrigin, drawScale, dir, 0);
                DrawData backWingBot = new DrawData(wingTexture.Value, drawPosition - new Vector2(12 * drawPlayer.direction, 10).RotatedBy(rotation), backFrame, drawColor * 0.6f, backWingBotRot, wingOrigin, drawScale, dir, 0);

                Vector2 jetOffset = new Vector2(-16 * drawPlayer.direction, -12);
                DrawData jet = new DrawData(jetTexture.Value, drawPosition + jetOffset, null, drawColor, rotation + (0.15f * drawPlayer.direction), jetTexture.Size() * 0.5f, drawScale, dir, 0);
               
                playerDrawData.Add(backWingTop);
                playerDrawData.Add(backWingBot);
                playerDrawData.Add(jet);
                playerDrawData.Add(frontWingTop);
                playerDrawData.Add(frontWingBot);
            }

            return false;
        }
    }
}
