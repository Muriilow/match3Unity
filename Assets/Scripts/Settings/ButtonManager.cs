using System;
using System.Collections;
using System.Collections.Generic;
using Settings;
using Systems.Persistence;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

//Class that handles the change between button sprites when they are selected 
public class ButtonManager : MonoBehaviour
{
    
    [SerializeField] private GameObject _ptSelected;
    [SerializeField] private GameObject _ptUnselected;
    [SerializeField] private GameObject _enSelected;
    [SerializeField] private GameObject _enUnselected;
    
    [SerializeField] private GameObject _locManager;
    

    //Changes which sprites each button will have depending on the game language
    public void ChangeLanguage(int language)
    {
        if (language == 0)
            ActiveEnglish();
        else
            ActivePortuguese();
    }

    public void Awake()
    {
        _locManager = GameObject.FindGameObjectWithTag("LocalizationManager");
    }

    //Check for the language saved by the game and set the buttons with the respective sprites 
    public void Start()
    {
        if (_locManager == null)
            return;

        int language = _locManager.GetComponent<LocaleManager>().localizationId;
        
        ChangeLanguage(language);
    }

    private void ActivePortuguese()
    {
            _ptSelected.SetActive(true);
            _ptUnselected.SetActive(false);
            
            _enSelected.SetActive(false);
            _enUnselected.SetActive(true);
    }

    private void ActiveEnglish()
    {
            _ptSelected.SetActive(false);
            _ptUnselected.SetActive(true);
            
            _enSelected.SetActive(true);
            _enUnselected.SetActive(false);
    }
}
