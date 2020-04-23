using UnityEngine;
using System.Collections;


[RequireComponent(typeof(Manipulate))]
public class JumpOff : Manipulator
{
    public float jumpHeight = 5;
    float jumpSpeed;

    private void Start()
    {
        jumpSpeed = Controller2D.GetJumpSpeed(jumpHeight);   

        player.controls.Player.Jump.performed += ctx => TryPerform();
    }
    
    public override void Perform()
    {
        if (!manipulate.isManipulating)
            return;

        Vector2 dir = player.myRigid.velocity.normalized;
        Manipulable manipulable = manipulate.FindManipulateable(dir);

        if (!manipulable)
            return;

        Rigidbody2D otherRigid = manipulable.rigid;

        Vector2 aim = player.lastPotentAim.normalized;
        Vector2 deltaVel = aim * jumpSpeed;
        player.myRigid.velocity = Vector2.zero;
        Bending.AddVelocity(player.myRigid, deltaVel);
        Bending.AddVelocity(otherRigid, -deltaVel);

        EffectManager.CreateRockHitEffect(transform.position, deltaVel);
        EffectManager.CreateRockHitEffect(otherRigid.position, -deltaVel);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.Lerp(Manipulate.debugColor, Color.black, 0.3f);

        Vector2 start = (Vector2)transform.position + Manipulate.debugOffset * 2f;
        Vector2 velocity = Controller2D.GetJumpSpeed(jumpHeight) * Vector2.up + Manipulate.debugOffset * 2f;
        Vector2 end = GizmosExtension.DrawParabel(start, velocity, Physics2D.gravity.y);

        //Block
        Vector2 startBlock = start - velocity.normalized * 2;
        Gizmos.DrawLine(startBlock, startBlock - velocity);
        Gizmos.DrawWireCube(startBlock, Vector3.one);
        //Player
        Gizmos.color = PlayerController.debugColor;
        //Player start
        Gizmos.DrawWireCube(start, Vector3.one);
        //Player end
        Gizmos.DrawWireCube(end, Vector3.one);

    }
}
