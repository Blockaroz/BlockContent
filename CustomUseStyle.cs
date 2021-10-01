using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;

namespace BlockContent
{
    public struct CustomUseStyle
    {
        public static void LiftWeight(Player player, Vector2 offset)
        {
            Player.CompositeArmStretchAmount stretch = Player.CompositeArmStretchAmount.Full;

            float lerpUp = Utils.GetLerpValue(player.itemAnimationMax, player.itemAnimationMax / 2, player.itemAnimation, true);
            float lerpDown = Utils.GetLerpValue(player.itemAnimationMax / 2, 0, player.itemAnimation, true);

            float rotation = (
                MathHelper.SmoothStep(MathHelper.ToRadians(10), MathHelper.ToRadians(-80), lerpUp) +
                //up
                MathHelper.Lerp(MathHelper.ToRadians(10), MathHelper.ToRadians(80), lerpDown)
                //down
                ) * player.direction;

            if (player.itemAnimation > (player.itemAnimationMax * 0.25f) && player.itemAnimation < (player.itemAnimationMax * 0.75f))
                stretch = Player.CompositeArmStretchAmount.Quarter;

            player.SetCompositeArmFront(true, stretch, rotation);
            player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.None, rotation);

            player.itemLocation = player.GetFrontHandPosition(stretch, rotation) + offset.RotatedBy(rotation);
            player.itemRotation = rotation + (MathHelper.PiOver4 * player.direction);
        }

        public static void SquatFloating(Player player)
        {
            float lerpUp = Utils.GetLerpValue(player.itemAnimationMax, player.itemAnimationMax / 1.75f, player.itemAnimation, true);
            float lerpDown = Utils.GetLerpValue(player.itemAnimationMax / 2, 0, player.itemAnimation, true);

            float squatHeight =
                //up
                MathHelper.Lerp(0, -5, lerpUp) +
                //down
                MathHelper.SmoothStep(0, 5, lerpDown);

            player.legFrameCounter = 0.0;

            int heightOffset = player.legFrame.Height;

            if (player.itemAnimation > (player.itemAnimationMax * 0.25f) && player.itemAnimation < (player.itemAnimationMax * 0.75f))
                heightOffset = player.legFrame.Height * 5;

            player.legFrame.Y = heightOffset - (int)squatHeight;
        }
    }
}
