using Unity.Hierarchy;
using UnityEngine;

public class WallClimb : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] Transform oriantation;
    [SerializeField] Rigidbody playerRigidbody;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] LayerMask whatIsWall;

    [Header("Climbing")]
    [SerializeField] float climbSpeed;
    [SerializeField] float maxClimbTime;
    private float climbTimer;

    private bool isClimbing;

    [Header("ClimbJump")]
    [SerializeField] float climbJumpUpForce;
    [SerializeField] float climbJumpBackForce;

    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    [SerializeField] int climbJumps;
    [SerializeField] int climbJumpsLeft;

    [Header("Detection")]
    [SerializeField] float detectionLength;
    [SerializeField] float sphereCastRadius;
    [SerializeField] float maxWallLookAngle;
    private float wallLookAngle;

    private RaycastHit frontWallHit;
    private bool wallFront;

    private Transform lastWall;
    private Vector3 lastWallNormal;
    [SerializeField] float minWallNormalAngleChange;

    [Header("Exiting")]
    public bool exitingWall;
    [SerializeField] float exitWallTime;
    [SerializeField] float exitWallTimer;

    private void Update()
    {
        WallCheck();
        StateMachine();

        if (isClimbing && !exitingWall) ClimbingMovement();
    }

    private void StateMachine()
    {
        // state 1 - climbing
        if (wallFront && Input.GetKey(KeyCode.W) && wallLookAngle < maxWallLookAngle && !exitingWall)
        {
            if (!isClimbing && climbTimer > 0) StartClimbing();

            // Timer
            if (climbTimer > 0) climbTimer -= Time.deltaTime;
            if (climbTimer < 0) StopClimbing();
        }

        // state 2 - exiting
        else if (exitingWall)
        {
            if (isClimbing) StopClimbing();

            if (exitWallTimer > 0) exitWallTimer -= Time.deltaTime;
            if (exitWallTimer < 0) exitingWall = false;

        }
        // state 3 - none
        else
        {
            if (isClimbing) StopClimbing();
        }

        if (wallFront && Input.GetKeyDown(jumpKey) && climbJumpsLeft > 0) ClimbJump();
    }

    #region wallclimb

    private void WallCheck()
    {
        wallFront = Physics.SphereCast(transform.position, sphereCastRadius, oriantation.forward, out frontWallHit, detectionLength, whatIsWall);
        wallLookAngle = Vector3.Angle(oriantation.forward, -frontWallHit.normal);

        bool newWall = frontWallHit.transform != lastWall || Mathf.Abs(Vector3.Angle(lastWallNormal, frontWallHit.normal)) > minWallNormalAngleChange;

        if ((wallFront && newWall) || playerMovement.grounded)
        {
            climbTimer = maxClimbTime;
            climbJumpsLeft = climbJumps;
        }
    }

    private void StartClimbing()
    {
        isClimbing = true;
        playerMovement.climbing = true;

        lastWall = frontWallHit.transform;
        lastWallNormal = frontWallHit.normal;
    }

    private void ClimbingMovement()
    {
        playerRigidbody.linearVelocity = new Vector3(playerRigidbody.linearVelocity.x, climbSpeed, playerRigidbody.linearVelocity.z);
    }

    private void StopClimbing()
    {
        isClimbing = false;
        playerMovement.climbing = false;
    }
    #endregion

    #region ClimbJump

    private void ClimbJump()
    {
        exitingWall = true;
        exitWallTimer = exitWallTime;

        Vector3 forceToApply = transform.up * climbJumpUpForce + frontWallHit.normal * climbJumpBackForce;

        playerRigidbody.linearVelocity = new Vector3(playerRigidbody.linearVelocity.x, 0f, playerRigidbody.linearVelocity.z);
        playerRigidbody.AddForce(forceToApply, ForceMode.Impulse);

        climbJumpsLeft--;
    }

    #endregion
}
