using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class Manipulate : MonoBehaviour
{
    Color color = Color.red;
    public float manipulateRadius = 5;
    public LayerMask manipulateLayer;
    public bool isManipulating;
    public bool canManipulate = false;

    PlayerController player;
    public Vector2 aim, mov, lastPotentAim;
    public Collider2D manipulateCollider;
    // Manager for all actions for Creation
    private void Start()
    {
        player = GetComponent<PlayerController>();
    }

    private void Update()
    {
        //Input
        aim = player.controls.Player.Aim.ReadValue<Vector2>();
        lastPotentAim = aim.magnitude > 0.1f ? aim : lastPotentAim;
        mov = player.controls.Player.Movement.ReadValue<Vector2>();
        isManipulating = player.controls.Player.Manipulate.phase == UnityEngine.InputSystem.InputActionPhase.Performed;

        //Creating
        canManipulate = isManipulating;
        if (isManipulating)
            Creating(lastPotentAim);
    }


    public void Creating(Vector2 input)
    {
        input = input.normalized;
        int sections = 10;
        float deltaAngle = 360 / (sections);

        //Find Collision in Radius with Circular raycasts
        Vector2 rod = input * manipulateRadius;
        RaycastHit2D hit;
        for (int i = 0; i < sections; i++)
        {
            float angle = deltaAngle * i;

            //right
            Vector2 dir = RotateVector(rod, angle);
            hit = Physics2D.Raycast((Vector2)transform.position, dir, dir.magnitude, manipulateLayer);
            Debug.DrawRay(transform.position, dir, color);
            //left
            if (!hit)
            {
                dir = RotateVector(rod, -angle);
                hit = Physics2D.Raycast((Vector2)transform.position, dir, dir.magnitude, manipulateLayer);

                Debug.DrawRay(transform.position, dir,color);
            }


            if (hit)
            {
                manipulateCollider = hit.collider;
                return;
            }
        }


        canManipulate = false;
    }

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

    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawWireSphere(transform.position, manipulateRadius);

    }

}
