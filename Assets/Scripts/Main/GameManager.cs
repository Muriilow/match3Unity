using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

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

    [SerializeField] private TextMeshProUGUI timerText;
    public float storeTime;
    public  float time;  //time between matches

    public  int levelNormal; //The level we're in needs to be public
    public  int bestLevelFast; //store our best level
    public  int levelFast; //The level in the fast gameMode
    private int moves; //The amount of moves left in the level 
    private int points; //The points that we have

    [SerializeField] private int remainingCandies1 = 0;
    [SerializeField] private int remainingCandies2 = 0;
    [SerializeField] private int remainingCandies3 = 0;

    [SerializeField] private TMP_Text pointsTxt;    //Points text displayed in the UI
    [SerializeField] private TMP_Text movesTxt;     //Moves text displayed in the UI
    [SerializeField] private TMP_Text levelTxt;    //Level text displayed in the UI

    //Reference to the sliders and it images
    public ObjectiveSlider slider1;
    public ObjectiveSlider slider2;
    public ObjectiveSlider slider3;

    public int gameMode; 
    public bool isGameEnded; //If its true the board wont check for matches;
    public bool IsPaused;
    private bool loseGame = false;

    private void Awake()
    {
        Instance = this;
        loseGame = false;
        LoadGame();
        Initialize();
        CreateObjective();
        CheckTime();
    }

    public void Initialize()
    {
        if (gameMode == 1)
        {
            moves = 40;
        }
        else if(gameMode == 2)
        {
            PauseGame();
            levelFast = 1;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(gameMode == 1)
        {
            movesTxt.text = moves.ToString();
            levelTxt.text = levelNormal.ToString();
        }
        else if(gameMode == 2)
        {
            if (!IsPaused)
            {
                time -= Time.deltaTime;
            }
            timerText.text = time.ToString("N2");
            levelTxt.text = levelFast.ToString();

            if (time <= 0f && !loseGame)
            {
                LoseGame();
                loseGame = true;
            }
        }

        pointsTxt.text = points.ToString();
    }

    //Attached to a button to change scene when winning 
    private void WinGame()
    {   
        if(gameMode == 1) 
        {
            PauseGame();
            isGameEnded = true;
            levelNormal++;
            victoryPanel.SetActive(true);
        }

        else if(gameMode == 2)
        {
            levelFast++;
            CheckTime();
            CreateObjective();
        }
        SaveGame();
    }

    private void LoseGame()
    {
        PauseGame();
        isGameEnded = true;            

        losePanel.SetActive(true);

        if(gameMode == 2)
        {
            if(levelFast > bestLevelFast)
            {
                bestLevelFast = levelFast;
            }

            levelFast = 1;
        }

        SaveGame();
    }
    //Check to see if the match was caused by the player (reducing the moves) && if the removed candies are the same type as the ones in the objective
    // && adds the points to our score 
    public void ProcessTurn(int _pointsToGain, bool _reduceMoves, List<Candy> _candiesRemoved)
    {
        if (_reduceMoves)
        {
            for (int i = 0; i < _candiesRemoved.Count; i++)
            {
                if (_candiesRemoved[i].wasSelected)
                {
                    CreateFloatingText(_pointsToGain, _candiesRemoved[i].transform.position);
                    break;
                }
            }
        }
        else
        {
            int random = UnityEngine.Random.Range(0, _candiesRemoved.Count);
            CreateFloatingText(_pointsToGain, _candiesRemoved[random].transform.position);
        }
        if(_reduceMoves) moves--;

        if(gameMode == 2)
        {
            time = storeTime;
        }
        points += _pointsToGain;
        foreach (Candy candy in _candiesRemoved)
        {
           //    Debug.Log("Checking the candies");
            if (candy.candyColor == candiesObjective[0].candyColor && remainingCandies1 < 20)
            {
                remainingCandies1++;
                slider1.SetValue(remainingCandies1);
                //Debug.Log(remainingCandies1);
            }
            else if(candy.candyColor == candiesObjective[1].candyColor && remainingCandies2 < 20)
            {
                remainingCandies2++;
                slider2.SetValue(remainingCandies2);
                //Debug.Log(remainingCandies2);
            }
            else if(candy.candyColor == candiesObjective[2].candyColor && remainingCandies3 < 20)
            {
                remainingCandies3++;
                slider3.SetValue(remainingCandies3);
                //Debug.Log(remainingCandies3);
            }
        }
        if (remainingCandies1 >= 20 && remainingCandies2 >= 20 && remainingCandies3 >= 20)
        {
            WinGame();
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
        levelNormal = data.level;
    }

    private void CreateObjective()
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

            //Set what candy needs to be destroyed to win the game(ignore this)
            candiesObjective[i] = candiesPrefabs[randomIndex];

            //Set the image of the candie to the blank square (CandiesdPrefabs are the game object prefabs, candiesSprites are Ui Image, both arrays have the same order)
            imgSlider[i].sprite = candiesSprites[randomIndex].sprite;
            //Debug.Log(candiesObjective[i]);   
        }
    }
    private void CheckTime()
    {
        if (gameMode == 2)
        {
            if (levelFast <= 5) //Between levels 1 and 5
            {
                storeTime = 15f;
                time = storeTime;
            }

            else if (5 < levelFast && levelFast <= 10) //Between levels 6 and 10
            {
                storeTime = 12f;
                time = storeTime;
            }

            else if (10 < levelFast && levelFast <= 15) //Between levels 11 and 15
            {
                storeTime = 9f;
                time = storeTime;
            }

            else if (15 < levelFast && levelFast <= 20) //Between levels 16 and 20
            {
                storeTime = 7f;
                time = storeTime;
            }

            else                                        //After level 20
            {
                storeTime = 5f;
                time = storeTime;
            }
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
}
