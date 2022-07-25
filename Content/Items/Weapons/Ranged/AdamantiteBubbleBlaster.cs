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
            Tooltip.SetDefault("33% chance to not consume ammo" +
                "\nThrows harmful bubbles");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 24;
            Item.useTime = 8;
            Item.useAnimation = 8;
            SoundStyle shootNoise = SoundID.Item87;
            shootNoise.MaxInstances = 0;
            shootNoise.Volume = 0.7f;
            shootNoise.PitchVariance = 0.4f;
            Item.UseSound = shootNoise;
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.DamageType = DamageClass.Ranged;
            Item.useAmmo = AmmoID.Gel;
            Item.damage = 35;
            Item.crit = 10;
            Item.rare = ItemRarityID.Red;
            Item.value = Item.buyPrice(0, 11);

            Item.shoot = ModContent.ProjectileType<Projectiles.Weapons.Ranged.HarmfulBubble>();
            Item.shootSpeed = 12f;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player) => !Main.rand.NextBool(3);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile bubble = Projectile.NewProjectileDirect(source, position - new Vector2(0, 10), velocity.RotatedByRandom(0.2f), type, damage, knockback, player.whoAmI);
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
