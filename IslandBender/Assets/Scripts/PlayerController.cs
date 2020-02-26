using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour {

    [Header("Input")]
    public InputMaster controls;
    public float inputDeadZone = 0.1f;
    Vector2 lastPotentAim;
    Vector2 aim;
    Vector2 mov;
    [Header("Movement")]
    public float walkSpeed = 10;
    public float jumpSpeed = 10;
    public float deltaTimeSky = 0.5f;
    public float deltaTimeGround = 0.05f;
    public LayerMask obstacleLayer;
    public Rigidbody2D myRigid;
    Vector2 smoothVelocity;

    [Header("Jumps")]
    public int skyJumpsLeft = 0;
    public float lastJump;
    public float jumpReload = 0.1f;
    public int maxSkyJumps = 1;


    private void Awake()
    {
        controls = new InputMaster();
        controls.Player.Jump.performed += ctx => Jump();
    }

    private void Start()
    {
        myRigid = GetComponent<Rigidbody2D>();
    }


    public void Update()
    {
        //SkyJumps Reset
        skyJumpsLeft = IsOnGround() ? maxSkyJumps : skyJumpsLeft;
        //Input
        aim = controls.Player.Aim.ReadValue<Vector2>();
        lastPotentAim = aim.magnitude > 0.1f ? aim : lastPotentAim;
        mov = controls.Player.Movement.ReadValue<Vector2>();

        //Bending
        bool isManipulating = controls.Player.Manipulate.phase == UnityEngine.InputSystem.InputActionPhase.Performed;
        bool isCreating = controls.Player.Create.phase == UnityEngine.InputSystem.InputActionPhase.Performed;
        bool isBending = isManipulating || isCreating;

        //Movement
        Vector2 movement = mov;
        movement = isBending ? Vector2.zero : mov;
        Move(movement);
    }

    public void Move(Vector2 input)
    {
        if (input.magnitude < inputDeadZone)
            input = Vector2.zero;

        Vector2 velocity = myRigid.velocity;
        float goalSpeed = input.x * walkSpeed;
        float smoothTime = IsOnGround() ? deltaTimeGround : deltaTimeSky;
        velocity.x = Mathf.SmoothDamp(velocity.x, goalSpeed, ref smoothVelocity.x, smoothTime);

        //Stick on x>0 change
        if (Mathf.Sign(input.x) != Mathf.Sign(velocity.x) && Mathf.Sign(input.x)!=0)
            velocity.x = 0;

        myRigid.velocity = velocity;
    }

    public void Jump()
    {
        Vector2 velocity = myRigid.velocity;
        bool isOnGround = IsOnGround();

        if (Time.time - lastJump > jumpReload)
        {
            // GroundJump
            if (isOnGround)
            {
                velocity.y = jumpSpeed;
                lastJump = Time.time;
            }
            // SkyJump
            if (!isOnGround && skyJumpsLeft > 0)
            {
                velocity.y = jumpSpeed;
                skyJumpsLeft--;

                lastJump = Time.time;
            }
        }
        myRigid.velocity = velocity;
    }

    public bool IsOnGround()
    {
        int count = 3;
        float stepSize = transform.lossyScale.x / (count-1);
        Vector2 bottomLeft = transform.position - transform.lossyScale / 2;
        for(int i = 0; i < count; i++)
        {
            Vector2 position = bottomLeft + Vector2.right * stepSize*i;
            RaycastHit2D hit = Physics2D.Raycast(position, Vector2.down * 0.1f, 0.1f, obstacleLayer);
            Debug.DrawRay(position, Vector2.down);
            if (hit)
                return true;
        }
        //No Raycast found ground
        return false;
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}
