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
        //Setups
        rigidbody = GetComponent<Rigidbody2D>();
        this.originateCollider = originateCollider;
        Physics2D.IgnoreCollision(originateCollider, GetComponent<Collider2D>());
        transform.parent = originateCollider.transform;
        //Positions
        Vector2 up = dir.normalized;
        startPos = transform.position - (Vector3)up * transform.localScale.y / 2;
        endPos = transform.position + (Vector3)up * transform.localScale.y / 2;
        transform.position = startPos;
        //Times
        startTime = Time.time;
        endTime = startTime + (startPos - endPos).magnitude / speed;
        //To Localspace
        startPos = transform.parent.InverseTransformPoint(startPos);
        endPos = transform.parent.InverseTransformPoint(endPos);
    }

    private void FixedUpdate()
    {
        float percent = (Time.time - startTime) / (endTime - startTime);
        percent = Mathf.Clamp01(percent);
        Vector2 position = percent * (endPos - startPos);
        
        //To Worldspace
        if (transform.parent)
        {
            position = transform.parent.TransformPoint(position);
            rigidbody.MovePosition(position);
            //transform.position = position;
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if (transform.parent)
        {

            Gizmos.DrawLine(transform.parent.TransformPoint(startPos), transform.parent.TransformPoint(endPos));
        }
    }
}
