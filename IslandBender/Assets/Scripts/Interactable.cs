using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public Rigidbody2D rigidbody;
    PlayerController selector;
    SpriteRenderer renderer;
    private void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
        rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!selector)
            return;
        Debug.DrawLine(transform.position, selector.transform.position);
    }

    public void Select(PlayerController selector)
    {
        //Already Occupied
        if (this.selector && this.selector != selector)
            return;

        // Select
        this.selector = selector;
    }
    public void Unselect()
    {
        this.selector = null;
    }



    
}
