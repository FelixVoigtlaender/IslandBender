using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class PlayerController : MonoBehaviour
{
    public static Color debugColor = Color.blue;
    [HideInInspector]
    public Color color = Color.black;

    [Header("Input")]
    public float deadZone = 0.2f;
    public Vector2 lastPotentAim;
    public Vector2 aim;
    public Vector2 mov;

    [HideInInspector]
    public Rigidbody2D myRigid;
    [HideInInspector]
    public InputMaster controls;
    [HideInInspector]
    public Controller2D controller;
    [HideInInspector]
    public Create create;
    [HideInInspector]
    public Manipulate manipulate;



    private void Awake()
    {
        controls = new InputMaster();

        create = GetComponent<Create>();
        manipulate = GetComponent<Manipulate>();

        color = GetComponent<SpriteRenderer>().color;

        print("START");
        //Jump
        controls.Player.Jump.performed += ctx => JumpStart();
        controls.Player.Jump.started += ctx => JumpStart();
        controls.Player.Jump.canceled += ctx => JumpStop();
    }

    private void Start()
    {
        myRigid = GetComponent<Rigidbody2D>();
        controller = GetComponent<Controller2D>();
    }


    public void Update()
    {
        //Bending
        bool isBending = create.isCreating || manipulate.isManipulating;

        //Input
        mov = controls.Player.Movement.ReadValue<Vector2>();
        aim = controls.Player.Aim.ReadValue<Vector2>();
        if (aim.magnitude > deadZone)
            lastPotentAim = aim;

        //Don't move once bending
        //Vector2 movInput = isBending ? Vector2.zero : mov;

        Move(mov);

        //Test
        DebugExtension.DrawShape(ShapeExtension.CreateCirclePoints(transform.position, 2.5f, 20));

    }
    public void Move(Vector2 input)
    {
        if (input.magnitude < deadZone)
            input = Vector2.zero;

        controller.SetDirectionalInput(input);
    }
    public void JumpStart()
    {
        controller.OnJumpStart();
    }

    public void JumpStop()
    {
        controller.OnJumpStop();
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
        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, lastPotentAim.normalized*10);
    }
}