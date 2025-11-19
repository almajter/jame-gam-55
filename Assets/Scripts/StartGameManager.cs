using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        SceneManager.LoadScene("PrehtScene");
    }

    // Menu buttons
    public void OpenOptions()
    {
        Debug.Log("Options clicked");
        // TODO
    }

    public void OpenCredits()
    {
        Debug.Log("Credits clicked");
        // TODO
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
