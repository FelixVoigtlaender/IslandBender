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

        Manipulable manipulable = manipulate.FindManipulateable(dir);

        if (!manipulable)
            return;

        Rigidbody2D otherRigid = manipulable.rigid;
        if (otherRigid && (otherRigid.mass <= maxMass || otherRigid.bodyType == RigidbodyType2D.Kinematic))
            otherRigid.gameObject.SendMessage("Break");
    }
}
