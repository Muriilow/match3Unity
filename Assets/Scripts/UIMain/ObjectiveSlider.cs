using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveSlider : MonoBehaviour
{
    public Slider slider;

    public void SetValue(int points)
    {
        slider.value = points;
    }

    public void SetValue(float points)
    {
        slider.value = points;
    }
}
