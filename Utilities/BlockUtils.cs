using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;

namespace BlockContent
{
    public partial class BlockUtils
    {
        ///Credits
        ///Seraph for Bezier help
        ///NotLe0n for Bezier help

        /// <summary>
        /// Modular sparkle drawer. Works best with symmetrical textures.
        /// </summary>
        public static void DrawSparkle(Asset<Texture2D> texture, SpriteEffects dir, Vector2 drawCenter, Vector2 origin, float scale, float thickness, float width, float height, float rotation, Color drawColor, Color shineColor, float opacity = 1f, byte colorAlpha = 0)
        {
            Color color1 = drawColor * opacity;
            color1.A = colorAlpha;
            Color color2 = shineColor * opacity;
            color2.A = colorAlpha;

            Vector2 vector1 = new Vector2(thickness, width) * scale;
            Vector2 vector2 = new Vector2(thickness, height) * scale;

            Main.EntitySpriteDraw(texture.Value, drawCenter, null, color1, rotation, origin, vector1, dir, 0);
            Main.EntitySpriteDraw(texture.Value, drawCenter, null, color1, rotation + MathHelper.PiOver2, origin, vector2, dir, 0);
            Main.EntitySpriteDraw(texture.Value, drawCenter, null, color2, rotation, origin, vector1 * 0.56f, dir, 0);
            Main.EntitySpriteDraw(texture.Value, drawCenter, null, color2, rotation + MathHelper.PiOver2, origin, vector2 * 0.56f, dir, 0);
        }

        /// <summary>
        /// Useful for sparkles that don't use symmetrical textures.
        /// </summary>
        public static void DrawStreak(Asset<Texture2D> texture, SpriteEffects dir, Vector2 drawCenter, Vector2 origin, float scale, float width, float height, float rotation, Color drawColor, Color shineColor, float opacity = 1f, byte colorAlpha = 0)
        {
            Color color1 = drawColor * opacity;
            color1.A = colorAlpha;
            Color color2 = shineColor * opacity;
            color2.A = colorAlpha;

            Vector2 vector = new Vector2(width, height) * scale;

            Main.EntitySpriteDraw(texture.Value, drawCenter, null, color1, rotation + MathHelper.PiOver2, origin, vector, dir, 0);
            Main.EntitySpriteDraw(texture.Value, drawCenter, null, color2, rotation + MathHelper.PiOver2, origin, vector * 0.56f, dir, 0);
        }

        public static float GetSquareLerp(float start, float middle, float end, float value)
        {
            return Utils.GetLerpValue(start, middle, value, true) * Utils.GetLerpValue(end, middle, value, true);
        }

        public static float GetSquareLerp(float start, float middleOne, float middleTwo, float end, float value)
        {
            return Utils.GetLerpValue(start, middleOne, value, true) * Utils.GetLerpValue(end, middleTwo, value, true);
        }

        public static float GetCircle(float counter, float total, float piMultiplier = 1f)
        {
            return ((MathHelper.TwoPi * piMultiplier) / (total)) * counter;
        }

        public static Vector2 GetPositionAroundTarget(Vector2 center, float radius, bool careAboutTiles)
        {
            Vector2 rotation = -(Vector2.UnitY * radius).RotatedByRandom(MathHelper.Pi);

            while (careAboutTiles == true)
            {
                Vector2 position = center + rotation;
                if (!Collision.SolidCollision(position, 1, 1))
                {
                    return position;
                }
                else
                {
                    rotation = rotation.RotatedByRandom(MathHelper.Pi);
                }
            }
            return center + rotation;
        }

        public static List<Vector2> ChainPoints(Vector2 startPoint, Vector2 endPoint, int pointCount)
        {
            List<Vector2> controlPoints = new List<Vector2>();
            float interval = 1f / pointCount;
            for (float i = 0; i < pointCount - 1; i += interval)
            {
                controlPoints.Add(Vector2.Lerp(startPoint, endPoint, i));
            }
            return controlPoints;
        }

        public static List<Vector2> QuadraticBezierPoints(Vector2 startPoint, Vector2 midPoint, Vector2 endPoint, int pointCount)
        {
            List<Vector2> controlPoints = new List<Vector2>();
            float interval = 1f / (pointCount - 1);

            if (pointCount <= 2)
            {
                controlPoints.Add(startPoint);
                controlPoints.Add(midPoint);
                controlPoints.Add(endPoint);
            }
            else
            {
                for (float i = 0; i < pointCount - 1; i += interval)
                {
                    Vector2 startToMid = Vector2.Lerp(startPoint, midPoint, i);
                    Vector2 MidToEnd = Vector2.Lerp(midPoint, endPoint, i);
                    //Vector2 startToEnd = Vector2.Lerp(startPoint, endPoint, i);
                    //excluding this because the difference is negligable

                    Vector2 control = Vector2.Lerp(startToMid, MidToEnd, i);
                    controlPoints.Add(control);
                }
            }
            return controlPoints;
        }

        public static List<Vector2> CubicBezierPoints(Vector2 startPoint, Vector2 firstMidPoint, Vector2 secondMidPoint, Vector2 endPoint, int pointCount)
        {
            List<Vector2> controlPoints = new List<Vector2>();
            float interval = 1f / (pointCount - 1);

            if (pointCount <= 2)
            {
                controlPoints.Add(startPoint);
                controlPoints.Add(firstMidPoint);
                controlPoints.Add(secondMidPoint);
                controlPoints.Add(endPoint);
            }
            else
            {
                for (float i = 0; i < pointCount - 1; i += interval)
                {
                    float j = Math.Max(0, (int)(i * (pointCount - 1)));
                    List<Vector2> midA = QuadraticBezierPoints(startPoint, firstMidPoint, secondMidPoint, pointCount);
                    List<Vector2> midB = QuadraticBezierPoints(firstMidPoint, secondMidPoint, endPoint, pointCount);

                    Vector2 control = Vector2.Lerp(midA[(int)j], midB[(int)j], i);

                    controlPoints.Add(control);
                }
            }
            return controlPoints;
        }
    }
}