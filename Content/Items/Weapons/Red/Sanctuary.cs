using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using BlockContent.Content.Projectiles.Weapons.Red;
using BlockContent.Common;

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
            Item.useAnimation = 10;
            Item.useTime = Item.useAnimation;
            Item.damage = 170;
            Item.DamageType = DamageClass.Ranged;
            Item.crit = 10;
            Item.knockBack = 2;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Red;
            Item.value = Item.sellPrice(gold: 20);
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.shootSpeed = 2f;
            Item.shoot = ModContent.ProjectileType<SanctuaryHeld>();
            Item.useAmmo = AmmoID.Bullet;
            Item.channel = true;
        }

        public override bool Shoot(Player player, ProjectileSource_Item_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 point = player.RotatedRelativePoint(player.MountedCenter, true);
            int projType = ModContent.ProjectileType<SanctuaryHeld>();
            Projectile p = Projectile.NewProjectileDirect(source, point, velocity, projType, damage, knockback, player.whoAmI);
            p.rotation = velocity.ToRotation();
            return false;
        }

        public override bool AltFunctionUse(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                if (player.GetModPlayer<SpecialWeaponPlayer>().sanctuaryMode == 1)
                    player.GetModPlayer<SpecialWeaponPlayer>().sanctuaryMode = 0;
                else
                    player.GetModPlayer<SpecialWeaponPlayer>().sanctuaryMode = 1;
            }
            return true;
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
