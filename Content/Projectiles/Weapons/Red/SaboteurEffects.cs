using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace BlockContent.Content.Projectiles.Weapons.Red
{
    public struct SaboteurEffects
    {
        public static List<int> guns = new List<int>
        {
            ItemID.FlintlockPistol,
            ItemID.QuadBarrelShotgun,
            ItemID.Handgun,
            ItemID.PhoenixBlaster,
            ItemID.Shotgun,
            ItemID.OnyxBlaster,
            ItemID.Megashark,
            ItemID.SuperStarCannon,
            ItemID.SniperRifle,
            ItemID.VenusMagnum,
            ItemID.ChainGun,
            ItemID.Xenopopper,
            ItemID.VenusMagnum,
            ItemID.Celeb2,
            ItemID.SDMG
        };

        public void DrawGun(Vector2 position, float rotation, int index, float power)
        {
            Asset<Texture2D> gunTexture = TextureAssets.Item[guns[index]];

            Vector2 origin = gunTexture.Size() * new Vector2(1f, 0.5f);
            //Main.EntitySpriteDraw(gunTexture.Value, )
        }
    }
}
