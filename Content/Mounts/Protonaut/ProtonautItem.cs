using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContent.Content.Mounts.Protonaut
{
	public class ProtonautItem : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Protonaut summon");
			Tooltip.SetDefault("");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

        public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 30;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.value = Item.buyPrice(1, 50);
			Item.rare = ItemRarityID.Yellow;
			Item.UseSound = SoundID.Item79;
			Item.noMelee = true;
			Item.mountType = ModContent.MountType<ProtonautMount>();
		}
	}
}
