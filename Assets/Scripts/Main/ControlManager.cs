using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlManager : MonoBehaviour
{
    [SerializeField] private GameManager _manager;
    [SerializeField] private Board _board;
    private void Update()
    {
        CheckClick();
    }
    
    #region Controls
    private void CheckClick()
    {
        if (!Input.GetMouseButtonDown(0))
            return;
        
        RaycastHit2D hit;
        Ray ray;
        Candy candy;

        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        hit = Physics2D.Raycast(ray.origin, ray.direction);

        if (hit.collider != null && hit.collider.gameObject.GetComponent<Candy>())
        {
            //If we're already moving a piece
            if (_board.IsProcessingMove() || _manager.IsPaused) 
                return;

            candy = hit.collider.gameObject.GetComponent<Candy>();

            _board.SelectCandy(candy);
        }
    }
    #endregion
}
