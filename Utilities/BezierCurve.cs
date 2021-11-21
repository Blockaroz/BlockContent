using Microsoft.Xna.Framework;
using System.Collections.Generic;

public struct BezierCurve
{
    private List<Vector2> _controls;
    private int _segments;

    public BezierCurve(List<Vector2> controlPoints, int segments)
    {
        _controls = controlPoints;
        _segments = segments;
    }

    private Vector2 CalculatePoint(List<Vector2> points, float progress)
    {
        if (points.Count <= 2)
            return Vector2.Lerp(points[0], points[1], progress);
        else
        {
            List<Vector2> pointList = new List<Vector2>();
            for (int i = 0; i < points.Count - 1; i++)
            {
                pointList.Add(Vector2.Lerp(points[i], points[i + 1], progress));
            }
            return CalculatePoint(pointList, progress);
        }
    }

    public List<Vector2> Value
    {
        get 
        {
            List<Vector2> final = new List<Vector2>();
            for (int i = 0; i < _segments; i++)
            {
                float progress = (float)i / _segments;
                final.Add(CalculatePoint(_controls, progress));
            }
            return final; 
        }
    }
}
