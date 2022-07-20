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

    public enum PuzzleType
    {
        Possible,
        Impossible
    }
}
