using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using BlockContent.Content.Items.Weapons;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.IO;

namespace BlockContent.Content.Projectiles.NegastaffMinions
{
    public partial class NegastaffMinion : ModProjectile
    {
        public override string Texture => "BlockContent/Content/Projectiles/NegastaffMinions/NegastaffMinionCounter";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Negapaint Goon");
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
            ProjectileID.Sets.TrailingMode[Type] = 1;
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
        }

        public override void SetDefaults()
        {
            minionType = (int)MinionType.Nobody;
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.minion = true;
            Projectile.minionSlots = 0f;
            Projectile.penetrate = -1;
        }

        public enum MinionType
        {
            Nobody,
            Nari,
            Caligulas,
            ee,
            Blockaroz,
            Seeksery,
            Moonburn,
            Carleah,
            Tigershark,
            Potato
        }

        public int minionType;
        public Vector2 idlePos;
        public Vector2 targetPos;

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(idlePos);
            writer.WriteVector2(targetPos);
            writer.Write(minionType);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            idlePos = reader.ReadVector2();
            targetPos = reader.ReadVector2();
            minionType = reader.Read();
        }

        public override bool MinionContactDamage() => false;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            switch (minionType)
            {
                default:
                    return base.Colliding(projHitbox, targetHitbox);
                case (int)MinionType.Nobody:
                    return false;
                case (int)MinionType.Seeksery:
                    return false;

            }
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (Projectile.Distance(targetPos) < 0.7f)
                Projectile.position = targetPos;

            //Projectile.originalDamage = 15;
            Projectile.originalDamage = (int)MathHelper.Lerp(15, 50, player.ownedProjectileCounts[Type] / 9f);

            if (player.dead || !player.active)
                player.ClearBuff(ModContent.BuffType<NegastaffBuff>());

            if (player.HasBuff(ModContent.BuffType<NegastaffBuff>()))
                Projectile.timeLeft = 2;

            if (minionType == (int)MinionType.Nobody || player.dead || !player.active)
                Projectile.Kill();

            idlePos = player.Center + new Vector2((-66 - (Projectile.ai[0] * Projectile.ai[0] * 0.45f)) * player.direction, -20 - (Projectile.ai[0] * 3)).RotatedBy(MathHelper.TwoPi / 9f * Projectile.ai[0] * player.direction);
            //idlePos = player.Center + new Vector2((66 + (Projectile.ai[0] * 20)) * -player.direction, -20 * player.gravDir);

            switch (minionType)
            {
                case (int)MinionType.Nari:
                    AI_Nari();
                    break;
                case (int)MinionType.Caligulas:
                    break;
                case (int)MinionType.Seeksery:
                    AI_Seeksery();
                    break;
                case (int)MinionType.Moonburn:
                    break;
            }
        }

        public bool FindTarget_NPC(Player owner, out int index, float maxDistance = 700f)
        {
            index = -1;
            float distance = maxDistance;
            bool hasTarget = false;
            if (owner.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[owner.MinionAttackTargetNPC];
                if (npc.active && Projectile.Distance(npc.Center) < 2000f)
                {
                    targetPos = npc.Center;
                    distance = Projectile.Distance(npc.Center);
                    index = npc.whoAmI;
                    hasTarget = true;
                }
            }
            else
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.active && npc.CanBeChasedBy(this))
                    {
                        bool inRange = Projectile.Distance(npc.Center) < distance;
                        bool lineOfSight = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height);
                        bool closeThroughWall = Projectile.Distance(npc.Center) < 100f;
                        if ((inRange || !hasTarget) && (lineOfSight || closeThroughWall))
                        {
                            distance = Projectile.Distance(npc.Center);
                            index = npc.whoAmI;
                            hasTarget = true;
                        }
                    }
                }
            }
            return hasTarget;
        }

        public bool FindTarget_ProjReflectable(Player owner, out int index, float maxDistance = 500f)
        {
            index = -1;
            float distance = maxDistance;
            bool hasTarget = false;
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.active && proj.CanBeReflected() && proj.hostile && proj.type != Type && proj.owner != owner.whoAmI)
                {
                    bool inRange = Projectile.Distance(proj.Center) < distance;
                    bool lineOfSight = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, proj.position, proj.width, proj.height);
                    bool closeThroughWall = Projectile.Distance(proj.Center) < 100f;
                    if ((inRange || !hasTarget) && (lineOfSight || closeThroughWall))
                    {
                        distance = Projectile.Distance(proj.Center);
                        index = proj.whoAmI;
                        hasTarget = true;
                    }
                }
            }
            return hasTarget;
        }

        public SpriteEffects GetSpriteEffects() => Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

        public override bool PreDraw(ref Color lightColor) => false;

        public override void PostDraw(Color lightColor)
        {
            switch (minionType)
            {
                case (int)MinionType.Nari:
                    Draw_Nari();
                    break;
                case (int)MinionType.Seeksery:
                    Draw_Seeksery();
                    break;
            }
        }
    }
}
