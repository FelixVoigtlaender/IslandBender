using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bending : MonoBehaviour
{
    public static Vector2 RotateVector(Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }


    public RaycastHit2D CircleCast(Vector2 input, int sections, float radius , LayerMask layer)
    {
        float deltaAngle = 360 / (sections);

        //Find Collision in Radius with Circular raycasts
        Vector2 rod = input * radius;
        RaycastHit2D hit;
        for (int i = 0; i < 5; i++)
        {
            //right 
            float angle = deltaAngle * i;
            Vector2 dir = RotateVector(rod, angle);
            Vector2 nextDir = RotateVector(rod, angle + deltaAngle);
            Vector3 orthDir = nextDir - dir;

            hit = Physics2D.Raycast((Vector2)transform.position, dir, dir.magnitude, layer);

            if (!hit)
            {
                hit = Physics2D.Raycast((Vector2)transform.position + dir, orthDir, orthDir.magnitude, layer);
                //Debug.DrawRay((Vector2)transform.position + dir, orthDir);
            }

            //right
            if (!hit)
            {
                angle = -deltaAngle * i;
                dir = RotateVector(rod, angle);
                nextDir = RotateVector(rod, angle - deltaAngle);
                orthDir = nextDir - dir;

                hit = Physics2D.Raycast((Vector2)transform.position, dir, dir.magnitude, layer);

                if (!hit)
                {
                    hit = Physics2D.Raycast((Vector2)transform.position + dir, orthDir, orthDir.magnitude, layer);
                    //Debug.DrawRay((Vector2)transform.position + dir, orthDir);
                }
            }

            if (hit)
            {
                return hit;
            }
        }
        return new RaycastHit2D();
    }

    public RaycastHit2D StarCast(Vector2 input, int sections, float radius, LayerMask layer)
    {
        input = input.normalized;
        float deltaAngle = 360 / (sections);

        //Find Collision in Radius with Circular raycasts
        Vector2 rod = input * radius;
        RaycastHit2D hit;
        for (int i = 0; i < sections; i++)
        {
            float angle = deltaAngle * i;

            //right
            Vector2 dir = RotateVector(rod, angle);
            hit = Physics2D.Raycast((Vector2)transform.position, dir, dir.magnitude, layer);
            //left
            if (!hit)
            {
                dir = RotateVector(rod, -angle);
                hit = Physics2D.Raycast((Vector2)transform.position, dir, dir.magnitude, layer);
            }


            if (hit)
            {
                return hit;
            }
        }

        return new RaycastHit2D();
    }

    public static void AddVelocity(Rigidbody2D rigid, Vector2 deltaVel)
    {
        Vector2 velocity = rigid.velocity;
        velocity += deltaVel;
        rigid.velocity = velocity;
    }
}
