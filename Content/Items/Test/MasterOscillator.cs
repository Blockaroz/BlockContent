using Microsoft.Xna.Framework;
using ParticleEngine;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContent.Content.Items.Test
{
    public class MasterOscillator : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Master Oscillator");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 24;
            Item.useTime = 25;
            Item.useAnimation = 25;
            SoundStyle shootNoise = SoundID.Item84;
            shootNoise.MaxInstances = 0;
            shootNoise.Volume = 0.8f;
            shootNoise.PitchVariance = 0.4f;
            Item.UseSound = shootNoise;
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.rare = ItemRarityID.Red;
            Item.shootSpeed = 2f;
            Item.shoot = ModContent.ProjectileType<SonsAndDaughters.Content.DoomBall>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
                SonsAndDaughters.RedFilter.Active = !SonsAndDaughters.RedFilter.Active;
            else
            {
                Projectile.NewProjectileDirect(source, Main.MouseWorld, velocity, type, 20, knockback, Main.myPlayer);

                Particle bolt = Particle.NewParticle(Particle.ParticleType<SonsAndDaughters.Content.DoomBolt>(), position, Main.rand.NextVector2Circular(5, 5), Color.Red, Main.rand.NextFloat(1f, 2f));
                Vector2 boltStart = position;
                Vector2 boltEnd = Main.MouseWorld;
                float strength = boltStart.Distance(boltEnd) / 100f;
                bolt.data = new LightningData(boltStart, Vector2.SmoothStep(boltStart, boltEnd, 0.5f) + Main.rand.NextVector2Circular(100, 80).RotatedBy(boltStart.AngleTo(boltEnd)), boltEnd, strength * 0.8f, (int)(strength * 1.5f)).Value;

            }

            return false;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            player.itemRotation = player.AngleTo(Main.MouseWorld) - MathHelper.Pi * (player.direction > 0 ? 0 : 1);
            player.ChangeDir(player.DirectionTo(Main.MouseWorld).X > 0 ? 1 : -1);
        }

        public override Vector2? HoldoutOffset() => new Vector2(1, 0);

        public override Vector2? HoldoutOrigin() => new Vector2(9);

        public override void AddRecipes()
        {
            CreateRecipe()
                .Register();
        }
    }
}
