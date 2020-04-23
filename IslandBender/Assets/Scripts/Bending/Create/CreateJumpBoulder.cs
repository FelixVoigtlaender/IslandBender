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
        speed = Controller2D.GetJumpSpeed(jumpHeight);

        player.controls.Player.JumpBoulder.performed += ctx => TryPerform();
    }

    public override void TryPerform()
    {
        if (create.isCreating)
            Perform();
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
        if (!create.creatable)
            return;

        //Stick spawnGoalPos to spawnPos
        create.spawnPosition = create.spawnGoalPosition;

        //Angle
        var dir = create.spawnDir;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;

        //Create
        GameObject boulderObj = Instantiate(boulderPrefab, create.spawnPosition, Quaternion.Euler(0, 0, angle));
        Boulder boulder = boulderObj.GetComponent<Boulder>();
        boulder.speed = speed;
        boulder.Setup(create.creatable.collider, dir);

        //Effects
        //EffectManager.CreateBoulderCreateEffect(create.createPosition, Vector2.up);
        CameraController.Shake(0.5f);
    }

    private void OnDrawGizmosSelected()
    {
        //Darker Green
        Gizmos.color = Color.Lerp(Create.debugColor, Color.black, 0.3f);
        Vector2 spawnPos = (Vector2)transform.position + Create.debugOffset * 0;

        

        //Boulder
        Gizmos.DrawWireCube(spawnPos + Vector2.up * (1f), new Vector3(1, 2, 1)); 
        //Trail
        Gizmos.DrawLine(spawnPos, spawnPos + Vector2.up * jumpHeight);
        //Player
        Gizmos.color = PlayerController.debugColor;
        Gizmos.DrawWireCube(spawnPos + Vector2.up * (jumpHeight + 0.5f), new Vector3(1, 1, 1));
    }
}
