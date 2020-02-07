using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    protected Image fadeImage;
    [SerializeField]
    protected float fadeDuration = 1f;

    private bool currentlyLoading = false;

    protected virtual void Start()
    {
        fadeImage.DOFade(1f, fadeDuration).From();
    }

    protected virtual void Update()
    {
        if (gameObject.scene.name == "MainMenu")
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            BackToMainMenu();
        }
    }

    public void StartGame()
    {
        FadeOut(() => { UnityEngine.SceneManagement.SceneManager.LoadScene(1, LoadSceneMode.Single); });
    }

    public void BackToMainMenu()
    {
        FadeOut(() => { UnityEngine.SceneManagement.SceneManager.LoadScene(0, LoadSceneMode.Single); });
    }

    public void ExitGame()
    {
        FadeOut(() => { Application.Quit(); });
    }

    protected void FadeOut(TweenCallback _callback)
    {
        if (currentlyLoading) return;

        currentlyLoading = true;
        fadeImage.DOFade(1f, fadeDuration).OnComplete(_callback);
    }
}
