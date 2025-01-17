using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Systems.Persistence;

public class GameManagerFast : GameManager, IBind<FastData>
{
    [SerializeField]
    private TextMeshProUGUI timerText;

    private string id = "fastMnger";
    public string Id 
    {
        get { return id; }
        set { id = value; }
    } 
    
    [SerializeField]
    FastData data;

    public int BestLevelFast { get; set; }
    
    private int LevelFast { get; set; }

    public void Bind(FastData data)
    {
        this.data = data;
        this.data.Id = Id;
        BestLevelFast = data.bestLevelFast;
    }
    protected override void Awake()
    {
        base.Awake();

        Initialize();
        CheckTime();
    }

    protected override void Update()
    {
        if (!IsPaused)
        {
            TimeFast -= Time.deltaTime;
        }

        timerText.text = TimeFast.ToString("N2");
        levelTxt.text = LevelFast.ToString();

        if (TimeFast <= 0f && !loseGame)
        {
            LoseGame();
            loseGame = true;
        }

        base.Update();
    }
    public void Initialize()
    {
        PauseGame();
        LevelFast = 1;
    }
    protected override void WinGame()
    {
        LevelFast++;
        CheckTime();
        CreateObjective();

        base.WinGame();
    }

    private void CheckTime()
    {
        switch (LevelFast)
        {
            case <= 5:
                {
                    StoreTime = 15f;
                    TimeFast = StoreTime;
                    break;
                }
            case <= 10:
                {
                    StoreTime = 12f;
                    TimeFast = StoreTime;
                    break;
                }
            case <= 15:
                {
                    StoreTime = 9f;
                    TimeFast = StoreTime;
                    break;
                }
            case <= 20:
                {
                    StoreTime = 7f;
                    TimeFast = StoreTime;
                    break;
                }
            default:
                {
                    StoreTime = 5f;
                    TimeFast = StoreTime;
                    break;
                }
        }
    }

    public override void ProcessTurn(int pointsToGain, bool reduceMoves, List<Candy> candiesRemoved)
    {
        TimeFast = StoreTime;
        base.ProcessTurn(pointsToGain, reduceMoves, candiesRemoved);
    }

    public override void DeadLocked()
    {
        TimeFast = StoreTime;
    }
    protected override void LoseGame()
    {
        if (LevelFast > BestLevelFast)
        {
            BestLevelFast = LevelFast;
        }

        data.lastLevelFast = LevelFast;
        data.bestLevelFast = BestLevelFast;
        
        saveSystem.gameData.fastData = data; 
        LevelFast = 1;

        base.LoseGame();
    }
}

[Serializable]
public class FastData : ISaveable
{
    [field: SerializeField] public string Id { get; set; }
    public int bestLevelFast;
    public int lastLevelFast;
}