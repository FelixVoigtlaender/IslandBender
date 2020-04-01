using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateJumpBoulder : Creator
{

    public float jumpHeight = 5;
    public GameObject boulderPrefab;

    float speed;
    // Start is called before the first frame update

    void Start()
    {

        float gravity = -Physics2D.gravity.y;
        speed = Mathf.Sqrt(2 * Mathf.Abs(gravity) * jumpHeight);

        player.controls.Player.JumpBoulder.performed += ctx => TryPerform();
    }

    public override void Perform()
    {
        //Where to put the boulder
        Vector2 rayDir = Vector2.down;
        if (!player.controller.collisions.below)
        {
            if(player.controller.collisions.left)
                rayDir = Vector2.left;
            if (player.controller.collisions.right)
                rayDir = Vector2.right;
        }

        //Setup Create
        create.Creating(rayDir);
        create.position = create.goalPosition; //Should be instant

        //Angle
        var dir = create.dir;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;

        //Create
        GameObject boulderObj = Instantiate(boulderPrefab, create.position, Quaternion.Euler(0, 0, angle));
        Boulder boulder = boulderObj.GetComponent<Boulder>();
        boulder.speed = speed;
        boulder.Setup(create.creatable.collider, dir);

        //Effects
        //EffectManager.CreateBoulderCreateEffect(create.createPosition, Vector2.up);
        CameraController.Shake(0.5f);
    }
}
