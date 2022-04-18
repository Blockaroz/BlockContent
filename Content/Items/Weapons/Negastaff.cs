using System;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using BlockContent.Content.Projectiles.NegastaffMinions;

namespace BlockContent.Content.Items.Weapons
{
    public class Negastaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
            ItemID.Sets.StaffMinionSlotsRequired[Item.type] = 1;
            DisplayName.SetDefault("Negapaint Staffbrush");
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.damage = 6;
            Item.mana = 10;
            Item.DamageType = DamageClass.Summon;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ItemRarityID.Green;
            Item.shoot = ModContent.ProjectileType<NegastaffMinion>();
            Item.shootSpeed = 1f;
            Item.knockBack = 10;
            Item.noMelee = true;
            Item.value = Item.sellPrice(gold: 50);
            Item.buffType = NegastaffMinion.BuffType;
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;

        public override void UseAnimation(Player player)
        {

        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            Projectile minion = Projectile.NewProjectileDirect(source, player.Center, velocity, type, damage, knockback, player.whoAmI);
            minion.ai[0] = -1;
            minion.owner = player.whoAmI;
            if (player.ownedProjectileCounts[type] > 0 && player.ownedProjectileCounts[type] * 0.5f < player.maxMinions)
            {
                minion.ai[0] = player.ownedProjectileCounts[type];
                if (minion.ModProjectile is NegastaffMinion nsminion)
                    nsminion.minionType = (int)NegastaffMinion.MinionType.Seeksery;
            }
            else
            {
                minion.ai[0] = 0;
                if (minion.ModProjectile is NegastaffMinion nsminion)
                    nsminion.minionType = (int)NegastaffMinion.MinionType.Seeksery;
            }
            minion.originalDamage = Item.damage;

            return false;
        }
    }
}
