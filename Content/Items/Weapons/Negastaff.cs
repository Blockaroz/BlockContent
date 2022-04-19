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
            Item.shoot = ModContent.ProjectileType<NegastaffMinionCounter>();
            Item.shootSpeed = 1f;
            Item.knockBack = 10;
            Item.noMelee = true;
            Item.value = Item.sellPrice(gold: 50);
            Item.buffType = ModContent.BuffType<NegastaffBuff>();
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;

        public override void UseAnimation(Player player)
        {

        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            Projectile minion = Projectile.NewProjectileDirect(source, player.Center, velocity, type, damage, knockback, player.whoAmI);
            minion.owner = player.whoAmI;
            minion.originalDamage = Item.damage;

            return false;
        }
    }
}
