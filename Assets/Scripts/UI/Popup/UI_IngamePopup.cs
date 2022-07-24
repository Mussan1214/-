using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
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
        CharacterPanel,
    }

    enum Texts
    {
        TurnCountText,
    }

    enum Images
    {
        BossImage
    }
    
    private Vector2Int _mapSize = new Vector2Int(5, 5);
    private UI_TileItem[,] _uiTileItems;
    private List<UI_TileItem> _bingoItems = new List<UI_TileItem>();
    private List<UI_TileItem> _actionItems = new List<UI_TileItem>();
    
    private int _turn = 1;
    private int _turnActionCount = 2;

    private List<UI_CharacterItem> _characterItems = new List<UI_CharacterItem>();

    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        Bind<GameObject>(typeof(GameObjects));
        BindText(typeof(Texts));
        BindImage(typeof(Images));

        MakeMap();
        MakeCharacter();

        return true;
    }

    void OnDropItem(UI_TileItem item)
    {
        Debug.Log($"{item.Row}x{item.Col}");
        if (item != null && item.PuzzleItem != null)
        {
            item.PuzzleItem.SetState(Define.PuzzleState.Impossible);

            // Managers.UI.MakeSubItem<UI_PuzzleItem>(Get<GameObject>((int) GameObjects.PuzzleItems).transform);

            // CheckBingo();
            // CheckBingoCount(item);

            CheckActionCount(item);
        }
    }

    private void CheckActionCount(UI_TileItem item)
    {
        item.Active(false);
        _actionItems.Add(item);
        
        _turnActionCount--;
        if (_turnActionCount == 1)
        {
            GameObject parent = Get<GameObject>((int) GameObjects.PuzzleItems);
            HorizontalLayoutGroup horizontal = parent.GetComponent<HorizontalLayoutGroup>();
            horizontal.enabled = false;
            
            int colDistance = 2 - item.Col;
            int rowDistance = 2 - item.Row;
            
            for (int row = 0; row < _uiTileItems.GetLength(0); row++)
            {
                for (int col = 0; col < _uiTileItems.GetLength(1); col++)
                {
                    UI_TileItem tileItem = _uiTileItems[row, col];
                    if (tileItem.PuzzleItem == null)
                    {
                        tileItem.Active(false);
                    }
                }
            }

            _uiTileItems[2 + rowDistance, 2 + colDistance].Active(true);
        }
        else if (_turnActionCount == 0)
        {
            CheckBingoCount(_actionItems[0]);
            CheckBingoCount(_actionItems[1]);
            _actionItems.Clear();

            for (int row = 0; row < _uiTileItems.GetLength(0); row++)
            {
                for (int col = 0; col < _uiTileItems.GetLength(1); col++)
                {
                    UI_TileItem tileItem = _uiTileItems[row, col];
                    if (tileItem.PuzzleItem == null)
                    {
                        tileItem.Active(true);
                    }
                }
            }

            _turnActionCount = 2;
            RefreshTurnCount();


            GameObject parent = Get<GameObject>((int) GameObjects.PuzzleItems);
            Managers.UI.MakeSubItem<UI_PuzzleItem>(parent.transform);
            Managers.UI.MakeSubItem<UI_PuzzleItem>(parent.transform);
            
            HorizontalLayoutGroup horizontal = parent.GetComponent<HorizontalLayoutGroup>();
            horizontal.enabled = true;
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform) horizontal.transform);
            horizontal.enabled = false;
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
            UI_TileItem tile = _uiTileItems[row - 1, tileItem.Col];
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
            UI_TileItem tile = _uiTileItems[row + 1, tileItem.Col];
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
            UI_TileItem tile = _uiTileItems[tileItem.Row, col - 1];
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
            UI_TileItem tile = _uiTileItems[tileItem.Row, col + 1];
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
            UI_TileItem tile = _uiTileItems[row - 1, col - 1];
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
            UI_TileItem tile = _uiTileItems[row + 1, col + 1];
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
            UI_TileItem tile = _uiTileItems[row + 1, col - 1];
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
            UI_TileItem tile = _uiTileItems[row - 1, col + 1];
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
        int tempIdx = 0;
        foreach (UI_TileItem bingoItem in _bingoItems)
        {
            Color particleColor =
                bingoItem.PuzzleItem.PuzzleType == Define.PuzzleType.Fire ? Color.red :
                bingoItem.PuzzleItem.PuzzleType == Define.PuzzleType.Water ? Color.blue :
                bingoItem.PuzzleItem.PuzzleType == Define.PuzzleType.Earth ? Color.yellow :
                bingoItem.PuzzleItem.PuzzleType == Define.PuzzleType.Wind ? Color.green :
                Color.white;
            
            GameObject go = Managers.Resource.Instantiate($"Particle/UI/UIParticle_001");
            ParticleSystem particleSystem = Utils.FindChild<ParticleSystem>(go, recursive: true);
            particleSystem.startColor = particleColor;

            go.transform.SetParent(gameObject.transform);
            go.transform.position = bingoItem.transform.position;

            refreshDelayTime = particleSystem.startLifetime;
            particleSystem.loop = true;

            GameObject targetObject = _characterItems[
                bingoItem.PuzzleItem.PuzzleType == Define.PuzzleType.Fire ? 0 :
                bingoItem.PuzzleItem.PuzzleType == Define.PuzzleType.Water ? 1 :
                bingoItem.PuzzleItem.PuzzleType == Define.PuzzleType.Earth ? 3 :
                bingoItem.PuzzleItem.PuzzleType == Define.PuzzleType.Wind ? 2 :
                4
            ].gameObject;
            
            Vector3[] paths = new Vector3[3];
            paths.SetValue(go.transform.position, 0);
            paths.SetValue(targetObject.transform.position, 2);
            paths.SetValue(new Vector3( paths[0].x + (paths[2].x - paths[0].x), paths[0].y + (paths[2].y - paths[0].y) * 0.5f, 0), 1);

            go.transform.DOPath(paths, 1.0f, PathType.CatmullRom).SetEase(Ease.OutCubic).SetDelay(1.0f).OnComplete(() =>
            {
                go.transform.DOScale(Vector3.one * 0.7f, 0.3f).OnComplete(() =>
                {
                    GameObject particleObject = Managers.Resource.Instantiate("Particle/UI/UIParticle_002");
                    particleObject.transform.SetParent(transform);
                    particleObject.transform.position = GetImage((int) Images.BossImage).transform.position;
                    
                    Vector3 originPosition = particleObject.transform.position;
                    originPosition.y -= 320.0f;

                    int degree = Random.Range(0, 360);
                    float radian = degree * Mathf.PI / 180;
                    float distance = Random.Range(10f, 150f);
                    originPosition.x += distance * Mathf.Cos(radian);
                    originPosition.y += distance * Mathf.Sin(radian);
                    
                    particleObject.transform.position = originPosition;
                    
                    ParticleSystem[] particleSystems = particleObject.GetComponentsInChildren<ParticleSystem>();
                    foreach (ParticleSystem system in particleSystems)
                    {
                        system.loop = true;
                        system.startColor = particleColor;

                        system.transform.DOScale(Vector3.one * 0.5f, 1.0f)
                            .OnComplete(() => Managers.Resource.Destroy(particleObject));
                    }

                    
                    GetImage((int) Images.BossImage).transform.DOPunchRotation(Vector3.one, 1.5f).SetDelay(0.5f).OnComplete(() =>
                        GetImage((int) Images.BossImage).transform.DORotate(Vector3.zero, 0.25f));
                    Managers.Resource.Destroy(go);
                });
            });

            // go.transform.DOMove(_characterItems[0].transform.position, 1.0f).SetDelay(particleSystem.startLifetime).OnComplete(() =>
            // {
            //     go.transform.DOScale(Vector3.one * 0.7f, 0.15f).OnComplete(() =>
            //     {
            //         Managers.Resource.Destroy(go);
            //     });
            // });
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
                _uiTileItems[row, col] = uiTileItem;
                _uiTileItems[row, col].SetInfo(row, col, OnDropItem);

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
            Managers.UI.MakeSubItem<UI_PuzzleItem>(parent.transform);
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
                    uiTileItem.Active(true);
                }
            }
        }
    }

    void RefreshTurnCount()
    {
        _turn++;
        TextMeshProUGUI text = GetText((int) Texts.TurnCountText);
        text.text = $"{_turn} Turn";

        text.gameObject.transform.DOPunchScale(Vector3.one, 0.5f, 1);
    }

    void MakeCharacter()
    {
        GameObject parent = GetObject((int) GameObjects.CharacterPanel);
        foreach (Transform t in parent.transform)
            Managers.Resource.Destroy(t.gameObject);

        for (int i = 0; i < 5; i++)
        {
            if (_characterItems.Count <= i)
                _characterItems.Add(Managers.UI.MakeSubItem<UI_CharacterItem>(parent.transform));

            Data.CharacterData characterData = null;
            Managers.Data.CharacterDict.TryGetValue(i + 1, out characterData);
            _characterItems[i].SetCharacterData(characterData);
        }
    }
}
