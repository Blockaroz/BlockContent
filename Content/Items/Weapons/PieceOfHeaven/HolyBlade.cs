﻿using BlockContent.Content.Projectiles.Weapons.PieceOfHeaven;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleEngine;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContent.Content.Items.Weapons.PieceOfHeaven
{
    public class HolyBlade : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Blade");
            Tooltip.SetDefault("Shiny \nRight click to guard");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ItemID.Sets.ItemNoGravity[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 64;
            Item.height = 72;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 18;
            Item.useAnimation = 18;
            Item.reuseDelay = 5;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.channel = true;
            Item.shoot = ModContent.ProjectileType<HolyBladeHeld>();
            Item.shootSpeed = 7f;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.attackSpeedOnlyAffectsWeaponAnimation = false;

            Item.DamageType = DamageClass.Melee;
            Item.damage = 230;
            Item.crit = 50;
            Item.knockBack = 0.5f;
            Item.rare = ModContent.RarityType<DeepBlueRarity>();
            Item.value = Item.buyPrice(0, 50);
        }

        public override void PostUpdate()
        {
            Item.position.Y -= (float)Math.Sin(Item.timeSinceItemSpawned / 60f) * 0.12f;
            if (Main.rand.NextBool() && Item.timeSinceItemSpawned % 5 == 0)
            {
                Color randColor = Color.Lerp(HeavenColors.Melee, HeavenColors.MeleeDark, Main.rand.Next(2));
                Vector2 pos = Item.Center + Main.rand.NextVector2CircularEdge(10, 20) + Main.rand.NextVector2Circular(30, 50);
                Particle.NewParticle(Particle.ParticleType<Particles.HeavenSpark>(), pos, -Vector2.UnitY * Main.rand.NextFloat(), randColor, Main.rand.NextFloat(0.2f, 0.6f));
            }
        }

        public override void HoldItem(Player player)
        {
            player.hasRaisableShield = true;

            Lighting.AddLight(player.Center, HeavenColors.MeleeDark.ToVector3() * 0.7f);
        }

        public override Color? GetAlpha(Color lightColor) => Color.Lerp(lightColor, Color.White, 0.7f);

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Asset<Texture2D> shadow = ModContent.Request<Texture2D>(Texture + "Shadow");
            Asset<Texture2D> bloom = ModContent.Request<Texture2D>(Texture + "Bloom");

            Color bloomColor = Color.Lerp(Color.GhostWhite, HeavenColors.Melee, 0.3f) * 0.3f;
            bloomColor.A = 0;
            spriteBatch.Draw(shadow.Value, position, null, Color.Black * 0.3f, 0, origin + new Vector2(14), scale, 0, 0);
            spriteBatch.Draw(texture.Value, position, null, drawColor, 0, origin, scale, 0, 0);
            spriteBatch.Draw(bloom.Value, position, null, bloomColor, 0, origin + new Vector2(14), scale, 0, 0);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Asset<Texture2D> shadow = ModContent.Request<Texture2D>(Texture + "Shadow");
            Asset<Texture2D> bloom = ModContent.Request<Texture2D>(Texture + "Bloom");

            Vector2 origin = new Vector2(36, 36);
            Color bloomColor = Color.Lerp(Color.GhostWhite, HeavenColors.Melee, 0.3f) * 0.3f;
            bloomColor.A = 0;
            spriteBatch.Draw(shadow.Value, Item.Center - Main.screenPosition, null, Color.Black * 0.3f, rotation - MathHelper.PiOver4, origin + new Vector2(14), scale, 0, 0);
            spriteBatch.Draw(texture.Value, Item.Center - Main.screenPosition, null, alphaColor, rotation - MathHelper.PiOver4, origin, scale, 0, 0);
            spriteBatch.Draw(bloom.Value, Item.Center - Main.screenPosition, null, bloomColor, rotation - MathHelper.PiOver4, origin + new Vector2(14), scale, 0, 0);
            //DrawStar(spriteBatch, Item.Center - new Vector2(-2, -19).RotatedBy(rotation));

            return false;
        }

        private void DrawStar(SpriteBatch spriteBatch, Vector2 position)
        {
            Asset<Texture2D> star = ModContent.Request<Texture2D>($"{nameof(BlockContent)}/Assets/Textures/GlowStar");
            Asset<Texture2D> bloom = ModContent.Request<Texture2D>($"{nameof(BlockContent)}/Assets/Textures/GlowSoft");

            float shineStrength = 0.3f + (float)Math.Sin(Item.timeSinceItemSpawned / 40f) * 0.2f;

            Color shineColor = Color.White * 0.5f;
            shineColor.A = 0;            
            Color bloomColor = HeavenColors.Melee * (0.2f + shineStrength * 0.5f);
            bloomColor.A = 0;

            spriteBatch.Draw(star.Value, position - Main.screenPosition, null, bloomColor, 0, star.Size() * 0.5f, 1f, 0, 0);
            spriteBatch.Draw(star.Value, position - Main.screenPosition, null, shineColor, 0, star.Size() * 0.5f, 0.5f, 0, 0);
            spriteBatch.Draw(bloom.Value, position - Main.screenPosition, null, bloomColor * 0.3f * shineStrength, 0, bloom.Size() * 0.5f, 1.5f, 0, 0);
        }

        public override bool AltFunctionUse(Player player) => !player.HasBuff(BuffID.ParryDamageBuff);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.ownedProjectileCounts[type] < 1)
            {
                float ai0 = 0;
                float ai1 = 0;

                if (player.altFunctionUse == 2)
                    ai0 = 1;
                if (player.HasBuff(BuffID.ParryDamageBuff))
                    ai0 = 2;

                //ai0 = 2;

                Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI, ai0, ai1);
                proj.spriteDirection = player.direction;
                proj.direction = -1;
                proj.scale = player.GetAdjustedItemScale(Item) * 1.2f;
            }

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Zenith)
                .Register();
        }
    }
}
