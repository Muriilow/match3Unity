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

public class GameManagerNormal : GameManager
{
    protected override void Awake()
    {
        base.Awake();

        Initialize();
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

        base.WinGame();
    }

    public override void ProcessTurn(int _pointsToGain, bool _reduceMoves, List<Candy> _candiesRemoved)
    {
        if (_reduceMoves)
            moves--;

        base.ProcessTurn(_pointsToGain, _reduceMoves, _candiesRemoved);
    }

    #region Save system

    //public void Save(ref NormalSaveData data)
    //{
    //    data.levelNormal = levelNormal;
    //}

    //public void Load(ref NormalSaveData data)
    //{
    //    levelNormal = data.levelNormal;
    //}
    #endregion
}

//[System.Serializable]
//public struct NormalSaveData
//{
//    public int levelNormal;
//}

