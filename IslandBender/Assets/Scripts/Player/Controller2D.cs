using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Controller2D : RaycastController
{
    [Header("Movement")]
    public float maxJumpHeight = 4;
    public float minJumpHeight = 1;
    public float timeToJumpApex = .4f;
    float accelerationTimeAirborne = .2f;
    float accelerationTimeGrounded = .1f;
    float moveSpeed = 6;

    public Vector2 wallJumpClimb = new Vector2(7.5f, 16);
    public Vector2 wallJumpOff = new Vector2(8.5f, 7);
    public Vector2 wallLeap = new Vector2(18,17);

    public float wallSlideSpeedMax = 3;
    public float wallStickTime = .25f;
    float timeToWallUnstick;
    
    float maxJumpVelocity;
    float minJumpVelocity;
    float velocityXSmoothing;

    public float jumpBufferTime;
    float jumpBuffer;
    bool isJumpBuffer;

    public float coyoteTime = 0.2f;
    float lastGroundedTime;

    public float minJumpCornerCorrectionSpeed = 2;

    Vector2 directionalInput;
    bool wallSliding;
    public float maxSlopeAngle = 55;
    int wallDirX;

    Rigidbody2D rigid;

    public override void Start()
    {
        base.Start();
        rigid = GetComponent<Rigidbody2D>();

        //Jump Velocities
        float gravity = -Physics2D.gravity.y;
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
    }
    public void Update()
    {
        UpdateCollisions(rigid.velocity);

        HandleWallSliding();
        HandleJumpBuffer();
        CheckJumpCornerCorrection();
        HandleMovement();
        HandleCoyoteTime();
        //Wall Sliding
    }
    //Input
    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;
    }
    //Jumps
    public void OnJumpStart()
    {
        Vector2 velocity = rigid.velocity;
        if (wallSliding)
        {
            if (Mathf.Sign(wallDirX) == Mathf.Sign(directionalInput.x))
            {
                velocity.x = -wallDirX * wallJumpClimb.x;
                velocity.y = wallJumpClimb.y;

                print("WallJumpClimb");
            }
            else if (directionalInput.x <= 0.1f)
            {
                velocity.x = -wallDirX * wallJumpOff.x;
                velocity.y = wallJumpOff.y;
                
                print("wallJumpOff");
            }
            else
            {
                velocity.x = -wallDirX * wallLeap.x;
                velocity.y = wallLeap.y;

                print("wallLeap");
            }
        }else 
        if (collisions.below || HasFlag(collisions.bottomEdges) || Time.time - lastGroundedTime < coyoteTime)
        {
            if (collisions.slopeAngle > maxSlopeAngle)
            {
                velocity.y = maxJumpVelocity * collisions.slopeNormal.y;
                velocity.x = maxJumpVelocity * collisions.slopeNormal.x;


                print("SlopeJump");
                /*if (Mathf.Sign(directionalInput.x) != -Mathf.Sign(collisions.slopeNormal.x))
                { // not jumping against max slope
                    velocity.y = maxJumpVelocity * collisions.slopeNormal.y;
                    velocity.x = maxJumpVelocity * collisions.slopeNormal.x;
                }*/
            }
            else
            {
                velocity.y = maxJumpVelocity;
            }
        }
        else
        {
            //If Player prematurely tries to jump
            jumpBuffer = Time.unscaledTime;
            isJumpBuffer = true;
        }
        rigid.velocity = velocity;
    }
    public void OnJumpStop()
    {
        Vector2 velocity = rigid.velocity;
        if (velocity.y > minJumpVelocity && velocity.y <= maxJumpVelocity)
        {
            velocity.y = minJumpVelocity;
        }
        rigid.velocity = velocity;
    }

    //Movement
    void HandleWallSliding()
    {
        Vector2 velocity = rigid.velocity;
        wallDirX = collisions.leftWhiskers[horizontalRayCount - 1] ? -1 : 1;
        wallSliding = false;
        int leftRightWhiskerCount = FlagCount(collisions.leftWhiskers) + FlagCount(collisions.rightWhiskers);
        if (leftRightWhiskerCount >=2 && !collisions.below && velocity.y < 0)
        {
            wallSliding = true;

            if (velocity.y < -wallSlideSpeedMax)
            {
                velocity.y = -wallSlideSpeedMax;
            }

            if (timeToWallUnstick > 0)
            {
                velocityXSmoothing = 0;
                //velocity.x = 0;

                if (Mathf.Sign(directionalInput.x) != Mathf.Sign(wallDirX) && directionalInput.x != 0)
                {
                    timeToWallUnstick -= Time.deltaTime;
                }
                else
                {
                    timeToWallUnstick = wallStickTime;
                }
            }
            else
            {
                timeToWallUnstick = wallStickTime;
            }

        }
        rigid.velocity = velocity;
    }
    public void CheckJumpCornerCorrection()
    {
        Vector2 velocity = rigid.velocity;
        if (velocity.y < 2)
            return;
        if (FlagCount(collisions.topWhiskers) > 1)
            return;
        if (collisions.left || collisions.right)
            return;

        float rayLength = 0.1f + skinWidth;
        float minSlip = skinWidth + 0.02f;
        //Right Slip
        if (collisions.topWhiskers[0] || collisions.topEdges[0])
        {
            Vector2 rayOrigin = raycastOrigins.topLeft + Vector2.right * verticalRaySpacing + Vector2.up * rayLength;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.left, verticalRaySpacing, collisionMask);

            Color debugColor = Color.red;
            if (hit)
            {
                float slipDistance = minSlip + (verticalRaySpacing - hit.distance);
                rigid.MovePosition(rigid.position + Vector2.right * (slipDistance));
                debugColor = Color.green;
            }
            else
            {
                rigid.MovePosition(rigid.position + Vector2.right * minSlip);
            }
            Debug.DrawRay(rayOrigin, Vector2.left * verticalRaySpacing, debugColor);
        }
            
        //Left Slip
        if (collisions.topWhiskers[verticalRayCount-1] || collisions.topEdges[1])
        {
            Vector2 rayOrigin = raycastOrigins.topRight + Vector2.left * verticalRaySpacing + Vector2.up * rayLength;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right, verticalRaySpacing, collisionMask);

            Color debugColor = Color.red;
            if (hit)
            {
                float slipDistance = minSlip + (verticalRaySpacing - hit.distance);
                rigid.MovePosition(rigid.position + Vector2.left * (slipDistance));
                debugColor = Color.green;
            }
            else
            {
                rigid.MovePosition(rigid.position + Vector2.left * minSlip);
            }
            Debug.DrawRay(rayOrigin, Vector2.right * verticalRaySpacing, debugColor);
        }
    }
    public void HandleJumpBuffer()
    {
        if (!isJumpBuffer)
            return;
        if (Time.unscaledTime - jumpBuffer > jumpBufferTime)
            return;

        if(wallSliding || collisions.below || HasFlag(collisions.bottomEdges))
        {
            isJumpBuffer = false;
            OnJumpStart();
        }
    }
    public void HandleMovement()
    {
        Debug.DrawRay(Vector2.one, collisions.slopeNormal, Color.green);

        Vector2 velocity = rigid.velocity;
        float xDir = Mathf.Sign(velocity.x);
        float xDirTarget = Mathf.Sign(directionalInput.x);

        float targetSpeed = moveSpeed;

        if (!collisions.below || wallSliding || velocity.y > Mathf.Abs(velocity.x))
        {
            //In Air, wallsliding or jumping.
            velocity.x = Mathf.SmoothDamp(velocity.x, targetSpeed * directionalInput.x, ref velocityXSmoothing, accelerationTimeAirborne);
        }
        else
        {
            //Sliding
            if (collisions.slopeAngle > maxSlopeAngle)
                return;


            //Grounded / Ascending / Descending
            // normalVelocity= velocity parallel to normal of slope
            // parallelVelocity = velocity parallel to slope
            // lostSpeed = Speed lost when landing on ground (Vertical movement is removed)
            Vector2 normalVelocity = Vector2.Dot(collisions.slopeNormal.normalized, velocity) * collisions.slopeNormal.normalized;
            Vector2 parallelVelocity = velocity - normalVelocity;
            float lostSpeed = normalVelocity.magnitude;
            float parallelSpeed = parallelVelocity.magnitude;
            velocity = parallelVelocity;

            // newSpeed = Speed parallel to ground, (neg/pos) for going left&right 
            float newSpeed = Mathf.SmoothDamp(parallelSpeed * xDir, targetSpeed * directionalInput.x, ref velocityXSmoothing, accelerationTimeGrounded);

            //Landing roll. Keep some of the lostSpeed by doing a landing roll
            newSpeed += lostSpeed * directionalInput.x;
            float maxSpeedMagnitude = 2;
            newSpeed = Mathf.Clamp(newSpeed, -targetSpeed* maxSpeedMagnitude, targetSpeed * maxSpeedMagnitude);

            // newSpeed is transfered to velocity, which is parallel to ground
            float yDir = Mathf.Sign(newSpeed) == Mathf.Sign(collisions.slopeNormal.x) ? -1 : 1;
            velocity.x = Mathf.Cos(collisions.slopeAngle * Mathf.Deg2Rad) * newSpeed;
            velocity.y = Mathf.Sin(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(newSpeed) * yDir;
        }
        rigid.velocity = velocity;
    }
    public void HandleCoyoteTime()
    {
        if (collisions.below)
            lastGroundedTime = Time.time;
    }
}
