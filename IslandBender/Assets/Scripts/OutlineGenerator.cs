using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Source:
// https://www.h3xed.com/programming/create-2d-mesh-outline-in-unity-silhouette
// https://www.youtube.com/watch?v=8AKhr3c4HOs


// OutlineGenerator attaches an GameObject with a linerenderer, to an object with any 2DCollider.
public class OutlineGenerator : MonoBehaviour
{
    public float lineWidth = 0.1f;
    public Material material;
    public bool defaultFlag = false;
    float vertexResolution = 0.2f;

    [HideInInspector]
    public LineRenderer line;
    [HideInInspector]
    public Vector2[] localVertices;
    [HideInInspector]
    public Vector2[] worldVertices;
    [HideInInspector]
    public float lineLength;


    private void Start()
    {
        //Attach outline to current GameObnject
        localVertices = GetVertices(GetComponent<Collider2D>());
        worldVertices = LocalToWorldVerts(localVertices, transform);
        worldVertices = IncreaseOutlineResolution(worldVertices, vertexResolution);
        localVertices = WorldToLocalVerts(worldVertices,transform);
        lineLength = CalculateLineLength(worldVertices);
        line = GenerateOutline(localVertices, transform, lineWidth, material).GetComponent<LineRenderer>();
        SetActive(defaultFlag);
    }

    public static float CalculateLineLength(Vector2[] vertices)
    {
        float length = 0;
        for(int i = 0; i < vertices.Length; i++)
        {
            length += Vector2.Distance(vertices[i], vertices[(i + 1) % vertices.Length]);
        }
        return length;
    }

    public float GetLocalPos(Vector2 pos)
    {
        return GetPos(pos, worldVertices) / lineLength;
    }


    public float GetPos(Vector2 pos, Vector2[] vertices)
    {
        float length = 0;
        float tolerance = 1f;
        Debug.DrawLine(pos, pos + Vector2.up);
        for (int i = 0; i < vertices.Length; i++)
        {
            // Vert0 -----> pos ------> Vert1
            Vector2 vert0 = vertices[i];
            Vector2 vert1 = vertices[(i + 1) % vertices.Length];
            Debug.DrawLine(vert0, vert1, Color.red);
            float deltaVerts = Vector2.Distance(vert0, vert1);
            float deltaVert0Pos = Vector2.Distance(vert0, pos);
            float deltaVert1Pos = Vector2.Distance(pos, vert1);
            if(deltaVert0Pos + deltaVert1Pos <= deltaVerts + tolerance)
            {
                length += deltaVert0Pos;
                return length;
            }
            else
            {
                length += deltaVerts;
            }
        }
        return -1;
    }

    public void Toggle()
    {
        if (line)
            line.enabled = !line.enabled;
    }
    public void SetActive(bool flag)
    {
        if (line)
            line.enabled = flag;
    }

    public static GameObject GenerateOutline(GameObject geometry, float lineWidth, Material material)
    {
        Vector2[] vertices = GetVertices(geometry.GetComponent<Collider2D>());
        print("Vertices " + vertices.Length);
        return GenerateOutline(vertices, geometry.transform, lineWidth, material);
    }

    public static GameObject GenerateOutline(Vector2[] vertices, Transform parent, float lineWidth, Material material)
    {
        // Create line prefab
        LineRenderer linePrefab = new GameObject().AddComponent<LineRenderer>();
        linePrefab.transform.name = "Line";
        linePrefab.useWorldSpace = false;
        linePrefab.positionCount = 0;
        linePrefab.material = material;
        linePrefab.loop = true;
        linePrefab.widthMultiplier = lineWidth;
        linePrefab.startWidth = linePrefab.endWidth = lineWidth;

        // Create first line
        LineRenderer line = Instantiate(linePrefab.gameObject, parent).GetComponent<LineRenderer>();
        //line.transform.parent = parent;
        print(line.transform.parent.name);

        // This vector3 gets added to each line position, so it sits in front of the mesh
        // Change the -0.1f to a positive number and it will sit behind the mesh
        Vector3 bringFoward = new Vector3(0f, 0f, -0.1f);

        //2DVertices to 3DPoints
        Vector3[] points = new Vector3[vertices.Length];
        for(int i = 0; i < vertices.Length; i++)
        {
            points[i] =  ((Vector3) vertices[i]) + bringFoward;
        }

        //Fill points in line
        line.positionCount = vertices.Length;
        line.SetPositions(points);

        return line.gameObject;
    }

