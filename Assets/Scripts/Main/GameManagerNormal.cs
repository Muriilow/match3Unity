using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Systems.Persistence;

public class GameManagerNormal : GameManager, IBind<NormalData>
{
    private string id = "normalMnger";
    private int bestLevelNormal;
    public string Id 
    {
        get { return id; }
        set { id = value; }
    }

    public NormalData data;
    public void Bind(NormalData data)
    {
        this.data = data;
        this.data.Id = Id;
        bestLevelNormal = data.bestLevelNormal;

        //TODO: Fix this if statement 
        if (data.bestLevelNormal == 0)
        {
            moves = 40;
            points = 0;
        }
        else
        {
            points = data.points;
            moves = data.moves;
        }
    }
    protected override void Awake()
    {
        base.Awake();

        //Initialize();
    }

    protected override void Update()
    {
        movesTxt.text = moves.ToString();
        levelTxt.text = levelNormal.ToString();

        base.Update();
    }
    public void Initialize()
    {
        moves = 40;
    }
    protected override void WinGame()
    {
        PauseGame();
        isGameEnded = true;
        levelNormal++;
        victoryPanel.SetActive(true);

        recordInfo();
        saveSystem.gameData.normalData = data;
        saveSystem.SaveGame();

        base.WinGame();
    }

    private void recordInfo()
    {
        if(levelNormal > bestLevelNormal)
            bestLevelNormal = levelNormal;
        
        data.bestLevelNormal = bestLevelNormal;
        data.points = 0;
        data.moves = 40; 
        
        //TODO: Make a method to return a random number of moves to data.moves
    }
    protected override void LoseGame()
    {
        recordInfo();
        base.LoseGame();
        
    }

    public override void ProcessTurn(int _pointsToGain, bool _reduceMoves, List<Candy> _candiesRemoved)
    {
        if (_reduceMoves)
            moves--;
        
        data.moves = moves;
        
        base.ProcessTurn(_pointsToGain, _reduceMoves, _candiesRemoved);
    }

    public override void QuitGame()
    {
        base.QuitGame();
       
        data.points = points;
        saveSystem.gameData.normalData = data;
        saveSystem.SaveGame();
    }
}

[Serializable]
public class NormalData : ISaveable
{
    [field: SerializeField] public string Id { get; set; }
    public int bestLevelNormal;
    public int moves;
    public int points;
}

