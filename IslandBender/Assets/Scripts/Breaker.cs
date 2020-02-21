using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breaker : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.relativeVelocity.magnitude < 10)
            return;
        //if(collision.)
        foreach(ContactPoint2D contact in collision.contacts)
        {
            float dotProd = Vector2.Dot(collision.relativeVelocity, contact.normal);
            if (dotProd < 10)
                return;
        }


        float otherMass = 100;
        if(collision.rigidbody)
            otherMass = collision.rigidbody.mass;

        float myMass = 100;
        if (collision.otherRigidbody)
            myMass = collision.otherRigidbody.mass;


        if (otherMass == myMass)
        {
            Breaker otherBreaker = collision.gameObject.GetComponent<Breaker>();
            if (otherBreaker)
                otherBreaker.Break();
        }
        if (myMass <= otherMass)
        {
            print("myMass:" + myMass + " otherMass:" + otherMass);
            Break();
        }
    }
    public void Break()
    {
        Destroy(gameObject);
    }
}
