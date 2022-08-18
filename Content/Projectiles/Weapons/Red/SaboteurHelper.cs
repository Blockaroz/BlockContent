using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace BlockContent.Content.Projectiles.Weapons.Red
{
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct SaboteurHelper
    {
        public struct SaboteurProfile
        {
            public SaboteurProfile(Color color)
            {
                this.color = color;
            }

            private Color color;
        }

        private static Dictionary<int, SaboteurProfile> profiles = new Dictionary<int, SaboteurProfile>
        {
            {ItemID.FlintlockPistol, new SaboteurProfile(Color.Beige) },
            {ItemID.QuadBarrelShotgun, new SaboteurProfile(Color.Beige) },
            {ItemID.Handgun, new SaboteurProfile(Color.Beige) },
            {ItemID.PhoenixBlaster, new SaboteurProfile(Color.Beige) },
            {ItemID.Shotgun, new SaboteurProfile(Color.Beige) },
            {ItemID.OnyxBlaster, new SaboteurProfile(Color.Beige) },
            {ItemID.Minishark, new SaboteurProfile(Color.Beige) },
            {ItemID.Megashark, new SaboteurProfile(Color.Beige) },
            {ItemID.StarCannon, new SaboteurProfile(Color.Beige) },
            {ItemID.SuperStarCannon, new SaboteurProfile(Color.Beige) },
            {ItemID.SniperRifle, new SaboteurProfile(Color.Beige) },
            {ItemID.VenusMagnum, new SaboteurProfile(Color.Beige) },
            {ItemID.ChainGun, new SaboteurProfile(Color.Beige) },
            {ItemID.Xenopopper, new SaboteurProfile(Color.Beige) },
            {ItemID.Celeb2, new SaboteurProfile(Color.Beige) },
            {ItemID.SDMG, new SaboteurProfile(Color.Beige) }
        };

        private static SaboteurProfile defaultProfile = new SaboteurProfile(new Color(255, 33, 33));

        public static SaboteurProfile GetProfile(int gunID)
        {
            if (!profiles.TryGetValue(gunID, out SaboteurProfile value))
                return defaultProfile;
            return value;
        }
    }
}
