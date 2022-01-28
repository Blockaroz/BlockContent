using BlockContent.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;   

namespace BlockContent.Content.Mounts.Protonaut
{
    public class ProtonautMount : ModMount
    {
        public override string Texture => "Content/Mounts/Protonaut/ProtonautTorso";

        public override void SetStaticDefaults()
        {
            MountData.jumpHeight = 16;
            MountData.acceleration = 0.1f;
            MountData.jumpSpeed = 8f;
            MountData.blockExtraJumps = true;
            MountData.constantJump = false;
            MountData.heightBoost = 30;
            MountData.fallDamage = 0f;
            MountData.runSpeed = 4f;
            MountData.dashSpeed = 5f;
            MountData.flightTimeMax = 0;
            MountData.fatigueMax = 0;
            MountData.buff = ModContent.BuffType<ProtonautBuff>();
            MountData.spawnDust = 0;

            MountData.totalFrames = 1;
            MountData.xOffset = 0;
            MountData.yOffset = 0;
            MountData.bodyFrame = 0;
            MountData.playerYOffsets = Enumerable.Range(16, 1).ToArray();
            MountData.playerHeadOffset = 24;

            if (!Main.dedServ)
            {
                MountData.textureWidth = 20;
                MountData.textureHeight = 28;
            }
        }

        public override void SetMount(Player player, ref bool skipDust)
        {
            for (int i = 0; i < Main.rand.Next(20, 30); i++)
                Particle.NewParticle(Particle.ParticleType<Particles.Ember>(), player.Center + Main.rand.NextVector2Circular(20, 24), -Vector2.UnitY, Color.Cyan, Main.rand.NextFloat());

            skipDust = true;
        }

        public override void Dismount(Player player, ref bool skipDust)
        {
            for (int i = 0; i < Main.rand.Next(20, 30); i++)
                Particle.NewParticle(Particle.ParticleType<Particles.Ember>(), player.Center + Main.rand.NextVector2Circular(20, 24), -Vector2.UnitY, Color.Cyan, Main.rand.NextFloat());

            skipDust = true;
        }

        public override void UpdateEffects(Player player)
        {
            player.gravity *= 0.8f;
        }

        public override bool Draw(List<DrawData> playerDrawData, int drawType, Player drawPlayer, ref Texture2D texture, ref Texture2D glowTexture, ref Vector2 drawPosition, ref Rectangle frame, ref Color drawColor, ref Color glowColor, ref float rotation, ref SpriteEffects spriteEffects, ref Vector2 drawOrigin, ref float drawScale, float shadow)
        {
            Asset<Texture2D> bodyTexture = Mod.Assets.Request<Texture2D>("Content/Mounts/Protonaut/Protonaut_Body");
            Asset<Texture2D> legTexture = Mod.Assets.Request<Texture2D>("Content/Mounts/Protonaut/Protonaut_Leg");
            Asset<Texture2D> armTexture = Mod.Assets.Request<Texture2D>("Content/Mounts/Protonaut/Protonaut_Arm");

            Rectangle frameBody = new Rectangle(0, 18, bodyTexture.Width(), 36);
            Rectangle frameHead = new Rectangle(0, 0, bodyTexture.Width(), 18);


            //upper
            //lower
            Rectangle frameFoot = new Rectangle(0, 44, legTexture.Width(), 10);

            //back arm


            //back leg

            //body

            Vector2 bodyPosition = drawPosition + new Vector2(0, 22).RotatedBy(rotation);
            Vector2 headPosition = bodyPosition + new Vector2(0, -28).RotatedBy(rotation);

            DrawData body = new DrawData(bodyTexture.Value, bodyPosition, frameBody, drawColor, rotation, frameBody.Size() * new Vector2(0.5f, 1f), drawScale, spriteEffects, 0);
            DrawData head = new DrawData(bodyTexture.Value, headPosition, frameHead, drawColor, rotation, frameHead.Size() * new Vector2(0.5f, 1f), drawScale, spriteEffects, 0);
            
            //front leg
            

            //front arm

            if (drawType == 2)
            {


                playerDrawData.Add(body);
                playerDrawData.Add(head);
            }


            return false;
        }
    }
}
