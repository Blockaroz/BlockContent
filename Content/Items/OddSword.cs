using BlockContent.Content.NPCs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

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
            //Item.shoot = ModContent.ProjectileType<Projectiles.NPCProjectiles.NightEmpressProjectiles.CurseSkull>();
            Item.shootSpeed = 1f;
        }

        public override bool? UseItem(Player player)
        {
            NPC.NewNPC((int)player.MountedCenter.X, (int)player.MountedCenter.Y - 300, ModContent.NPCType<ShaderTest>());
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().Register();
        }
    }
}
