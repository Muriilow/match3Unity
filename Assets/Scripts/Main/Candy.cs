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
    public bool isMatched; //Check if its in a match 

    public bool wasSelected; //Check if the candy is gonna show up the points

    public bool isClicked; //Check if the candy clicked at the candy 

    //where it is right now
    //private Vector2 currentPos;
    //Where do it want to be
    //private Vector2 targetPos;

    //Var to control when the obj is moving 
    public bool isMoving;
    [SerializeField] private new Animator animation;
    private string animationSelected;

    public void Start()
    {
        switch(candyColor)
        {
            case CandyColor.PeDeMlk: 
                animationSelected = "PeDeMlkSelected";
                break;
            case CandyColor.Snickers:
                animationSelected = "SnickersSelected";
                break;
            case CandyColor.BolinhoCandy:
                animationSelected = "BolinhoCandySelected";
                break;
            case CandyColor.MentaCandy:
                animationSelected = "MentaCandySelected";
                break;
            case CandyColor.ChocolateTriang:
                animationSelected = "ChocolateTriangSelected";
                break;
        }
        animation = GetComponent<Animator>();
    }
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

    public void Update()
    {
        if (isClicked) 
        {
            animation.SetBool("isSelected", true);
        }
        else 
        {
            animation.SetBool("isSelected", false);
        }
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
        float duration = 0.2f;

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
    PeDeMlk,
    Snickers,
    ChocolateTriang,
    MentaCandy,
    BolinhoCandy
}
