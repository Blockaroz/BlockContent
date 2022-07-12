using BlockContent.Content.Projectiles.Weapons.PieceOfHeaven;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
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
            Tooltip.SetDefault("something about it being shiny");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ItemID.Sets.ItemNoGravity[Type] = true;
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
            Item.damage = 200;
            Item.knockBack = 0.5f;
            Item.rare = ModContent.RarityType<RoseRarity>();
            Item.value = Item.buyPrice(0, 50);
        }

        public override void PostUpdate()
        {
            Item.position.Y -= (float)Math.Sin(Item.timeSinceItemSpawned / 60f) * 0.12f;
        }

        public override void HoldItem(Player player) => Lighting.AddLight(Item.Center, Color.SlateGray.ToVector3() * 0.4f);

        public override Color? GetAlpha(Color lightColor) => Color.Lerp(lightColor, Color.White, 0.7f);

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Asset<Texture2D> shadow = ModContent.Request<Texture2D>(Texture + "Shadow");
            Asset<Texture2D> bloom = ModContent.Request<Texture2D>(Texture + "Bloom");

            Color bloomColor = Color.Lerp(Color.GhostWhite, HeavenColors.Melee, 0.3f) * 0.33f;
            bloomColor.A = 0;
            spriteBatch.Draw(shadow.Value, position, null, Color.Black * 0.2f, 0, origin + new Vector2(14), scale, 0, 0);
            spriteBatch.Draw(texture.Value, position, null, drawColor, 0, origin, scale, 0, 0);
            spriteBatch.Draw(bloom.Value, position, null, bloomColor, 0, origin + new Vector2(14), scale, 0, 0);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Asset<Texture2D> shadow = ModContent.Request<Texture2D>(Texture + "Shadow");
            Asset<Texture2D> bloom = ModContent.Request<Texture2D>(Texture + "Bloom");

            Color bloomColor = Color.Lerp(Color.GhostWhite, HeavenColors.Melee, 0.3f) * 0.33f;
            bloomColor.A = 0;
            spriteBatch.Draw(shadow.Value, Item.Center - Main.screenPosition, null, Color.Black * 0.2f, rotation - MathHelper.PiOver4, Item.Size * 0.5f + new Vector2(14), scale, 0, 0);
            spriteBatch.Draw(texture.Value, Item.Center - Main.screenPosition, null, alphaColor, rotation - MathHelper.PiOver4, Item.Size * 0.5f, scale, 0, 0);
            spriteBatch.Draw(bloom.Value, Item.Center - Main.screenPosition, null, bloomColor, rotation - MathHelper.PiOver4, Item.Size * 0.5f + new Vector2(14), scale, 0, 0);

            DrawStar(spriteBatch, Item.Center - new Vector2(-2, -19).RotatedBy(rotation));

            return false;
        }

        private void DrawStar(SpriteBatch spriteBatch, Vector2 position)
        {
            Asset<Texture2D> star = ModContent.Request<Texture2D>($"{nameof(BlockContent)}/Assets/Textures/GlowStar");
            Asset<Texture2D> bloom = ModContent.Request<Texture2D>($"{nameof(BlockContent)}/Assets/Textures/GlowSoft");

            float shineStrength = 0.3f + (float)Math.Sin(Item.timeSinceItemSpawned / 40f) * 0.2f;

            Color shineColor = Color.White * 0.5f;
            shineColor.A = 0;            
            Color bloomColor = HeavenColors.Melee * (0.2f + shineStrength * 0.5f);
            bloomColor.A = 0;

            spriteBatch.Draw(star.Value, position - Main.screenPosition, null, bloomColor, 0, star.Size() * 0.5f, 1f, 0, 0);
            spriteBatch.Draw(star.Value, position - Main.screenPosition, null, shineColor, 0, star.Size() * 0.5f, 0.5f, 0, 0);
            spriteBatch.Draw(bloom.Value, position - Main.screenPosition, null, bloomColor * 0.3f * shineStrength, 0, bloom.Size() * 0.5f, 1.5f, 0, 0);
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
