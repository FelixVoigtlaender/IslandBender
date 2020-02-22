using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Create : MonoBehaviour
{
    Color color = Color.green;
    public float createRadius = 3;
    public LayerMask createLayer;
    public bool isCreating;
    public bool canCreate = false;

    PlayerController player;
    Vector2 aim, mov, lastPotentAim;
    public Vector2 createPosition, createDir;
    public Collider2D createCollider;
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
        isCreating = player.controls.Player.Create.phase == UnityEngine.InputSystem.InputActionPhase.Performed;

        //Creating
        canCreate = (createPosition - (Vector2)transform.position).magnitude < createRadius + 0.1f && isCreating;
        if (isCreating)
            Creating(lastPotentAim);
    }

    public void Creating(Vector2 input)
    {
        input = input.normalized;
        int sections = 10;
        float deltaAngle = 360 / (sections);

        //Find Collision in Radius with Circular raycasts
        Vector2 rod = input * createRadius;
        RaycastHit2D hit;
        for (int i = 0; i < 5; i++)
        {
            //right 
            float angle = deltaAngle * i;
            Vector2 dir = RotateVector(rod, angle);
            Vector2 nextDir = RotateVector(rod, angle + deltaAngle);
            Vector3 orthDir = nextDir - dir;

            hit = Physics2D.Raycast((Vector2)transform.position , dir, dir.magnitude, createLayer);

            if (!hit)
            {
                hit = Physics2D.Raycast((Vector2)transform.position + dir, orthDir, orthDir.magnitude, createLayer);
                //Debug.DrawRay((Vector2)transform.position + dir, orthDir);
            }

            //right
            if (!hit)
            {
                angle = -deltaAngle * i;
                dir = RotateVector(rod, angle);
                nextDir = RotateVector(rod, angle - deltaAngle);
                orthDir = nextDir - dir;

                hit = Physics2D.Raycast((Vector2)transform.position, dir, dir.magnitude, createLayer);

                if (!hit)
                {
                    hit = Physics2D.Raycast((Vector2)transform.position + dir, orthDir, orthDir.magnitude, createLayer);
                    //Debug.DrawRay((Vector2)transform.position + dir, orthDir);
                }
            }

            if (hit)
            {
                createPosition = hit.point;
                createDir = hit.normal;
                canCreate = true;
                createCollider = hit.collider;
                return;
            }
        }
        canCreate = false;
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
        Gizmos.DrawWireSphere(transform.position, createRadius);
        if (canCreate)
            Gizmos.DrawRay(createPosition, createDir);
            
    }
}
