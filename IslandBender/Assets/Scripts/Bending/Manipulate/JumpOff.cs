using UnityEngine;
using System.Collections;


[RequireComponent(typeof(Manipulate))]
public class JumpOff : MonoBehaviour
{
    public float jumpHeight = 5;
    float jumpSpeed;
    Manipulate manipulate;
    PlayerController player;

    private void Start()
    {
        manipulate = GetComponent<Manipulate>();
        player = GetComponent<PlayerController>();

        float gravity = -Physics2D.gravity.y;
        jumpSpeed = Mathf.Sqrt(2 * Mathf.Abs(gravity) * jumpHeight);

        player.controls.Player.Jump.performed += ctx => Perform();
    }

    public void Perform()
    {
        if (!manipulate.isManipulating)
            return;

        Vector2 dir = player.myRigid.velocity.normalized;
        Rigidbody2D otherRigid = manipulate.FindManipulateable(dir);

        if (!otherRigid)
            return;

        Vector2 aim = player.lastPotentAim.normalized;
        Vector2 deltaVel = aim * jumpSpeed;
        Bending.AddVelocity(player.myRigid, deltaVel);
        Bending.AddVelocity(otherRigid, -deltaVel);

        EffectManager.CreateRockHitEffect(transform.position, deltaVel);
        EffectManager.CreateRockHitEffect(otherRigid.position, -deltaVel);
    }
}
