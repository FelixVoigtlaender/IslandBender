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
        player.controls.Player.Hit.started += ctx => TryPerform();
        player.controls.Player.Hit.canceled += ctx => TryPerform();
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
        //selPos = SelectedPosition
        Vector2 selPos = selection.position;
        float distance = (selPos - myPos).magnitude;

        //If out of reach... Let go
        if(distance > manipulate.radius)
        {
            selection = null;
            return;
        }

        //
        //Actual Redirection
        //
        Vector2 dir = (selPos - myPos).normalized;


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

    }

}
