using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
    [SerializeField] private Image[] splashImage;
    [SerializeField] private string loadLevel;
    [SerializeField] private int number;

    IEnumerator Start()
    {
        number = Random.Range(0, splashImage.Length -1);
        
        for(int i = 0; i < splashImage.Length; i++)
        {
            splashImage[i].gameObject.SetActive(false);
            splashImage[i].canvasRenderer.SetAlpha(0f);
        }
        FadeIn();
        yield return new WaitForSeconds(3.5f);

        FadeOut();
        yield return new WaitForSeconds(3.5f);

        SceneManager.LoadScene(loadLevel);
    }

    void FadeIn()
    {
        splashImage[number].gameObject.SetActive(true);
        splashImage[number].CrossFadeAlpha(1.0f, 1.5f, false);
    }

    void FadeOut()
    {
        splashImage[number].CrossFadeAlpha(0f, 3.5f, false);
    }
}
