using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateRock : Creator
{
    public float jumpHeight;
    public GameObject rockPrefab;

    float createSpeed;

    // Start is called before the first frame update
    void Start()
    {
        player.controls.Player.Rock.performed += ctx => TryPerform();

        createSpeed = Controller2D.GetJumpSpeed(jumpHeight);
    }

    public override void Perform()
    {

        //Angle
        var dir = create.spawnDir.normalized;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        //Create
        GameObject rock = Instantiate(rockPrefab, create.spawnPosition, Quaternion.Euler(0,0,angle));
        Rigidbody2D rigid = rock.GetComponent<Rigidbody2D>();
        rigid.velocity = dir * createSpeed;

        //Effects
        EffectManager.CreateRockCreateEffect(create.spawnPosition, dir);
        CameraController.Shake(0.3f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.Lerp(Create.debugColor, Color.black, 0.3f);

        Vector2 start = (Vector2)transform.position + Create.debugOffset*4;
        Vector2 velocity = Controller2D.GetJumpSpeed(jumpHeight) * Vector2.up + Create.debugOffset * 2f;
        Vector2 end = GizmosExtension.DrawParabel(start, velocity, Physics2D.gravity.y);

        //Block start
        Gizmos.DrawWireCube(start, Vector3.one);
        //Block end
        Gizmos.DrawWireCube(end, Vector3.one);
    }
}
