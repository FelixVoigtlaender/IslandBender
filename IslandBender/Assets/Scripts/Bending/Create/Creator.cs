using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Create))]
[RequireComponent(typeof(PlayerController))]
public class Creator : MonoBehaviour
{
    protected Create create;
    protected PlayerController player;
    void Awake()
    {
        create = GetComponent<Create>();
        player = GetComponent<PlayerController>();
    }

    public virtual void TryPerform()
    {
        if (create.canCreate && create.creatable)
            Perform();
    }

    public virtual void Perform()
    {

    }
}
