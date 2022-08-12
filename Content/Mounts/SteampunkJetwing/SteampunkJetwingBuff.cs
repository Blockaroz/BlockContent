using Terraria;
using Terraria.ModLoader;

namespace BlockContent.Content.Mounts.SteampunkJetwing
{
	public class SteampunkJetwingBuff : ModBuff
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Steampunk Jetwing");
			Description.SetDefault("Lickety-split");
			Main.buffNoTimeDisplay[Type] = true;
			Main.buffNoSave[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex) {
			player.mount.SetMount(ModContent.MountType<SteampunkJetwing>(), player);
			player.buffTime[buffIndex] = 10;
		}
	}
}
