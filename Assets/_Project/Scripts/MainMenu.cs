using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public string mainSceneName;
    
    void StartGame()
    {
        // SceneManager.LoadScene(mainSceneName, LoadSceneMode.Single);
    }
    
    void ExitGame()
    {
        Application.Quit();
    }
}
