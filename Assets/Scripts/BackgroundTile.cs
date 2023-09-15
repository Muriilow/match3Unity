using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BackgroundTile : MonoBehaviour
{
    //All the dots objects in this array
    [SerializeField] public GameObject[] dots;
    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Initialize() 
    {

        //Random number that is going to determine what dot will be EX: dot[1]  = redDot
        int dotToUse = Random.Range(0, dots.Length);
        //Creating the gameObject
        GameObject dot = Instantiate(dots[dotToUse], transform.position, Quaternion.identity);
        //the parent is the background tile 
        dot.transform.parent = this.transform;
        //The name of the dot is the name of the background tile that create it
        dot.name = this.gameObject.name;
    }
}
