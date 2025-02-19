using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

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
