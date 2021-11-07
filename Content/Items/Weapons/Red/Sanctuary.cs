using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using BlockContent.Content.Projectiles.Red;

namespace BlockContent.Content.Items.Weapons.Red
{
    public class Sanctuary : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sanctuary");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 5;
            Item.useTime = 5;
            Item.damage = 170;
            Item.DamageType = DamageClass.Ranged;
            Item.crit = 10;
            Item.knockBack = 2;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Red;
            Item.value = Item.sellPrice(gold: 20);
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.useAmmo = AmmoID.Bullet;
            Item.shootSpeed = 20f;
            Item.shoot = ModContent.ProjectileType<SanctuaryProjectile>();
        }

        public override bool Shoot(Player player, ProjectileSource_Item_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int projType = ModContent.ProjectileType<SanctuaryProjectile>();
            Vector2 point = player.RotatedRelativePoint(player.MountedCenter, true);
            Projectile.NewProjectile(source, point, velocity, projType, damage, knockback, player.whoAmI, 5 * Main.rand.Next(0, 2), 0);
            return false;
        }

        public override Color? GetAlpha(Color lightColor) => new(255, 255, 255, lightColor.A - Item.alpha);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.QuadBarrelShotgun)
                .AddIngredient(ItemID.PhoenixBlaster)
                .AddIngredient(ItemID.OnyxBlaster)
                .AddIngredient(ItemID.Flamethrower)
                .AddIngredient(ItemID.VenusMagnum)
                .AddIngredient(ItemID.SniperRifle)
                .AddIngredient(ItemID.ChainGun)
                .AddIngredient(ItemID.Xenopopper)
                .AddIngredient(ItemID.SDMG)
                .AddIngredient(ItemID.Celeb2)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
