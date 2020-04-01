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


        float gravity = -Physics2D.gravity.y;
        createSpeed = Mathf.Sqrt(2 * Mathf.Abs(gravity) * jumpHeight);
    }

    public override void Perform()
    {

        //Angle
        var dir = create.dir.normalized;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        //Create
        GameObject rock = Instantiate(rockPrefab, create.position, Quaternion.Euler(0,0,angle));
        Rigidbody2D rigid = rock.GetComponent<Rigidbody2D>();
        rigid.velocity = dir * createSpeed;

        //Effects
        EffectManager.CreateRockCreateEffect(create.position, dir);
        CameraController.Shake(0.3f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.position,Vector3.one * jumpHeight * 2);
    }
}
