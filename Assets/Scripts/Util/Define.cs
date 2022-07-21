using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public enum UIEvent
    {
        Click,
        // Pressed,
        PointerDown,
        PointerUp,
        
        BeginDrag,
        Drag,
        EndDrag,
        
        PointerEnter,
        OnDrop,
        PointerExit,
    }

    public enum Scene
    {
        Unknown,
        Main,
        Game
    }

    public enum PuzzleState
    {
        Possible,
        Impossible
    }

    public enum PuzzleType
    {
        None = 0,
        Fire = 1,
        Water = 2,
        Earth = 3,
        Wind = 4,
    }
}
