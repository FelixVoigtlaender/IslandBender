using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyParticleSystem : MonoBehaviour
{
    private void Start()
    {
        Destroy(this.gameObject, 3f);
    }
}
