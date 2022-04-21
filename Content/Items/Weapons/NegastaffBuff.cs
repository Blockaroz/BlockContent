using BlockContent.Content.Projectiles.NegastaffMinions;
using Microsoft.Xna.Framework;
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
			if (player.ownedProjectileCounts[ModContent.ProjectileType<NegastaffMinionCounter>()] > 0)
				player.buffTime[buffIndex] = 18000;
			else
			{
				player.DelBuff(buffIndex);
				buffIndex--;
			}
			if (player.whoAmI == Main.myPlayer)
				UpdateActiveMinions(player);
		}

		private void UpdateActiveMinions(Player player)
		{
			int counterType = ModContent.ProjectileType<NegastaffMinionCounter>();
			int minionType = ModContent.ProjectileType<NegastaffMinion>();
			if (player.ownedProjectileCounts[counterType] < player.ownedProjectileCounts[minionType])
            {
				int lastIndex = -1;
				int lastMinion = -1;
				for (int i = 0; i < Main.maxProjectiles; i++)
				{
					Projectile proj = Main.projectile[i];
					if (proj.active && proj.owner == player.whoAmI && proj.type != minionType && proj.ai[0] > lastMinion)
					{
						lastIndex = proj.whoAmI;
						lastMinion = (int)proj.ai[0];
					}
				}
				Projectile toKill = Main.projectile[lastIndex];
				if (toKill.active && toKill.owner == player.whoAmI && toKill.type != minionType && toKill.ai[0] == lastMinion)
					toKill.Kill();
			}

			else if (player.ownedProjectileCounts[minionType] < player.ownedProjectileCounts[counterType])
			{
				Projectile minion = Projectile.NewProjectileDirect(player.GetProjectileSource_Misc(0), player.Center, Vector2.Zero, minionType, 0, 0f, player.whoAmI);
				if (minion.ModProjectile is NegastaffMinion nsminion)
					nsminion.minionType = (int)NegastaffMinion.MinionType.Seeksery;//player.ownedProjectileCounts[counterType];
				minion.ai[0] = player.ownedProjectileCounts[minionType];
			}
		}
	}
}
