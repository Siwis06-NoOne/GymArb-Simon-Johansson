using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ThirdPersonCam : MonoBehaviour
{

    [Header("Refrences")]
    [SerializeField] Transform orientation;
    [SerializeField] Transform player;
    [SerializeField] Transform PlayerObj;
    [SerializeField] Transform cameraTransform;
    [SerializeField] Rigidbody myRb;

    [Header("Door Stuf")]
    [SerializeField] int amountOfKeys;
    [SerializeField] int currentKeys;
    [SerializeField] bool hasAllKeys;
    
    private string currentCeneName;

    [SerializeField] float rotationspeed;
    private void Awake()
    {
        currentCeneName = SceneManager.GetActiveScene().name;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentKeys = 0;
        hasAllKeys = false;
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

        if (currentKeys >= amountOfKeys)
        {
            hasAllKeys = true;
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Key")
        {
            currentKeys = currentKeys + 1;
            collision.gameObject.SetActive(false);
        }

        if (collision.gameObject.tag == "Door" && hasAllKeys == true)
        {
            SceneManager.LoadScene(2);
        }

        if (collision.gameObject.tag == "Death")
           SceneManager.LoadScene(currentCeneName);
    }

}
