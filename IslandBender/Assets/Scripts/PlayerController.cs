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
    bool isCreating;
    bool isInteracting;
    bool isBending;
    [Header("Movement")]
    public float walkSpeed = 10;
    public float jumpSpeed = 10;
    public float deltaTimeSky = 0.5f;
    public float deltaTimeGround = 0.05f;
    public LayerMask obstacleLayer;
    Rigidbody2D myRigid;
    Vector2 smoothVelocity;

    [Header("Jumps")]
    public int skyJumpsLeft = 0;
    public float lastJump;
    public float jumpReload = 0.1f;
    public int maxSkyJumps = 1;

    [Header("Bending")]
    public float bendingRadius = 4;
    public float hitSpeed = 20;
    public float createSpeed = 5;
    [Range(0,1f)]
    public float creationRange = 0.5f;
    public GameObject rockPrefab;
    public GameObject boulderPrefab;
    Vector2 createPosition = Vector2.zero;
    Interactable interactable;


    private void Awake()
    {
        controls = new InputMaster();
        controls.Player.Jump.performed += ctx => Jump();
        controls.Player.Rock.performed += ctx => CreateRock();
        controls.Player.Boulder.performed += ctx => CreateBoulder();
        controls.Player.Hit.performed += ctx => Hit();
    }

    private void Start()
    {
        myRigid = GetComponent<Rigidbody2D>();
    }

    public void OnDamage()
    {
        //playerInput.Vibrate(0.5f,0.3f);
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
        isCreating = controls.Player.Create.phase == UnityEngine.InputSystem.InputActionPhase.Performed;
        isInteracting = controls.Player.Interact.phase == UnityEngine.InputSystem.InputActionPhase.Performed;
        isBending = isCreating || isInteracting;
        //Create
        if (isCreating)
            Create(lastPotentAim);
        //Interact
        if (isInteracting)
            Interact(aim);
        HasInteractable();

        //Movement
        Vector2 movement = mov;
        movement = isBending ? Vector2.zero : movement;
        Move(movement);
    }
    //
    //Bending
    //
    public void Create(Vector2 input)
    {
        //Check for Spawnplace
        Vector2 rayPostion = (Vector2)transform.position + Mathf.Sign(input.x) * bendingRadius * Vector2.right*creationRange;
        RaycastHit2D hit = Physics2D.Raycast(rayPostion, Vector2.down,10,obstacleLayer);
        if (!hit)
            return;
        //Draw Spawn place
        createPosition = hit.point;
        Debug.DrawLine(createPosition, createPosition + Vector2.up);
    }

    public void CreateRock()
    {
        if (!isCreating)
            return;

        GameObject rock = Instantiate(rockPrefab, createPosition, Quaternion.Euler(0, 0, 0));

        CameraController.Shake(0.3f);
    }
    public void CreateBoulder()
    {
        if (!isCreating)
            return;

        GameObject boulder = Instantiate(boulderPrefab, createPosition, Quaternion.Euler(0, 0, 0));

        CameraController.Shake(0.4f);
    }

    public void Interact(Vector2 input)
    {
        FindInteractable(input);
        if(interactable)
            Debug.DrawLine((Vector2)interactable.transform.position + input.normalized*hitSpeed*0.1f, interactable.transform.position);
    }

    bool HasInteractable()
    {
        if (!interactable)
            return false;
        if((interactable.transform.position - transform.position).magnitude > bendingRadius)
        {
            interactable.Unselect();
            return false;
        }
        return true;
    }

    public void FindInteractable(Vector2 input)
    {
        //Check for colliders in Circle
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, bendingRadius, input, 0, obstacleLayer);
        if (!hit)
            return;
        //Check for interact
        Interactable interact = hit.transform.gameObject.GetComponent<Interactable>();
        if (!interact)
            return;
        //Select
        if (interactable)
            interactable.Unselect();
        interact.Select(this);
    }

    public void Hit()
    {
        if (!IsOnGround())
            return;
        if (!interactable)
            FindInteractable(lastPotentAim);
        if (!interactable || isCreating)
            return;
        if ((interactable.transform.position - transform.position).magnitude > bendingRadius)
        {
            interactable.Unselect();
            return;
        }

        interactable.rigidbody.bodyType = RigidbodyType2D.Dynamic;
        Vector2 input = aim;
        Vector2 velocity = interactable.rigidbody.velocity;
        velocity += input.normalized * hitSpeed;
        interactable.rigidbody.velocity = velocity;

        interactable.Unselect();

        CameraController.Shake(0.5f);
    }

    public void SetSelection(Interactable interactable)
    {
        this.interactable = interactable;
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

    private void OnDrawGizmos()
    {
        Color color = Color.black;
        if (isCreating)
            color += Color.green;
        if (isInteracting)
            color += Color.red;
        Gizmos.color = color;

        Gizmos.DrawWireSphere(transform.position, bendingRadius);
        Gizmos.DrawWireSphere(transform.position, bendingRadius*creationRange);
        if (interactable)
        {
            Gizmos.DrawWireCube(transform.position, Vector3.one);
        }
    }
}
