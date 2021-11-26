using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent.Bestiary;
using Terraria.ModLoader;
using Terraria.UI;

namespace BlockContent.Content.BestiaryElements
{
    public class NightEmpressPortraitBackground : IBestiaryInfoElement, IBestiaryBackgroundImagePathAndColorProvider
    {
        private Mod Mod
        {
            get => ModContent.GetInstance<BlockContent>();
        }

        public Color? GetBackgroundColor() => null;

        public Asset<Texture2D> GetBackgroundImage() => Mod.Assets.Request<Texture2D>("Content/BestiaryElements/NightEmpressPortraitBackground");

        public UIElement ProvideUIElement(BestiaryUICollectionInfo info) => null;
    }
}
