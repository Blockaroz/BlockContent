using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContent.Content.Items.Weapons.Miracle
{
    public class ExoBow : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Godly Wind");
        }

        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 48;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 40;
            Item.useTime = 40;

            Item.damage = 200;
            Item.crit = 10;
            Item.DamageType = DamageClass.Ranged;
        }
    }
}
