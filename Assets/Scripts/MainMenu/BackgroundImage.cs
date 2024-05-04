using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundImage : MonoBehaviour
{
    [SerializeField] private RawImage rawImg;
    [SerializeField] private float xSpeed, ySpeed;
    void Update()
    {
        rawImg.uvRect = new Rect(rawImg.uvRect.position + new Vector2(xSpeed, ySpeed) * Time.deltaTime, rawImg.uvRect.size);
    }
}
