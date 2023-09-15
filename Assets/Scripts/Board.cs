using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    //How tall the board needs to be 
    [SerializeField] public int width;
    [SerializeField] public int height;

    //prefab of our tile
    [SerializeField] public GameObject tilePrefab;

    //2d array 
    private BackgroundTile[,] allTiles;
    void Start()
    {
        allTiles = new BackgroundTile[width, height];
        SetUp();
    }

    private void SetUp()
    {
        //Left to right
        for(int i = 0; i < width; i++)
        {
            //down to up
            for(int j = 0; j < height; j++)
            {
                //The float position of the board obj NOT THE TILE
                float fx = this.transform.position.x;
                float fy = this.transform.position.y;

                //When creating the tile it will have the position of the board as reference
                Vector2 tempPosition = new Vector2(fx + (float)i, fy + (float)j);
                //Putting the gameObject in the temp var backgroundTile
                GameObject backgroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity) as GameObject;
                //Saying that the father of this tile is the gameObject board 
                backgroundTile.transform.parent = this.transform;
                //Putting a specific name to this tile
                backgroundTile.name = "( " + i + "," + j + " )";
            }
        }
    }
}
