using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using Utilities;
using UnityEngine.XR;

public class Board : MonoBehaviour
{
    //Define the size of the board
    public int width;
    public int height;

    //Define some spacing for the board
    public float spacingX;
    public float spacingY;
    private int multiplier = 1;

    public GameManager manager;
    public GameObject candiesBoardGO;
    //layoutArray
    public ArrayLayout arrayLayout;
    //public static of board
    public static Board Instance;

    //List to hold the value of the candies that will be destroyed 
    public List<GameObject> candiesToDestroy = new();

    //Get a reference to the collection of backgroundTile board + GO
    private BackgroundTile[,] backgroundTiles;

    //Get a reference to our candies prefabs
    [SerializeField] private GameObject[] candiesPrefabs;

    //The candy being selected 
    [SerializeField] private Candy selectedCandy;

    //Getting the gameObj of the parent of the candies
    [SerializeField] private GameObject candyParent;

    //Is it already moving?
    [SerializeField] private bool isProcessingMove;

    //The list have the candies were going to remove
    [SerializeField] public List<Candy> candiesToRemove = new();

    [SerializeField] private GameObject background;
    [SerializeField] private int gameMode;

    //If I clcked at a candy collider, do the SelectCandy method  
    private void Update()
    {
        CheckClick();
        //BackgroundColor();
    }

