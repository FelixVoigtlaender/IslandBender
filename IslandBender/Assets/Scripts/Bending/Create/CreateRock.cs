using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Create))]
[RequireComponent(typeof(PlayerController))]
public class CreateRock : MonoBehaviour
{
    public float jumpHeight;
    public GameObject rockPrefab;

    float createSpeed;
    Create create;
    PlayerController player;
    // Start is called before the first frame update
    void Start()
    {
        create = GetComponent<Create>();
        player = GetComponent<PlayerController>();
        player.controls.Player.Rock.performed += ctx => MakeRock();


        float gravity = -Physics2D.gravity.y;
        createSpeed = Mathf.Sqrt(2 * Mathf.Abs(gravity) * jumpHeight);
    }

    public void MakeRock()
    {
        if (!create.canCreate)
            return;

        //Angle
        var dir = create.createDir.normalized;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        //Create
        GameObject rock = Instantiate(rockPrefab, create.createPosition, Quaternion.Euler(0,0,angle));
        Rigidbody2D rigid = rock.GetComponent<Rigidbody2D>();
        rigid.velocity = dir * createSpeed;

        //Effects
        EffectManager.CreateRockCreateEffect(create.createPosition, Vector2.up);
        CameraController.Shake(0.3f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.position,Vector3.one * jumpHeight * 2);
    }
}
