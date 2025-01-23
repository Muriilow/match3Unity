using UnityEngine;

public class BackgroundTile
{
    public bool IsUsable { get; set; }
    public GameObject Candy { get; set; }
    
    //Constructor
    public BackgroundTile(bool isUsable, GameObject candy)
    {
        this.IsUsable = isUsable;
        this.Candy = candy;
    }
}
