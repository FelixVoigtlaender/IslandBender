using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager instance;

    public GameObject[] rockCreateEffectPrefabs;
    public GameObject[] rockHitEffectPrefabs;
    private void Awake()
    {
        instance = this;
    }

    public static void CreateRockCreateEffect(Vector2 position, Vector2 dir)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        GameObject effectPrefab = instance.rockCreateEffectPrefabs[Random.Range(0, instance.rockCreateEffectPrefabs.Length - 1)];

        GameObject effect = Instantiate(effectPrefab, position, Quaternion.Euler(0, 0, 0));

        effect.GetComponent<ParticleSystem>().startRotation = angle -90;

    }

    public static void CreateRockHitEffect(Vector2 position, Vector2 dir)
    {
        dir = dir.normalized;
        float angle = Mathf.Atan2(-dir.y, dir.x) ;
        GameObject effectPrefab = instance.rockHitEffectPrefabs[Random.Range(0, instance.rockHitEffectPrefabs.Length - 1)];

        GameObject effect = Instantiate(effectPrefab, position, Quaternion.Euler(0, 0, 0));
        effect.GetComponent<ParticleSystem>().startRotation = angle;

    }


}
