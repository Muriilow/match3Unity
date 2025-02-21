using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class Button : MonoBehaviour
{
    [SerializeField] private Animator _transition;
    [SerializeField] private float _transitionTime = 2f;
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
        StartCoroutine(DoTransition("Credits"));
    }

    private IEnumerator DoTransition(string levelName)
    {
        _transition.SetTrigger("Start");
        yield return new WaitForSeconds(_transitionTime);
        SceneManager.LoadScene(levelName);
    }
}
