using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;

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
            Item.useAnimation = 7;
            Item.useTime = 7;
            Item.damage = 100;
            Item.DamageType = DamageClass.Ranged;
            Item.crit = 10;
            Item.knockBack = 2;
            Item.UseSound = SoundID.Item125.WithPitchVariance(0.1f);//SoundLoader.GetLegacySoundSlot(Mod, "Assets/Sounds/Items/PaleSlash").WithPitchVariance(0.33f);
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Red;
            Item.value = Item.sellPrice(gold: 20);
            Item.noMelee = true;
            Item.shootSpeed = 24f;
            Item.useAmmo = AmmoID.Bullet;
            Item.shoot = ProjectileID.Bullet;
            Item.noUseGraphic = true;
            Item.channel = true;
        }

        public override Color? GetAlpha(Color lightColor) => new(255, 255, 255, lightColor.A - Item.alpha);

        public override Vector2? HoldoutOffset() => new Vector2(-20, 0);

        public override bool Shoot(Player player, ProjectileSource_Item_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position - new Vector2(0, 5), velocity, type, damage, knockback, player.whoAmI);
            return false;
        }

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
