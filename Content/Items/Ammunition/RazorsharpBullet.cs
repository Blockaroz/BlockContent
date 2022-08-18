using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Terraria.ID;
using BlockContent.Content.Projectiles.Ammunition;

namespace BlockContent.Content.Items.Ammunition
{
    public class RazorsharpBullet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Razorsharp Bullet");
            Tooltip.SetDefault("Draws blood from its victims");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 99;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.SilverBullet);
            Item.ammo = AmmoID.Bullet;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(0, 0, 1, 0);
            Item.damage = 16;
            Item.crit = 10;
            Item.knockBack = 0f;
            Item.consumable = true;
            Item.shoot = ModContent.ProjectileType<RazorsharpBulletProj>();
            Item.shootSpeed = 25f;
        }

        public override void AddRecipes()
        {
            CreateRecipe(60)
                .AddIngredient(ItemID.EmptyBullet, 50)
                .AddIngredient(ItemID.Chain)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
