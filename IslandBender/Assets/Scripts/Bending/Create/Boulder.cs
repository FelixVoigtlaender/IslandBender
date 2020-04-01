using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boulder : MonoBehaviour
{
    Rigidbody2D rigidbody;
    public float speed = 10f;
    Vector2 startPos;
    Vector2 endPos;
    float startTime, endTime;
    Collider2D originateCollider;

    public void Setup(Collider2D originateCollider, Vector2 dir)
    {
        //Positions
        Vector2 up = dir.normalized;
        startPos = transform.position - (Vector3)up * transform.localScale.y / 2;
        endPos = transform.position + (Vector3)up * transform.localScale.y / 2;
        transform.position = startPos;
        //Setups
        rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.bodyType = RigidbodyType2D.Kinematic;
        this.originateCollider = originateCollider;
        Physics2D.IgnoreCollision(originateCollider, GetComponent<Collider2D>());
        transform.parent = originateCollider.transform;
        //Times
        startTime = Time.time;
        endTime = startTime + (startPos - endPos).magnitude / speed;
        //To Localspace
        startPos = transform.parent.InverseTransformPoint(startPos);
        endPos = transform.parent.InverseTransformPoint(endPos);
    }

    private void FixedUpdate()
    {
        if (!transform.parent)
            return;

        float percent = (Time.time - startTime) / (endTime - startTime);
        percent = Mathf.Clamp01(percent);
        Vector2 position = percent * (endPos - startPos) + startPos;

        position = transform.parent.TransformPoint(position);
        //transform.position = position;
        rigidbody.MovePosition(position);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (transform.parent)
        {
            Gizmos.DrawLine(transform.parent.TransformPoint(startPos), transform.parent.TransformPoint(endPos));
            Gizmos.DrawWireSphere(transform.parent.TransformPoint(endPos), 0.1f);
        }
    }

    public void Hit()
    {
        Physics2D.IgnoreCollision(originateCollider, GetComponent<Collider2D>(), false);
        if (transform.parent)
            transform.parent = null;
        rigidbody.bodyType = RigidbodyType2D.Dynamic;
    }
}
