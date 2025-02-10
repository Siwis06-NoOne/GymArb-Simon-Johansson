using UnityEngine;
using Rigidbody = UnityEngine.Rigidbody;


public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed;
    [SerializeField] float walkSpeed;
    [SerializeField] float sprintSpeed;

    [SerializeField] float groundDrag;

    [Header("Jumping")]
    [SerializeField] float jumpForce;
    [SerializeField] float jumpCooldown;
    [SerializeField] float airMultiplier;
    [SerializeField] bool readyToJump;


    [Header("Keybinds")]
    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    [SerializeField] KeyCode sprintKey = KeyCode.LeftShift;

    [Header("Ground Chek")]
    [SerializeField] float playerHeight;
    [SerializeField] LayerMask whatIsGround;
    [SerializeField] bool grounded;

    [SerializeField] Transform oriantation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody myRb;

    public MovementState state;

    public enum MovementState
    {
        walking,
        sprinting,
        air,
    }

    private void Start()
    {
        myRb = GetComponent<Rigidbody>();
        myRb.freezeRotation = true;
        readyToJump = true;
    }
    private void Update()
    {
        // Ground Check
        RaycastHit hit; // Lägg till denna rad för att definiera hit
        bool hitSomething = Physics.Raycast(transform.position + Vector3.up * 0.3f, Vector3.down, out hit, playerHeight * 0.5f + 0.4f, whatIsGround);

        grounded = hitSomething; // Uppdatera grounded baserat på raycast-resultatet

        Debug.DrawRay(transform.position + Vector3.up * 0.3f, Vector3.down * (playerHeight * 0.5f + 0.4f), Color.red);

        if (hitSomething)
        {
            Debug.Log("Ground detected: " + hit.collider.gameObject.name);
        }
        else
        {
            Debug.Log("No ground detected");
        }

        // Handle drag
        if (grounded)
        {
            myRb.linearDamping = groundDrag;
        }
        else
        {
            myRb.linearDamping = 0f;
        }

        MyInput();
        SpeedControl();
        StateHandeler();
    }
    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // when to jump
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        } 
    }

    private void StateHandeler()
    {
        // Mode - sprinting
        if (grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }
        // Mode - wlaking
        else if (grounded)
        {
            state |= MovementState.walking;
            moveSpeed = walkSpeed;
        }
        // Mode - air
        else
        {
            state = MovementState.air;
        }
    }

    private void MovePlayer()
    {
        // Calculate movement direction
        moveDirection = oriantation.forward * verticalInput + oriantation.right * horizontalInput;

        // on ground
        if(grounded)
            myRb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // in air
        else if(!grounded)
            myRb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(myRb.linearVelocity.x, 0f, myRb.linearVelocity.z);

        // Limit velocity if needed
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitVel = flatVel.normalized * moveSpeed;
            myRb.linearVelocity = new Vector3(limitVel.x, myRb.linearVelocity.y, limitVel.z);
        }
    }

    private void Jump()
    {
        // reset Y velocity
        myRb.linearVelocity = new Vector3(myRb.linearVelocity.x, 0f, myRb.linearVelocity.z);

        myRb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;
    } 
}
