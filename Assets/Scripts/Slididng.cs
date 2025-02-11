using System.Runtime.CompilerServices;
using UnityEngine;

public class Slididng : MonoBehaviour
{
    [Header("Refrences")]
    [SerializeField] Transform orientation;
    [SerializeField] Transform playerObject;
    [SerializeField] Rigidbody myRb;
    private PlayerMovement pm;

    [Header("Sliding")]
    [SerializeField] float maxSlideTime;
    [SerializeField] float slideForce;
    private float slideTimer;

    [SerializeField] float slideYScale;
    [SerializeField] float staryYscale;

    [Header("Input")]
    [SerializeField] KeyCode slideKey = KeyCode.LeftControl;
    private float horizontalinput;
    private float verticalinput;

    [SerializeField] bool isSliding;

    private void Start()
    {
        myRb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();

        staryYscale = playerObject.lossyScale.y;
    }

    private void Update()
    {
        horizontalinput = Input.GetAxisRaw("Horizontal");
        verticalinput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(slideKey) && (horizontalinput != 0 || verticalinput != 0))
        {
            StartSlide();
        }
        if (Input.GetKeyUp(slideKey) && isSliding)
        {
            StopSlide();
        }
    }

    private void FixedUpdate()
    {
        if (isSliding)
        {
            SlidingMovement();
        }
    }

    private void StartSlide()
    {
        isSliding = true;
        transform.localScale = new Vector3(transform.localScale.x, slideYScale, transform.localScale.z);
        myRb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        slideTimer = maxSlideTime;
    }

    private void SlidingMovement()
    {
        Vector3 inputDirection = orientation.forward * verticalinput + orientation.right * horizontalinput;

        myRb.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);

        if (slideTimer <= 0)
        {
            StopSlide();
        }
    }

    private void StopSlide()
    {
        isSliding = false;
        transform.localScale = new Vector3(transform.localScale.x, staryYscale, transform.localScale.z);

    }
}
