using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Magnetized : MonoBehaviour
{
    public float pullStrength = 1;
    public float killDistance = 3;
    Rigidbody2D rigidbody;
    Vector3 startPostion;
    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        startPostion = transform.position;
    }

    private void FixedUpdate()
    {
        Vector2 deltaPos = transform.position - startPostion;
        if (deltaPos.magnitude > killDistance)
        {
            Destroy(this);
            return;
        }

        // F = -k*u
        Vector2 force = -pullStrength * deltaPos;
        rigidbody.AddForce(force*rigidbody.mass);


    }
}
