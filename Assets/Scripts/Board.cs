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
    }
}
