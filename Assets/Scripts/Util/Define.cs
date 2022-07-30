using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public enum UIEvent
    {
        Click,
        Pressed,
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

    public enum ElementType
    {
        None = 0,
        Fire = 1,
        Water = 2,
        Earth = 3,
        Wind = 4,
    }

    public enum MonsterGrade
    {
        Normal = 0,
        Boss = 1,
    }

    public enum AnimState
    {
        None,
        Idle,
        Damage,
        
    }

    public const int TeamCountMax = 5;

    public static Color HitColor = new Color(1.0f, 0.5f, 0.5f, 1.0f);

    public Color ElementColor(ElementType elementType)
    {
        Color color = Color.white;

        switch (elementType)
        {
            case ElementType.Fire:
                color = Color.red;
                break;
            case ElementType.Water:
                color = Color.blue;
                break;
            case ElementType.Earth:
                color = Color.yellow;;
                break;
            case ElementType.Wind:
                color = Color.green;
                break;
            default:
                break;
        }

        return color;
    }

    public static Color PuzzleColor(PuzzleType puzzleType)
    {
        Color color = Color.white;

        switch (puzzleType)
        {
            case PuzzleType.Fire:
                color = Color.red;
                break;
            case PuzzleType.Water:
                color = Color.blue;
                break;
            case PuzzleType.Earth:
                color = Color.yellow;
                break;
            case PuzzleType.Wind:
                color = Color.green;
                break;
            default:
                break;
        }

        return color;
    }
}
