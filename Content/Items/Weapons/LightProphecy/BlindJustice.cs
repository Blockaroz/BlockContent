using BlockContent.Content.Projectiles.Weapons.LightProphecy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContent.Content.Items.Weapons.LightProphecy
{
    public class BlindJustice : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blind Justice");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 72;
            Item.height = 72;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.channel = true;
            Item.shoot = ModContent.ProjectileType<BlindJusticeHeld>();
            Item.shootSpeed = 5f;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.attackSpeedOnlyAffectsWeaponAnimation = false;

            Item.DamageType = DamageClass.Melee;
            Item.damage = 95;
            Item.knockBack = 0.5f;
            Item.rare = ItemRarityID.Red;
            Item.value = Item.buyPrice(0, 10, 50);
        }

        public static Color PulseColor(float speed = 3f, float offset = 0f) => Color.Lerp(Color.White, Color.Green, (float)Math.Pow(Math.Sin((Main.GlobalTimeWrappedHourly + offset * 0.1f) * speed % MathHelper.TwoPi), 2f) * 0.5f);

        public static Color QuasarColor(float speed = 3f, float offset = 0f) => Color.Lerp(new Color(113, 168, 77), new Color(64, 173, 98), (float)Math.Pow(Math.Sin((Main.GlobalTimeWrappedHourly + offset * 0.1f) * speed % MathHelper.TwoPi), 2f));

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture + "Held");
            Asset<Texture2D> glow = ModContent.Request<Texture2D>(Texture + "Glow");
            Asset<Texture2D> bloom = ModContent.Request<Texture2D>(Texture + "Bloom");

            spriteBatch.Draw(texture.Value, Item.Center - Main.screenPosition, null, lightColor, rotation, texture.Size() * 0.5f, scale, 0, 0);
            Color glowColor = PulseColor(2f);
            glowColor.A /= 2;
            Color bloomColor = QuasarColor(2f);
            bloomColor.A = 0;
            spriteBatch.Draw(glow.Value, Item.Center - Main.screenPosition, null, glowColor, rotation, texture.Size() * 0.5f, scale, 0, 0);
            spriteBatch.Draw(bloom.Value, Item.Center - Main.screenPosition, null, bloomColor, rotation, texture.Size() * 0.5f + new Vector2(8), scale, 0, 0);

            return false;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture + "Held");
            Asset<Texture2D> glow = ModContent.Request<Texture2D>(Texture + "Glow");
            Asset<Texture2D> bloom = ModContent.Request<Texture2D>(Texture + "Bloom");

            spriteBatch.Draw(texture.Value, position, frame, drawColor, 0, origin, scale, 0, 0);

            Color glowColor = PulseColor(2f);
            glowColor.A /= 2;
            Color bloomColor = QuasarColor(2f);
            bloomColor.A = 0;
            spriteBatch.Draw(glow.Value, position, frame, glowColor, 0, origin, scale, 0, 0);
            spriteBatch.Draw(bloom.Value, position, null, bloomColor, 0, origin + new Vector2(8), scale, 0, 0);

            return false;
        }

        public override void PostUpdate() => Lighting.AddLight(Item.Center, Color.SeaGreen.ToVector3() * 0.33f);

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int ai0 = 0;
            if (player.altFunctionUse == 2)
                ai0 = 1;
            Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI, ai0, -5);
            proj.spriteDirection = player.direction;
            proj.direction = 1;

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .Register();
        }
    }
}
