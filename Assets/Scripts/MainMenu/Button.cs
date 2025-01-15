using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Button : MonoBehaviour
{
    [SerializeField] private Animator transition;
    [SerializeField] private float transitionTime = 1f;
    /*
     * 0 - MainMenu
     * 1 - Main
     * 2 - Credits
     * 3 - mainFastGame
     * 4 - GameMenu
     */
    public void ClickOnPlay()
    {
        StartCoroutine(DoTransition("MenuGame"));
        //SceneManager.LoadSceneAsync("Main");
    }
    public void ClickOnNormal()
    {
        StartCoroutine(DoTransition("MainNormalGame"));
        //SceneManager.LoadSceneAsync("Main");
    }
    public void ClickOnFast()
    {
        StartCoroutine(DoTransition("MainFastGame"));
    }

    public void BackToMenu()
    {
        StartCoroutine(DoTransition("MainMenu"));
        //SceneManager.LoadSceneAsync("MainMenu");
    }

    public void ClickOnCredits()
    {
        SceneManager.LoadSceneAsync("Credits");
    }

    public void QuitGame()
    {
        Application.Quit();
    }


    IEnumerator DoTransition(string levelName)
    {
        //Set the parameter start to true
        transition.SetTrigger("Start");
        //Waiting for 1 second
        yield return new WaitForSeconds(transitionTime);
        //Loading the scene
        SceneManager.LoadScene(levelName);
    }
}
