using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boulder : MonoBehaviour
{
    Rigidbody2D rigidbody;
    public float speed = 10f;
    Vector2 startPos;
    Vector2 endPos;
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        Vector2 up = transform.TransformDirection(Vector2.up).normalized;
        startPos = transform.position - (Vector3)up * transform.localScale.y / 2;
        endPos = transform.position + (Vector3)up * transform.localScale.y / 2;

        transform.position = startPos;
    }
    private void Start()
    {

        Collider2D mycollider = GetComponent<Collider2D>(); int numColliders = 10;
        Collider2D[] colliders = new Collider2D[numColliders];
        ContactFilter2D contactFilter = new ContactFilter2D();
        mycollider.OverlapCollider(contactFilter, colliders);
        foreach(Collider2D collider in colliders)
        {
            if (collider)
            {
                Physics2D.IgnoreCollision(mycollider, collider);
                transform.parent = collider.transform;
                break;
            }
        }


        //Physics.IgnoreCollision(bullet.GetComponent<Collider>(), GetComponent<Collider>());
    }

    


    private void Update()
    {
        float percent = ((Vector2)transform.position - startPos).magnitude / (endPos - startPos).magnitude;

        if (percent >= 1)
        {
            rigidbody.velocity = Vector2.zero;
            transform.position = endPos;
            return;
        }

        Vector2 up = transform.TransformDirection(Vector2.up).normalized;
        Vector2 velocity = up * speed;
        rigidbody.velocity = velocity;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(startPos, endPos);
    }
}
