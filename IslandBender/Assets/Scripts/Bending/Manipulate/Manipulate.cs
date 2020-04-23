using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class Manipulate : MonoBehaviour
{
    public static Color debugColor = Color.cyan;
    public static Vector2 debugOffset = Vector2.right;

    public float radius = 5;

    public LayerMask layerMask;
    public bool isManipulating;

    PlayerController player;

    public Manipulable[] manipulables;
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
        foreach (Manipulable m in manipulables)
            m.UnSelect();

        manipulables = FindManipulables();

        foreach (Manipulable m in manipulables)
            m.Select(PlayerController.debugColor);
    }

    //Finds all Manipulables (Rigidbody2D) in its manipulateRadius.
    public Manipulable[] FindManipulables()
    {
        if (!isManipulating)
            return new Manipulable[0];

        List<Manipulable> manipulables = new List<Manipulable>();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, layerMask);
        foreach(Collider2D c in colliders)
        {
            if (c.attachedRigidbody)
            {
                Manipulable manipulable = c.GetComponent<Manipulable>();
                if (manipulable)
                    manipulables.Add(manipulable);
            }
                    
        }

        return manipulables.ToArray();
    }

    //Finds closest manipulables relative to an direction
    public Manipulable FindManipulateable(Vector2 dir, float maxMass = 1000)
    {
        if (!isManipulating)
            return null;

        Vector2 myPosition = (Vector2)transform.position + dir * radius;

        Manipulable closest = null;
        float closestDistance = radius*2 + 10;
        foreach (Manipulable m in manipulables)
        {
            float distance = ((Vector2)myPosition - m.rigid.position).magnitude;
            distance += ((Vector2)transform.position - m.rigid.position).magnitude * 2;
            if (distance < closestDistance && m.rigid.mass < maxMass)
            {
                closest = m;
                closestDistance = distance;
            }
        }
        return closest;
    }

    public Manipulable FindClosestManipulable(float maxMass = 1000)
    {
        if (!isManipulating)
            return null;

        Manipulable closest = null;
        float closestDistance = radius+10;
        foreach(Manipulable m in manipulables)
        {
            float distance = ((Vector2)transform.position - m.rigid.position).magnitude;
            if(distance < closestDistance && m.rigid.mass < maxMass)
            {
                closest = m;
                closestDistance = distance;
            }
        }
        return closest;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Manipulate.debugColor;
        Gizmos.DrawWireSphere(transform.position, radius);

        if(manipulables != null)
            foreach (Manipulable m in manipulables)
                if(m)
                    Gizmos.DrawWireSphere(m.rigid.position, 0.1f);

    }

}
