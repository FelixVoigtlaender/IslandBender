using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class PlayerController : MonoBehaviour
{

    [Header("Input")]
    public float deadZone = 0.2f;
    Vector2 lastPotentAim;
    Vector2 aim;
    Vector2 mov;

    [HideInInspector]
    public Rigidbody2D myRigid;
    [HideInInspector]
    public InputMaster controls;
    [HideInInspector]
    public Controller2D controller;


    private void Awake()
    {
        controls = new InputMaster();

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
        bool isManipulating = controls.Player.Manipulate.phase == UnityEngine.InputSystem.InputActionPhase.Performed;
        bool isCreating = controls.Player.Create.phase == UnityEngine.InputSystem.InputActionPhase.Performed;
        bool isBending = isManipulating || isCreating;

        //Input
        mov = controls.Player.Movement.ReadValue<Vector2>();
        aim = controls.Player.Aim.ReadValue<Vector2>();

        Move(mov);


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
}