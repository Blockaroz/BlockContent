using Terraria;
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

namespace BlockContent.Content.Items
{
    public class HeatBlade : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Heat Blade");
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
            Item.UseSound = SoundID.Item15;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Red;
            Item.value = Item.sellPrice(gold: 20);
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.shootSpeed = 16f;
            Item.shoot = ModContent.ProjectileType<HeatBladeProjectile>();
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
                bool targetAcquired = BlockUtils.GetNPCTarget(player, reachPoint, 700, out index);

                if (targetAcquired)
                    distance = Main.npc[index].Center - player.MountedCenter;

                bool isTwo = (animation == 2);
                if (!isTwo && !targetAcquired)
                    isTwo = true;

                if (isTwo)
                    distance += Main.rand.NextVector2Circular(150f, 150f);
            }

            velocityVector = distance / 2f;
            float ai = Main.rand.Next(-100, 101);
            Projectile.NewProjectileDirect(source, position, velocityVector, type, damage, knockback, player.whoAmI, ai);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Zenith)
                .AddIngredient(ItemID.MeteoriteBar, 12)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
