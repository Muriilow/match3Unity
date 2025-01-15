using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.XR;
using System.Security.Cryptography.X509Certificates;

public class GameManagerFast : GameManager
{
    [SerializeField]
    private TextMeshProUGUI timerText;
    public int BestLevelFast { get; set; }
    private int LevelFast { get; set; }

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

    public override void ProcessTurn(int _pointsToGain, bool _reduceMoves, List<Candy> _candiesRemoved)
    {
        TimeFast = StoreTime;
        base.ProcessTurn(_pointsToGain, _reduceMoves, _candiesRemoved);
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

        LevelFast = 1;

        base.LoseGame();

    }
}
