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


    public int level; //The level we're in 
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
        LoadGame();
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

        switch (level)
        {
            case 0: moves = 99; goal = 999; break;
            case 1: moves = 99; goal = 999; break;
            case 2: moves = 99; goal = 999; break;
            case 3: moves = 99; goal = 999; break;
            case 4: moves = 9999; goal = 9999; break;
        }
    }

    //Attached to a button to change scene when winning 
    public void WinGame()
    {
        level++;
        SaveGame();
        
    }

    public void LoseGame()
    {
        SaveGame();
    }

    public void ProcessTurn(int _pointsToGain, bool _reduceMoves)
    {
        points += _pointsToGain;

        if(_reduceMoves) moves--;    

        if(points >= goal)
        {

            WinGame();
            //you've won the game
            isGameEnded = true;

            //display a victory screen
            //backgroundPanel.SetActive(true);
            //victoryPanel.SetActive(true);
            return;
        }

        if(moves == 0) 
        {

            LoseGame();
            //you've lost
            isGameEnded = true;

            //backgroundPanel.SetActive(true);
            //losePanel.SetActive(true);
            return;
        }
    }

    public void SaveGame()
    {
        SaveSystem.SavePlayer(this);
        Debug.Log("Salvei uhul");
    }

    public void LoadGame()
    {
        GameManagerData data = SaveSystem.LoadPlayer();
        Debug.Log("carreguei uhul");
        level = data.level;
    }

    public int CalculateLevel(int level)
    {
        goal = level;
        return 3;
    }
}
