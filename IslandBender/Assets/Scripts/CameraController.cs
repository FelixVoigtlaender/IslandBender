using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraController : MonoBehaviour {
    //Objects
    GameObject[] ballz;
    public static CameraController instance;
    
    [Header("Movement")]
    public bool lockX;
    public float horizontalSmoothTime;
    public float verticalSmoothTime;
    public float angleSmoothTime;
    public Vector2 offset;
    float smoothVelocityX;
    float smoothVelocityY;
    Vector3 middle;

    [Header("Size")]
    public bool lockSize;
    public float sizeSmoothTime;
    public float skinWidth;
    public float minimumSize;
    public float goalSize;
    float smoothVelocitySize;
    Vector2 sizeTest;

    [Header("Screen Shake")]
    public float shakeSmoothTime;
    public float maxRotation;
    float shakeMagnitude;
    float shakeVelocity;
    public float rotation;
    float rotationSmoothVelocity;

    [Header("Color")]
    public Color startColor;
    public Color endColor;
    public float distance;
    Color currentColor;

    //Color
    //Color standardColor;
    Stack<ColorChange> colors = new Stack<ColorChange>();

    //Camera
    Camera mainCamera;
    void Start()
    {
        instance = this;
        mainCamera = Camera.main;
        goalSize = mainCamera.orthographicSize;
        //newSize = newSize >= minimumSize ? newSize : minimumSize;

        currentColor = mainCamera.backgroundColor;
    }
    public void SetMiddle(Vector3 middle)
    {
        this.middle = middle;
    }
    public void SetPosition(Vector3 position)
    {
        middle = position - (Vector3)offset;
    }

    void LateUpdate()
    {
        Debug.DrawLine(middle, middle + Vector3.up * sizeTest.y);
        Debug.DrawLine(middle, middle + Vector3.right * sizeTest.x);

        Vector3 focusPosition = middle + (Vector3)offset;
        focusPosition.z = transform.position.z;
        focusPosition.x = Mathf.SmoothDamp(transform.position.x, focusPosition.x, ref smoothVelocityX, horizontalSmoothTime);
        focusPosition.x = lockX ? 0 : focusPosition.x;
        focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothVelocityY, verticalSmoothTime);
        transform.position = (Vector3)focusPosition;
        if (!lockSize)
        {
            float oSize = Mathf.SmoothDamp(Camera.main.orthographicSize, goalSize, ref smoothVelocitySize, sizeSmoothTime);
            mainCamera.orthographicSize = oSize > minimumSize ? oSize : minimumSize;
        }

        float rotation = Mathf.SmoothDampAngle(transform.rotation.z, 0, ref rotationSmoothVelocity, angleSmoothTime);
        //transform.rotation = Quaternion.Euler(0, 0, rotation);

        //boxCollider.size.x = camera.aspect * 2f * camera.orthographicSize;
        //boxCollider.size.y = 2f * camera.orthographicSize;

        ScreenShake();
    }
    void FixedUpdate()
    {
        //ManageColors();
    }

    public void SetObject(GameObject o) //Hacky as fuck
    {
        GameObject[] objects = { o };
        SetObjects(objects);
    }
    public void SetObjects(GameObject[] objects)
    {
        ballz = objects;
        List<Transform> points = new List<Transform>();
        foreach(GameObject ob in objects)
        {
            points.Add(ob.transform);
        }

        middle = FindCenter(points.ToArray());
        goalSize = ToOrthographicSize(FindMax(middle, points.ToArray()));
        middle.z = transform.position.z;
    }

    public float ToOrthographicSize(Vector2 size)
    {
        this.sizeTest = new Vector2(size.x,size.y);
        Debug.DrawLine(middle, middle + Vector3.up * size.y);
        float oSize = 0;
        size = new Vector2(size.x + skinWidth, size.y + skinWidth);
        size = size * 2;

        if (size.x > size.y * Camera.main.aspect)
        {
            size.y = size.x / Camera.main.aspect;
        }

        oSize = size.y/2;
        return oSize;
    }
    public static bool IsInView(Vector2 position, Vector2 xBounds , Vector2 yBounds)
    {
        Vector2 point = Camera.main.WorldToViewportPoint(position);
        bool inX = point.x > xBounds.x && point.x < xBounds.y;
        bool inY = point.y > yBounds.x && point.y < yBounds.y;

        return inX && inY;
    }

    Vector3 FindCenter(Transform[] points)
    {
        float mult = points.Length;
        Vector3 middle = Vector3.zero;
        float xMax=points[0].position.x, xMin = points[0].position.x, yMax = points[0].position.y, yMin = points[0].position.y;
        foreach(Transform point in points)
        {
            float x;
            x = point.position.x + point.lossyScale.x / 2;
            xMax = x > xMax ?x : xMax;
            x = point.position.x - point.lossyScale.x / 2;
            xMin = x < xMin ? x : xMin;

            float y;
            y = point.position.y + point.lossyScale.y / 2;
            yMax = y > yMax ? y : yMax;
            y = point.position.y - point.lossyScale.y / 2;
            yMin = y < yMin ? y : yMin;
        }

        middle = new Vector3((xMax + xMin) / 2, (yMax + yMin) / 2);
        return middle;
    }

    

    Vector2 FindMax(Vector3 center, Transform[] points)
    {
        Vector2 size = Vector2.zero;
        foreach (Transform point in points)
        {
            float xMax = point.position.x + point.lossyScale.x / 2;
            if (size.x < Mathf.Abs(xMax - center.x))
            {
                size.x = Mathf.Abs(xMax - center.x);
            }
            float xMin = point.position.x - point.lossyScale.x / 2;
            if (size.x < Mathf.Abs(xMin - center.x))
            {
                size.x = Mathf.Abs(xMin - center.x);
            }

            float yMax = point.position.y + point.lossyScale.y / 2;
            if (size.y < Mathf.Abs(yMax - center.y))
            {
                size.y = Mathf.Abs(yMax - center.y);
            }
            float yMin = point.position.y - point.lossyScale.y / 2;
            if (size.y < Mathf.Abs(yMin - center.y))
            {
                size.y = Mathf.Abs(yMin - center.y);
            }
        }
        return size;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(middle + Vector3.up, middle + Vector3.down);
        Gizmos.DrawLine(middle + Vector3.left, middle + Vector3.right);
    }

    //
    //Screenshake
    //
    void ScreenShake()
    {
        shakeMagnitude = Mathf.SmoothDamp(shakeMagnitude, 0,ref shakeVelocity, shakeSmoothTime);
        Vector3 position = transform.position;
        position += Random.Range(-1f, 1f) * shakeMagnitude * Vector3.up;
        position += Random.Range(-1f, 1f) * shakeMagnitude * Vector3.right;
        transform.Rotate(Vector3.forward * Random.Range(-1f, 1f) * shakeMagnitude * 10);
        transform.position = position;

        rotation = transform.rotation.z*Mathf.Rad2Deg;
        rotation += Random.Range(-1f, 1f) * shakeMagnitude * 10;
        rotation = rotation > maxRotation ? maxRotation : rotation;
        rotation = rotation < -maxRotation ? -maxRotation : rotation;
        transform.rotation = Quaternion.Euler(Vector3.forward * rotation);
    }
    public void Push(Vector2 push)
    {
        smoothVelocityX += push.x;
        smoothVelocityY += push.y;
    }
    public static void Shake(float magnitude)
    {
        instance.shakeMagnitude += magnitude;
    }
    //
    //Abilities
    //
    void ManageColors()     //colors ist ein Stack mit den structs ColorChange
    {
        float percent = 0.5f - Mathf.Cos(transform.position.y / distance);
        currentColor = Color.Lerp(startColor, endColor, percent);

        if (colors.Count > 0)
        {
            if (Time.time > colors.Peek().timeStamp)
            {
                colors.Pop();
                if (colors.Count > 0)
                {
                    mainCamera.backgroundColor = colors.Peek().color;
                }
                else
                {
                    ResetColor();
                }
            }
        }
        else
        {
            mainCamera.backgroundColor = currentColor;
        }
    }
    struct ColorChange
    {
        public float timeStamp { get; set; }
        public Color color { get; set; }
    }
    public void ChangeColor(Color color, float interval)
    {
        mainCamera.backgroundColor = color;
        ColorChange colorChange = new ColorChange();
        colorChange.color = color;
        colorChange.timeStamp = Time.time + interval;
        colors.Push(colorChange);
    }
    public void ResetColor()
    {
        mainCamera.backgroundColor = currentColor;

        print("Test");
    }
    
}
