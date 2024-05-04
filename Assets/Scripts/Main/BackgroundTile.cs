using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BackgroundTile
{
    /*Boolean value that allow us to create non retangular shapes
    by determing if this tile can or cannot have a candy*/
    public bool isUsable;

    //storing the candy inside the tile
    public GameObject candy;
    
    //Constructor
    public BackgroundTile(bool _isUsable, GameObject _candy)
    {
        isUsable = _isUsable;
        candy = _candy;

    }
}
