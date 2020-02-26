using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Create))]
[RequireComponent(typeof(PlayerController))]
public class CreateRock : MonoBehaviour
{
    public float createSpeed;
    public GameObject rockPrefab;


    Create create;
    PlayerController player;
    // Start is called before the first frame update
    void Start()
    {
        create = GetComponent<Create>();
        player = GetComponent<PlayerController>();

        player.controls.Player.Rock.performed += ctx => MakeRock();
    }

    public void MakeRock()
    {
        if (!create.canCreate)
            return;

        //Angle
        var dir = create.createDir;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        //Create
        GameObject rock = Instantiate(rockPrefab, create.createPosition, Quaternion.Euler(0,0,angle));
        Rigidbody2D rigid = rock.GetComponent<Rigidbody2D>();
        rigid.velocity = dir * createSpeed;

        //Effects
        EffectManager.CreateRockCreateEffect(create.createPosition, Vector2.up);
        CameraController.Shake(0.3f);
    }
}
