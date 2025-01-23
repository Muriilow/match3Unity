using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class SplashScreen : MonoBehaviour
{
    [SerializeField] private Image[] _splashImage;
    [SerializeField] private string _loadLevel;
    [SerializeField] private int _number;

    private IEnumerator Start()
    {
        _number = Random.Range(0, _splashImage.Length -1);
        
        foreach (var img in _splashImage)
        {
            img.gameObject.SetActive(false);
            img.canvasRenderer.SetAlpha(0f);
        }
        FadeIn();
        yield return new WaitForSeconds(3.5f);

        FadeOut();
        yield return new WaitForSeconds(3.5f);

        SceneManager.LoadScene(_loadLevel);
    }

    private void FadeIn()
    {
        _splashImage[_number].gameObject.SetActive(true);
        _splashImage[_number].CrossFadeAlpha(1.0f, 1.5f, false);
    }

    private void FadeOut()
    {
        _splashImage[_number].CrossFadeAlpha(0f, 3.5f, false);
    }
}
