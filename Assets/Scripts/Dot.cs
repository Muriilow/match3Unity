using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Dot : MonoBehaviour
{
    public int column;
    public int row;

    //x and y position of the dot 
    public int targetY;
    public int targetX;

    private Board board;

    private GameObject otherDot;

    //The first and last touch
    private Vector2 firstTouchPos;
    private Vector2 finalTouchPos;
    private Vector2 tempPosition;

    //The angle of the player's touch
    public float swipeAngle = 0;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        targetX = (int)transform.position.x;
        targetY = (int)transform.position.y;
        //Maaaybe change this later----------------------------------------
        row = targetY;
        column = targetX;
    }

    // Update is called once per frame
    void Update()
    {
        targetX = column;
        targetY = row;
        //If we change our target but we are not in the position of our target move the dot /Abs return the positive value 
        if(Mathf.Abs(targetX - transform.position.x) > .1)
        {
            //Move Towards the target
            //The position that the dot is going to 
            tempPosition = new Vector2(targetX, transform.position.y);
            //With a lerp change the position of the dot to the temp position
            transform.position = Vector2.Lerp(transform.position, tempPosition, .4f);
        }
        else
        {
            //If the position of the dot is the target Directly set the position
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempPosition;
            //Update the board
            board.allDots[column, row] = this.gameObject;
        }
        //If we change our target but we are not in the position of our target move the dot /Abs return the positive value 
        if (Mathf.Abs(targetY - transform.position.y) > .1)
        {
            //Move Towards the target
            //The position that the dot is going to 
            tempPosition = new Vector2(transform.position.x, targetY);
            //With a lerp change the position of the dot to the temp position
            transform.position = Vector2.Lerp(transform.position, tempPosition, .4f);
        }
        else
        {
            //If the position of the dot is the target Directly set the position
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition;
            //Update the board
            board.allDots[column, row] = this.gameObject;
        }
    }

    private void OnMouseDown()
    {
        /* Checking the position of the players touch but Unity will give the pixels cordinates 
        and not the squares cordinates so we are going to use this method
        Camera.main.ScreenToWorldPoint */
        firstTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Debug.Log(firstTouchPos);
    }

    private void OnMouseUp() 
    {
        //Getting the cordinates when the user releases the mouse button
        //ITs called even when the mouse is not in the collider cordinates
        finalTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CalculateAngle();
        //Debug.Log(finalTouchPos);
    }

    void CalculateAngle()
    {
        /* Getting the angle by this atan2 function, but the function
        return radian values and I hate this shit so converting to NORMAL angles */
        swipeAngle = Mathf.Atan2(finalTouchPos.y - firstTouchPos.y, finalTouchPos.x - firstTouchPos.x) * 180/Mathf.PI;
        MovePieces();
        Debug.Log(swipeAngle);
    }

    void MovePieces()
    {
        if(swipeAngle > -45 && swipeAngle <= 45 && column < board.width)
        {
            //Right swipe
            //Getting the other dot position and changing the column 
            otherDot = board.allDots[column + 1, row];
            otherDot.GetComponent<Dot>().column -= 1;
            //The dot that we swipe will have the column changed too!
            column += 1;
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height)
        {
            //Up swipe
            //Getting the other dot position and changing the row
            otherDot = board.allDots[column, row + 1];
            otherDot.GetComponent<Dot>().row -= 1;
            //The dot that we swipe will have the row changed too!
            row += 1;
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
        {
            //Left swipe
            //Getting the other dot position and changing the column
            otherDot = board.allDots[column - 1, row];
            otherDot.GetComponent<Dot>().column += 1;
            //The dot that we swipe will have the column changed too!
            column -= 1;
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
        {
            //Down swipe
            //Getting the other dot position and changing the row
            otherDot = board.allDots[column, row - 1];
            otherDot.GetComponent<Dot>().row += 1;
            //The dot that we swipe will have the row changed too!
            row -= 1;
        }
    }
}
