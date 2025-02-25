using System.Collections.Generic;
using Systems.Persistence;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;

public abstract class GameManager : MonoBehaviour, IPausable
{
	[SerializeField] protected Candy[] candiesPrefabs;
	//The array containing the candies that need to be destroyed in order to win the game
	[SerializeField] protected Candy[] candiesObjective = new Candy[3];
	
    [SerializeField] private int _multiplier = 1;
	
	[SerializeField] protected int remainingCandies1 = 0;
	[SerializeField] protected int remainingCandies2 = 0;
	[SerializeField] protected int remainingCandies3 = 0;

	//Text in the UI 
	[SerializeField] protected Text pointsTxt;
	[SerializeField] protected Text movesTxt;
	[SerializeField] protected Text levelTxt;

	//Indexes of the candies off the objective
	[SerializeField] protected int[] candiesIndex;
	
	//Reference to the sliders and it images
	public ObjectiveSlider slider1;
	public ObjectiveSlider slider2;
	public ObjectiveSlider slider3;

	//Array to get the sprites of candies
	public Image[] candiesSprites;

	//Array to get the UI Images for the sliders
	public Image[] imgSlider;

	//reference for the floating text
	public GameObject[] floatingText;  
	
	//Panels of victory or defeat 
	public GameObject victoryPanel;
	public GameObject losePanel;
	
	public bool isGameEnded;
	
	//ManagerFast and ManagerNormal could use these vars
	protected int moves;
	protected int points;
	protected SaveSystem saveSystem;
	protected bool loseGame;
	protected bool isFreshGame;

	public bool IsPaused { get; protected set; }
	protected virtual void Update()
	{
		pointsTxt.text = points.ToString();
	}

	private void CreateFloatingText(int floatPoints, Vector3 position)
	{
		int random;
		GameObject text;
		
		random = UnityEngine.Random.Range(0, 3);
		
		text = Instantiate(floatingText[random], position, Quaternion.identity);
		text.GetComponent<TextMeshPro>().text = floatPoints.ToString();
	}

	public abstract void DeadLocked();
	
	#region Start Game
	protected virtual void Awake()
	{
		loseGame = false;

		saveSystem = FindObjectOfType<SaveSystem>();
	}

	protected void CreateObjective()
	{
		isFreshGame = false;
		//resetting the value of the slider too
		slider1.SetValue(remainingCandies1);
		slider2.SetValue(remainingCandies2);
		slider3.SetValue(remainingCandies3);
		
		
		for(int i = 0; i < 3; i++)
		{
			candiesIndex[i] = UnityEngine.Random.Range(0, candiesPrefabs.Length);
			
			//Set what candy needs to be destroyed to win the game(ignore this)
			candiesObjective[i] = candiesPrefabs[candiesIndex[i]];
			imgSlider[i].sprite = candiesSprites[candiesIndex[i]].sprite;
		}
	}
    public void LoadObjective()
    {
        ResumeGame();
        isGameEnded = false;
        isFreshGame = false; 
        IsPaused = false;
        
        for (int i = 0; i < 3; i++)
        {
			candiesObjective[i] = candiesPrefabs[candiesIndex[i]];
            imgSlider[i].sprite = candiesSprites[candiesIndex[i]].sprite;
        }

        slider1.SetValue(remainingCandies1);
        slider2.SetValue(remainingCandies2);
        slider3.SetValue(remainingCandies3);
    }
	#endregion
	
	#region Check turns
	//Check to see if the match was caused by the player (reducing the moves) && if the removed candies are the same type as the ones in the objective
	public virtual void ProcessTurn(bool reduceMoves, List<Candy> candiesRemoved, bool multiplyPoints)
	{
		int newPoints; 
		
		newPoints = CalculatePoints(multiplyPoints, candiesRemoved);
		points += newPoints; 

		DisplayPoints(candiesRemoved, newPoints);

		if (remainingCandies1 >= 20 && remainingCandies2 >= 20 && remainingCandies3 >= 20)
			WinGame();
	}

	private void DisplayPoints(List<Candy> candiesRemoved, int newPoints)
	{
		int random = UnityEngine.Random.Range(0, candiesRemoved.Count);
		bool wasTextCreated = false;
		
		//Check if each candy being destroyed is one of the objective candies
		foreach (Candy candy in candiesRemoved)
		{
			if (candy.WasSelected && !wasTextCreated)
			{
				wasTextCreated = true;
				CreateFloatingText(newPoints, candy.transform.position);
			}

			CheckCandyColor(candy);
		}

		//Create floating text
		if(!wasTextCreated)
			CreateFloatingText(newPoints, candiesRemoved[random].transform.position);
	}

	private void CheckCandyColor(Candy candy)
	{
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
			slider3.SetValue(remainingCandies3);
		}
	}
	#endregion

	#region Win/Lose
	protected abstract void WinGame();

	protected virtual void LoseGame()
	{
		
	}
	#endregion

	#region Points
	
    private int CalculatePoints(bool multiplyPoints, List<Candy> candiesRemoved)
    {
        int newPoints = 0;

        foreach (Candy candy in candiesRemoved)
        {
            int randomNum = UnityEngine.Random.Range(09, 19);
            newPoints += 100 + randomNum;
        }

        if (multiplyPoints)
            _multiplier *= 2;
        else
            _multiplier = 1;
        
        newPoints *= _multiplier;

        return newPoints;
    }

	#endregion
	
	#region Finish Game

	protected virtual void FinishGame()
	{
        moves = 40;
        
        remainingCandies1 = 0;
        remainingCandies2 = 0;
        remainingCandies3 = 0;
	}
	#endregion
	
	#region Pause Game
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


