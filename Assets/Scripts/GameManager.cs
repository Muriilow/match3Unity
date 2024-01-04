using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance; //static reference

    public GameObject backgroundPanel; //Background
    public GameObject victoryPanel;
    public GameObject losePanel;

    public int goal; //The amount of points to win
    public int moves; //The amount of moves left in the level 
    public int points; //The points that we have

    [SerializeField] public TMP_Text pointsTxt;
    [SerializeField] public TMP_Text movesTxt;
    [SerializeField] public TMP_Text goalTxt;

    public bool isGameEnded;

    private void Awake()
    {
        Instance = this;
    }

    public void Initialize(int _moves, int _goal)
    {
        moves = _moves;
        goal = _goal;
    }

    // Update is called once per frame
    void Update()
    {
        pointsTxt.text = "Points: " + points.ToString();
        movesTxt.text = "Moves: " + moves.ToString();
        goalTxt.text = "Goal: " + goal.ToString();
    }

    //Attached to a button to change scene when winning 
    public void WinGame()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void LoseGame()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void ProcessTurn(int _pointsToGain, bool _reduceMoves)
    {
        points += _pointsToGain;

        if(_reduceMoves) moves--;    

        if(points >= goal)
        {
            //you've won the game
            isGameEnded = true;

            //display a victory screen
            //backgroundPanel.SetActive(true);
            //victoryPanel.SetActive(true);
            return;
        }

        if(moves == 0) 
        {
            //you've lost
            isGameEnded = true;
            //backgroundPanel.SetActive(true);
            //losePanel.SetActive(true);
            return;
        }
    }
}
