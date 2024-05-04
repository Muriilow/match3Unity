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
        StartCoroutine(DoTransition(4));
        //SceneManager.LoadSceneAsync("Main");
    }
    public void ClickOnNormal()
    {
        StartCoroutine(DoTransition(1));
        //SceneManager.LoadSceneAsync("Main");
    }
    public void ClickOnFast()
    {
        StartCoroutine(DoTransition(3));
    }

    public void BackToMenu()
    {
        StartCoroutine(DoTransition(0));
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


    IEnumerator DoTransition(int levelIndex)
    {
        //Set the parameter start to true
        transition.SetTrigger("Start");
        //Waiting for 1 second
        yield return new WaitForSeconds(transitionTime);
        //Loading the scene
        SceneManager.LoadScene(levelIndex);
    }
}
