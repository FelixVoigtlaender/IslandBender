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
    }
}
