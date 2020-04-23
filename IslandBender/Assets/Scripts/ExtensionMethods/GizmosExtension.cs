using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmosExtension : MonoBehaviour
{
    public static Vector2 DrawParabel(Vector2 start, Vector2 velocity, float gravity, float time = 1.5f, bool stopOnY = true)
    {
        int steps = 50;

        float deltaTime = time / steps;
        Vector2 position = start;
        for (float i = 0; i < steps; i++)
        {
            Vector2 lastPosition = position;

            velocity += Vector2.up * gravity * deltaTime;
            position += velocity * deltaTime;


            if (position.y < start.y)
            {
                Vector2 deltaPosition = position - lastPosition;
                deltaPosition *= 1 - (position.y - start.y)/deltaPosition.y;
                position = lastPosition + deltaPosition;
                Gizmos.DrawLine(lastPosition, position);
                return position;
            }

            Gizmos.DrawLine(lastPosition, position);

        }

        return position;
    }
}
