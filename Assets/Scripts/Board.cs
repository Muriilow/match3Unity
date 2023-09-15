using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    //How tall the board needs to be 
    [SerializeField] public int width;
    [SerializeField] public int height;
    [SerializeField] public GameObject[] dots;

    public GameObject[,] allDots;
    private BackgroundTile[,] allTiles;

    //prefab of our tile
    [SerializeField] public GameObject tilePrefab;

    void Start()
    {
        allTiles = new BackgroundTile[width, height];
        allDots = new GameObject[width, height];
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
                //-------------------The tiles----------------------------//
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
                backgroundTile.name = "( " + i + "," + j + " tile )";

                //----------------------The dots------------------------------//

                //Random number that is going to determine what dot will be EX: dot[1]  = redDot
                int dotToUse = Random.Range(0, dots.Length);
                //Creating the gameObject
                GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                //the parent is the board
                dot.transform.parent = this.transform;
                //The name of the dot
                dot.name = "( " + i + "," + j + " dot )";
                /* this specific dot created in this loop will be
                put in a 2d array created in the start event of the
                size declared in the unity engine ( width height) */
                allDots[i, j] = dot;
            }
        }
    }
}
