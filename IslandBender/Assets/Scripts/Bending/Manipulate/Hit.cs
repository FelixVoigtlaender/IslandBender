using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Manipulate))]
public class Hit : Manipulator
{
    public float speed = 20;
    public float maxMass = 10;

    private void Start()
    {
        player.controls.Player.Hit.performed += ctx => TryPerform();
    }
    public override void Perform()
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

}
