using System;
using System.Collections.Generic;
using UnityEngine;
using Systems.Persistence;
using UnityEngine.Serialization;

public class GameManagerNormal : GameManager, IBind<NormalData>
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
        isFreshGame = data.isFreshGame;
        
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

    protected void Start()
    {
        _levelNormal = _bestLevelNormal;
        
        if (isFreshGame)
            CreateObjective();
        else
            LoadObjective();
    }

    #endregion
    
    #region Win/Lose Game
    protected override void WinGame()
    {
        isFreshGame = true;
        
        victoryPanel.SetActive(true);
        
        PauseGame();
        FinishGame();
    }

    protected override void LoseGame()
    {
        isFreshGame = true;
        
		losePanel.SetActive(true);
        
        PauseGame();
        FinishGame();
    }
    #endregion
    
    #region Save/Load Game
    private void SaveGame()
    {
        saveSystem.gameData.normalData = _data;
        saveSystem.SaveGame();
    }

    protected override void FinishGame()
    {
        base.FinishGame();
        
        points = 0;
        isGameEnded = true;
        _levelNormal++;
        
        if(_levelNormal > _bestLevelNormal)
            _bestLevelNormal = _levelNormal;
    }
    #endregion

    
    public void QuitGame()
    {
        if(!isFreshGame)
            _data.candiesIndex = candiesIndex;
        
        _data.isFreshGame = isFreshGame;
        _data.moves = moves;
        _data.points = points;
        _data.bestLevelNormal = _bestLevelNormal;
        
        _data.remainingCandies1 = remainingCandies1;
        _data.remainingCandies2 = remainingCandies2;
        _data.remainingCandies3 = remainingCandies3;
        
        SaveGame(); 
    }
    
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
        
        if(moves <= 0)
            LoseGame();
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

