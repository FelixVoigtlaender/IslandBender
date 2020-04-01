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
        Manipulable manipulable = manipulate.FindManipulateable(dir);

        if (!manipulable)
            return;

        Rigidbody2D otherRigid = manipulable.rigid;

        otherRigid.SendMessage("Hit", SendMessageOptions.DontRequireReceiver);
        Vector2 velocity = otherRigid.velocity;
        velocity += dir * speed;
        otherRigid.velocity = velocity;

        EffectManager.CreateRockHitEffect(otherRigid.position, dir);
    } 

}
