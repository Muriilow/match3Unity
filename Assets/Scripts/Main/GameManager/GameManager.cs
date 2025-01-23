using System.Collections.Generic;
using Systems.Persistence;
using TMPro;
using UnityEngine;
public abstract class GameManager : MonoBehaviour
{
	[SerializeField] private Candy[] _candiesPrefabs;
	//The array containing the candies that need to be destroyed in order to win the game
	[SerializeField] private Candy[] _candiesObjective = new Candy[3];
	
    [SerializeField] private int _multiplier = 1;
	
	[SerializeField] private int _remainingCandies1 = 0;
	[SerializeField] private int _remainingCandies2 = 0;
	[SerializeField] private int _remainingCandies3 = 0;

	//Text in the UI 
	[SerializeField] protected TMP_Text pointsTxt;
	[SerializeField] protected TMP_Text movesTxt;
	[SerializeField] protected TMP_Text levelTxt;

	//Reference to the sliders and it images
	public ObjectiveSlider slider1;
	public ObjectiveSlider slider2;
	public ObjectiveSlider slider3;

	//Array to get the sprites of candies
	public UnityEngine.UI.Image[] candiesSprites;

	//Array to get the UI Images for the sliders
	public UnityEngine.UI.Image[] imgSlider;

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
		
		CreateObjective();
	}

	protected void CreateObjective()
	{
		//Resetting the var that contains the number of matched candies of the same type
		_remainingCandies1 = 0;
		_remainingCandies2 = 0;
		_remainingCandies3 = 0;
			
		//resetting the value of the slider too
		slider1.SetValue(_remainingCandies1);
		slider2.SetValue(_remainingCandies2);
		slider2.SetValue(_remainingCandies3);
		
		for(int i = 0; i < 3; i++)
		{
			int randomIndex;
			
			randomIndex = UnityEngine.Random.Range(0, _candiesPrefabs.Length);
			
			//Set what candy needs to be destroyed to win the game(ignore this)
			_candiesObjective[i] = _candiesPrefabs[randomIndex];
			imgSlider[i].sprite = candiesSprites[randomIndex].sprite;
		}
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

		if (_remainingCandies1 >= 20 && _remainingCandies2 >= 20 && _remainingCandies3 >= 20)
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
		if (candy.candyColor == _candiesObjective[0].candyColor && _remainingCandies1 < 20)
		{
			_remainingCandies1++;
			slider1.SetValue(_remainingCandies1);
		}
		else if(candy.candyColor == _candiesObjective[1].candyColor && _remainingCandies2 < 20)
		{
			_remainingCandies2++;
			slider2.SetValue(_remainingCandies2);
		}
		else if(candy.candyColor == _candiesObjective[2].candyColor && _remainingCandies3 < 20)
		{
			_remainingCandies3++;
			slider3.SetValue(_remainingCandies3);
		}
	}
	#endregion

	#region Win/Lose
	protected abstract void WinGame();
	protected virtual void LoseGame()
	{
		isGameEnded = true;            

		losePanel.SetActive(true);
		
		saveSystem.SaveGame();
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
}


