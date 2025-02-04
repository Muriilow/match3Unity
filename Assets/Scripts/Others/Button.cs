using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class Button : MonoBehaviour
{
    [SerializeField] private Animator _transition;
    [SerializeField] private float _transitionTime = 1f;
    public void ClickOnPlay()
    {
        StartCoroutine(DoTransition("MenuGame"));
    }

    public void ClickOnSettings()
    {
        StartCoroutine(DoTransition("Settings"));
    }
    public void ClickOnNormal()
    {
        StartCoroutine(DoTransition("NormalGame"));
    }
    public void ClickOnFast()
    {
        StartCoroutine(DoTransition("FastGame"));
    }

    public void BackToMenu()
    {
        StartCoroutine(DoTransition("MainMenu"));
    }

    public void ClickOnCredits()
    {
        SceneManager.LoadSceneAsync("Credits");
    }
    
    private IEnumerator DoTransition(string levelName)
    {
        //Set the parameter start to true
        _transition.SetTrigger("Start");
        //Waiting for 1 second
        yield return new WaitForSeconds(_transitionTime);
        //Loading the scene
        SceneManager.LoadScene(levelName);
    }
}
