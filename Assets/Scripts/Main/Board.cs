using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;

public class Board : MonoBehaviour
{
    private const int HighestPos = 99;
    //Define the size of the board
    [SerializeField] private int _width;
    [SerializeField] private int _height;

    //Define some spacing for the board
    private float _spacingX;
    private float _spacingY;
    
    [SerializeField] private ArrayLayout _arrayLayout;

    //List to hold the value of the candies that will be destroyed 
    [SerializeField] private List<Candy> _candiesToRemove = new();

    //Get a reference to our candies prefabs
    [SerializeField] private GameObject[] _candiesPrefabs;
    [SerializeField] private GameObject _candyParent;
    
    [SerializeField] private Candy _selectedCandy;

    [SerializeField] private GameManager _manager;
    
    private BackgroundTile[,] _tiles;
    
    [SerializeField] private float _waitProcess = 0.3f;
    //List to hold all the candies in the board
    private List<GameObject> _candies = new();

    #region Initialize the game
    void Start()
    {
        InitializeBoard();
    }

    //Clears the board and then fill it again with random candies
    private void InitializeBoard()
    {
        DestroyCandies();
        _tiles = new BackgroundTile[_width, _height];

        //Variables that control where should the candies be placed
        _spacingX = (float)(_width - 1) / 2 - 3; //-0.5
        _spacingY = (float)(_height - 1) / 2; //3.5

        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                Vector2 position;

                position = new Vector2(x - _spacingX, y - _spacingY);

                //If a tile is marked in the inspect, make a background tile unusable 
                if (_arrayLayout.rows[y].row[x])
                    _tiles[x, y] = new BackgroundTile(false, null);
                else
                    CreateCandies(position, x, y);
            }
        }
        
        if (CheckBoard() || IsDeadLocked())
            InitializeBoard();
    }

    private void CreateCandies(Vector2 position, int x, int y)
    {
        int randomIndex;
        GameObject candy;

        randomIndex = UnityEngine.Random.Range(0, _candiesPrefabs.Length);

        candy = Instantiate(_candiesPrefabs[randomIndex], position, Quaternion.identity);
        candy.GetComponent<Candy>().XIndex = x;
        candy.GetComponent<Candy>().YIndex = y;
        candy.transform.SetParent(_candyParent.transform);

        _tiles[x, y] = new BackgroundTile(true, candy);
        _candies.Add(candy);
    }

    private void DestroyCandies()
    {
        if (_candies != null)
        {
            foreach (GameObject candy in _candies)
            {
                Destroy(candy);
            }
            _candies.Clear();
        }
    }

    #endregion
    
    #region Matching Logic
    //returns true if we find a match, false otherwise 
    private bool CheckBoard()
    {
        if (_manager.isGameEnded)
            return false;

        bool hasMatched = false;

        //cleaning the list created by the class that have the candies matched
        _candiesToRemove.Clear();

        foreach (BackgroundTile tile in _tiles)
        {
            if (tile.Candy != null)
                tile.Candy.GetComponent<Candy>().IsMatched = false;
        }

        //check every candy inside the board 
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                //Checking if the tile is usable and have a candy
                if (_tiles[x, y].IsUsable != true || _tiles[x, y].Candy == null)
                    continue;
                
                Candy candy;

                candy = _tiles[x, y].Candy.GetComponent<Candy>();
                candy.WasSelected = false;
                    
                hasMatched = CheckCandy(hasMatched, candy);
            }
        }
        
        return hasMatched;
    }

    //Getting the candy and checking for every match possibility, if we find a match 
    //Set the isMatched variable to true and return true
    private bool CheckCandy(bool hasMatched, Candy candy)
    {
        MatchResult matchedCandies;
        MatchResult superMatchedCandies;
        
        if (candy.IsMatched)
            return hasMatched;

        matchedCandies = IsConnected(candy);

        if (matchedCandies.connectedCandies.Count < 3)
            return hasMatched;
        
        //Check if we have more candies connected
        superMatchedCandies = SuperMatch(matchedCandies);

        _candiesToRemove.AddRange(superMatchedCandies.connectedCandies);

        foreach (Candy cand in superMatchedCandies.connectedCandies)
            cand.IsMatched = true;
        
        return true;
    }

    //check if the match has more than 3 candies connected, return the same match if otherwise
    //TODO: refactor this 
    private MatchResult SuperMatch(MatchResult matchedResults)
    {
        switch (matchedResults.direction)
        {
            case MatchDirection.Horizontal:
            case MatchDirection.LongHorizontal:
            {
                foreach (Candy candy in matchedResults.connectedCandies)
                {
                    List<Candy> extraConnectedCandies = new();
                    
                    //CheckDirection up
                    CheckDirection(candy, new Vector2Int(0, 1), extraConnectedCandies);
                    //CheckDirection down
                    CheckDirection(candy, new Vector2Int(0, -1), extraConnectedCandies);

                    if (extraConnectedCandies.Count >= 2)
                    {
                        //Debug.Log("I have a super Horizontal Match");
                        extraConnectedCandies.AddRange(matchedResults.connectedCandies);

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
                    connectedCandies = matchedResults.connectedCandies,
                    direction = matchedResults.direction
                };
            }
            
            case MatchDirection.Vertical:
            case MatchDirection.LongVertical:
            {
                foreach (Candy candy in matchedResults.connectedCandies)
                {
                    List<Candy> extraConnectedCandies = new();
                    
                    //CheckDirection left
                    CheckDirection(candy, new Vector2Int(-1, 0), extraConnectedCandies);
                    //CheckDirection right
                    CheckDirection(candy, new Vector2Int(1, 0), extraConnectedCandies);

                    if (extraConnectedCandies.Count >= 2)
                    {
                        // Debug.Log("I have a super Vertical Match");
                        extraConnectedCandies.AddRange(matchedResults.connectedCandies);

                        return new MatchResult
                        {
                            connectedCandies = extraConnectedCandies,
                            direction = MatchDirection.Super
                        };
                    }
                }
                return new MatchResult
                {
                    connectedCandies = matchedResults.connectedCandies,
                    direction = matchedResults.direction
                };
            }
            default:
                return null;
        }
    }

    //Check if the candy have connections horizontally or vertically 
    private MatchResult IsConnected(Candy candy)
    {
        //The color set in the enum inside the candy script
        List<Candy> connectedCandies = new();

        
        connectedCandies.Add(candy);

        CheckDirection(candy, new Vector2Int(1, 0), connectedCandies);
        CheckDirection(candy, new Vector2Int(-1, 0), connectedCandies);

        if (connectedCandies.Count == 3)
        {
            return new MatchResult
            {
                connectedCandies = connectedCandies,
                direction = MatchDirection.Horizontal
            };
        }
        else if (connectedCandies.Count > 3)
        {
            return new MatchResult
            {
                connectedCandies = connectedCandies,
                direction = MatchDirection.LongHorizontal
            };
        };
        
        connectedCandies.Clear();
        connectedCandies.Add(candy);
        
        //Check up 
        CheckDirection(candy, new Vector2Int(0, 1), connectedCandies);
        //Check down
        CheckDirection(candy, new Vector2Int(0, -1), connectedCandies);

        if (connectedCandies.Count == 3)
        {
            return new MatchResult
            {
                connectedCandies = connectedCandies,
                direction = MatchDirection.Vertical
            };
        }
        else if (connectedCandies.Count > 3)
        {
            return new MatchResult
            {
                connectedCandies = connectedCandies,
                direction = MatchDirection.LongVertical
            };
        }
        else
        {
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
        CandyColor candyColor;
        int x, y;
        
        candyColor = candy.candyColor;
        
        x = candy.XIndex + direction.x;
        y = candy.YIndex + direction.y;

        //Check that we're within the boundaries of the board
        while (CheckBoundaries(x, y))
        {
            if (_tiles[x, y].IsUsable == true && _tiles[x, y].Candy != null)
            {
                Candy otherCandy = _tiles[x, y].Candy.GetComponent<Candy>();

                if (!otherCandy.IsMatched && otherCandy.candyColor == candyColor)
                {
                    connectedCandies.Add(otherCandy);

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
    //Check for some variables of the candy to see if the player wants to mark the candy or the contrary 
    public void SelectCandy(Candy candy)
    {
        if (_selectedCandy == null)
        {
            _selectedCandy = candy;
            candy.IsClicked = true;
        }
        else if (_selectedCandy == candy)
        {
            _selectedCandy = null;
            candy.IsClicked = false;
        }
        //Attempt a swap if it's other candy
        else if (_selectedCandy != candy)
        {
            SwapCandy(_selectedCandy, candy);
            _selectedCandy = null;
        }
    }

    //Swap a candy 
    private void SwapCandy(Candy currentCandy, Candy targetCandy)
    {
        currentCandy.IsClicked = false;
        if (!IsAdjacent(currentCandy, targetCandy))
            return;

        if(currentCandy != null && targetCandy != null)
            DoSwap(currentCandy, targetCandy);

        StartCoroutine(ProcessMatches(currentCandy, targetCandy));
    }

    //Swapping the information between both candies 
    private void DoSwap(Candy currentCandy, Candy targetCandy)
    {
        GameObject temp;
        int tempXIndex, tempYIndex;
        
        temp = _tiles[currentCandy.XIndex, currentCandy.YIndex].Candy;
        _tiles[currentCandy.XIndex, currentCandy.YIndex].Candy = _tiles[targetCandy.XIndex, targetCandy.YIndex].Candy;
        _tiles[targetCandy.XIndex, targetCandy.YIndex].Candy = temp;

        //Swap the indexes of each candy 
        tempXIndex = currentCandy.XIndex;
        tempYIndex = currentCandy.YIndex;
        
        currentCandy.XIndex = targetCandy.XIndex;
        currentCandy.YIndex = targetCandy.YIndex;
        
        targetCandy.XIndex = tempXIndex;
        targetCandy.YIndex = tempYIndex;

        //Make each candy move 
        currentCandy.MoveToTarget(_tiles[targetCandy.XIndex, targetCandy.YIndex].Candy.transform.position);
        targetCandy.MoveToTarget(_tiles[currentCandy.XIndex, currentCandy.YIndex].Candy.transform.position);
    }

    #endregion

    #region Process Matches

    private IEnumerator ProcessMatches(Candy currentCandy, Candy targetCandy)
    {
        yield return new WaitForSeconds(0.3f);

        //If we find a match process the points and move on, otherwise swap to their positions again 
        if (CheckBoard())
        {
            targetCandy.WasSelected = true;
            currentCandy.WasSelected = true;
            
            StartCoroutine(DoMatch(true, false));
        }   
        else
            DoSwap(currentCandy, targetCandy);
        
    }
    //When we got a match and after that another match, this one was automatic so the player did nothing to happen
    private IEnumerator DoMatch(bool subtractMoves, bool multiplyPoints)
    {
        //Every candy that is on the list will have the variable asMatched as false
        foreach (Candy candyToRemove in _candiesToRemove)
            candyToRemove.IsMatched = false;

        _manager.ProcessTurn(subtractMoves, _candiesToRemove, multiplyPoints);

        //Refill the board
        RemoveAndRefill(_candiesToRemove);

        yield return new WaitForSeconds(_waitProcess);
        
        //Look for net matches 
        if (CheckBoard())
            StartCoroutine(DoMatch(false, true));
        else if(IsDeadLocked())
            InitializeBoard();
        
    }
    
    #endregion
    
    #region Cascading Candies

    //Remove the candies and refill the board with new ones
    private void RemoveAndRefill(List<Candy> candiesToRemove)
    {
        foreach (Candy candy in candiesToRemove)
        {
            int xIndex = candy.XIndex;
            int yIndex = candy.YIndex;

            Destroy(candy.gameObject);
            _candies.Remove(candy.gameObject);
            _tiles[xIndex, yIndex] = new BackgroundTile(true, null);
        }

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                if (_tiles[x, y].Candy == null && _tiles[x, y].IsUsable == true)
                    RefillCandy(x, y);
            }
        }
    }
    
    //Find the first candy above the current position, and move it down
    //If there's no candy above the current position spawn one. 
    private void RefillCandy(int x, int y)
    {
        int yOffset;
        
        yOffset = y + 1;
        
        //Get the highest position in the board without candy  
        while ((yOffset < _height && _tiles[x, yOffset].Candy == null && _tiles[x, yOffset].IsUsable)
               || (yOffset < _height && !_tiles[x, yOffset].IsUsable))
            yOffset++;
       
        //If we found a candy 
        if (yOffset < _height && _tiles[x, yOffset].Candy != null)
            MoveCandyDown(x, y, yOffset);

        //If we've hit the top of the board without finding a candy 
        if (yOffset == _height) 
            SpawnCandyAtTop(x);
    }

    private void MoveCandyDown(int x, int y, int yOffset)
    {
        Candy candyAbove;
        Vector3 targetPos;
            
        candyAbove = _tiles[x, yOffset].Candy.GetComponent<Candy>();
        targetPos = new Vector3(x - _spacingX, y - _spacingY, candyAbove.transform.position.z);

        //Move it to the correct location
        candyAbove.MoveToTarget(targetPos);
        candyAbove.XIndex = x;
        candyAbove.YIndex = y;

        _tiles[x, y] = _tiles[x, yOffset];

        _tiles[x, yOffset] = new BackgroundTile(true, null);
    }

    //Spawn a candy and put it to the lowest position without a candy 
    private void SpawnCandyAtTop(int x)
    {
        int index;
        int locationToMoveTo;
        int randomIndex;
        GameObject newCandy;
        Vector3 newTargetPos;
        Candy candyClass;
        
        index = FindIndexOfLowestNull(x);
        locationToMoveTo = _height - index;
        
        randomIndex = UnityEngine.Random.Range(0, _candiesPrefabs.Length);
        newCandy = Instantiate( _candiesPrefabs[randomIndex],
                             new Vector2(x - _spacingX, _height - _spacingY), 
                                Quaternion.identity);

        candyClass = newCandy.GetComponent<Candy>();
        
        //Updating information 
        _candies.Add(newCandy);
        newCandy.transform.SetParent(_candyParent.transform);
        
        candyClass.XIndex = x;
        candyClass.YIndex = index;
        
        _tiles[x, index] = new BackgroundTile(true, newCandy);

        //Move it to that position
        newTargetPos = new Vector3( newCandy.transform.position.x,
                                    newCandy.transform.position.y - locationToMoveTo,
                                    newCandy.transform.position.z);
        
        candyClass.MoveToTarget(newTargetPos);
    }

    //Finds the lowest part of the grid without a candy 
    private int FindIndexOfLowestNull(int x)
    {
        int lowestNull = HighestPos;
        
        for (int y = _height - 1; y >= 0; y--)
        {
            if (_tiles[x, y].Candy == null && _tiles[x, y].IsUsable == true)
                lowestNull = y;
        }
        
        return lowestNull;
    }
    #endregion

    #region DeadLock
    
    //Change every candy in the board and check every time until we find at least one match 
    private bool IsDeadLocked()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                if (_tiles[x, y].IsUsable == true && _tiles[x, y].Candy != null && x < _width - 1)
                {
                    if (SwitchCheck(x, y, Vector2Int.right))
                        return false;
                }
                if (_tiles[x, y].IsUsable == true && _tiles[x, y].Candy != null && y < _height - 1)
                {
                    if (SwitchCheck(x, y, Vector2Int.up))
                        return false;
                }
            }
        }

        _manager.DeadLocked();
        
        Debug.Log("Dead locked");
        return true;
    }

    //Switch two candies to see if forms a match, if not, revert the actions
    private bool SwitchCheck(int x, int y, Vector2Int direction)
    {
        SwitchCandies(x, y, direction);

        if (CheckForMatches())
        {
            SwitchCandies(x, y, direction);
            return true;
        }

        SwitchCandies(x, y, direction);
        return false;
    }

    //Check if there's any possible match in the board return it. 
    //It is important to check for matches without using the CheckBoard(),
    //because the same can change some properties of the candies 
    private bool CheckForMatches()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                if (HasMatched(x, y, Direction.Right))
                    return true;

                if (HasMatched(x, y, Direction.Up))
                    return true;
            }
        }
        
        return false;
    }
    //Switch every candy on the board
    private void SwitchCandies(int x, int y, Vector2Int direction)
    {
        //OtherX and OtherY is the position of the current candy + the direction we want to switch
        int otherX; 
        int otherY;
        GameObject temp;

        otherY = y + direction.y;
        otherX = x + direction.x;

        if (CheckBoundaries(otherX, otherY))
        {
            temp = _tiles[otherX, otherY].Candy;

            _tiles[otherX, otherY].Candy = _tiles[x, y].Candy;
            _tiles[x, y].Candy = temp;
        }
    }

    //Check for the right or up direction at the current position to 3 consecutive candies
    private bool HasMatched(int x, int y, Direction dir)
    {
        switch (dir)
        {
            case Direction.Right:
            {
                //Check on every backgroundTile, and if its usable and have a candy
                if (_tiles[x, y].IsUsable == true && _tiles[x, y].Candy != null && x < _width - 2)
                {
                    //Check if the candy at the right and the candy after that exists
                    if (CheckRightDir(x, y))
                        return false;

                    Candy candy = _tiles[x, y].Candy.GetComponent<Candy>();
                    Candy otherCandy1 = _tiles[x + 1, y].Candy.GetComponent<Candy>();
                    Candy otherCandy2 = _tiles[x + 2, y].Candy.GetComponent<Candy>();

                    //if all the 3 candies are the same, it's possible to do a match
                    if (candy.candyColor == otherCandy1.candyColor &&
                        candy.candyColor == otherCandy2.candyColor)
                        return true;
                }
                break;
            }
            case Direction.Up:
            {
                //Same logic but were checking up or down
                if (_tiles[x, y].IsUsable == true && _tiles[x, y].Candy != null && y < _height - 2)
                {
                    if (CheckUpDir(x, y))
                        return false;
                    
                    Candy candy = _tiles[x, y].Candy.GetComponent<Candy>();
                    Candy otherCandy1 = _tiles[x, y + 1].Candy.GetComponent<Candy>();
                    Candy otherCandy2 = _tiles[x, y + 2].Candy.GetComponent<Candy>();

                    if (candy.candyColor == otherCandy1.candyColor &&
                        candy.candyColor == otherCandy2.candyColor)
                        return true;
                }
                break;
            }
        }

        return false;
    }

    #endregion

    #region Checks

    public bool IsProcessingMove()
    {
        foreach (GameObject candy in _candies)
        {
            if (candy.GetComponent<Candy>().IsMoving)
                return true;
        }
        
        return false;
    }
    private bool CheckBoundaries(int x, int y) => x >= 0 && x < _width && y >= 0 && y < _height;
    
    private bool CheckRightDir(int x, int y)
    {
        return !_tiles[x + 1, y].IsUsable || _tiles[x + 1, y].Candy == null ||
               !_tiles[x + 2, y].IsUsable || _tiles[x + 2, y].Candy == null;
    }

    private bool CheckUpDir(int x, int y)
    {
        return !_tiles[x, y + 1].IsUsable || _tiles[x, y + 1].Candy == null ||
               !_tiles[x, y + 2].IsUsable || _tiles[x, y + 2].Candy == null;
    }
    
    private bool IsAdjacent(Candy currentCandy, Candy targetCandy)
    {
        int currentX = currentCandy.XIndex;
        int currentY = currentCandy.YIndex;
        int targetX = targetCandy.XIndex;
        int targetY = targetCandy.YIndex;
        
        return Mathf.Abs(currentX - targetX) + Mathf.Abs(currentY - targetY) == 1;
    }
    #endregion
}