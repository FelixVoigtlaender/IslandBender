using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class RaycastController : MonoBehaviour
{

    public LayerMask collisionMask;

    public const float skinWidth = .015f;
    const float dstBetweenRays = .25f;
    [HideInInspector]
    public int horizontalRayCount;
    [HideInInspector]
    public int verticalRayCount;

    [HideInInspector]
    public float horizontalRaySpacing;
    [HideInInspector]
    public float verticalRaySpacing;

    [HideInInspector]
    public BoxCollider2D collider;
    public RaycastOrigins raycastOrigins;
    public CollisionInfo collisions;

    public virtual void Awake()
    {
        collider = GetComponent<BoxCollider2D>();
    }

    public virtual void Start()
    {
        CalculateRaySpacing();
        collisions = new CollisionInfo();
    }

    public void UpdateRaycastOrigins()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    public void UpdateCollisions(Vector2 velocity)
    {
        collisions.Reset();

        //Edges
        collisions.topEdges = EdgeCollisions(velocity, 1);
        collisions.bottomEdges = EdgeCollisions(velocity, -1);

        //Vertical
        collisions.bottomWhiskers = VerticalCollisions(velocity, -1);
        collisions.topWhiskers = VerticalCollisions(velocity, 1);
        collisions.below = HasFlag(collisions.bottomWhiskers);
        collisions.above = HasFlag(collisions.topWhiskers);

        //Horizontal
        collisions.leftWhiskers = HorizontalCollisions(velocity, -1);
        collisions.rightWhiskers = HorizontalCollisions(velocity, 1);
        collisions.left = HasFlag(collisions.leftWhiskers);
        collisions.right = HasFlag(collisions.rightWhiskers);


    }

    public void CalculateRaySpacing()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        float boundsWidth = bounds.size.x;
        float boundsHeight = bounds.size.y;

        horizontalRayCount = Mathf.RoundToInt(boundsHeight / dstBetweenRays);
        verticalRayCount = Mathf.RoundToInt(boundsWidth / dstBetweenRays);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    public bool HasFlag(bool[] array, bool flag = true)
    {
        foreach (bool b in array)
            if (b == flag)
                return true;
        return false;
    }

    public int FlagCount(bool[] array, bool flag = true)
    {
        int count = 0;
        foreach (bool b in array)
            if (b == flag)
                count++;
        return count;
    }

    bool[] HorizontalCollisions(Vector2 velocity, float directionX = 0)
    {
        UpdateRaycastOrigins();
        if (directionX == 0)
            directionX = Mathf.Sign(velocity.x);

        Vector2 moveAmount = velocity * Time.deltaTime;
        float rayLength = 0.1f + skinWidth;

        bool[] whiskers = new bool[horizontalRayCount];

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);


            Color debugColor = Color.red;
            if (hit)
            {
                whiskers[i] = true;
                debugColor = Color.green;
            }
            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, debugColor);
        }

        return whiskers;
    }

    bool[] EdgeCollisions(Vector2 velocity, float directionY = 0)
    {
        UpdateRaycastOrigins();
        if (directionY == 0)
            directionY = Mathf.Sign(velocity.y);

        Vector2 moveAmount = velocity * Time.deltaTime;
        float rayLength = 0.1f + skinWidth;

        bool[] whiskers = new bool[2];

        for (int i = 0; i < 2; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (horizontalRaySpacing * (horizontalRayCount-1) * i);
            float xDir = i == 0 ? -1 : 1;
            Vector2 dir = Vector2.up * directionY + Vector2.right * xDir;
            dir = dir.normalized;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, dir, rayLength, collisionMask);


            Color debugColor = Color.red;

            if (hit)
            {
                whiskers[i] = true;

                if (directionY < 0)
                {
                    float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                    collisions.slopeAngle = slopeAngle;
                    collisions.slopeNormal = hit.normal;
                }

                debugColor = Color.green;
            }
            Debug.DrawRay(rayOrigin, dir * rayLength, debugColor);
        }

        return whiskers;
    }

    public bool[] VerticalCollisions(Vector2 velocity, float directionY = 0)
    {
        UpdateRaycastOrigins();
        if (directionY == 0)
            directionY = Mathf.Sign(velocity.y);

        Vector2 moveAmount = Time.deltaTime * velocity;
        float rayLength = 0.1f + skinWidth;

        bool[] whiskers = new bool[verticalRayCount];

        for (int i = 0; i < verticalRayCount; i++)
        {

            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + moveAmount.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);


            Color debugColor = Color.red;

            if (hit)
            {
                if (directionY < 0)
                {
                    float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                    collisions.slopeAngle = slopeAngle;
                    collisions.slopeNormal = hit.normal;
                }

                whiskers[i] = true;
                debugColor = Color.green;
            }


            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, debugColor);
        }

        return whiskers;
    }

    public struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        public bool[] topWhiskers;
        public bool[] bottomWhiskers;
        public bool[] leftWhiskers;
        public bool[] rightWhiskers;

        public bool[] topEdges;
        public bool[] bottomEdges;

        public float slopeAngle;
        public Vector2 slopeNormal;

        public void Reset()
        {
            above = below = false;
            left = right = false;
        }
    }
}