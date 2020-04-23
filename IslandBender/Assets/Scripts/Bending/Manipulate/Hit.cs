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

        Vector2 dir = player.lastPotentAim.normalized;
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.Lerp(Manipulate.debugColor, Color.black, 0.3f);

        Vector2 start = (Vector2)transform.position + Manipulate.debugOffset * 3;
        Vector2 velocity = (Manipulate.debugOffset + Vector2.up).normalized * speed;
        Vector2 end = GizmosExtension.DrawParabel(start, velocity, Physics2D.gravity.y);

        //Block start
        Gizmos.DrawWireCube(start, Vector3.one);
        //Trail
        Gizmos.DrawLine(start, end);
        //Block end
        Gizmos.DrawWireCube(end, Vector3.one);
    }

}
