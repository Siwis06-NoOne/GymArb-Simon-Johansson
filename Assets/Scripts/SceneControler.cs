using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControler : MonoBehaviour
{

    public static SceneControler Instance;

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }

    public void ChangeScene(int scene)
    {
        SceneManager.LoadScene(scene);
    }

}
