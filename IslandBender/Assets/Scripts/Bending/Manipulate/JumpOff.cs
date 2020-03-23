using UnityEngine;
using System.Collections;


[RequireComponent(typeof(Manipulate))]
public class JumpOff : Manipulator
{
    public float jumpHeight = 5;
    float jumpSpeed;

    private void Start()
    {
        float gravity = -Physics2D.gravity.y;
        jumpSpeed = Mathf.Sqrt(2 * Mathf.Abs(gravity) * jumpHeight);

        player.controls.Player.Jump.performed += ctx => TryPerform();
    }

    public override void Perform()
    {
        if (!manipulate.isManipulating)
            return;

        Vector2 dir = player.myRigid.velocity.normalized;
        Rigidbody2D otherRigid = manipulate.FindManipulateable(dir);

        if (!otherRigid)
            return;

        Vector2 aim = player.lastPotentAim.normalized;
        Vector2 deltaVel = aim * jumpSpeed;
        player.myRigid.velocity = Vector2.zero;
        Bending.AddVelocity(player.myRigid, deltaVel);
        Bending.AddVelocity(otherRigid, -deltaVel);

        EffectManager.CreateRockHitEffect(transform.position, deltaVel);
        EffectManager.CreateRockHitEffect(otherRigid.position, -deltaVel);
    }
}
