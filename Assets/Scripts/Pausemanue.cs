using UnityEngine;

public class Pausemanue : MonoBehaviour
{
    [SerializeField] GameObject pausCanvas;

    private void Start()
    {
        pausCanvas.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && Time.timeScale == 1f)
        {
            Debug.Log("tab presed");
            Time.timeScale = 0;
            pausCanvas.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (Input.GetKeyDown(KeyCode.Tab) && Time.timeScale == 0f)
        {
            Debug.Log("Tab depressed");
            Time.timeScale = 1;
            pausCanvas.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
