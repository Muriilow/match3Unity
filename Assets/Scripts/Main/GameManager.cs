using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Systems.Persistence;
using TMPro;
using UnityEngine;
using Systems.Persistence;
public class GameManager : MonoBehaviour
{
    public SaveSystem saveSystem;
    protected string gameName;
    public float StoreTime { get; set; }
    public float TimeFast { get; set; }

    public Board board;
    //Get a reference to our candies prefabs
    [SerializeField] private Candy[] candiesPrefabs;

    //The array containing the candies taht need to be destroyed in order to win the game
    [SerializeField] private Candy[] candiesObjective = new Candy[3];

    //Array to get the sprites of candies
    public UnityEngine.UI.Image[] candiesSprites;

    //Array to get the UI Images for the sldiers
    public UnityEngine.UI.Image[] imgSlider;

    public GameObject[] floatingText; //Reference to the floating text 
    
    public static GameManager Instance; //static reference

    public GameObject victoryPanel; //blank gameObject that cotains UI image of victory
    public GameObject losePanel;    //blank gameObject that cotains UI image of lost



    public  int levelNormal; //The level we're in needs to be public
    [SerializeField] protected int moves; //The amount of moves left in the level 
    private int points; //The points that we have

    [SerializeField] private int remainingCandies1 = 0;
    [SerializeField] private int remainingCandies2 = 0;
    [SerializeField] private int remainingCandies3 = 0;

    //Text in the UI 
    [SerializeField] protected TMP_Text pointsTxt;
    [SerializeField] protected TMP_Text movesTxt;
    [SerializeField] protected TMP_Text levelTxt;

    //Reference to the sliders and it images
    public ObjectiveSlider slider1;
    public ObjectiveSlider slider2;
    public ObjectiveSlider slider3;

    public bool isGameEnded; //If its true the board wont check for matches;
    public bool IsPaused;
    protected bool loseGame;

    protected virtual void Awake()
    {
        Instance = this;
        loseGame = false;

        gameName = saveSystem.gameData.Name;
        saveSystem.LoadGame(gameName);
        
        CreateObjective();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        pointsTxt.text = points.ToString();
    }

    //Attached to a button to change scene when winning 
    protected virtual void WinGame()
    {   
        //SaveGame();
    }

    protected virtual void LoseGame()
    {
        PauseGame();
        isGameEnded = true;            

        losePanel.SetActive(true);
        
        saveSystem.SaveGame();
    }

    //Check to see if the match was caused by the player (reducing the moves) && if the removed candies are the same type as the ones in the objective
    // && adds the points to our score 
    public virtual void ProcessTurn(int _pointsToGain, bool _reduceMoves, List<Candy> _candiesRemoved)
    {
        int random;
        bool wasTextCreated;

        wasTextCreated = false;
        random = UnityEngine.Random.Range(0, _candiesRemoved.Count);
        points += _pointsToGain;

        foreach (Candy candy in _candiesRemoved)
        {
            if (candy.wasSelected && !wasTextCreated)
            {
                wasTextCreated = true;
                CreateFloatingText(_pointsToGain, candy.transform.position);
            }

            if (candy.candyColor == candiesObjective[0].candyColor && remainingCandies1 < 20)
            {
                remainingCandies1++;
                slider1.SetValue(remainingCandies1);
            }
            else if(candy.candyColor == candiesObjective[1].candyColor && remainingCandies2 < 20)
            {
                remainingCandies2++;
                slider2.SetValue(remainingCandies2);
            }
            else if(candy.candyColor == candiesObjective[2].candyColor && remainingCandies3 < 20)
            {
                remainingCandies3++;
                slider3.SetValue(remainingCandies3);;
            }   
        }
        if(!wasTextCreated)
            CreateFloatingText(_pointsToGain, _candiesRemoved[random].transform.position);

        if (remainingCandies1 >= 20 && remainingCandies2 >= 20 && remainingCandies3 >= 20)
        {
            WinGame();
            return;
        }
    }

    protected void CreateObjective()
    {
        for(int i = 0; i < 3; i++) 
        {
            //Reseting the var that contains the number of matched cndies of the same type
            remainingCandies1 = 0;
            remainingCandies2 = 0;
            remainingCandies3 = 0;
            //reseting the value of the slider too
            slider1.SetValue(remainingCandies1);
            slider2.SetValue(remainingCandies2);
            slider2.SetValue(remainingCandies3);

            //random number
            int randomIndex = UnityEngine.Random.Range(0, candiesPrefabs.Length);
            Debug.Log(randomIndex);
            //Set what candy needs to be destroyed to win the game(ignore this)
            candiesObjective[i] = candiesPrefabs[randomIndex];

            //Set the image of the candie to the blank square (CandiesdPrefabs are the game object prefabs, candiesSprites are Ui Image, both arrays have the same order)
            imgSlider[i].sprite = candiesSprites[randomIndex].sprite;
            //Debug.Log(candiesObjective[i]);   
        }
    }


    public void CreateFloatingText(int _points, Vector3 _position)
    {
        int random = UnityEngine.Random.Range(0, 3);
        GameObject text = Instantiate(floatingText[random], _position, Quaternion.identity);
        
        text.GetComponent<TextMeshPro>().text = _points.ToString();
    }

    #region Pause

    public void PauseGame()
    {
        IsPaused = true;
    }

    public void ResumeGame()
    {
        IsPaused = false;
    }
    #endregion

    public virtual void DeadLocked()
    {
        return; 
    }

}


