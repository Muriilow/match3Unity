using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveSlider : MonoBehaviour
{
    public Slider slider;

    public void SetValue(int _points)
    {
        slider.value = _points;
    }
}
