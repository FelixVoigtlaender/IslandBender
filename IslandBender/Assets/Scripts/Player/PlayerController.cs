using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [Header("Input")]
    public InputMaster controls;
    public float deadZone = 0.2f;
    Vector2 lastPotentAim;
    Vector2 aim;
    Vector2 mov;
    Player player;
    public Rigidbody2D myRigid;


    private void Awake()
    {
        controls = new InputMaster();

        //Jump
        controls.Player.Jump.performed += ctx => JumpStart();
        controls.Player.Jump.started += ctx => JumpStart();
        controls.Player.Jump.canceled += ctx => JumpStop();
    }

    private void Start()
    {
        myRigid = GetComponent<Rigidbody2D>();
        player = GetComponent<Player>();
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
        input = input.magnitude < deadZone ? Vector2.zero : input;
        player.SetDirectionalInput(input);
    }
    public void JumpStart()
    {
        player.OnJumpInputDown();
    }

    public void JumpStop()
    {
        player.OnJumpInputUp();
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