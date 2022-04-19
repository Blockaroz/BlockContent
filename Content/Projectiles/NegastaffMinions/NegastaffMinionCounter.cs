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
    public partial class NegastaffMinionCounter : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Negapaint Staffbrush");
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.minion = true;
            Projectile.minionSlots = 0.5f;
            Projectile.penetrate = -1;
            Projectile.hide = true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.Center = player.Center;

            if (player.dead || !player.active)
                player.ClearBuff(ModContent.BuffType<NegastaffBuff>());

            if (player.HasBuff(ModContent.BuffType<NegastaffBuff>()))
                Projectile.timeLeft = 2;

            
        }
        public override bool PreDraw(ref Color lightColor) => false;
    }
}
