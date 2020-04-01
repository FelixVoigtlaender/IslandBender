using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Redirect : Manipulator
{
    public float maxMass = 2.5f;
    public float launchSpeed = 20;
    Manipulable selection;

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
        Vector2 selPos = selection.rigid.position;

        //If out of reach... Let go
        float distance = Vector2.Distance(myPos,selPos);
        if (distance > manipulate.radius)
        {
            selection = null;
            return;
        }

        // Check if its getting closer or further?
        Vector2 nextMyPos = myPos + player.myRigid.velocity * Time.deltaTime;
        Vector2 nextSelPos = selPos + selection.rigid.velocity * Time.deltaTime;
        float nextdistance = Vector2.Distance(nextMyPos,nextSelPos);
        // If getting closer, do nothing
        if (nextdistance < distance)
            return;

        //Redirect
        Vector2 dir = (selPos - myPos).normalized;
        Vector2 tangent = Vector2.Perpendicular(dir).normalized;
        Vector2 velocity = selection.rigid.velocity;
        float tangentDir = Mathf.Sign(Vector2.Dot(tangent, velocity));

        velocity = tangent * tangentDir * velocity.magnitude;
        selection.rigid.velocity = velocity;

        Debug.DrawRay(selection.rigid.position, velocity);


    }


    public override void Perform()
    {
        if (selection)
            return;
        
        Vector2 dir = player.lastPotentAim;
        selection = manipulate.FindManipulateable(dir, maxMass);
    }
    public void Cancel()
    {
        if (!selection)
            return;

        Vector2 velocity = selection.rigid.velocity;
        if (velocity.magnitude < launchSpeed)
            velocity = velocity.normalized * launchSpeed;
        selection.rigid.velocity = velocity;
        
        EffectManager.CreateRockHitEffect(selection.rigid.position, velocity.normalized);

        selection = null;

    }

}
