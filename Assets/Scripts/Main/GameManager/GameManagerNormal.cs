using System;
using System.Collections.Generic;
using UnityEngine;
using Systems.Persistence;
using UnityEngine.Serialization;

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
        _data.isFreshGame = data.isFreshGame;
        
        if (data.isNew)
        {
            moves = 40;
            data.isNew = false;
        }
        else
        {
            points = data.points;
            moves = data.moves;
            
            if (data.isFreshGame)
                return;
            
            (remainingCandies1, remainingCandies2, remainingCandies3) = data.ReturnCandies();
            candiesIndex = _data.candiesIndex;
        }
    }

    private void LoadObjective()
    {
        for (int i = 0; i < 3; i++)
        {
			candiesObjective[i] = candiesPrefabs[candiesIndex[i]];
            imgSlider[i].sprite = candiesSprites[candiesIndex[i]].sprite;
        }

        slider1.SetValue(remainingCandies1);
        slider2.SetValue(remainingCandies2);
        slider3.SetValue(remainingCandies3);
    }
    protected void Start()
    {
        _levelNormal = _bestLevelNormal;
        
        if (_data.isFreshGame)
            CreateObjective();
        else
            LoadObjective();
        
    }
    #endregion
    
    #region Win/Lose Game
    protected override void WinGame()
    {
        _data.isFreshGame = true;
        
        PauseGame();
        isGameEnded = true;
        _levelNormal++;
        victoryPanel.SetActive(true);

        FinishGame();
        SaveGame();
    }

    protected override void LoseGame()
    {
        _data.isFreshGame = true;
        
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
        IsPaused = true;
    }

    public void ResumeGame()
    {
        IsPaused = false;
    }

    public void QuitGame()
    {
        //Assigning the values to save 
        _data.isFreshGame = false;
        _data.moves = moves;
        _data.points = points;
        _data.bestLevelNormal = _bestLevelNormal;
        
        _data.remainingCandies1 = remainingCandies1;
        _data.remainingCandies2 = remainingCandies2;
        _data.remainingCandies3 = remainingCandies3;
        
        _data.candiesIndex = candiesIndex;
        
        SaveGame(); 
    }
    #endregion
    
    public override void DeadLocked()
    {
        return;
    }
    public override void ProcessTurn(bool reduceMoves, List<Candy> candiesRemoved, bool multiplyPoints)
    {
        if (reduceMoves)
            moves--;
        
        _data.moves = moves;
        
        base.ProcessTurn(reduceMoves, candiesRemoved, multiplyPoints);
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
    
    public bool isFreshGame = true;
    public bool isNew = true;
    public int bestLevelNormal;
    public int moves;
    public int points;
    
    public int remainingCandies1;
    public int remainingCandies2;
    public int remainingCandies3;

    public int[] candiesIndex;
    public (int, int, int) ReturnCandies()
    {
        return (remainingCandies1, remainingCandies2, remainingCandies3);
    }
}

