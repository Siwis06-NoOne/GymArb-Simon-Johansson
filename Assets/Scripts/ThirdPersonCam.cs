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
        // Se till att kamerans forward inte påverkar y-axeln (så att den inte lutar uppåt eller neråt)
        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0f; // Nollställ y för att undvika att spelaren lutar
        orientation.forward = camForward.normalized;

        // Spelarens rotation efter kameran
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (inputDir.magnitude > 0.1f) // Se till att det finns rörelseinput
        {
            Quaternion targetRotation = Quaternion.LookRotation(inputDir.normalized);
            PlayerObj.rotation = Quaternion.Slerp(PlayerObj.rotation, targetRotation, Time.deltaTime * rotationspeed);
        }
    }   

}
