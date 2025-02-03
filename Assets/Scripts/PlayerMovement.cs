using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed;

    [SerializeField] float groundDrag;

    [Header("Ground Chek")]
    [SerializeField] float playerHeigt;
    [SerializeField] LayerMask whatIsGround;
    bool grounded;

    [SerializeField] Transform oriantation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody myRb;

    private void Start()
    {
        myRb = GetComponent<Rigidbody>();
        myRb.freezeRotation = true;
    }
    private void Update()
    {
        // ground Check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeigt * 0.5f + 0.2f, whatIsGround);

        // handle drag
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
    }
    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void MovePlayer()
    {
        // Calculate movement direction
        moveDirection = oriantation.forward * verticalInput + oriantation.right * horizontalInput;

        myRb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
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
}
