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
        CalculateVelocity();
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
            }
            else if (directionalInput.x <= 0.1f)
            {
                velocity.x = -wallDirX * wallJumpOff.x;
                velocity.y = wallJumpOff.y;
            }
            else
            {
                velocity.x = -wallDirX * wallLeap.x;
                velocity.y = wallLeap.y;
            }
        }else 
        if (collisions.below || HasFlag(collisions.bottomEdges))
        {
            if (collisions.slopeAngle > maxSlopeAngle)
            {
                velocity.y = maxJumpVelocity * collisions.slopeNormal.y;
                velocity.x = maxJumpVelocity * collisions.slopeNormal.x;

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
        if (velocity.y > minJumpVelocity)
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
        if ((collisions.leftWhiskers[horizontalRayCount-1] || collisions.rightWhiskers[horizontalRayCount - 1]) && !collisions.below && velocity.y < 0)
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
        float minSlip = 0.1f;
        //Right Slip
        if (collisions.topWhiskers[0] || collisions.topEdges[0])
        {
            Vector2 rayOrigin = raycastOrigins.topLeft + Vector2.right * verticalRaySpacing + Vector2.up * rayLength;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.left , verticalRaySpacing, collisionMask);
            if (hit)
            {
                rigid.MovePosition(rigid.position + Vector2.right * (minSlip + hit.distance));
            }
            else
            {
                rigid.MovePosition(rigid.position + Vector2.right * minSlip);
            }
        }
            
        //Left Slip
        if (collisions.topWhiskers[verticalRayCount-1] || collisions.topEdges[1])
        {
            Vector2 rayOrigin = raycastOrigins.topRight + Vector2.left * verticalRaySpacing + Vector2.up * rayLength;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right, verticalRaySpacing, collisionMask);
            if (hit)
            {
                rigid.MovePosition(rigid.position + Vector2.left * (minSlip + hit.distance));
            }
            else
            {
                rigid.MovePosition(rigid.position + Vector2.left * minSlip);
            }
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
    public void CalculateVelocity()
    {
        Vector2 velocity = rigid.velocity;
        float targetVelocityX = directionalInput.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, collisions.below ? accelerationTimeGrounded : accelerationTimeAirborne);
        rigid.velocity = velocity;
    }

}
