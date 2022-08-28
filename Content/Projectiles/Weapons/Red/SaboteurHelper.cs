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
            {ItemID.FlintlockPistol, new SaboteurProfile(Color.DarkKhaki) },
            {ItemID.QuadBarrelShotgun, new SaboteurProfile(Color.OrangeRed) },
            {ItemID.Handgun, new SaboteurProfile(Color.LightCoral) },
            {ItemID.PhoenixBlaster, new SaboteurProfile(Color.Orange) },
            {ItemID.Shotgun, new SaboteurProfile(Color.Gainsboro) },
            {ItemID.OnyxBlaster, new SaboteurProfile(Color.DarkOrchid) },
            {ItemID.Minishark, new SaboteurProfile(Color.DarkOrange) },
            {ItemID.Megashark, new SaboteurProfile(Color.DodgerBlue) },
            {ItemID.StarCannon, new SaboteurProfile(Color.MediumBlue) },
            {ItemID.SuperStarCannon, new SaboteurProfile(Color.Goldenrod) },
            {ItemID.SniperRifle, new SaboteurProfile(Color.DimGray) },
            {ItemID.VenusMagnum, new SaboteurProfile(Color.Lime) },
            {ItemID.ChainGun, new SaboteurProfile(Color.DarkRed) },
            {ItemID.Xenopopper, new SaboteurProfile(Color.Magenta) },
            {ItemID.Celeb2, new SaboteurProfile(Color.GhostWhite) },
            {ItemID.SDMG, new SaboteurProfile(Color.Aquamarine) }
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
