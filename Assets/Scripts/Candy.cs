using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Candy : MonoBehaviour
{
    public CandyColor candyColor;
    //My x and y position
    public int xIndex;
    public int yIndex;

    //is it matched?
    public bool isMatched;

    //where it is right now
    //private Vector2 currentPos;
    //Where do it want to be
    //private Vector2 targetPos;

    //Var to control when the obj is moving 
    public bool isMoving;

    //Constructor
    public Candy(int x, int y)
    {
        xIndex = x;
        yIndex = y;
    }
    //method that does the same as the constructor
    public void SetIndicies(int x, int y)
    {
        xIndex = x;
        yIndex = y;
    }

    //MoveToTarget
    public void MoveToTarget(Vector2 _targetPos)
    {
        StartCoroutine(MoveCoroutine(_targetPos));
    }
    //MoveCoroutine
    private IEnumerator MoveCoroutine(Vector2 _targetPos)
    {
        isMoving = true;
        float duration = 0.4f;

        Vector2 startPosition = transform.position;
        float elaspedTime = 0f;

        while(elaspedTime < duration)
        {
            float t = elaspedTime / duration;

            transform.position = Vector2.Lerp(startPosition, _targetPos, t);
            elaspedTime += Time.deltaTime;

            yield return null;
        }
        transform.position = _targetPos;
        isMoving = false;
    }
}

public enum CandyColor
{
    ChocolatePiece,
    Snickers,
    ChocolateTriang,
    WhiteChocolateCircle,
    WhiteChocolateRect,
    SpecialCookie
}
