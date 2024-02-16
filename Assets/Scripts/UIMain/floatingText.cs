using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class floatingText : MonoBehaviour
{
    public Vector3 randomIntensity = new Vector3(4, 2, 0);
    private float destroyTime = 3f;
    void Start()
    {
        GetComponent<Renderer>().material.color  = new Color(0,0 ,0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
