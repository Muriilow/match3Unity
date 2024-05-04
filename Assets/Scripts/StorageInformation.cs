using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StorageInformation : MonoBehaviour
{
    [SerializeField] public TMP_Text bestFastLevel;
    public int bestFastLevelContent;

    [SerializeField] public TMP_Text normalLevel;
    public int normalLevelContent;

    public void Awake()
    {
        GameManagerData data = SaveSystem.LoadPlayer();
        bestFastLevelContent = data.bestFastLevel;
        normalLevelContent = data.level;
    }
    void Start()
    {
        bestFastLevel.text = bestFastLevelContent.ToString();
        normalLevel.text = normalLevelContent.ToString();
    }
}
