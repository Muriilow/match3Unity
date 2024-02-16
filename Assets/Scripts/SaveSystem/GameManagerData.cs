using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameManagerData
{
    public int level;

    public GameManagerData(GameManager gameManager)
    {
        level = gameManager.level;
    }
}