    #region Initialize the game
    //Unity method that initialize the code when the obj with this script is called
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        InitializeBoard();
    }

    void InitializeBoard()
    {
        DestroyCandies();
        //Settign the size of this 2d array in this variable
        backgroundTiles = new BackgroundTile[width, height];

        //Variables that control where should the candies be placed
        spacingX = (float)(width - 1) / 2 - 3; //-0.5
        spacingY = (float)(height - 1) / 2; //3.5

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                //pos for the candy 
                Vector2 position;

                position = new Vector2(x - spacingX, y - spacingY);

                /*If I checked the buttons in the inspector of this obj, the tile with the
                same pos as the inspector button will not work and will not have a candy on it*/
                if (arrayLayout.rows[y].row[x])
                {
                    backgroundTiles[x, y] = new BackgroundTile(false, null);
                }
                else
                {
                    int randomIndex;
                    GameObject candy;

                    randomIndex = UnityEngine.Random.Range(0, candiesPrefabs.Length);

                    candy = Instantiate(candiesPrefabs[randomIndex], position, Quaternion.identity);
                    candy.GetComponent<Candy>().SetIndicies(x, y);
                    candy.transform.SetParent(candyParent.transform);

                    backgroundTiles[x, y] = new BackgroundTile(true, candy);
                    candiesToDestroy.Add(candy);
                }

            }
        }
        //Its not a match so dont remove and refill the board
        if (CheckBoard())
        {
            //Debug.Log("We have matches lets recreate the board");
            InitializeBoard();
        }
        else if (IsDeadLocked())
        {
            Debug.LogError("acabou a possibilidade de match na criacao");
            InitializeBoard();
        }
    }

    private void DestroyCandies()
    {
        if (candiesToDestroy != null)
        {
            foreach (GameObject candy in candiesToDestroy)
            {
                Destroy(candy);
            }
            candiesToDestroy.Clear();
        }
    }

    #endregion
    #region Matching Logic
    //Check the board to look for a match
    public bool CheckBoard()
    {
        if (manager.isGameEnded)
            return false;

        bool hasMatched = false;

        //cleaning the list created by the class that have the candies matched
        candiesToRemove.Clear();

        foreach (BackgroundTile backgroundTile in backgroundTiles)
        {
            if (backgroundTile.candy != null)
            {
                backgroundTile.candy.GetComponent<Candy>().isMatched = false;
            }
        }

        //check every candy inside the board 
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //Checking if the tile is usable and have a candy
                if (backgroundTiles[x, y].isUsable == true && backgroundTiles[x, y].candy != null)
                {
                    //Then proceed to get candy class in tile
                    Candy candy;

                    candy = backgroundTiles[x, y].candy.GetComponent<Candy>();
                    candy.wasSelected = false;
                    hasMatched = CheckCandy(hasMatched, candy);
                }
            }
        }
        //Its possible to do a match now?

        if (IsDeadLocked())
        {
            Debug.LogError("acabou a possibilidade de match");

            InitializeBoard();
        }
        return hasMatched;
    }

    private bool CheckCandy(bool hasMatched, Candy candy)
    {
        if (!candy.isMatched)
        {
            //For every candy check for connections, return a class that have the connected candies and the direction of the match
            MatchResult matchedCandies;
            MatchResult superMatchedCandies;

            matchedCandies = IsConnected(candy);

            if (matchedCandies.connectedCandies.Count >= 3)
            {
                //Take the class inside matchesCandies var and put in the method supermatch to see if has a super match
                 superMatchedCandies = SuperMatch(matchedCandies);

                candiesToRemove.AddRange(superMatchedCandies.connectedCandies);

                foreach (Candy cand in superMatchedCandies.connectedCandies)
                {
                    cand.isMatched = true;
                }
                return true;
            }
        }
        return hasMatched;
    }

    private MatchResult SuperMatch(MatchResult _matchedResults)
    {
        //if the class used as a argument here have a horizontal or long horizontal match
        if (_matchedResults.direction == MatchDirection.Horizontal || _matchedResults.direction == MatchDirection.LongHorizontal)
        {
            //loop through the candies in my match
            foreach (Candy candy in _matchedResults.connectedCandies)
            {

                //Create a new list of candies 'extra matches'
                List<Candy> extraConnectedCandies = new();
                //CheckDirection up
                CheckDirection(candy, new Vector2Int(0, 1), extraConnectedCandies);
                //CheckDirection down
                CheckDirection(candy, new Vector2Int(0, -1), extraConnectedCandies);

                //If the checkDirection has find connectedCandies and put them into the List that we created before
                if (extraConnectedCandies.Count >= 2)
                {
                    //Debug.Log("I have a super Horizontal Match");
                    //Putting the candies that formed the horizontal match into the list that have the other perpendicular candies
                    extraConnectedCandies.AddRange(_matchedResults.connectedCandies);

                    //we've made a super match - retunr a new matchresult of type super
                    MatchResult match = new MatchResult
                    {
                        connectedCandies = extraConnectedCandies,
                        direction = MatchDirection.Super
                    };
                    return match;
                }
            }
            return new MatchResult
            {
                connectedCandies = _matchedResults.connectedCandies,
                direction = _matchedResults.direction
            };
        }

        //if the class used as a argument here have a vertical or long vertical match
        else if (_matchedResults.direction == MatchDirection.Vertical || _matchedResults.direction == MatchDirection.LongVertical)
        {
            //loop through the candies in my match
            foreach (Candy candy in _matchedResults.connectedCandies)
            {
                //Create a new list of candies 'extra matches'
                List<Candy> extraConnectedCandies = new();
                //CheckDirection left
                CheckDirection(candy, new Vector2Int(-1, 0), extraConnectedCandies);
                //CheckDirection right
                CheckDirection(candy, new Vector2Int(1, 0), extraConnectedCandies);

                //Do we have 2 or more extra matches.
                if (extraConnectedCandies.Count >= 2)
                {
                    // Debug.Log("I have a super Vertical Match");
                    extraConnectedCandies.AddRange(_matchedResults.connectedCandies);

                    //we've made a super match - retunr a new matchresult of type super
                    return new MatchResult
                    {
                        connectedCandies = extraConnectedCandies,
                        direction = MatchDirection.Super
                    };
                }
            }
            return new MatchResult
            {
                connectedCandies = _matchedResults.connectedCandies,
                direction = _matchedResults.direction
            };
        }
        return null;
    }

    //Is connected with the candies of same type
    MatchResult IsConnected(Candy candy)
    {
        //Creating a list of candies connected 
        List<Candy> connectedCandies = new();

        //The color setted in the enum inside the candy script
        CandyColor candyColor = candy.candyColor;

        //Adding the candy 
        connectedCandies.Add(candy);

        //check right until the boundaries hit (theres a while loop to never end the cicle) if the candy next is the same type do put them inside the list
        CheckDirection(candy, new Vector2Int(1, 0), connectedCandies);
        //check left until the boundaries hit (theres a while loop to never end the cicle) and if the candy next is the same type do put them inside the list
        CheckDirection(candy, new Vector2Int(-1, 0), connectedCandies);

        //Does the list have 3 candies?
        if (connectedCandies.Count == 3)
        {
            // Debug.Log("I have a normal horizontal match, the color of my match is: " + connectedCandies[0].candyColor);
            //Return the class that i've create below, and this class is going to collect the list of candies connected and the direction of the match
            return new MatchResult
            {
                connectedCandies = connectedCandies,
                direction = MatchDirection.Horizontal
            };
        }
        //Checking for more than 3 (Long horizontal match)
        else if (connectedCandies.Count > 3)
        {
            //Debug.Log("I have a long horizontal match, the color of my match is: " + connectedCandies[0].candyColor);

            return new MatchResult
            {
                connectedCandies = connectedCandies,
                direction = MatchDirection.LongHorizontal
            };
        };

        //clear out the list that contains the connected candies
        connectedCandies.Clear();


        //read again our initial candy
        connectedCandies.Add(candy);
        //The same Idea but vertical now
        //check up
        CheckDirection(candy, new Vector2Int(0, 1), connectedCandies);
        //check down
        CheckDirection(candy, new Vector2Int(0, -1), connectedCandies);

        if (connectedCandies.Count == 3)
        {
            //Debug.Log("I have a normal vertical match, the color of my match is:" + connectedCandies[0].candyColor);

            return new MatchResult
            {
                connectedCandies = connectedCandies,
                direction = MatchDirection.Vertical
            };
        }

        else if (connectedCandies.Count > 3)
        {
            // Debug.Log("I have a long vertical match, the color of my match is: " + connectedCandies[0].candyColor);

            return new MatchResult
            {
                connectedCandies = connectedCandies,
                direction = MatchDirection.LongVertical
            };
        }
        //if we had no matches
        else
        {
            //Return the class that i've create below, and this class is going to collect the list of candies connected (0) and the direction of the match (none)
            return new MatchResult
            {
                connectedCandies = connectedCandies,
                direction = MatchDirection.None
            };
        }
    }
    //Check Direction of this connection
    void CheckDirection(Candy candy, Vector2Int direction, List<Candy> connectedCandies)
    {
        //The variable candycolor has the value of the color of the candy passed at this method
        CandyColor candyColor = candy.candyColor;

        //the x position of this candy + the position of the next tile/candy
        //If I wanna check the right candy I do myPos + 1, finding the position of that candy
        int x = candy.xIndex + direction.x;
        int y = candy.yIndex + direction.y;

        //Check that we're within the boundaries of the board
        while (CheckBoundaries(x, y))
        {
            if (backgroundTiles[x, y].isUsable == true && backgroundTiles[x, y].candy != null)
            {
                //Debug.Log("Checking the candie in the x and y pos: " + x + " " + y);
                //Getting the candy next to us
                Candy otherCandy = backgroundTiles[x, y].candy.GetComponent<Candy>();

                //Does our candyColor Match? it must also not be matched
                if (!otherCandy.isMatched && otherCandy.candyColor == candyColor)
                {
                    //putting the other candy in the list of connedcted candies
                    connectedCandies.Add(otherCandy);

                    //Adding value to our x and y to continue the loop 
                    x += direction.x;
                    y += direction.y;
                }
                else break;
            }
            else break;
        }
    }

    #endregion
    #region Swapping candies
    //Select a candy
    public void SelectCandy(Candy _candy)
    {
        //If we don�t have a candy currently selected, then set the candy I just clicked to my selectedCandy
        if (selectedCandy == null)
        {
            selectedCandy = _candy;
            _candy.isClicked = true;
        }
        //If we select the same candy twice selectedCandy = null 
        else if (selectedCandy == _candy)
        {
            selectedCandy = null;
            _candy.isClicked = false;
        }
        //If selectedCandy is not null and its not in the current position, attempt a swap 
        //selectedCandy back to null 
        else if (selectedCandy != _candy)
        {
            SwapCandy(selectedCandy, _candy);
            selectedCandy = null;
        }
    }

    //Swap a candy  - logic
    private void SwapCandy(Candy _currentCandy, Candy _targetCandy)
    {
        _currentCandy.isClicked = false;
        //if its not adjacent dont do anything
        if (!IsAdjacent(_currentCandy, _targetCandy))
        {
            return;
        }

        if(_currentCandy != null && _targetCandy != null)
        {
            //DoSwap
            DoSwap(_currentCandy, _targetCandy);
        }


        isProcessingMove = true;
        //StartCourotine ProcessMatches, have I got a match? If yes do one thing if no do another thing 
        StartCoroutine(ProcessMatches(_currentCandy, _targetCandy));

    }

    //Do swap
    private void DoSwap(Candy _currentCandy, Candy _targetCandy)
    {

        //storing the location of the first candy 
        GameObject temp = backgroundTiles[_currentCandy.xIndex, _currentCandy.yIndex].candy;

        //Changing the index of the current candy to the target one 
        backgroundTiles[_currentCandy.xIndex, _currentCandy.yIndex].candy = backgroundTiles[_targetCandy.xIndex, _targetCandy.yIndex].candy;

        backgroundTiles[_targetCandy.xIndex, _targetCandy.yIndex].candy = temp;

        //update indicies
        //Save the current x and y in the temp
        int tempXIndex = _currentCandy.xIndex;
        int tempYIndex = _currentCandy.yIndex;
        //Now change the currentcandy x and y to the target x and y 
        _currentCandy.xIndex = _targetCandy.xIndex;
        _currentCandy.yIndex = _targetCandy.yIndex;
        //And now change the x and y of the target to the current. It means that I stored the current candy X and Y in temp variable, and then swap both current and target X's and Y's
        _targetCandy.xIndex = tempXIndex;
        _targetCandy.yIndex = tempYIndex;

        //Move the candy 
        _currentCandy.MoveToTarget(backgroundTiles[_targetCandy.xIndex, _targetCandy.yIndex].candy.transform.position);
        _targetCandy.MoveToTarget(backgroundTiles[_currentCandy.xIndex, _currentCandy.yIndex].candy.transform.position);
    }

    private IEnumerator ProcessMatches(Candy _currentCandy, Candy _targetCandy)
    {
        //Wait for some seconds
        yield return new WaitForSeconds(0.3f);

        if (CheckBoard())
        {
            _targetCandy.wasSelected = true;
            _currentCandy.wasSelected = true;
            //Start coroutine to process the matches in our turn
            StartCoroutine(ProcessTurnOnMatchedBoard(true, false));
        }   
        else
        {
            DoSwap(_currentCandy, _targetCandy);
        }
        
        isProcessingMove = false;
    }
    //isAdjecent
    private bool IsAdjacent(Candy _currentCandy, Candy _targetCandy)
    {
        return Mathf.Abs(_currentCandy.xIndex - _targetCandy.xIndex) + Mathf.Abs(_currentCandy.yIndex - _targetCandy.yIndex) == 1;
    }
    //ProcessMatches
    #endregion
    #region Cascading Candies

    //When we got a match and after that another match, this one was automatic so the player did nothing to happen
    public IEnumerator ProcessTurnOnMatchedBoard(bool _subtractMoves, bool _multiplyPoints)
    {
        //Create the variable responsible for storing the points
        int points;

        //Every candy that is on the list will have the variable asMatched as false
        foreach (Candy candyToRemove in candiesToRemove)
        {
            candyToRemove.isMatched = false;
        }

        //If its the match caused by the cascade calculate the points and double it 
        points = CalculatePoints(_multiplyPoints);

        //Calculate the points and subtract the moves if necessary 
        manager.ProcessTurn(points, _subtractMoves, candiesToRemove);

        //Remove the candies in the list
        RemoveAndRefill(candiesToRemove);


        yield return new WaitForSeconds(0.1f);

        //Check teh board again, if we have a match start this method again but doenst subtract the move and multiply the points
        if (CheckBoard())
        {
            StartCoroutine(ProcessTurnOnMatchedBoard(false, true));
        }
    }

    private void RemoveAndRefill(List<Candy> _candiesToRemove)
    {
        //Removing the candy and clearing the board at that location
        foreach (Candy candy in _candiesToRemove)
        {
            //Getting it's x and y indicies and storing them
            int _xIndex = candy.xIndex;
            int _yIndex = candy.yIndex;

            //Destroy the candy
            Destroy(candy.gameObject);
            candiesToDestroy.Remove(candy.gameObject);
            backgroundTiles[_xIndex, _yIndex] = new BackgroundTile(true, null);
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //In the empty space that is usable and have no candies refill it
                if (backgroundTiles[x, y].candy == null && backgroundTiles[x, y].isUsable == true)
                {

                    //Debug.Log("The location X: " + x + " Y: " + y + " is empty, attempting to refill it");
                    RefillCandy(x, y);
                }
            }
        }
    }
    private void RefillCandy(int x, int y)
    {
        //y offset 
        int yOffset = 1;

        //While the cell above our current cell have no candy and is usable and we're below the height of the board OR the cell above us is not usable and we're below the height of the boards
        while ((y + yOffset < height && backgroundTiles[x, y + yOffset].candy == null && backgroundTiles[x, y + yOffset].isUsable == true) || (y + yOffset < height && backgroundTiles[x, y + yOffset].isUsable == false))
        {
            //Increment y offset
            //Debug.Log("The candy abover me is null and Im not at the top of the board yet, so Ill add to my yOffset and try again " + yOffset + "and my x and y position is: " + x + " " + y);
            yOffset++;
        }


        if (y + yOffset < height && backgroundTiles[x, y + yOffset].candy != null)
        {
            //we've found a candy
            Candy candyAbove = backgroundTiles[x, y + yOffset].candy.GetComponent<Candy>();

            //Getting the location
            Vector3 targetPos = new Vector3(x - spacingX, y - spacingY, candyAbove.transform.position.z);

            //Move it to the correct location
            candyAbove.MoveToTarget(targetPos);

            //Update indicies
            candyAbove.SetIndicies(x, y);

            //Update our backgroundTiles
            backgroundTiles[x, y] = backgroundTiles[x, y + yOffset];

            //Set the location the candy came from to null
            backgroundTiles[x, y + yOffset] = new BackgroundTile(true, null);
        }

        //If we've hit the top of the board without finding a candy 
        if (y + yOffset == height)
        {
            //Debug.Log("I have reached the top of the board without finding a candy at the x location of:" + x + "and y: " + y);
            SpawnCandyAtTop(x);
        }
    }

    private void SpawnCandyAtTop(int x)
    {
        int index = FindIndexOfLowestNull(x);
        //How mucn we need to go down
        int locationToMoveTo = height - index;

        //Debug.Log("About to spawn a candy, ideally i'd like to put it in the index of: " + index + " and the x pos is:" + x );

        //Get a random candy
        int randomIndex = UnityEngine.Random.Range(0, candiesPrefabs.Length);
        GameObject newCandy = Instantiate(candiesPrefabs[randomIndex], new Vector2(x - spacingX, height - spacingY), Quaternion.identity);

        //If the board need to be reseted this new cnady will be destroyed
        candiesToDestroy.Add(newCandy);

        //Setting the parent
        newCandy.transform.SetParent(candyParent.transform);

        //Set indicies
        newCandy.GetComponent<Candy>().SetIndicies(x, index);

        //Set it on the board
        backgroundTiles[x, index] = new BackgroundTile(true, newCandy);

        //Move it to that position
        Vector3 newTargetPos = new Vector3(newCandy.transform.position.x, newCandy.transform.position.y - locationToMoveTo, newCandy.transform.position.z);

        newCandy.GetComponent<Candy>().MoveToTarget(newTargetPos);
    }

    //Finds the lowest part of the grid
    private int FindIndexOfLowestNull(int x)
    {
        int lowestNull = 99;
        for (int y = height - 1; y >= 0; y--)
        {
            if (backgroundTiles[x, y].candy == null && backgroundTiles[x, y].isUsable == true)
            {
                lowestNull = y;
            }
        }
        return lowestNull;
    }
    #endregion
    #region Points

    private int CalculatePoints(bool _multiplyPoints)
    {
        int points = 0;

        foreach (Candy candyToRemove in candiesToRemove)
        {
            int randomNumber = UnityEngine.Random.Range(09, 19);
            points += 100 + randomNumber;
        }

        if (_multiplyPoints) multiplier *= 2;
        else multiplier = 1;
        points *= multiplier;

        //Debug.Log("I made " + points + " points");
        return points;
    }

    #endregion
    #region DeadLock
    private bool IsDeadLocked()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //loop trough all the board checking right and up on every candy 
                if (backgroundTiles[x, y].isUsable == true && backgroundTiles[x, y].candy != null && x < width - 1)
                {
                    if (SwitchCheck(x, y, Vector2Int.right)) return false;
                }
                if (backgroundTiles[x, y].isUsable == true && backgroundTiles[x, y].candy != null && y < height - 1)
                {
                    if (SwitchCheck(x, y, Vector2Int.up)) return false;
                }
            }
        }

        manager.DeadLocked();

        return true; // Is dead locked
    }

    //Switch two candies to see if forms a match, if not, revert the actions
    private bool SwitchCheck(int _x, int _y, Vector2Int _direction)
    {
        SwitchCandies(_x, _y, _direction);

        if (CheckForMatches())
        {
            SwitchCandies(_x, _y, _direction);
            return true;
        }

        SwitchCandies(_x, _y, _direction);
        return false;
    }

    //Switch every candy on the board
    private void SwitchCandies(int _x, int _y, Vector2Int _direction)
    {
        //OtherX and OtherY is the position of the current candy + the direction we want to switch
        int otherX; 
        int otherY;
        GameObject temp;

        otherY = _y + _direction.y;
        otherX = _x + _direction.x;

        if (CheckBoundaries(otherX, otherY))
        {
            temp = backgroundTiles[otherX, otherY].candy;

            backgroundTiles[otherX, otherY].candy = backgroundTiles[_x, _y].candy;
            backgroundTiles[_x, _y].candy = temp;
        }
    }

    //Check if theres any possible match in the board
    private bool CheckForMatches()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //Check on every backgroundTile, and if its usable and have a candy
                if (backgroundTiles[x, y].isUsable == true && backgroundTiles[x, y].candy != null && x < width - 2)
                {
                    //Check if the candy at the right and the candy after that exists
                    if (backgroundTiles[x + 1, y].isUsable == true && backgroundTiles[x + 1, y].candy != null &&
                        backgroundTiles[x + 2, y].isUsable == true && backgroundTiles[x + 2, y].candy != null)
                    {
                        //Take the component candy of these 3 background tiles
                        Candy candy = backgroundTiles[x, y].candy.GetComponent<Candy>();
                        Candy otherCandy1 = backgroundTiles[x + 1, y].candy.GetComponent<Candy>();
                        Candy otherCandy2 = backgroundTiles[x + 2, y].candy.GetComponent<Candy>();

                        //if all of the 3 candies are the same, itspossible to do a match
                        if (candy.candyColor == otherCandy1.candyColor &&
                            candy.candyColor == otherCandy2.candyColor)
                        {
                            return true;
                        }
                    }
                }
                //Same logic but were checking up or down (i dont remember lol)
                if (backgroundTiles[x, y].isUsable == true && backgroundTiles[x, y].candy != null && y < height - 2)
                {
                    if (backgroundTiles[x, y + 1].isUsable == true && backgroundTiles[x, y + 1].candy != null &&
                        backgroundTiles[x, y + 2].isUsable == true && backgroundTiles[x, y + 2].candy != null)
                    {
                        Candy candy = backgroundTiles[x, y].candy.GetComponent<Candy>();
                        Candy otherCandy1 = backgroundTiles[x, y + 1].candy.GetComponent<Candy>();
                        Candy otherCandy2 = backgroundTiles[x, y + 2].candy.GetComponent<Candy>();

                        if (candy.candyColor == otherCandy1.candyColor &&
                            candy.candyColor == otherCandy2.candyColor)
                        {
                            return true;
                        }
                    }
                }
            }

        }
        return false;
    }
    #endregion
    #region Controls
    private void CheckClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit;
            Ray ray;
            Candy candy;

            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider != null && hit.collider.gameObject.GetComponent<Candy>())
            {
                //If we're already moving a piece
                if (isProcessingMove || manager.isPaused) 
                    return;

                candy = hit.collider.gameObject.GetComponent<Candy>();

                SelectCandy(candy);
            }
        }
    }
    #endregion

    public bool CheckBoundaries(int x, int y) => x >= 0 && x < width && y >= 0 && y < height;
}