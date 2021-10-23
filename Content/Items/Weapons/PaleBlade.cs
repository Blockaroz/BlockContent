﻿using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Terraria.ID;
using BlockContent.Content.Graphics;
using Terraria.GameContent.Drawing;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.Graphics.Renderers;
using BlockContent.Content.Projectiles;
using Terraria.DataStructures;

namespace BlockContent.Content.Items.Weapons
{
    public class PaleBlade : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Blade");
            Tooltip.SetDefault("'The pinnacle of control'");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 30;
            Item.useTime = Item.useAnimation / 3;
            Item.damage = 190;
            Item.DamageType = DamageClass.Melee;
            Item.crit = 10;
            Item.knockBack = 5f;
            Item.UseSound = Mod.GetLegacySoundSlot(SoundType.Item, "Assets/Sounds/Item/PaleSlash");
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Red;
            Item.value = Item.sellPrice(gold: 23);
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.shootSpeed = 16f;
            Item.shoot = ModContent.ProjectileType<PaleBladeProjectile>();
        }

        public override Color? GetAlpha(Color lightColor) => new(255, 255, 255, lightColor.A - Item.alpha);

        public override bool Shoot(Player player, ProjectileSource_Item_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int animation = (player.itemAnimationMax - player.itemAnimation) / player.itemTime;

            Vector2 velocityVector = Main.MouseWorld - Main.screenPosition - player.RotatedRelativePoint(player.MountedCenter, true);

            Vector2 reachPoint = Main.MouseWorld;
            player.LimitPointToPlayerReachableArea(ref reachPoint);
            Vector2 distance = reachPoint - player.MountedCenter;

            if (animation < 3)
            {
                int index;
                bool targetAcquired = MoreUtils.GetNPCTarget(player, reachPoint, 700, out index);

                if (targetAcquired)
                    distance = Main.npc[index].Center - player.MountedCenter;

                bool isTwo = (animation == 2);
                if (!isTwo && !targetAcquired)
                    isTwo = true;

                if (isTwo)
                    distance += Main.rand.NextVector2Circular(16, 16);
            }

            velocityVector = distance / 2f;
            float slashSize = Main.rand.Next(-50, 50);
            Projectile.NewProjectileDirect(source, position, velocityVector, type, damage, knockback, player.whoAmI, slashSize);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Zenith)
                .AddIngredient(ItemID.LunarBar, 16)
                .AddIngredient(ItemID.BeetleHusk, 90)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}