using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    //How tall the board needs to be 
    [SerializeField] public int width;
    [SerializeField] public int height;

    //prefab of our tile
    public GameObject tilePrefab;
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
                Vector2 tempPosition = new Vector2(i, j);
                Instantiate(tilePrefab, tempPosition, Quaternion.identity);
            }
        }
    }
}
