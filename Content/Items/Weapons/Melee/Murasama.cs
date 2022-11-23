using BlockContent.Content.Projectiles.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.PlayerDrawLayer;

namespace BlockContent.Content.Items.Weapons.Melee
{
    public class Murasama : ModItem
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Murasama");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Mod == "Terraria" && line.Name == "ItemName")
            {
                Color brightColor = Color.Lerp(Color.White, Color.Red, 0.5f + (float)Math.Sin(Main.GlobalTimeWrappedHourly % 1f * MathHelper.TwoPi) * 0.1f);
                brightColor.A = 0;
                Color bloomColor = Color.DarkRed * 0.8f;
                bloomColor.A = 0;

                for (int i = 0; i < 8; i++)
                {
                    Vector2 offset = new Vector2(4f + (float)Math.Sin(Main.GlobalTimeWrappedHourly % 1f * MathHelper.TwoPi), 0).RotatedBy(i / 8f * MathHelper.TwoPi);
                    Utils.DrawBorderStringFourWay(Main.spriteBatch, line.Font, line.Text, line.X + offset.X, line.Y + offset.Y, new Color(30, 0, 0, 50), Color.Black * 0.1f, line.Origin);
                }
                Utils.DrawBorderStringFourWay(Main.spriteBatch, line.Font, line.Text, line.X, line.Y, brightColor, new Color(128, 0, 0, 128), line.Origin);
                return false;
            }

            return base.PreDrawTooltipLine(line, ref yOffset);
        }

        public override void SetDefaults()
        {
            Item.width = 60;
            Item.height = 84;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noUseGraphic = true;
            Item.channel = true;

            Item.damage = 300;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.knockBack = 2f;
            Item.rare = ModContent.RarityType<DeepBlue>();
            Item.value = Item.buyPrice(1);
            Item.shootSpeed = 5f;
            Item.shoot = ModContent.ProjectileType<Projectiles.Weapons.Melee.MurasamaHeld>();
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;

        public override bool CanUseItem(Player player) => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.ownedProjectileCounts[type] <= 0)
                Projectile.NewProjectileDirect(source, position, Vector2.Zero, type, Item.damage, 0f, player.whoAmI, 0, -1);

            return false;
        }
    }

    //public class MurasamaLayer : PlayerDrawLayer
    //{
    //    public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.Wings);

    //    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.drawPlayer.HeldItem.type == ModContent.ItemType<Murasama>();

    //    protected override void Draw(ref PlayerDrawSet drawInfo)
    //    {
    //        Asset<Texture2D> swordTex = ModContent.Request<Texture2D>($"{nameof(BlockContent)}/Content/Items/Weapons/Melee/Murasama");

    //        SpriteEffects direction = drawInfo.drawPlayer.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
    //        float rotation = (2.4f - Utils.GetLerpValue(0, 30, Math.Abs(drawInfo.drawPlayer.velocity.X) * 0.2f, true)) * drawInfo.drawPlayer.direction;

    //        if (drawInfo.drawPlayer.ownedProjectileCounts[ModContent.ProjectileType<MurasamaHeld>()] <= 0)
    //        {
    //            DrawData sword = new DrawData(swordTex.Value, drawInfo.Center + new Vector2(-10 * drawInfo.drawPlayer.direction, 12).RotatedBy(drawInfo.rotation) - Main.screenPosition, null, Color.White, drawInfo.rotation - rotation, swordTex.Size() * 0.5f, 1f, direction, 0);
    //            drawInfo.DrawDataCache.Add(sword);
    //        }

    //    }
    //}
}
