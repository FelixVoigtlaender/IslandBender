using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Manipulate))]
public class Hit : MonoBehaviour
{
    public float speed  =20;
    public float maxMass = 10;
    Manipulate manipulate;
    PlayerController player;
    private void Start()
    {
        manipulate = GetComponent<Manipulate>();
        player = GetComponent<PlayerController>();

        player.controls.Player.Hit.performed += ctx => Perform();
    }
    void Perform()
    {
        if (!manipulate.isManipulating)
            return;

        Vector2 dir = player.lastPotentAim;
        Rigidbody2D otherRigid = manipulate.FindManipulateable(dir);

        if (!otherRigid)
            return;

        Vector2 velocity = otherRigid.velocity;
        velocity += dir * speed;
        otherRigid.velocity = velocity;

    }
    /*
    void Perform()
    {
        if (!manipulate.canManipulate)
            return;
        if (!manipulate.manipulateCollider)
            return;

        Rigidbody2D rigid = manipulate.manipulateCollider.attachedRigidbody;
        if (!rigid)
            return;
        if (rigid.mass > maxMass)
            return;

        Vector2 aim = manipulate.lastPotentAim;
        Vector2 velocity = rigid.velocity;
        velocity += aim * speed;
        rigid.velocity = velocity;
        
    }
    */

}
