using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using BlockContent.Content.Projectiles.Weapons.Holy;

namespace BlockContent.Content.Items.Weapons.Holy
{
    public class HolyBlade : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Blade");
            Tooltip.SetDefault("'An antique, well-kept blade." +
                "\nOne may slash that which they cannot reach.'");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 32;
            Item.useTime = Item.useAnimation;
            Item.damage = 240;
            Item.DamageType = DamageClass.Melee;
            Item.crit = 12;
            Item.knockBack = 5;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Purple;
            Item.value = Item.sellPrice(gold: 23);
            Item.noUseGraphic = true;
            Item.useTurn = false;
            Item.noMelee = true;
            Item.shootSpeed = 2f;
            Item.shoot = ModContent.ProjectileType<HolyBladeProj>();
            Item.channel = true;
        }

        public override Color? GetAlpha(Color lightColor) => new(255, 255, 255, lightColor.A - Item.alpha);

        public override bool Shoot(Player player, ProjectileSource_Item_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float angle = MathHelper.ToRadians(120);
            Projectile proj = Projectile.NewProjectileDirect(source, position, velocity.SafeNormalize(Vector2.Zero), ModContent.ProjectileType<HolyBladeProj>(), damage, knockback, player.whoAmI, angle, angle);
            proj.direction = Main.rand.NextBool().ToDirectionInt();
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Zenith)
                .AddIngredient(ItemID.BeetleHusk, 90)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
