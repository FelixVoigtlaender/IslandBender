using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class OutsideCollider : MonoBehaviour
{
    Collider2D collider;
    int counter = 0;
    private void Awake()
    {
        collider = GetComponent<Collider2D>();
    }
    void OnTriggerEnter(Collider other)
    {
        counter++;
    }
    void OnTriggerExit(Collider other)
    {
        print("EXIT");
        counter--;
        if (counter <= 0)
        {
            collider.isTrigger = false;
            Destroy(this);
        }
    }
}
