using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
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

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
    public enum CandyColor
    {
        PeDeMlk,
        Snickers,
        ChocolateTriang,
        MentaCandy,
        BolinhoCandy
    }
}