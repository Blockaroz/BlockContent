using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContent.Content.Items.Weapons.Ranged
{
    public class AdamantiteBubbleBlaster : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Adamantite Bubble Blaster");
            Tooltip.SetDefault("Throws harmful bubbles");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 24;
            Item.useTime = 8;
            Item.useAnimation = 16;
            SoundStyle bubbleNoise = SoundID.Item111;
            bubbleNoise.MaxInstances = 0;
            bubbleNoise.Volume = 0.7f;
            bubbleNoise.PitchVariance = 0.4f;
            Item.UseSound = bubbleNoise;
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 30;
            Item.crit = 10;

            Item.shoot = ModContent.ProjectileType<Projectiles.Weapons.Ranged.HarmfulBubble>();
            Item.shootSpeed = 13f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile bubble = Projectile.NewProjectileDirect(source, position, velocity.RotatedByRandom(0.4f) * Main.rand.NextFloat(0.7f, 1.2f), type, damage, knockback, player.whoAmI);
            bubble.scale = Main.rand.NextFloat(0.7f, 1.2f);
            return false;
        }

        public override Vector2? HoldoutOffset() => new Vector2(3, 0);

        public override Vector2? HoldoutOrigin() => new Vector2(9);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.AdamantiteBar, 10)
                .AddIngredient(ItemID.BubbleWand)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
