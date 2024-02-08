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

    //Get a reference to our candies prefabs
    [SerializeField] private Candy[] candiesPrefabs;

    //The array containing the candies taht need to be destroyed in order to win the game
    [SerializeField] private Candy[] candiesObjective = new Candy[3];

    //Array to get the sprites of candies
    public Sprite[] candiesSprites;

    //Array to get the UI Images for the sldiers
    public UnityEngine.UI.Image[] imgSlider;

    public GameObject[] floatingText; //Reference to the floating text 
    
    public static GameManager Instance; //static reference

    public GameObject backgroundPanel; //Background
    public GameObject victoryPanel;
    public GameObject losePanel;

    public int level; //The level we're in needs to be public
    private int goal; //The amount of points to win
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

    public bool isGameEnded;

    private void Awake()
    {
        Instance = this;
        LoadGame();
        Initialize(100000, 100000);
        CreateObjective();
        
    }

    public void Initialize(int _moves, int _goal)
    {
        moves = _moves;
        goal = _goal;
    }

    // Update is called once per frame
    void Update()
    {
        pointsTxt.text = points.ToString();
        movesTxt.text = moves.ToString();
        levelTxt.text = level.ToString();
    }

    //Attached to a button to change scene when winning 
    private void WinGame()
    {
        isGameEnded = true;
        level++;
        SaveGame();
        //backgroundPanel.SetActive(true);
        victoryPanel.SetActive(true);
        //SceneManager.LoadScene("Main");
    }

    private void LoseGame()
    {
        isGameEnded = true;            
        //backgroundPanel.SetActive(true);
        //losePanel.SetActive(true);
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
        
        points += _pointsToGain;

        if(_reduceMoves) moves--;


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

        if (moves == 0) LoseGame();
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

    private void CreateObjective()
    {
        for(int i = 0; i < 3; i++) 
        {
            //random number
            int randomIndex = UnityEngine.Random.Range(0, candiesPrefabs.Length);

            candiesObjective[i] = candiesPrefabs[randomIndex];
            imgSlider[i].sprite = candiesSprites[randomIndex];
            //Debug.Log(candiesObjective[i]);
        }
    }

    public void CreateFloatingText(int _points, Vector3 _position)
    {
        int random = UnityEngine.Random.Range(0, 3);
        GameObject text = Instantiate(floatingText[random], _position, Quaternion.identity);
        
        text.GetComponent<TextMeshPro>().text = _points.ToString();
    }
}
