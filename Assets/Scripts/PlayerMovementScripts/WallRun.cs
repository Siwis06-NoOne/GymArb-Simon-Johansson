using UnityEngine;

public class WallRun : MonoBehaviour
{
    [Header("WallRunning")]
    [SerializeField] LayerMask whatIsWall;
    [SerializeField] LayerMask whatIsGround;
    [SerializeField] float wallRunForce;
    [SerializeField] float maxWallRunTime;
    [SerializeField] float wallClimbSpeed;
    private float wallRunTimer;
    public bool isWallRunning;

    [Header("Input")]
    private float horizontalInput;
    private float verticalInput;
    [SerializeField] KeyCode upwardsRunKey = KeyCode.LeftShift;
    [SerializeField] KeyCode downwardsRunKey = KeyCode.LeftControl;
    private bool upwardsRunning;
    private bool downwardsRunning;

    [Header("Detection")]
    [SerializeField] float wallCheckDistance;
    [SerializeField] float minJumpHeight;
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    private bool wallLeft;
    private bool wallRight;

    [Header("Refrences")]
    [SerializeField] Transform orientation;
    private PlayerMovement pm;
    [SerializeField] Rigidbody myRb;

    private void Start()
    {
        myRb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
        wallRunTimer = maxWallRunTime;
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
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallCheckDistance, whatIsWall);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallCheckDistance, whatIsWall);
    }

    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }

    private void StateMachine()
    {
        // get input
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        upwardsRunning = Input.GetKey(upwardsRunKey);
        downwardsRunning = Input.GetKey(downwardsRunKey);

        // State 1 - Wallrunning
        if ((wallLeft || wallRight) && verticalInput > 0 && AboveGround())
        { 
            if (!pm.wallrunning)
            {
                StartWallrun();
            }
        }
        // State 3 - None
        if ((pm.wallrunning && wallRunTimer <= 1f) || (pm.wallrunning && !wallRight && !wallLeft))
        {
            EndWallRun();
        }
    }

    private void StartWallrun()
    {
        pm.wallrunning = true;
        Debug.Log("WallRunnStarted");
    }

    private void WallRunningMovement()
    {
        myRb.useGravity = false;
        myRb.linearVelocity = new Vector3(myRb.linearVelocity.x, 0f, myRb.linearVelocity.z);

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if ((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
        {
            wallForward = -wallForward;
        }

        // force forward
        myRb.AddForce(wallForward * wallRunForce, ForceMode.Force);

        // up and down force
        if (upwardsRunning)
        {
            myRb.linearVelocity = new Vector3(myRb.linearVelocity.x, wallClimbSpeed, myRb.linearVelocity.z);
        }
        if (downwardsRunning)
        {
            myRb.linearVelocity = new Vector3(myRb.linearVelocity.x, -wallClimbSpeed, myRb.linearVelocity.z);
        }

        // push to wall force
        if (!(wallLeft && horizontalInput > 0) && !(wallRight && horizontalInput > 0))
        {
            myRb.AddForce(-wallNormal * 100, ForceMode.Force);
        }

        wallRunTimer -= Time.deltaTime;
    }

    private void EndWallRun()
    {
        Debug.Log("wallRunnEnd");
        pm.wallrunning = false;
        wallRunTimer = maxWallRunTime;
    }
}
