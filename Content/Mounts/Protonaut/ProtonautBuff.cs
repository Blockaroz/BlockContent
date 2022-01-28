using System;
using Terraria;
using Terraria.ModLoader;

namespace BlockContent.Content.Mounts.Protonaut
{
	public class ProtonautBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Protonaut");
			Description.SetDefault("");
			Main.buffNoTimeDisplay[Type] = true;
			Main.buffNoSave[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.mount.SetMount(ModContent.MountType<ProtonautMount>(), player);
			player.buffTime[buffIndex] = 10;
			player.noFallDmg = true;
			player.GetModPlayer<MountPlayer>().protonaut = true;
			player.GetModPlayer<MountPlayer>().exosuit = true;
		}
	}
}
