using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(Create))]
[RequireComponent(typeof(PlayerController))]
public class CreateBoulder : MonoBehaviour
{

    public float createSpeed;
    public GameObject boulderPrefab;


    Create create;
    PlayerController player;

    // Start is called before the first frame update
    void Start()
    {
        create = GetComponent<Create>();
        player = GetComponent<PlayerController>();

        player.controls.Player.Boulder.performed += ctx => MakeBoulder();
    }

    public void MakeBoulder()
    {
        if (!create.canCreate)
            return;

        //Angle
        var dir = create.createDir;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;

        //Create
        GameObject boulderObj = Instantiate(boulderPrefab, create.createPosition, Quaternion.Euler(0, 0, angle));
        Boulder boulder = boulderObj.GetComponent<Boulder>();
        boulder.Setup(create.createCollider,dir);

        //Effects
        //EffectManager.CreateBoulderCreateEffect(create.createPosition, Vector2.up);
        CameraController.Shake(0.5f);
    }
}
