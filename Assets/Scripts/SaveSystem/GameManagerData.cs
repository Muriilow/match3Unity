using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameManagerData
{
    public int level;
    public int bestFastLevel;

    public GameManagerData(GameManager gameManager)
    {
        level = gameManager.levelNormal;
        bestFastLevel = gameManager.bestLevelFast;
    }
}
