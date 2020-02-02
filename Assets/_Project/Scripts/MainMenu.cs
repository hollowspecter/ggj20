using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public string mainSceneName;
    
    public void StartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1, LoadSceneMode.Single);
    }
    
    public void ExitGame()
    {
        Application.Quit();
    }
}
