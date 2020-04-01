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
    [HideInInspector]
    public Vector2 position, goalPosition, dir, smoothVelocity;
    public Creatable creatable;
    
    // Manager for all actions for Creation
    private void Start()
    {
        player = GetComponent<PlayerController>();
    }

    private void Update()
    {
        isCreating = player.controls.Player.Create.phase == UnityEngine.InputSystem.InputActionPhase.Performed;

        //Unselect creatable if not creating
        if (!isCreating && creatable)
        {
            creatable.UnSelect();
            creatable = null;
        }

        position = Vector2.SmoothDamp(position, goalPosition, ref smoothVelocity, 0.1f);
    }

    private void FixedUpdate()
    {
        if (isCreating)
            Creating(player.lastPotentAim);

        canCreate = (position - (Vector2)transform.position).magnitude < createRadius + 0.1f && isCreating;
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
            // Impossible to spawn rigids on dynamic rigids...
            if (hit && hit.rigidbody && hit.rigidbody.bodyType == RigidbodyType2D.Dynamic)
                continue;

            //Found hit and its a creatable (Can spawn stuff on it)
            if (hit && hit.transform.GetComponent<Creatable>())
            {

                //Set Information for Creators
                goalPosition = hit.point;
                this.dir = hit.normal;
                canCreate = true;

                //Unselect old selection
                Creatable newCreatable = hit.transform.GetComponent<Creatable>();
                if (creatable && creatable != newCreatable)
                {
                    creatable.UnSelect();
                    //position = goalPosition;
                }
                creatable = newCreatable;
                creatable.Select(position, player.color);

                //Happy with first creatable.
                return;
            }
        }
        //Couldn't find any creatable
        canCreate = false;
        //Unselect creatable
        if (creatable)
        {
            creatable.UnSelect();
            creatable = null;
        }
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
            Gizmos.DrawRay(position, dir);
            
    }
}
