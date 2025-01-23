using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class BackgroundImage : MonoBehaviour
{
    [SerializeField] private RawImage _rawImg;
    [SerializeField] private float _xSpeed;
    [SerializeField] private float _ySpeed;

    void Update()
    {
        _rawImg.uvRect = new Rect(_rawImg.uvRect.position + new Vector2(_xSpeed, _ySpeed) * Time.deltaTime, _rawImg.uvRect.size);
    }
}
