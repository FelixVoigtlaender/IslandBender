﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CreateBoulder : Creator
{

    public float jumpHeight = 10;
    public GameObject boulderPrefab;

    float speed;
    // Start is called before the first frame update

    void Start()
    {

        float gravity = -Physics2D.gravity.y;
        speed = Mathf.Sqrt(2 * Mathf.Abs(gravity) * jumpHeight);

        player.controls.Player.Boulder.performed += ctx => TryPerform();
    }

    public override void Perform()
    {

        //Angle
        var dir = create.dir;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;

        //Create
        GameObject boulderObj = Instantiate(boulderPrefab, create.position, Quaternion.Euler(0, 0, angle));
        Boulder boulder = boulderObj.GetComponent<Boulder>();
        boulder.speed = speed;
        boulder.Setup(create.creatable.collider,dir);

        //Effects
        //EffectManager.CreateBoulderCreateEffect(create.createPosition, Vector2.up);
        CameraController.Shake(0.5f);
    }
}
