using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(OutlineGenerator))]
public class Creatable : MonoBehaviour
{
    public float width = 1;
    OutlineGenerator outline;

    [HideInInspector]
    public Collider2D collider;
    float nextTest = 0;
    
    private void Start()
    {
        outline = GetComponent<OutlineGenerator>();
        collider = GetComponent<Collider2D>();
    }
    public void Select(Vector2 pos, Color color)
    {
        //Positions
        float middle = outline.GetLocalPos(pos);
        float sideRadius = (width / 2) / outline.lineLength;
        float min = (middle - sideRadius);
        min = min < 0 ? min + 1 : min;
        float max = (middle + sideRadius);
        max = max > 1 ? max - 1 : max;

        //No middle found (middle is negative)
        if (middle < 0)
        {
           outline.line.enabled = false;
            return;
        }


        //Gradient Setup
        Gradient gradient = new Gradient();

        //Color
        GradientColorKey[] colorKeys = new GradientColorKey[2];
        colorKeys[0].color = color;
        colorKeys[0].time = 0.0f;
        colorKeys[1].color = color;
        colorKeys[1].time = 1.0f;

        //Alpha
        //Alpha Key count
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[5];
        //Left
        float leftDistance = Clock01Distance(0, middle);
        alphaKeys[0].time = 0;
        alphaKeys[0].alpha = leftDistance > sideRadius ? 0 : 1 - leftDistance/sideRadius;
        //Right
        float rightDistance = Clock01Distance(1, middle);
        alphaKeys[1].time = 1;
        alphaKeys[1].alpha = rightDistance > sideRadius ? 0 : 1 - rightDistance / sideRadius;
        //Min
        alphaKeys[2].time = min;
        alphaKeys[2].alpha = 0;
        //Max
        alphaKeys[3].time = max;
        alphaKeys[3].alpha = 0;
        //Min
        alphaKeys[4].time = middle;
        alphaKeys[4].alpha = 1;

        gradient.SetKeys(colorKeys, alphaKeys);

        outline.line.startColor = color;
        outline.line.endColor = color;
        outline.line.colorGradient = gradient;

        outline.line.enabled = true;
    }
    public void UnSelect()
    {
        outline.line.enabled = false;
    }

    float Clock01Distance(float start, float end)
    {
        float simpleDist = Mathf.Abs(start - end);

        float toLeft = start < end ? start : end;
        float toRight = start > end ? 1 - start : 1-end;
        float complexDist = toLeft + toRight;

        return Mathf.Min(simpleDist, complexDist);
    }
}
