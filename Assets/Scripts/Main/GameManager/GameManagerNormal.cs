using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Systems.Persistence;

public class GameManagerNormal : GameManager, IBind<NormalData>, IPausable
{
    private string _id = "normalMnger";
	private int _levelNormal;
    private int _bestLevelNormal;
    private NormalData _data;
   
    public string Id 
    {
        get => _id;
        set => _id = value;
    }

    #region Start Game
    public void Bind(NormalData data)
    {
        _data = data;
        _data.Id = Id;
        _bestLevelNormal = data.bestLevelNormal;

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

    public void Start()
    {
        _levelNormal = _bestLevelNormal;
    }
    #endregion
    
    #region Win/Lose Game
    protected override void WinGame()
    {
        PauseGame();
        isGameEnded = true;
        _levelNormal++;
        victoryPanel.SetActive(true);

        FinishGame();
        SaveGame();
    }

    protected override void LoseGame()
    {
        PauseGame();
        FinishGame();
        SaveGame();
        base.LoseGame();
    }
    #endregion
    
    #region Save/Load Game
    private void SaveGame()
    {
        saveSystem.gameData.normalData = _data;
        saveSystem.SaveGame();
        //TODO: Make a method to return a random number of moves to data.moves
    }

    private void FinishGame()
    {
        if(_levelNormal > _bestLevelNormal)
            _bestLevelNormal = _levelNormal;
        
        _data.bestLevelNormal = _bestLevelNormal;
        _data.points = 0;
        _data.moves = 40;
    }
    #endregion

    #region Pause System
    public void PauseGame()
    {
        isPaused = true;
    }

    public void ResumeGame()
    {
        isPaused = false;
    }

    public void QuitGame()
    {
        _data.moves = moves;
        _data.points = points;
        _data.bestLevelNormal = _bestLevelNormal;
        SaveGame(); 
    }
    #endregion
    
    public override void DeadLocked()
    {
        return;
    }
    public override void ProcessTurn(int pointsToGain, bool reduceMoves, List<Candy> candiesRemoved)
    {
        if (reduceMoves)
            moves--;
        
        _data.moves = moves;
        
        base.ProcessTurn(pointsToGain, reduceMoves, candiesRemoved);
    }
    protected override void Update()
    {
        movesTxt.text = moves.ToString();
        levelTxt.text = _levelNormal.ToString();

        base.Update();
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

