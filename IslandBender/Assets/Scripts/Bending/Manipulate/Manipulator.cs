using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Manipulator : MonoBehaviour
{
    protected Manipulate manipulate;
    protected PlayerController player;

    public virtual void Awake()
    {
        manipulate = GetComponent<Manipulate>();
        player = GetComponent<PlayerController>();
    }

    public void TryPerform()
    {
        if (manipulate.isManipulating)
            Perform();
    }

    public virtual void Perform()
    {

    }
}
