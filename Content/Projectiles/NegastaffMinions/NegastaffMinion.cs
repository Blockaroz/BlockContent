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

namespace BlockContent.Content.Projectiles.NegastaffMinions
{
    public partial class NegastaffMinion : ModProjectile
    {
        public override string Texture => "BlockContent/Assets/Textures/Empty";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Negastaff Goon");
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[Type] = true;
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
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon; 
            Projectile.minionSlots = 1f;
            Projectile.penetrate = -1;
        }

        public enum MinionType
        {
            Nobody,
            Nari,
            Caligulas,
            Seeksery,
            Moonburn,
        }

        public static int BuffType = ModContent.BuffType<NegastaffBuff>();
        public int minionType;
        public Vector2 idlePos;
        public Vector2 targetPos;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (Projectile.Distance(targetPos) < 0.7f)
                Projectile.position = targetPos;

            Projectile.localAI[0]++;

            if (player.dead || !player.active)
                player.ClearBuff(BuffType);

            if (player.HasBuff(BuffType))
                Projectile.timeLeft = 2;

            if (minionType == (int)MinionType.Nobody || player.dead || !player.active)
                Projectile.Kill();

            idlePos = player.Center + new Vector2((60 + (Projectile.localAI[1] * 5)) * -player.direction, -20 - (Projectile.localAI[1] * 3)).RotatedBy(MathHelper.TwoPi / 9f * Projectile.localAI[1] * player.direction);

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
                if (Projectile.Distance(npc.Center) < 2000f)
                {
                    distance = Projectile.Distance(npc.Center);
                    targetPos = npc.Center;
                    index = npc.whoAmI;
                    hasTarget = true;
                }
            }
            else
            {
                for (int i = 0; i < Main.npc.Length; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.CanBeChasedBy(this))
                    {
                        bool inRange = Projectile.Distance(npc.Center) < distance;
                        bool lineOfSight = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height);
                        bool closeThroughWall = Projectile.Distance(npc.Center) < 100f;
                        if ((inRange || !hasTarget) && (lineOfSight || closeThroughWall))
                        {
                            distance = Projectile.Distance(npc.Center);
                            hasTarget = true;
                        }
                    }
                }
            }
            return hasTarget;
        }

        public bool FindTarget_Proj(Player owner, out int index, float maxDistance = 500f)
        {
            index = -1;
            float distance = maxDistance;
            bool hasTarget = false;
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.CanBeReflected() && proj.hostile && proj.type != Type)
                {
                    bool inRange = Projectile.Distance(proj.Center) < distance;
                    bool lineOfSight = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, proj.position, proj.width, proj.height);
                    bool closeThroughWall = Projectile.Distance(proj.Center) < 100f;
                    if ((inRange || !hasTarget) && (lineOfSight || closeThroughWall))
                    {
                        distance = Projectile.Distance(proj.Center);
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
