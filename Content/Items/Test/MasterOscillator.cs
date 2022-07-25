using Microsoft.Xna.Framework;
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

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            int number = Main.LocalPlayer.GetModPlayer<MasterOscillatorActions>().actionNumber;
            string actionName = Main.LocalPlayer.GetModPlayer<MasterOscillatorActions>().actions[number].Name;
            TooltipLine actionLine = new TooltipLine(Mod, "Blockaroz:MasterOscillator", "[glyph:5] # " + number + " : " + actionName);
            tooltips.Add(actionLine);
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 24;
            Item.useTime = 15;
            Item.useAnimation = 15;
            SoundStyle shootNoise = SoundID.Item115;
            shootNoise.MaxInstances = 0;
            shootNoise.Volume = 0.8f;
            shootNoise.PitchVariance = 0.4f;
            Item.UseSound = shootNoise;
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.rare = ModContent.RarityType<Weapons.DeepBlueRarity>();
            Item.shootSpeed = 2f;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool? UseItem(Player player)
        {
            int number = player.GetModPlayer<MasterOscillatorActions>().actionNumber;
            if (player.itemAnimation == player.itemAnimationMax - 1)
            {
                if (player.altFunctionUse == 2)
                    player.GetModPlayer<MasterOscillatorActions>().actionNumber++;

                else
                    player.GetModPlayer<MasterOscillatorActions>().actions[number].Invoke();
            }
            if (player.itemAnimation == player.itemAnimationMax - 2)
            {
                if (player.altFunctionUse == 2)
                {
                    string actionName = player.GetModPlayer<MasterOscillatorActions>().actions[number].Name;
                    CombatText.NewText(new Rectangle((int)(player.Center.X - 12), (int)(player.Bottom.Y - 4), 24, 8), Color.DimGray, actionName, false, true);
                }
            }

            return false;
        }

        public override Vector2? HoldoutOffset() => new Vector2(1, 0);

        public override Vector2? HoldoutOrigin() => new Vector2(9);

        public override void AddRecipes()
        {
            CreateRecipe()
                .Register();
        }
    }

    public class MasterOscillatorActions : ModPlayer
    {
        public int actionNumber;

        public List<OscillatorAction> actions;

        public override void PreUpdate()
        {
            actions = new List<OscillatorAction>();
            actions.Add(new OscillatorAction(() =>
            {
                Projectile.NewProjectileDirect(new EntitySource_ItemUse(Player, Player.HeldItem), Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<SonsAndDaughters.Content.DoomBall>(), 20, 0, Main.myPlayer);
            }, "Sons n' Daughters"));
            actions.Add(new OscillatorAction(() =>
            {
                SonsAndDaughters.RedFilter.Active = !SonsAndDaughters.RedFilter.Active;
            }, "Doom Filter"));

            if (actionNumber > actions.Count - 1)
                actionNumber = 0;
            if (actionNumber < 0)
                actionNumber = actions.Count - 1;
        }

        public struct OscillatorAction
        {
            public OscillatorAction(Action action, string name)
            {
                this.name = name;
                this.action = action;
            }

            private string name;
            private Action action;

            public string Name { get => name; }

            public void Invoke() => action.Invoke();
        }
    }
}
