using BlockContent.Content.Projectiles.NegastaffMinions;
using System;
using Terraria;
using Terraria.ModLoader;

namespace BlockContent.Content.Items.Weapons
{
	public class NegastaffBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Negapaint");
			Description.SetDefault("...");

			Main.buffNoSave[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			if (player.ownedProjectileCounts[ModContent.ProjectileType<NegastaffMinion>()] > 0)
				player.buffTime[buffIndex] = 18000;
			else
			{
				player.DelBuff(buffIndex);
				buffIndex--;
			}
		}
	}
}
