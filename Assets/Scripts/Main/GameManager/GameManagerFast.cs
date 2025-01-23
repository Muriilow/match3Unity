using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Systems.Persistence;
using UnityEngine.Serialization;

public class GameManagerFast : GameManager, IBind<FastData>, IPausable
{
    //Time
    private float _storeTime;
    private float _timeFast;
    
    //Level
    private int _bestLevelFast;
    private int _levelFast;
    
    private string _id = "fastMnger";
    private bool _setPanel = false; 
    
    [SerializeField] private GameObject _explanationPanel;
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private GameObject _startPanel;
    [SerializeField] private FastData _data;
    
    public string Id 
    {
        get => _id;
        set => _id = value;
    }

    #region Start Game
    //Load information saved in the file
    public void Bind(FastData data)
    {
        _data = data;
        _data.Id = Id;
        _bestLevelFast = data.bestLevelFast;

        if(data.isNew)
        {
            _setPanel = true;
            data.isNew = false;
        }
    }
    
    protected override void Awake()
    {
        base.Awake();

        PauseGame();
        _levelFast = 1;
        
        CheckTime();
    }

    public void Start()
    {
        if(_setPanel)
            _explanationPanel.SetActive(true);
        else
            _startPanel.SetActive(true);
    }

    private void CheckTime()
    {
        switch (_levelFast)
        {
            case <= 5:
                {
                    _storeTime = 15f;
                    break;
                }
            case <= 10:
                {
                    _storeTime = 12f;
                    break;
                }
            case <= 15:
                {
                    _storeTime = 9f;
                    break;
                }
            case <= 20:
                {
                    _storeTime = 7f;
                    break;
                }
            default:
                {
                    _storeTime = 5f;
                    break;
                }
        }

        _timeFast = _storeTime;
    }
    #endregion

    #region Win/Lose Game
    public override void DeadLocked()
    {
        _timeFast = _storeTime;
    }
    protected override void LoseGame()
    {
        PauseGame();
        _levelFast = 1;

        base.LoseGame();
    }
    protected override void WinGame()
    {
        _levelFast++;
        CheckTime();
        CreateObjective();
    }
    #endregion
    
    #region Pause
    public void PauseGame()
    {
       IsPaused = true; 
    }

    public void ResumeGame()
    {
        IsPaused = false;
    }

    public void QuitGame()
    {
        SaveGame();
        saveSystem.SaveGame(); 
    }

    public void SaveGame()
    {
        if (_levelFast > _bestLevelFast)
        {
            _bestLevelFast = _levelFast;
        }

        _data.lastLevelFast = _levelFast;
        _data.bestLevelFast = _bestLevelFast;
        
        saveSystem.gameData.fastData = _data; 
    }
    #endregion
    
    protected override void Update()
    {
        if (!IsPaused)
        {
            _timeFast -= Time.deltaTime;
        }

        _timerText.text = _timeFast.ToString("N2");
        levelTxt.text = _levelFast.ToString();

        if (_timeFast <= 0f && !loseGame)
        {
            LoseGame();
            loseGame = true;
        }

        base.Update();
    }

    public override void ProcessTurn(bool reduceMoves, List<Candy> candiesRemoved)
    {
        _timeFast = _storeTime;
        base.ProcessTurn(reduceMoves, candiesRemoved);
    }
}

[Serializable]
public class FastData : ISaveable
{
    [field: SerializeField] public string Id { get; set; }
    public int bestLevelFast;
    public bool isNew = true;
    public int lastLevelFast;
}