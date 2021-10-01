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

namespace BlockContent.Content.Items
{
    public class OddSword : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Odd Sword");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;    
        }

        public override void SetDefaults()
        {
            Item.width = 47;
            Item.height = 47;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.rare = ModContent.RarityType<OddRarity>();
            //Item.shoot = ModContent.ProjectileType<OddProj>();
        }

        public override void AddRecipes()
        {
            CreateRecipe().Register();
        }
    }
}
