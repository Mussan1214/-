using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UI_IngamePopup : UI_Popup
{
    enum GameObjects
    {
        BingoItems,
        PuzzleItems,
    }
    
    private UI_TileItem[,] _uiTileItems;
    private List<UI_TileItem> _bingoItems = new List<UI_TileItem>();

    private int count = 0;

    private Vector2Int _mapSize = new Vector2Int(5, 5);
    
    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        Bind<GameObject>(typeof(GameObjects));

        MakeMap();

        return true;
    }

    void OnDropItem(UI_TileItem item)
    {
        Debug.Log($"{item.Row}x{item.Col}");
        if (item != null && item.PuzzleItem != null)
        {
            item.PuzzleItem.SetState(Define.PuzzleState.Impossible);
            
            Managers.UI.MakeSubItem<UI_PuzzleItem>(Get<GameObject>((int) GameObjects.PuzzleItems).transform).name = $"{count++}";

            // CheckBingo();
            CheckBingoCount(item);
        }
    }

    void CheckBingoCount(UI_TileItem tileItem)
    {
        List<UI_TileItem> bingoItems = new List<UI_TileItem>();
        
        _bingoItems.Clear();
        
        _bingoItems.Add(tileItem);
        
        int rowFlag = 1;
        int colFlag = 1;
        int crossRightFlag = 1;
        int crossLeftFlag = 1;

        for (int row = tileItem.Row; row > 0; row--)
        {
            UI_TileItem tile = _uiTileItems[tileItem.Col, row - 1];
            if (tile.PuzzleItem != null && tile.PuzzleItem.PuzzleType == tileItem.PuzzleItem.PuzzleType)
            {
                rowFlag++;
                bingoItems.Add(tile);
            }
            else
                break;
        }

        for (int row = tileItem.Row; row < _mapSize.y - 1; row++)
        {
            UI_TileItem tile = _uiTileItems[tileItem.Col, row + 1];
            if (tile.PuzzleItem != null && tile.PuzzleItem.PuzzleType == tileItem.PuzzleItem.PuzzleType)
            {
                rowFlag++;
                bingoItems.Add(tile);
            }
            else
                break;
        }

        if (rowFlag > 2)
        {
            foreach (UI_TileItem bingoItem in bingoItems)
                _bingoItems.Add(bingoItem);
        }
        bingoItems.Clear();
        
        for (int col = tileItem.Col; col > 0; col--)
        {
            UI_TileItem tile = _uiTileItems[col - 1, tileItem.Row];
            if (tile.PuzzleItem != null && tile.PuzzleItem.PuzzleType == tileItem.PuzzleItem.PuzzleType)
            {
                colFlag++;
                bingoItems.Add(tile);
            }
            else
                break;
        }

        for (int col = tileItem.Col; col < _mapSize.x - 1; col++)
        {
            UI_TileItem tile = _uiTileItems[col + 1, tileItem.Row];
            if (tile.PuzzleItem != null && tile.PuzzleItem.PuzzleType == tileItem.PuzzleItem.PuzzleType)
            {
                colFlag++;
                bingoItems.Add(tile);
            }
            else
                break;
        }

        if (colFlag > 2)
        {
            foreach (UI_TileItem bingoItem in bingoItems)
                _bingoItems.Add(bingoItem);
        }
        bingoItems.Clear();

        for (int row = tileItem.Row, col = tileItem.Col; row > 0 && col > 0; row--, col--)
        {
            UI_TileItem tile = _uiTileItems[col - 1, row - 1];
            if (tile.PuzzleItem != null && tile.PuzzleItem.PuzzleType == tileItem.PuzzleItem.PuzzleType)
            {
                crossRightFlag++;
                bingoItems.Add(tile);
            }
            else
                break;
        }
        
        for (int row = tileItem.Row, col = tileItem.Col; row < _mapSize.y - 1 && col < _mapSize.x - 1; row++, col++)
        {
            UI_TileItem tile = _uiTileItems[col + 1, row + 1];
            if (tile.PuzzleItem != null && tile.PuzzleItem.PuzzleType == tileItem.PuzzleItem.PuzzleType)
            {
                crossRightFlag++;
                bingoItems.Add(tile);
            }
            else
                break;
        }

        if (crossRightFlag > 2)
        {
            foreach (UI_TileItem bingoItem in bingoItems)
                _bingoItems.Add(bingoItem);
        }
        bingoItems.Clear();

        for (int row = tileItem.Row, col = tileItem.Col; row < _mapSize.y - 1 & col > 0; row++, col--)
        {
            UI_TileItem tile = _uiTileItems[col - 1, row + 1];
            if (tile.PuzzleItem != null && tile.PuzzleItem.PuzzleType == tileItem.PuzzleItem.PuzzleType)
            {
                crossLeftFlag++;
                bingoItems.Add(tile);
            }
            else
                break;
        }

        for (int row = tileItem.Row, col = tileItem.Col; row > 0 && col < _mapSize.x - 1; row--, col++)
        {
            UI_TileItem tile = _uiTileItems[col + 1, row - 1];
            if (tile.PuzzleItem != null && tile.PuzzleItem.PuzzleType == tileItem.PuzzleItem.PuzzleType)
            {
                crossLeftFlag++;
                bingoItems.Add(tile);
            }
            else
                break;
        }

        if (crossLeftFlag > 2)
        {
            foreach (UI_TileItem bingoItem in bingoItems)
                _bingoItems.Add(bingoItem);
        }

        float refreshDelayTime = 0;
        foreach (UI_TileItem bingoItem in _bingoItems)
        {
            GameObject go = Managers.Resource.Instantiate($"Particle/UI/UIParticle_001");
            ParticleSystem particleSystem = Utils.FindChild<ParticleSystem>(go, recursive: true);
            particleSystem.startColor =
                bingoItem.PuzzleItem.PuzzleType == Define.PuzzleType.Fire ? Color.red :
                bingoItem.PuzzleItem.PuzzleType == Define.PuzzleType.Water ? Color.blue :
                bingoItem.PuzzleItem.PuzzleType == Define.PuzzleType.Earth ? Color.yellow :
                bingoItem.PuzzleItem.PuzzleType == Define.PuzzleType.Wind ? Color.green :
                Color.white;

            go.transform.SetParent(bingoItem.transform);
            go.transform.localPosition = Vector3.zero;

            refreshDelayTime = particleSystem.startLifetime;
        }

        int puzzleCount = 0;
        for (int row = 0; row < _uiTileItems.GetLength(0); row++)
        {
            for (int col = 0; col < _uiTileItems.GetLength(1); col++)
            {
                if (_uiTileItems[row, col].PuzzleItem != null)
                    puzzleCount++;
            }
        }

        if (puzzleCount == _mapSize.x * _mapSize.y)
        {
            Invoke("RefreshMap", refreshDelayTime);
        }

        Debug.Log($"rowFlag => {rowFlag}, colFlag => {colFlag}, crossRightFlag => {crossRightFlag}, crossLeftFlag => {crossLeftFlag}");
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

    void MakeMap()
    {
        _uiTileItems = new UI_TileItem[_mapSize.y, _mapSize.x];
        
        // 타일 제거
        GameObject parent = Get<GameObject>((int) GameObjects.BingoItems);
        foreach (Transform t in parent.transform)
            Managers.Resource.Destroy(t.gameObject);

        // 타일 생성
        for (int row = 0; row < _mapSize.y; row++)
        {
            for (int col = 0; col < _mapSize.x; col++)
            {
                UI_TileItem uiTileItem = Managers.UI.MakeSubItem<UI_TileItem>(parent.transform);
                _uiTileItems[col, row] = uiTileItem;
                _uiTileItems[col, row].SetInfo(row, col, OnDropItem);

                if (row == 0 || row == 4 || col == 0 || col == 4 || (row == 2 && col == 2))
                {
                    UI_PuzzleItem puzzleItem = Managers.UI.MakeSubItem<UI_PuzzleItem>(uiTileItem.transform);
                    RectTransform rectTransform = puzzleItem.GetComponent<RectTransform>();
                    rectTransform.anchoredPosition = Vector2.zero;
                    
                    uiTileItem.SetPuzzleItem(puzzleItem);
                    uiTileItem.PuzzleItem.SetState(Define.PuzzleState.Impossible);
                }
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
    }

    void RefreshMap()
    {
        for (int row = 0; row < _uiTileItems.GetLength(0); row++)
        {
            for (int col = 0; col < _uiTileItems.GetLength(1); col++)
            {
                UI_TileItem uiTileItem = _uiTileItems[row, col];
                if (row == 0 || row == 4 || col == 0 || col == 4 || (row == 2 && col == 2))
                {
                    uiTileItem.PuzzleItem.SetPuzzleType((Define.PuzzleType) Random.Range(1, 5));
                }
                else
                {
                    uiTileItem.SetPuzzleItem(null);
                }
            }
        }
    }
}
