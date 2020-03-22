using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class Manipulate : MonoBehaviour
{
    Color color = Color.red;
    public float radius = 5;

    public LayerMask layerMask;
    public bool isManipulating;

    PlayerController player;

    public Rigidbody2D[] manipulateables;
    // Manager for all actions for Creation
    private void Start()
    {
        player = GetComponent<PlayerController>();
    }

    private void Update()
    {
        isManipulating = player.controls.Player.Manipulate.phase == UnityEngine.InputSystem.InputActionPhase.Performed;
    }

    private void FixedUpdate()
    {
        manipulateables = FindManipulateables();
    }

    //Finds all Manipulateables (Rigidbody2D) in its manipulateRadius.
    public Rigidbody2D[] FindManipulateables()
    {
        if (!isManipulating)
            return new Rigidbody2D[0];

        List<Rigidbody2D> manipulateables = new List<Rigidbody2D>();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, layerMask);
        foreach(Collider2D c in colliders)
        {
            if (c.attachedRigidbody)
                manipulateables.Add(c.attachedRigidbody);
        }

        return manipulateables.ToArray();
    }

    //Finds closest manipulateable relative to an direction
    public Rigidbody2D FindManipulateable(Vector2 dir, float maxMass = 1000)
    {
        if (!isManipulating)
            return null;

        Vector2 myPosition = (Vector2)transform.position + dir * radius;

        Rigidbody2D closest = null;
        float closestDistance = radius*2 + 10;
        foreach (Rigidbody2D r in manipulateables)
        {
            float distance = ((Vector2)myPosition - r.position).magnitude;
            if (distance < closestDistance && r.mass < maxMass)
            {
                closest = r;
                closestDistance = distance;
            }
        }
        return closest;
    }

    public Rigidbody2D FindClosestManipulateable(float maxMass = 1000)
    {
        if (!isManipulating)
            return null;

        Rigidbody2D closest = null;
        float closestDistance = radius+10;
        foreach(Rigidbody2D r in manipulateables)
        {
            float distance = ((Vector2)transform.position - r.position).magnitude;
            if(distance < closestDistance && r.mass < maxMass)
            {
                closest = r;
                closestDistance = distance;
            }
        }
        return closest;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawWireSphere(transform.position, radius);

        if(manipulateables != null)
            foreach (Rigidbody2D r in manipulateables)
                Gizmos.DrawWireSphere(r.position, 1);

    }

}
