using BlockContent.Content.Projectiles.Weapons.Red;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleEngine;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContent.Content.Items.Weapons.Red
{
    public class Saboteur : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Saboteur");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ItemID.Sets.gunProj[Type] = true; 
        }

        public override void SetDefaults()
        {
            Item.width = 64;
            Item.height = 70;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 5;
            Item.useAnimation = 5;
            Item.reuseDelay = 8;
            Item.autoReuse = true;
            Item.channel = true;
            Item.shoot = ModContent.ProjectileType<SaboteurHeld>();
            Item.shootSpeed = 5f;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.attackSpeedOnlyAffectsWeaponAnimation = false;

            Item.DamageType = DamageClass.Ranged;
            Item.damage = 150;
            Item.useAmmo = AmmoID.Bullet;
            Item.crit = 66;
            Item.knockBack = 0.5f;
            Item.rare = ItemRarityID.Red;
            Item.value = Item.buyPrice(0, 20, 50);
        }

        public override bool CanConsumeAmmo(Item ammo, Player player) => Main.rand.NextBool();

        public override Color? GetAlpha(Color lightColor) => Color.White;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile gun = Projectile.NewProjectileDirect(source, position, velocity, ModContent.ProjectileType<SaboteurHeld>(), damage, knockback, player.whoAmI, 3);
            gun.direction = player.direction;
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FlintlockPistol)
                .AddIngredient(ItemID.QuadBarrelShotgun)
                .AddIngredient(ItemID.PhoenixBlaster)
                .AddIngredient(ItemID.OnyxBlaster)
                .AddIngredient(ItemID.Megashark)
                .AddIngredient(ItemID.SuperStarCannon)
                .AddIngredient(ItemID.SniperRifle)
                .AddIngredient(ItemID.VenusMagnum)
                .AddIngredient(ItemID.ChainGun)
                .AddIngredient(ItemID.Xenopopper)
                .AddIngredient(ItemID.SDMG)
                .AddIngredient(ItemID.IllegalGunParts)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
