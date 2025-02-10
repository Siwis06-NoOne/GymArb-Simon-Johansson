using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ThirdPersonCam : MonoBehaviour
{
    [Header("Refrences")]
    [SerializeField] Transform orientation;
    [SerializeField] Transform player;
    [SerializeField] Transform PlayerObj;
    [SerializeField] Transform cameraTransform;
    [SerializeField] Rigidbody myRb;

    [SerializeField] float rotationspeed;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // roatate oriantaton
        Vector3 camForward = cameraTransform.forward;
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;


        // rotate player Obj
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (inputDir.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(inputDir.normalized);
            PlayerObj.rotation = Quaternion.Slerp(PlayerObj.rotation, targetRotation, Time.deltaTime * rotationspeed);
        }

    }

}
