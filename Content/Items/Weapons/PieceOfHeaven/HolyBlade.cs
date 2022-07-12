using BlockContent.Content.Projectiles.Weapons.PieceOfHeaven;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContent.Content.Items.Weapons.PieceOfHeaven
{
    public class HolyBlade : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Blade");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 64;
            Item.height = 70;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 18;
            Item.useAnimation = 18;
            Item.reuseDelay = 10;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.channel = true;
            Item.shoot = ModContent.ProjectileType<HolyBladeHeld>();
            Item.shootSpeed = 7f;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.attackSpeedOnlyAffectsWeaponAnimation = false;

            Item.DamageType = DamageClass.Melee;
            Item.damage = 110;
            Item.knockBack = 0.5f;
            Item.rare = ModContent.RarityType<RoseRarity>();
            Item.value = Item.buyPrice(0, 50);
        }

        public override Color? GetAlpha(Color lightColor) => Color.Lerp(lightColor, Color.White, 0.8f);

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Asset<Texture2D> bloom = ModContent.Request<Texture2D>(Texture + "Bloom");

            Color bloomColor = Color.Lerp(Color.GhostWhite, Color.MediumAquamarine, 0.5f) * 0.5f;
            bloomColor.A = 0;
            spriteBatch.Draw(texture.Value, position, frame, drawColor, 0, origin, scale, 0, 0);
            spriteBatch.Draw(bloom.Value, position, null, bloomColor, 0, origin + new Vector2(14), scale, 0, 0);

            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Asset<Texture2D> bloom = ModContent.Request<Texture2D>(Texture + "Bloom");

            Color bloomColor = Color.Lerp(Color.GhostWhite, Color.MediumAquamarine, 0.5f) * 0.5f;
            bloomColor.A = 0;
            spriteBatch.Draw(texture.Value, Item.Center - Main.screenPosition, null, alphaColor, rotation, Item.Size * 0.5f, scale, 0, 0);
            spriteBatch.Draw(bloom.Value, Item.Center - Main.screenPosition, null, bloomColor, rotation, Item.Size * 0.5f + new Vector2(14), scale, 0, 0);

            return false;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI, 0, -4);
            proj.spriteDirection = player.direction;
            proj.direction = -1;

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Zenith)
                .Register();
        }
    }
}
