using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugExtension : MonoBehaviour
{
    public static Color color = Color.white;

    public static void DrawCircle(Vector2 position, float radius)
    {
        DrawShape(ShapeExtension.CreateCirclePoints(position, radius));
    }
    public static void DrawShape(Vector2[] points)
    {
        for(int i = 0; i < points.Length; i++)
        {
            Vector2 p0 = points[i];
            Vector2 p1 = points[(i+1)%points.Length];

            Debug.DrawLine(p0, p1, color);
        }

        Debug.DrawLine(Vector2.zero, Vector2.one*10, color);
    }

    public static Vector2 DrawParabel(Vector2 start, Vector2 velocity, float gravity, float time = 10)
    {
        int steps = 50;
        
        float deltaTime = time / steps;
        Vector2 position = start;
        for(float i = 0; i < steps; i++)
        {
            Vector2 lastPosition = position;

            position += velocity * deltaTime;
            velocity -= Vector2.up * gravity * deltaTime;

            Debug.DrawLine(lastPosition, position);
        }

        return position;
    }
}