    //Returns vertices of any 2DCollider
    public static Vector2[] GetVertices(Collider2D collider)
    {
        List<Vector2> vertices = new List<Vector2>();
    
        //EdgeCollider2D
        if (collider is EdgeCollider2D)
        {
            EdgeCollider2D edgeCollider = (EdgeCollider2D)collider;
            vertices = new List<Vector2>(edgeCollider.points);
        }

        //PolygonCollider2D
        if (collider is PolygonCollider2D)
        {
            PolygonCollider2D polygonCollider = (PolygonCollider2D)collider;
            vertices = new List<Vector2>(polygonCollider.points);
        }

        //BoxCollider2D
        if (collider is BoxCollider2D)
        {
            BoxCollider2D boxCollider = (BoxCollider2D)collider;

            float top = boxCollider.offset.y + (boxCollider.size.y / 2f);
            float btm = boxCollider.offset.y - (boxCollider.size.y / 2f);
            float left = boxCollider.offset.x - (boxCollider.size.x / 2f);
            float right = boxCollider.offset.x + (boxCollider.size.x / 2f);

            Vector3 topLeft = new Vector3(left, top, 0f);
            Vector3 topRight = new Vector3(right, top, 0f);
            Vector3 btmLeft = new Vector3(left, btm, 0f);
            Vector3 btmRight = new Vector3(right, btm, 0f);

            vertices.Add(topLeft);
            vertices.Add(topRight);
            vertices.Add(btmRight);
            vertices.Add(btmLeft);
        }

        //CircleCollider2D
        if (collider is CircleCollider2D)
        {
            CircleCollider2D circleCollider = (CircleCollider2D)collider;
            Vector2 center = circleCollider.bounds.center;
            float radius = circleCollider.radius;
            int shapeCount = circleCollider.shapeCount;
            float angleStep = 360f / shapeCount;
            Quaternion quaternion = Quaternion.Euler(0.0f, 0.0f, angleStep);

            for (int i = 0; i < shapeCount; i++)
            {
                float x = radius * Mathf.Sin((2 * Mathf.PI * i) / shapeCount);
                float y = radius * Mathf.Cos((2 * Mathf.PI * i) / shapeCount);
                Vector2 vertex = center + new Vector2(x, y);
                vertices.Add(vertex);
            }
        }
        

        return vertices.ToArray();
    }

    public static Vector2[] IncreaseOutlineResolution(Vector2[] vertices, float resolution)
    {
        List<Vector2> betterVertecies = new List<Vector2>();
        for (int i = 0; i < vertices.Length; i++)
        {
            //Left and right vertex
            Vector2 vert0 = vertices[i];
            Vector2 vert1 = vertices[(i + 1) % vertices.Length];
            //Add left vertex
            betterVertecies.Add(vert0);
            //Skip if distance is better than resolution
            float distance = Vector2.Distance(vert0, vert1);
            if (distance < resolution)
                continue;
            //Increase resolution
            int steps = (int)(distance / resolution);
            float stepSize = distance / (steps + 2);
            Vector2 dir = (vert1 - vert0).normalized;
            for (int n = 1; n <= steps; n++)
            {
                Vector2 newVert = vert0 + dir * stepSize * n;
                betterVertecies.Add(newVert);
            }


        }

        return betterVertecies.ToArray();
    }

    public static Vector2[] LocalToWorldVerts(Vector2[] localVertices, Transform transform)
    {
        Vector2[] worldVertices = new Vector2[localVertices.Length];
        for(int i = 0; i < localVertices.Length; i++)
        {
            worldVertices[i] = transform.TransformPoint(localVertices[i]);
        }
        return worldVertices;
    }
    public static Vector2[] WorldToLocalVerts(Vector2[] worldVertices, Transform transform)
    {
        Vector2[] localVertices = new Vector2[worldVertices.Length];
        for (int i = 0; i < worldVertices.Length; i++)
        {
            localVertices[i] = transform.InverseTransformPoint(worldVertices[i]);
        }
        return localVertices;
    }

}
