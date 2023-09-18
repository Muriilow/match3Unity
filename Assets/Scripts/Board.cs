using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    //Define the size of the board
    public int width = 6;
    public int height = 8;

    //Define some spacing for the board
    public float spacingX;
    public float spacingY;
    //Get a reference to our candies prefabs
    public GameObject[] candiesPrefabs;
    //Get a reference to the collection of backgroundTile board + GO
    private BackgroundTile[,]  backgroundTiles;
    public GameObject candiesBoardGO;
    //layoutArray
    public ArrayLayout arrayLayout;
    //public static of board
    public static Board Instance;

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
        //Settign the size of this 2d array in this variable
        backgroundTiles = new BackgroundTile[width, height];

        spacingX = (float)(width - 1) / 2; //2.5
        spacingY = (float)(height - 1) / 2; //3.5

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++) 
            {
                //pos for the candy 
                Vector2 position = new Vector2(x - spacingX, y - spacingY);
                /*If I checked the buttons in the inspector of this obj, the tile with the
                same pos as the inspector button will not work and will not have a candy on it*/
                if (arrayLayout.rows[y].row[x])
                {
                    backgroundTiles[x, y] = new BackgroundTile(false, null);
                }
                else 
                {
                    //random number
                    int randomIndex = Random.Range(0, candiesPrefabs.Length);
                
                    GameObject candy = Instantiate(candiesPrefabs[randomIndex], position, Quaternion.identity);
                    candy.GetComponent<Candy>().SetIndicies(x, y);
                    backgroundTiles[x, y] = new BackgroundTile(true, candy);
                }
            }
        }
        CheckBoard();
    }

    //If we have a match 
    public bool CheckBoard()
    {
        Debug.Log("Checking Board");
        //If we have at least 1 check at the board and we have a match 
        bool hasMatched = false;

        List<Candy> candiesToRemove = new();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //Checking if the tile is usable
                if (backgroundTiles[x, y].isUsable)
                {
                    //Then proceed to get candy class in tile
                    Candy candy = backgroundTiles[x, y].candy.GetComponent<Candy>(); 

                    //Ensure its not matched
                    if(!candy.isMatched)
                    {
                        //Run some matching logic

                        MatchResult matchedCandies= IsConnected(candy);

                        //If we had a match 
                        if(matchedCandies.connectedCandies.Count <= 3)
                        {
                            //Complex matching

                            //put them in a list that we've created early in this function 
                            candiesToRemove.AddRange(matchedCandies.connectedCandies);

                            //And for each candy set their variable hasMatched to true (!= isMatched inside the candy script)
                            foreach (Candy cand in matchedCandies.connectedCandies)
                            {
                                hasMatched = true;
                            }
                        }
                    }
                }
            }
        } 
        return hasMatched;
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

        //check right
        CheckDirection(candy, new Vector2Int(1, 0), connectedCandies);
        //check left
        CheckDirection(candy, new Vector2Int(-1, 0), connectedCandies);
        //have we made a 3 match? (Horizontal Match)
        if(connectedCandies.Count == 3)
        {
            Debug.Log("I have a normal horizontal match, the color of my match is: " + connectedCandies[0].candyColor);
            //Return the class that i've create below, and this class is going to collect the list of candies connected and the direction of the match
            return new MatchResult
            {
                connectedCandies = connectedCandies,
                direction = MatchDirection.Horizontal
            };
        }
        //Checking for more than 3 (Long horizontal match)
        else if(connectedCandies.Count > 3)
        {
            Debug.Log("I have a long horizontal match, the color of my match is: " + connectedCandies[0].candyColor);
            //Return the class that i've create below, and this class is going to collect the list of candies connected and the direction of the match
            return new MatchResult
            {
                connectedCandies = connectedCandies,
                direction = MatchDirection.LongHorizontal
            };
        };
        //clear out the connectedCandies
        connectedCandies.Clear();
        //read our initial potion
        connectedCandies.Add(candy);

        //check up
        CheckDirection(candy, new Vector2Int(0, 1), connectedCandies);
        //check down
        CheckDirection(candy, new Vector2Int(0, -1), connectedCandies);
        //have we made a 3 match? (Vertical Match)
        if (connectedCandies.Count == 3)
        {
            Debug.Log("I have a normal vertical match, the color of my match is:" + connectedCandies[0].candyColor);
            //Return the class that i've create below, and this class is going to collect the list of candies connected and the direction of the match
            return new MatchResult
            {
                connectedCandies = connectedCandies,
                direction = MatchDirection.Vertical
            };
        }
        //Checking for more than 3 (Long vertical match)
        else if (connectedCandies.Count > 3)
        {
            Debug.Log("I have a long vertical match, the color of my match is: " + connectedCandies[0].candyColor);
            //Return the class that i've create below, and this class is going to collect the list of candies connected and the direction of the match
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
        while(x >= 0 && x < width && y >= 0 && y < height)
        {
            if (backgroundTiles[x, y].isUsable)
            {
                //Getting the candy next to us
                Candy otherCandy = backgroundTiles[x, y].candy.GetComponent<Candy>();

                //Does our candyColor Match? it must also not be matched
                if(!otherCandy.isMatched && otherCandy.candyColor == candyColor)
                {
                    //putting the other candy in the list of connedcted candies
                    connectedCandies.Add(otherCandy);

                    //Adding value to our x and y to continue the loop 
                    x += direction.x;
                    y += direction.y;

                    otherCandy.isMatched = true;
                    candy.isMatched = true;

                }
                else 
                { break; }
            }
            else
            { break; }
        }
    }
}

//To get easy to read the match result
public class MatchResult
{
    public List<Candy> connectedCandies;
    public MatchDirection direction;
}

public enum MatchDirection
{
    Vertical,
    Horizontal,
    LongVertical,
    LongHorizontal,
    Super,
    None
}