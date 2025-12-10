using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class menuManager : MonoBehaviour
{
    public void onPlay()
    {
        SaveScene("town");
        SceneManager.LoadScene("town");
    }

    public void onCredits()
    {
        SaveScene("Credits");
        SceneManager.LoadScene("Credits");
    }

    public void onQuit()
    {
        // Save that we quit from this scene
        SaveScene(SceneManager.GetActiveScene().name);

        Application.Quit();
    }

    public void onMenu()
    {
        SaveScene("mainMenu");
        SceneManager.LoadScene("mainMenu");
    }

    private void SaveScene(string sceneName)
    {
        PlayerPrefs.SetString("LastScene", sceneName);
        PlayerPrefs.Save();
    }
}
