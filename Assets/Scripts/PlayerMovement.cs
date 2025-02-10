using NUnit.Framework.Constraints;
using UnityEngine;
using Rigidbody = UnityEngine.Rigidbody;


public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] float walkSpeed;
    [SerializeField] float sprintSpeed;

    [SerializeField] float groundDrag;

    [Header("Jumping")]
    [SerializeField] float jumpForce;
    [SerializeField] float jumpCooldown;
    [SerializeField] float airMultiplier;
    [SerializeField] bool readyToJump;

    [Header("Crouching")]
    [SerializeField] float crouchSpeed;
    [SerializeField] float crouchYScale;
    [SerializeField] private float startYScale;


    [Header("Keybinds")]
    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    [SerializeField] KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground Chek")]
    [SerializeField] float playerHeight;
    [SerializeField] LayerMask whatIsGround;
    [SerializeField] bool grounded;

    [Header("Slope Handeling")]
    [SerializeField] float maxSlopeAngle;
    [SerializeField] private RaycastHit slopeHit;
    [SerializeField] private bool exitingSlope;

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
        crouching,
        air,
    }

    private void Start()
    {
        myRb = GetComponent<Rigidbody>();
        myRb.freezeRotation = true;
        readyToJump = true;
        startYScale = transform.localScale.y;
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

        // start crouch
        if (Input.GetKey(crouchKey) && grounded)
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            myRb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }
        // stop crouch
        if (Input.GetKeyUp(crouchKey) && grounded)
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    private void StateHandeler()
    {
        // Mode - crouching
        if (Input.GetKey(crouchKey) && grounded)
        {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }
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

        // On slope
        if (OnSlope() && !exitingSlope)
        {
            myRb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);

            if (myRb.linearVelocity.y > 0)
            {
                myRb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }

        // on ground
        if(grounded)
            myRb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // in air
        else if(!grounded)
            myRb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        myRb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        // Limit speed on slope
        if (OnSlope() && !exitingSlope)
        {
            if (myRb.linearVelocity.magnitude > moveSpeed)
            {
                myRb.linearVelocity = myRb.linearVelocity.normalized * moveSpeed;
            }
        }
        // Limit speed on ground and air
        else
        {
            Vector3 flatVel = new Vector3(myRb.linearVelocity.x, 0f, myRb.linearVelocity.z);

            // Limit velocity if needed
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitVel = flatVel.normalized * moveSpeed;
                myRb.linearVelocity = new Vector3(limitVel.x, myRb.linearVelocity.y, limitVel.z);
            }
        }
    }

    private void Jump()
    {
        exitingSlope = true;
        // reset Y velocity
        myRb.linearVelocity = new Vector3(myRb.linearVelocity.x, 0f, myRb.linearVelocity.z);

        myRb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    } 

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0f;
        }
        return false;
    }
    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }
}
