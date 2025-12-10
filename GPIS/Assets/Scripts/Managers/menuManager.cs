using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class menuManager : MonoBehaviour
{
    public void onPlay()
    {
        SceneManager.LoadSceneAsync("town");
    }

    public void onCredits()
    {
        SceneManager.LoadSceneAsync("Credits");
    }

    public void onQuit()
    {
        Application.Quit();
    }

    public void onMenu()
    {
        SceneManager.LoadSceneAsync("mainMenu");
    }

}
