using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class Button : MonoBehaviour
{
    [SerializeField] private Animator _transition;
    [SerializeField] private float _transitionTime = 2f;

    [SerializeField] private GameObject _firstUI;
    [SerializeField] private GameObject _secondUI;
    
    public void ClickOnSettings()
    {
        StartCoroutine(DoTransition("Settings"));
    }

    public void ClickOnPlay()
    {
       StartCoroutine(ActivateGameObject()); 
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
        StartCoroutine(DoTransition("Credits"));
    }

    private IEnumerator DoTransition(string levelName)
    {
        _transition.SetTrigger("Start");
        yield return new WaitForSeconds(_transitionTime);
        SceneManager.LoadScene(levelName);
    }

    private IEnumerator ActivateGameObject()
    {
        _transition.SetTrigger("Start");
        yield return new WaitForSeconds(_transitionTime);
        _firstUI.SetActive(false);
        _secondUI.SetActive(true);
    }
}
