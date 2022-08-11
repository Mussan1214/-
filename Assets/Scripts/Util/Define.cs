using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
        None = 0,
        
        Init,
        
        Play,
        
        Wait,
        
        NextTurn,
    }

    public enum BlockState
    {
        Possible,
        Impossible
    }

    public enum BlockType
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
    public const int TurnActionCountMax = 2;

    public static Color AlphaZero = new Color(1.0f, 1.0f, 1.0f, 0.0f);
    public static Color HitColor = new Color(1.0f, 0.5f, 0.5f, 1.0f);

    public static Color HalfRed = new Color(1.0f, 0.5f, 0.5f, 1.0f);
    public static Color HalfBlue = new Color(0.5f, 0.5f, 1.0f, 1.0f);
    public static Color HalfYellow = new Color(1.0f, 0.92f, 0.5f, 1.0f);
    public static Color HalfGrren = new Color(0.5f, 1.0f, 0.5f, 1.0f);

    public static Color ElementColor(ElementType elementType, bool halfColor = false)
    {
        Color color = Color.white;

        switch (elementType)
        {
            case ElementType.Fire:
                color = halfColor ? HalfRed : Color.red;
                break;
            case ElementType.Water:
                color = halfColor ? HalfBlue : Color.blue;
                break;
            case ElementType.Earth:
                color = halfColor ? HalfYellow : Color.yellow;;
                break;
            case ElementType.Wind:
                color = halfColor ? HalfGrren : Color.green;
                break;
            default:
                break;
        }

        return color;
    }

    public static Color PuzzleColor(BlockType blockType)
    {
        Color color = Color.white;

        switch (blockType)
        {
            case BlockType.Fire:
                color = Color.red;
                break;
            case BlockType.Water:
                color = Color.blue;
                break;
            case BlockType.Earth:
                color = Color.yellow;
                break;
            case BlockType.Wind:
                color = Color.green;
                break;
            default:
                break;
        }

        return color;
    }
}
