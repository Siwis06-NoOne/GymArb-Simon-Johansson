using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class WallRun : MonoBehaviour
{
    [Header("WallRunning")]
    [SerializeField] LayerMask whatIsWall;
    [SerializeField] LayerMask whatIsGround;
    [SerializeField] float wallRunForce;
    [SerializeField] float maxWallRunTime;
    private float wallRunTimeer;

    [Header("Input")]
    private float horizontalInput;
    private float verticalInput;

    [Header("Detection")]
    [SerializeField] float wallCheckDistance;
    [SerializeField] float minJumpHeigt;
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    private bool wallLeft;
    private bool wallRight;

    [Header("Refrences")]
    [SerializeField] Transform oriantation;
    private PlayerMovement pm;
    [SerializeField] Rigidbody myRb;

    private void Start()
    {
        myRb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        CheckForWall();
        StateMachine();
    }

    private void FixedUpdate()
    {
        if (pm.wallrunning)
        {
            WallRunningMovement();
        }
    }

    private void CheckForWall()
    {
        wallRight = Physics.Raycast(transform.position, oriantation.right, out rightWallHit, wallCheckDistance, whatIsWall);
        wallLeft = Physics.Raycast(transform.position, -oriantation.right, out leftWallHit, wallCheckDistance, whatIsWall);
    }

    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeigt, whatIsGround);
    }

    private void StateMachine()
    {
        // get input
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // State 1 - Wallrunning
        if ((wallLeft || wallRight) && verticalInput > 0 && AboveGround())
        { 
            if (pm.wallrunning)
            {
                StartWallrun();
            }
        }
        // State 3 - None
        if (pm.wallrunning)
        {
            EndWallRun();
        }
    }

    private void StartWallrun()
    {
        pm.wallrunning = true;
    }

    private void WallRunningMovement()
    {
        myRb.useGravity = false;
        myRb.linearVelocity = new Vector3(myRb.linearVelocity.x, 0f, myRb.linearVelocity.z);

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        // force forward
        myRb.AddForce(wallForward * wallRunForce, ForceMode.Force);
    }

    private void EndWallRun()
    {
        pm.wallrunning = false;
    }
}
