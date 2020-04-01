using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(OutlineGenerator))]
public class Manipulable : MonoBehaviour
{
    public Rigidbody2D rigid;
    public OutlineGenerator outline;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        outline = GetComponent<OutlineGenerator>();
    }

    public void Select(Color color)
    {
        if (outline)
        {
            outline.SetActive(true);
            outline.line.startColor = color;
            outline.line.endColor = color;
        }
    }
    public void UnSelect()
    {
        if (outline)
            outline.SetActive(false);
    }
}
