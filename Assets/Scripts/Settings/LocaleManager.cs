using System;
using System.Collections;
using Systems.Persistence;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace Settings
{
    public class LocaleManager : MonoBehaviour, IBind<LanguageData>
    {
        private string _id = "langMnger";
        private LanguageData _data;
        private int _localizationId;
        private SaveSystem _saveSystem;
        private bool _active;
       
        #region Initialization
        public string Id
        {
            get => _id;
            set => _id = value;
        }

        public void Bind(LanguageData data)
        {
            _data = data;
            _localizationId = data.localeID;
        }

        private void Awake()
        {
            _saveSystem = FindObjectOfType<SaveSystem>();
        }

        private void Start()
        {
            ChangeLocale(_localizationId);
            Debug.Log("Changing the localization to the index: " + _localizationId);
        }
        #endregion

        #region Change Localization 
        public void ChangeLocale(int localeID)
        {
            if (_active)
                return;
            
            StartCoroutine(SetLocale(localeID));
        }

        private IEnumerator SetLocale(int localeID)
        {
            _active = true;
            
            yield return LocalizationSettings.InitializationOperation;
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeID];
            
            _data.localeID = localeID;
            
            _active = false;
        }
        #endregion

        public void SaveGame()
        {
            _saveSystem.gameData.languageData = _data;
            _saveSystem.SaveGame(); 
        }
    }

    [Serializable]
    public class LanguageData : ISaveable
    {
        [field: SerializeField] public string Id { get; set; }
        public bool isNew;
        public int localeID;
    }
}
