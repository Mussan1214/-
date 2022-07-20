using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_IngamePopup : UI_Popup
{
    enum GameObjects
    {
        BingoItems,
        PuzzleItems,
    }
    
    private UI_TileItem[,] _uiTileItems;
    private List<UI_PuzzleItem> _uiPuzzleItems = new List<UI_PuzzleItem>();

    private int count = 0;
    
    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        Bind<GameObject>(typeof(GameObjects));

        GameObject parent = Get<GameObject>((int) GameObjects.BingoItems);
        foreach (Transform t in parent.transform)
            Managers.Resource.Destroy(t.gameObject);

        _uiTileItems = new UI_TileItem[5, 5];

        for (int row = 0; row < 5; row++)
        {
            for (int col = 0; col < 5; col++)
            {
                UI_TileItem uiTileItem = Managers.UI.MakeSubItem<UI_TileItem>(parent.transform);
                _uiTileItems[col, row] = uiTileItem;
                _uiTileItems[col, row].SetInfo(row, col, OnDropItem);
            }
        }


        parent = Get<GameObject>((int) GameObjects.PuzzleItems);
        foreach (Transform t in parent.transform)
            Managers.Resource.Destroy(t.gameObject);

        for (int i = 0; i < 4; i++)
        {
            UI_PuzzleItem puzzleItem = Managers.UI.MakeSubItem<UI_PuzzleItem>(parent.transform);
            puzzleItem.name = $"{count++}";;
        }

        return true;
    }

    void OnDropItem(UI_TileItem item)
    {
        Debug.Log($"{item.Row}x{item.Col}");
        if (item != null && item.PuzzleItem != null)
        {
            item.PuzzleItem.SetInfo(Define.PuzzleType.Impossible);
            
            Managers.UI.MakeSubItem<UI_PuzzleItem>(Get<GameObject>((int) GameObjects.PuzzleItems).transform).name = $"{count++}";

            CheckBingo();
        }
    }

    void CheckBingo()
    {
        int rowFlag, rowLineFlag;
        int colFlag, colLineFlag;
        int crossRightFlag = 0, crossRightLineFlag = 0;
        int crossLeftFlag = 0, crossLeftLineFlag = 0;
        int sum = 0;
        int row, col;
        
        for (row = 0; row < _uiTileItems.GetLength(0); row++)
        {
            rowFlag = 0;
            colFlag = 0;
            
            for (col = 0; col < _uiTileItems.GetLength(1); col++)
            {
                UI_TileItem uiTileItem = _uiTileItems[row, col];
                if (uiTileItem != null && uiTileItem.PuzzleItem != null)
                {
                    rowFlag++;
                }

                uiTileItem = _uiTileItems[col, row];
                if (uiTileItem != null && uiTileItem.PuzzleItem != null)
                {
                    colFlag++;
                }
            }

            if (colFlag == 5)
                sum++;

            if (rowFlag == 5)
                sum++;
            
            if (_uiTileItems[row, row].PuzzleItem != null)
                crossRightFlag++;

            if (_uiTileItems[5 - 1 - row, row].PuzzleItem != null)
                crossLeftFlag++;

            if (crossRightFlag == 5)
                sum++;

            if (crossLeftFlag == 5)
                sum++;
        }
        
        Debug.Log($"Bingo Count = {sum}");
    }
}
