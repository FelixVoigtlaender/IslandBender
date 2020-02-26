using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    Rigidbody2D rigidbody;
    public float speed = 5f;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();

        //Add Velocity up
        Vector2 velocity = rigidbody.velocity;
        Vector2 up = transform.TransformDirection(Vector2.up).normalized;
        velocity += up * speed;
        rigidbody.velocity = velocity;
    }
}
