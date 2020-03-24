using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Redirect : Manipulator
{
    public float maxMass = 2.5f;
    public float launchSpeed = 20;
    Rigidbody2D selection;

    private void Start()
    {
        player.controls.Player.Redirect.started += ctx => TryPerform();
        player.controls.Player.Redirect.performed += ctx => TryPerform();
        player.controls.Player.Redirect.canceled += ctx => Cancel();
    }

    private void Update()
    {
        //Nothing to do if there is no selection
        if (!selection)
            return;
        //Stopped manipulating... Let go
        if(!manipulate.isManipulating)
        {
            selection = null; //No Launch
            return;
        }

        Vector2 myPos = transform.position;
        Vector2 selPos = selection.position;

        //If out of reach... Let go
        float distance = Vector2.Distance(myPos,selPos);
        if (distance > manipulate.radius)
        {
            selection = null;
            return;
        }

        // Check if its getting closer or further?
        Vector2 nextMyPos = myPos + player.myRigid.velocity * Time.deltaTime;
        Vector2 nextSelPos = selPos + selection.velocity * Time.deltaTime;
        float nextdistance = Vector2.Distance(nextMyPos,nextSelPos);
        // If getting closer, do nothing
        if (nextdistance < distance)
            return;

        //Redirect
        Vector2 dir = (selPos - myPos).normalized;
        Vector2 tangent = Vector2.Perpendicular(dir).normalized;
        Vector2 velocity = selection.velocity;
        float tangentDir = Mathf.Sign(Vector2.Dot(tangent, velocity));

        velocity = tangent * tangentDir * velocity.magnitude;
        selection.velocity = velocity;

        Debug.DrawRay(selection.position, velocity);


    }


    public override void Perform()
    {
        if (selection)
            return;
        
        Vector2 dir = player.lastPotentAim;
        Rigidbody2D otherRigid = manipulate.FindManipulateable(dir, maxMass);
        selection = otherRigid;
    }
    public void Cancel()
    {
        if (!selection)
            return;

        Vector2 velocity = selection.velocity;
        if (velocity.magnitude < launchSpeed)
            velocity = velocity.normalized * launchSpeed;
        selection.velocity = velocity;
        
        EffectManager.CreateRockHitEffect(selection.position, velocity.normalized);

        selection = null;

    }

}
