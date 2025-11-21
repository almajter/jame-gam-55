using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "Credits" && Input.GetKeyDown(KeyCode.Escape))
        {
            BackToMenu();
        }
        if (sceneName == "Options" && Input.GetKeyDown(KeyCode.Escape))
        {
            BackToMenu();
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("DimnikScene");
    }

    public void OpenOptions()
    {
        Debug.Log("Options clicked");
        SceneManager.LoadScene("Options");
    }

    public void OpenCredits()
    {
        Debug.Log("Credits clicked");
        SceneManager.LoadScene("Credits");
    }

    public static void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Debug.Log("Quit clicked");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
