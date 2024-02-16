using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Button : MonoBehaviour
{
    public void ClickOnPlay()
    {
        SceneManager.LoadSceneAsync("Main");
    }

    public void BackToMenu()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }

    public void ClickOnCredits()
    {
        SceneManager.LoadSceneAsync("Credits");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
