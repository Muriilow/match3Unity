using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Candy : MonoBehaviour
{
    [SerializeField] public CandyColor candyColor;
    //My x and y position
    public int xIndex;
    public int yIndex;

    //is it matched?
    public bool isMatched;

    //where it is right now
    private Vector2 currentPos;
    //Where do it want to be
    private Vector2 targetPos;

    //Var to control when the obj is moving 
    public bool isMoving;

    //Constructor
    public Candy(int x, int y)
    {
        xIndex = x;
        yIndex = y;
    }
}

public enum CandyColor
{
    Green,
    Indigo,
    Orange,
    Pink,
    Salmon,
    Teal,
    Yellow,
    BrightBlue
}
