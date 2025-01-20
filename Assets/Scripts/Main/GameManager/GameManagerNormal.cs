using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Systems.Persistence;

public class GameManagerNormal : GameManager, IBind<NormalData>, IPausable
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
        //If this is the first save 
        if (data.isNew)
        {
            moves = 40;
            data.isNew = false;
        }
        else
        {
            points = data.points;
            moves = data.moves;
        }
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

        FinishGame();
        SaveGame();
    }

    private void SaveGame()
    {
        saveSystem.gameData.normalData = data;
        saveSystem.SaveGame();
        //TODO: Make a method to return a random number of moves to data.moves
    }

    private void FinishGame()
    {
        if(levelNormal > bestLevelNormal)
            bestLevelNormal = levelNormal;
        
        data.bestLevelNormal = bestLevelNormal;
        data.points = 0;
        data.moves = 40;
    }

    protected override void LoseGame()
    {
        PauseGame();
        FinishGame();
        SaveGame();
        base.LoseGame();
    }

    public override void ProcessTurn(int _pointsToGain, bool _reduceMoves, List<Candy> _candiesRemoved)
    {
        if (_reduceMoves)
            moves--;
        
        data.moves = moves;
        
        base.ProcessTurn(_pointsToGain, _reduceMoves, _candiesRemoved);
    }

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
        data.moves = moves;
        data.points = points;
        data.bestLevelNormal = bestLevelNormal;
        SaveGame(); 
    }

    public override void DeadLocked()
    {
        return;
    }
}

[Serializable]
public class NormalData : ISaveable
{
    [field: SerializeField] public string Id { get; set; }
    public bool isNew = true;
    public int bestLevelNormal;
    public int moves;
    public int points;
}

