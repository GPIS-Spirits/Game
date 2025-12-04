using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Scenes")]
    public GameObject dungeonRoot;

    private void Awake()
    {
        if (Instance != this && Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void EnterBattle()
    {
        if (dungeonRoot) dungeonRoot.SetActive(false);
        SceneManager.LoadSceneAsync("battle", LoadSceneMode.Additive);
    }

    public void ExitBattle()
    {
        if (dungeonRoot) dungeonRoot.SetActive(true);
        SceneManager.UnloadSceneAsync("battle");
    }
}