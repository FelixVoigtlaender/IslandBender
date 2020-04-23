using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ShapeExtension
{
    //Translate
    public static Vector2[] Translate(this Vector2[] points, Vector2 delta)
    {
        for (int i = 0; i < points.Length; i++)
        {
            points[i] += delta;
        }
        return points;
    }
    public static Vector2 Rotate(this Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }

    //Circle
    public static Vector2[] CreateCirclePoints(Vector2 position, float radius = 1, int segments = 20)
    {
        Vector2[] points = CreateLocalCirclePoints(radius, segments);
        points = points.Translate(position);
        return points;
    }

    public static Vector2[] CreateLocalCirclePoints(float radius = 1, int segments = 20)
    {
        float x, y;
        float angle = 0;

        Vector2[] points = new Vector2[segments];

        float angleSteps = 360f / segments;

        for (int i = 0; i < segments; i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle * i) * radius;
            y = Mathf.Cos(Mathf.Deg2Rad * angle * i) * radius;

            points[i] = new Vector2(x, y);
        }

        return points;
    }

}
