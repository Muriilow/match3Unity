using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;
public class Candy : MonoBehaviour
{
    [SerializeField] private Animator _animation;
    public CandyColor candyColor;
    
    //My x and y position
    public int XIndex { get; set; }
    public int YIndex { get; set; }

    //If the candy is going to show up the points
    public bool WasSelected { get; set; }
    public bool IsMatched { get; set; } 
    public bool IsClicked { get; set; }
    public bool IsMoving { get; private set; }
    
    public void Start()
    {
        _animation = GetComponent<Animator>();
    }

    public void Update()
    {
        if (IsClicked) 
            _animation.SetBool("isSelected", true);
        else 
            _animation.SetBool("isSelected", false);
    }
    //MoveToTarget
    public void MoveToTarget(Vector2 targetPos)
    {
        StartCoroutine(MoveCoroutine(targetPos));
    }
    //MoveCoroutine
    private IEnumerator MoveCoroutine(Vector2 targetPos)
    {
        IsMoving = true;
        float duration = 0.2f;

        Vector2 startPosition = transform.position;
        float elaspedTime = 0f;

        while(elaspedTime < duration)
        {
            float t = elaspedTime / duration;

            transform.position = Vector2.Lerp(startPosition, targetPos, t);
            elaspedTime += Time.deltaTime;

            yield return null;
        }
        transform.position = targetPos;
        IsMoving = false;
    }
}

