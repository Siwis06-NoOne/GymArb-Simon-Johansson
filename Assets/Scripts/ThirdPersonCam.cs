using UnityEngine;

public class ThirdPersonCam : MonoBehaviour
{
    [Header("Refrences")]
    [SerializeField] Transform oriantation;
    [SerializeField] Transform player;
    [SerializeField] Transform PlayerObj;
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
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        oriantation.forward = viewDir.normalized;

        // rotate player Obj
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 inputDir = oriantation.forward * verticalInput + oriantation.right * horizontalInput;

        if (inputDir != Vector3.zero)
        {
            PlayerObj.forward = Vector3.Slerp(PlayerObj.forward, inputDir.normalized, Time.deltaTime * rotationspeed);
        }

    }

}
