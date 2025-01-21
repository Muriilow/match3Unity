using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class Board : MonoBehaviour
{
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
    [SerializeField] private bool _isProcessingMove;

    [SerializeField] private GameManager _manager;
    
    private BackgroundTile[,] _tiles;
    
    [SerializeField] private int _multiplier = 1;
    [SerializeField] private float _waitProcess = 0.3f;
    //List to hold all the candies in the board
    private List<GameObject> _candies = new();

    //If I clicked at a candy collider, do the SelectCandy method  
    private void Update()
    {
        CheckClick();
    }

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
        
        if (CheckBoard())
            InitializeBoard();
        
        else if (IsDeadLocked())
            InitializeBoard();
    }

    private void CreateCandies(Vector2 position, int x, int y)
    {
        int randomIndex;
        GameObject candy;

        randomIndex = UnityEngine.Random.Range(0, _candiesPrefabs.Length);

        candy = Instantiate(_candiesPrefabs[randomIndex], position, Quaternion.identity);
        candy.GetComponent<Candy>().SetIndicies(x, y);
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
    public bool CheckBoard()
    {
        if (_manager.isGameEnded)
            return false;

        bool hasMatched = false;

        //cleaning the list created by the class that have the candies matched
        _candiesToRemove.Clear();

        foreach (BackgroundTile tile in _tiles)
        {
            if (tile.candy != null)
                tile.candy.GetComponent<Candy>().isMatched = false;
        }

        //check every candy inside the board 
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                //Checking if the tile is usable and have a candy
                if (_tiles[x, y].isUsable != true || _tiles[x, y].candy == null)
                    continue;
                
                Candy candy;

                candy = _tiles[x, y].candy.GetComponent<Candy>();
                candy.wasSelected = false;
                    
                hasMatched = CheckCandy(hasMatched, candy);
            }
        }
        
        //It's possible to do a match now?
        if (IsDeadLocked())
        {
            //Debug.LogError("No match possibility");
            InitializeBoard();
        }
        
        return hasMatched;
    }

    //Getting the candy and checking for every match possibility, if we find a match 
    //Set the isMatched variable to true and return true
    private bool CheckCandy(bool hasMatched, Candy candy)
    {
        MatchResult matchedCandies;
        MatchResult superMatchedCandies;
        
        if (candy.isMatched)
            return hasMatched;

        matchedCandies = IsConnected(candy);

        if (matchedCandies.connectedCandies.Count < 3)
            return hasMatched;
        
        //Check if we have more candies connected
        superMatchedCandies = SuperMatch(matchedCandies);

        _candiesToRemove.AddRange(superMatchedCandies.connectedCandies);

        foreach (Candy cand in superMatchedCandies.connectedCandies)
            cand.isMatched = true;
        
        return true;
    }

    //check if the match has more than 3 candies connected, return the same match if otherwise
    //TODO: refactor this 
    private MatchResult SuperMatch(MatchResult _matchedResults)
    {
        switch (_matchedResults.direction)
        {
            case MatchDirection.Horizontal:
            case MatchDirection.LongHorizontal:
            {
                foreach (Candy candy in _matchedResults.connectedCandies)
                {
                    List<Candy> extraConnectedCandies = new();
                    
                    //CheckDirection up
                    CheckDirection(candy, new Vector2Int(0, 1), extraConnectedCandies);
                    //CheckDirection down
                    CheckDirection(candy, new Vector2Int(0, -1), extraConnectedCandies);

                    if (extraConnectedCandies.Count >= 2)
                    {
                        //Debug.Log("I have a super Horizontal Match");
                        extraConnectedCandies.AddRange(_matchedResults.connectedCandies);

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
            
            case MatchDirection.Vertical:
            case MatchDirection.LongVertical:
            {
                foreach (Candy candy in _matchedResults.connectedCandies)
                {
                    List<Candy> extraConnectedCandies = new();
                    
                    //CheckDirection left
                    CheckDirection(candy, new Vector2Int(-1, 0), extraConnectedCandies);
                    //CheckDirection right
                    CheckDirection(candy, new Vector2Int(1, 0), extraConnectedCandies);

                    if (extraConnectedCandies.Count >= 2)
                    {
                        // Debug.Log("I have a super Vertical Match");
                        extraConnectedCandies.AddRange(_matchedResults.connectedCandies);

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
            default:
                return null;
        }
    }

    //Check if the candy have connections horizontally or vertically 
    MatchResult IsConnected(Candy candy)
    {
        //The color setted in the enum inside the candy script
        CandyColor candyColor;
        List<Candy> connectedCandies = new();

        
        candyColor= candy.candyColor;
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
        
        x = candy.xIndex + direction.x;
        y = candy.yIndex + direction.y;

        //Check that we're within the boundaries of the board
        while (CheckBoundaries(x, y))
        {
            if (_tiles[x, y].isUsable == true && _tiles[x, y].candy != null)
            {
                Candy otherCandy = _tiles[x, y].candy.GetComponent<Candy>();

                if (!otherCandy.isMatched && otherCandy.candyColor == candyColor)
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
    private void SelectCandy(Candy candy)
    {
        if (_selectedCandy == null)
        {
            _selectedCandy = candy;
            candy.isClicked = true;
        }
        else if (_selectedCandy == candy)
        {
            _selectedCandy = null;
            candy.isClicked = false;
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
        currentCandy.isClicked = false;
        if (!IsAdjacent(currentCandy, targetCandy))
            return;

        if(currentCandy != null && targetCandy != null)
            DoSwap(currentCandy, targetCandy);

        _isProcessingMove = true;
        StartCoroutine(ProcessMatches(currentCandy, targetCandy));
    }

    //Swapping the information between both candies 
    private void DoSwap(Candy currentCandy, Candy targetCandy)
    {
        GameObject temp;
        int tempXIndex, tempYIndex;
        
        temp = _tiles[currentCandy.xIndex, currentCandy.yIndex].candy;
        _tiles[currentCandy.xIndex, currentCandy.yIndex].candy = _tiles[targetCandy.xIndex, targetCandy.yIndex].candy;
        _tiles[targetCandy.xIndex, targetCandy.yIndex].candy = temp;

        //Swap the indexes of each candy 
        tempXIndex = currentCandy.xIndex;
        tempYIndex = currentCandy.yIndex;
        
        currentCandy.xIndex = targetCandy.xIndex;
        currentCandy.yIndex = targetCandy.yIndex;
        
        targetCandy.xIndex = tempXIndex;
        targetCandy.yIndex = tempYIndex;

        //Make each candy move 
        currentCandy.MoveToTarget(_tiles[targetCandy.xIndex, targetCandy.yIndex].candy.transform.position);
        targetCandy.MoveToTarget(_tiles[currentCandy.xIndex, currentCandy.yIndex].candy.transform.position);
    }

    private IEnumerator ProcessMatches(Candy currentCandy, Candy targetCandy)
    {
        yield return new WaitForSeconds(0.3f);

        //If we find a match process the points and move on, otherwise swap to their positions again 
        if (CheckBoard())
        {
            targetCandy.wasSelected = true;
            currentCandy.wasSelected = true;
            
            StartCoroutine(ProcessMatch(true, false));
        }   
        else
            DoSwap(currentCandy, targetCandy);
        
        _isProcessingMove = false;
    }

    private bool IsAdjacent(Candy currentCandy, Candy targetCandy)
    {
        int currentX = currentCandy.xIndex;
        int currentY = currentCandy.yIndex;
        int targetX = targetCandy.xIndex;
        int targetY = targetCandy.yIndex;
        
        return Mathf.Abs(currentX - targetX) + Mathf.Abs(currentY - targetY) == 1;
    }

    #endregion
    #region Cascading Candies

    //When we got a match and after that another match, this one was automatic so the player did nothing to happen
    private IEnumerator ProcessMatch(bool subtractMoves, bool multiplyPoints)
    {
        int points;

        //Every candy that is on the list will have the variable asMatched as false
        foreach (Candy candyToRemove in _candiesToRemove)
            candyToRemove.isMatched = false;

        points = CalculatePoints(multiplyPoints);
        _manager.ProcessTurn(points, subtractMoves, _candiesToRemove);

        //Refill the board
        RemoveAndRefill(_candiesToRemove);

        yield return new WaitForSeconds(_waitProcess);

        //Look for net matches 
        if (CheckBoard())
        {
            StartCoroutine(ProcessMatch(false, true));
        }
    }

    //Remove the candies and refill the board with new ones
    private void RemoveAndRefill(List<Candy> candiesToRemove)
    {
        foreach (Candy candy in candiesToRemove)
        {
            int xIndex = candy.xIndex;
            int yIndex = candy.yIndex;

            Destroy(candy.gameObject);
            _candies.Remove(candy.gameObject);
            _tiles[xIndex, yIndex] = new BackgroundTile(true, null);
        }

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                if (_tiles[x, y].candy == null && _tiles[x, y].isUsable == true)
                    RefillCandy(x, y);
            }
        }
    }
    private void RefillCandy(int x, int y)
    {
        int yOffset;
        
        yOffset = y + 1;
        
        //Get the highest position in the board without candy  
        while ((yOffset < _height && _tiles[x, yOffset].candy == null && _tiles[x, yOffset].isUsable)
               || (yOffset < _height && !_tiles[x, yOffset].isUsable))
            yOffset++;
       
        //If we found a candy 
        if (yOffset < _height && _tiles[x, yOffset].candy != null)
        {
            Candy candyAbove;
            Vector3 targetPos;
            
            candyAbove = _tiles[x, yOffset].candy.GetComponent<Candy>();
            targetPos = new Vector3(x - _spacingX, y - _spacingY, candyAbove.transform.position.z);

            //Move it to the correct location
            candyAbove.MoveToTarget(targetPos);
            candyAbove.SetIndicies(x, y);

            _tiles[x, y] = _tiles[x, yOffset];

            //Set the location the candy came from to null
            _tiles[x, yOffset] = new BackgroundTile(true, null);
        }

        //If we've hit the top of the board without finding a candy 
        if (yOffset == _height)
            SpawnCandyAtTop(x);
    }

    private void SpawnCandyAtTop(int x)
    {
        int index;
        int locationToMoveTo;
        int randomIndex;
        GameObject newCandy;
        Vector3 newTargetPos;
        
        index = FindIndexOfLowestNull(x);
        locationToMoveTo = _height - index;
        
        randomIndex = UnityEngine.Random.Range(0, _candiesPrefabs.Length);
        newCandy = Instantiate( _candiesPrefabs[randomIndex],
                             new Vector2(x - _spacingX, _height - _spacingY), 
                                Quaternion.identity);

        //Updating information 
        _candies.Add(newCandy);
        newCandy.transform.SetParent(_candyParent.transform);
        newCandy.GetComponent<Candy>().SetIndicies(x, index);
        _tiles[x, index] = new BackgroundTile(true, newCandy);

        //Move it to that position
        newTargetPos = new Vector3( newCandy.transform.position.x,
                                    newCandy.transform.position.y - locationToMoveTo,
                                    newCandy.transform.position.z);
        
        newCandy.GetComponent<Candy>().MoveToTarget(newTargetPos);
    }

    //Finds the lowest part of the grid
    private int FindIndexOfLowestNull(int x)
    {
        int lowestNull = 99;
        
        for (int y = _height - 1; y >= 0; y--)
        {
            if (_tiles[x, y].candy == null && _tiles[x, y].isUsable == true)
                lowestNull = y;
        }
        return lowestNull;
    }
    #endregion
    #region Points

    private int CalculatePoints(bool multiplyPoints)
    {
        int points = 0;

        foreach (Candy candy in _candiesToRemove)
        {
            int randomNum = UnityEngine.Random.Range(09, 19);
            points += 100 + randomNum;
        }

        if (multiplyPoints) _multiplier *= 2;
        else _multiplier = 1;
        points *= _multiplier;

        //Debug.Log("I made " + points + " points");
        return points;
    }

    #endregion
    #region DeadLock
    private bool IsDeadLocked()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                //loop trough all the board checking right and up on every candy 
                if (_tiles[x, y].isUsable == true && _tiles[x, y].candy != null && x < _width - 1)
                {
                    if (SwitchCheck(x, y, Vector2Int.right)) return false;
                }
                if (_tiles[x, y].isUsable == true && _tiles[x, y].candy != null && y < _height - 1)
                {
                    if (SwitchCheck(x, y, Vector2Int.up)) return false;
                }
            }
        }

        _manager.DeadLocked();

        return true; // Is dead locked
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
            temp = _tiles[otherX, otherY].candy;

            _tiles[otherX, otherY].candy = _tiles[x, y].candy;
            _tiles[x, y].candy = temp;
        }
    }

    //Check if theres any possible match in the board
    private bool CheckForMatches()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                //Check on every backgroundTile, and if its usable and have a candy
                if (_tiles[x, y].isUsable == true && _tiles[x, y].candy != null && x < _width - 2)
                {
                    //Check if the candy at the right and the candy after that exists
                    if (_tiles[x + 1, y].isUsable == true && _tiles[x + 1, y].candy != null &&
                        _tiles[x + 2, y].isUsable == true && _tiles[x + 2, y].candy != null)
                    {
                        //Take the component candy of these 3 background tiles
                        Candy candy = _tiles[x, y].candy.GetComponent<Candy>();
                        Candy otherCandy1 = _tiles[x + 1, y].candy.GetComponent<Candy>();
                        Candy otherCandy2 = _tiles[x + 2, y].candy.GetComponent<Candy>();

                        //if all of the 3 candies are the same, itspossible to do a match
                        if (candy.candyColor == otherCandy1.candyColor &&
                            candy.candyColor == otherCandy2.candyColor)
                        {
                            return true;
                        }
                    }
                }
                //Same logic but were checking up or down (i dont remember lol)
                if (_tiles[x, y].isUsable == true && _tiles[x, y].candy != null && y < _height - 2)
                {
                    if (_tiles[x, y + 1].isUsable == true && _tiles[x, y + 1].candy != null &&
                        _tiles[x, y + 2].isUsable == true && _tiles[x, y + 2].candy != null)
                    {
                        Candy candy = _tiles[x, y].candy.GetComponent<Candy>();
                        Candy otherCandy1 = _tiles[x, y + 1].candy.GetComponent<Candy>();
                        Candy otherCandy2 = _tiles[x, y + 2].candy.GetComponent<Candy>();

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
                if (_isProcessingMove || _manager.isPaused) 
                    return;

                candy = hit.collider.gameObject.GetComponent<Candy>();

                SelectCandy(candy);
            }
        }
    }
    #endregion

    public bool CheckBoundaries(int x, int y) => x >= 0 && x < _width && y >= 0 && y < _height;
}