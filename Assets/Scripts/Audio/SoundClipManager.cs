using System;
using System.Collections;
using System.Collections.Generic;
using Systems.Persistence;
using UnityEngine;
using Unity.Audio;
using UnityEngine.Audio;

public class SoundClipManager : MonoBehaviour, IBind<SoundData>
{
    private string _id = "SoundMnger";
    private SoundData _data;

    [SerializeField] private float _masterVol;
    [SerializeField] private float _musicVol;
    [SerializeField] private float _soundFxVol;
    
    [SerializeField] private AudioMixer _mixer;

    private SaveSystem _saveSystem;
    public string Id 
    {
        get => _id;
        set => _id = value;
    }

    public void Bind(SoundData data)
    {
        _data = data;
        _data.Id = Id;

        _masterVol = data.masterVol;
        _musicVol = data.musicVol;
        _soundFxVol = data.sfxVol;
    }

    public void Awake()
    {
        _saveSystem = FindObjectOfType<SaveSystem>();
    }

    private void Start()
    {
        SetMusicVolume(_musicVol);
    }
    public void SaveGame()
    {
        _saveSystem.gameData.soundData = _data;
        _saveSystem.SaveGame();
    }

    #region SetVolumes
    public void SetMasterVolume(float volume)
    {
        _mixer.SetFloat("MasterVol", Mathf.Log10(volume) * 20f);
        _data.masterVol = volume;
    }

    public void SetMusicVolume(float volume)
    {
        _mixer.SetFloat("MusicVol", Mathf.Log10(volume) * 20f);
        _data.musicVol = volume;
    }

    public void SetSfxVolume(float volume)
    {
        _mixer.SetFloat("SoundFxVol", Mathf.Log10(volume) * 20f);
        _data.sfxVol = volume;
    }
    #endregion
}

[Serializable]
public class SoundData : ISaveable
{
    [field: SerializeField] public string Id { get; set; }
    public float masterVol;
    public float musicVol;
    public float sfxVol;
}
