using UnityEngine;
using System.Collections;


[RequireComponent(typeof(Manipulate))]
public class JumpOff : MonoBehaviour
{
    public float speed = 15;
    Manipulate manipulate;
    PlayerController player;

    private void Start()
    {
        manipulate = GetComponent<Manipulate>();
        player = GetComponent<PlayerController>();

        player.controls.Player.Jump.performed += ctx => Perform();
    }

    public void Perform()
    {
        if (!manipulate.canManipulate)
            return;
        if (!manipulate.manipulateCollider)
            return;

        Rigidbody2D rigid = manipulate.manipulateCollider.attachedRigidbody;
        if (!rigid)
            return;

        Vector2 aim = manipulate.lastPotentAim;
        Vector2 deltaVel = aim * speed;
        //Bending.AddVelocity(player.myRigid, deltaVel);
        Bending.AddVelocity(rigid, -deltaVel);

        EffectManager.CreateRockHitEffect(transform.position, deltaVel);
        EffectManager.CreateRockHitEffect(rigid.position, -deltaVel);
    }
}
