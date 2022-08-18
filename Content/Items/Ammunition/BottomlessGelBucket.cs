using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Terraria.ID;

namespace BlockContent.Content.Items.Ammunition
{
    public class BottomlessGelBucket : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bottomless Gel Bucket");
            //Tooltip.SetDefault("");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 26;
            Item.ammo = AmmoID.Gel;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(0, 1, 50);
            Item.damage = 5;
            Item.crit = 4;
            Item.knockBack = 0.5f;
            //Item.consumable = false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Gel, 3996)
                .AddIngredient(ItemID.EmptyBucket)
                .AddTile(TileID.CrystalBall)
                .Register();
        }
    }
}
