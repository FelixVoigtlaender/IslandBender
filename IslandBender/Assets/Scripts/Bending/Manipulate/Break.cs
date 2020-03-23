using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Break : Manipulator
{
    public float maxMass = 1.5f;
    private void Start()
    {
        player.controls.Player.Break.performed += ctx => TryPerform();
    }

    public override void Perform()
    {
        Vector2 dir = player.lastPotentAim;
        Rigidbody2D otherRigid = manipulate.FindManipulateable(dir);
        if(otherRigid && otherRigid.mass <= maxMass)
            otherRigid.gameObject.SendMessage("Break");
    }
}
