using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;

public partial class MoreUtils
{
    /// <summary>
    /// Modular sparkle drawer. Works best with symmetrical textures.
    /// </summary>
    public static void DrawSparkle(Asset<Texture2D> texture, SpriteEffects dir, Vector2 drawCenter, Vector2 origin, float scale, float thickness, float width, float height, float rotation, Color drawColor, Color shineColor, float opacity = 1f, byte alpha = 0)
    {
        Color color1 = drawColor * opacity;
        color1.A = alpha;
        Color color2 = shineColor * opacity;
        color2.A = alpha;

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
    public static void DrawStreak(Asset<Texture2D> texture, SpriteEffects dir, Vector2 drawCenter, Vector2 origin, float scale, float width, float height, float rotation, Color drawColor, Color shineColor, float opacity = 1f, byte alpha = 0)
    {
        Color color1 = drawColor * opacity;
        color1.A = alpha;
        Color color2 = shineColor * opacity;
        color2.A = alpha;

        Vector2 vector = new Vector2(width, height) * scale;

        Main.EntitySpriteDraw(texture.Value, drawCenter, null, color1, rotation + MathHelper.PiOver2, origin, vector, dir, 0);
        Main.EntitySpriteDraw(texture.Value, drawCenter, null, color2, rotation + MathHelper.PiOver2, origin, vector * 0.56f, dir, 0);
    }

    public static float DualLerp(float start, float middle, float end, float value, bool clamp = true)
    {
        return Utils.GetLerpValue(start, middle, value, clamp) * Utils.GetLerpValue(end, middle, value, clamp);
    }

    public static float DualLerp(float start, float middleOne, float middleTwo, float end, float value, bool clamp = true)
    {
        return Utils.GetLerpValue(start, middleOne, value, clamp) * Utils.GetLerpValue(end, middleTwo, value, clamp);
    }

    public static float GetCircle(float counter, float total, float piMultiplier = 1f)
    {
        return ((MathHelper.TwoPi * piMultiplier) / (total)) * counter;
    }

    public static Vector2 GetPositionAround(Vector2 center, float radius, bool careAboutTiles)
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

    public static bool GetNPCTarget(object attacker, Vector2 position, float maxDistance, out int npcIndex)
    {
        npcIndex = 0;
        int? index = null;
        float realDistance = maxDistance;
        for (int i = 0; i < 200; i++)
        {
            NPC npc = Main.npc[i];
            if (npc.CanBeChasedBy(attacker))
            {
                float targetDistance = position.Distance(npc.Center);
                if (!(realDistance <= targetDistance))
                {
                    index = i;
                    realDistance = targetDistance;
                }
            }
        }
        if (!index.HasValue)
            return false;
        npcIndex = index.Value;
        return true;
    }

    [Obsolete]
    public static List<Vector2> ChainPointsOld(Vector2 startPoint, Vector2 endPoint, int pointCount)
    {
        List<Vector2> controlPoints = new List<Vector2>();
        float interval = 1f / pointCount;
        for (float i = 0; i < pointCount - 1; i += interval)
        {
            controlPoints.Add(Vector2.Lerp(startPoint, endPoint, i));
        }
        return controlPoints;
    }

    [Obsolete]
    public static List<Vector2> QuadraticBezierPointsOld(Vector2 startPoint, Vector2 midPoint, Vector2 endPoint, int pointCount)
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

    [Obsolete]
    public static List<Vector2> CubicBezierPointsOld(Vector2 startPoint, Vector2 firstMidPoint, Vector2 secondMidPoint, Vector2 endPoint, int pointCount)
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
                List<Vector2> midA = QuadraticBezierPointsOld(startPoint, firstMidPoint, secondMidPoint, pointCount);
                List<Vector2> midB = QuadraticBezierPointsOld(firstMidPoint, secondMidPoint, endPoint, pointCount);

                Vector2 control = Vector2.Lerp(midA[(int)j], midB[(int)j], i);

                controlPoints.Add(control);
            }
        }
        return controlPoints;
    }
}