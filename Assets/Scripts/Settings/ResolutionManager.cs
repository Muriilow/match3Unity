using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

namespace Settings
{
    public class ResolutionManager : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown _resolutionDropdown;
        
        private Resolution[] _resolutions;
        private List<Resolution> _filteredResolutions;

        private float _currentRefreshRate; 
        private int _currentResolutionIndex;

        //Gets all the resolutions that have the same refresh rate that the computer is using
        void Start()
        {
            List<string> options = new List<string>();
            
            _resolutions = Screen.resolutions;
            _filteredResolutions = new List<Resolution>();
            
            _resolutionDropdown.ClearOptions();
            
            _currentRefreshRate = Screen.currentResolution.refreshRate;

            //Loop through all the resolutions the computer have access and take only the ones
            //With the same refreshRates 
            foreach (Resolution resolution in _resolutions)
            {
                //Approximately because its float values
                if (Mathf.Approximately(resolution.refreshRate, _currentRefreshRate))
                    _filteredResolutions.Add(resolution);
            }

            //Make a string text to every option
            //And find the resolution the computer is using to set as the default value
            for (var i = 0; i < _filteredResolutions.Count; i++)
            {
                var resolution = _filteredResolutions[i];
                string resolutionOption = resolution.width + " x " + resolution.height + " " + resolution.refreshRate + " Hz";
                options.Add(resolutionOption);

                if (resolution.width == Screen.width && resolution.height == Screen.height)
                {
                    _currentResolutionIndex = i;
                }
            }
            
            _resolutionDropdown.AddOptions(options);
            _resolutionDropdown.value = _currentResolutionIndex;
            _resolutionDropdown.RefreshShownValue();
        }

        public void SetResolution(int resolutionIndex)
        {
            Resolution resolution = _filteredResolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }

        public void SetFullScreen(bool fullScreen)
        {
            if(fullScreen)
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
            else 
                Screen.fullScreenMode = FullScreenMode.Windowed;
        }
    }
}
