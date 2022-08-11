using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using static Define;

public class UI_IngamePopup : UI_Popup
{
    enum GameObjects
    {
        BingoItems,
        BlockItems,
        CharacterPanel,
        MonsterPanel,
    }

    enum Texts
    {
        TurnCountText,
    }

    enum CanvasGroups
    {
        BlockItems,
    }
    
    #region Puzzle
    public PuzzleState PuzzleState { get; private set; }
    
    private Vector2Int _mapSize = new Vector2Int(5, 5);
    private int _turn = 1;
    private int _turnActionCount = 2;
    
    private UI_TileItem[,] _uiTileItems;
    private List<UI_TileItem> _bingoItems = new List<UI_TileItem>();
    private List<UI_TileItem> _actionItems = new List<UI_TileItem>();
    #endregion
    
    #region Battle
    private List<UI_CharacterItem> _characterItems = new List<UI_CharacterItem>();
    private MonsterController _monsterController;
    #endregion

    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        BindObject(typeof(GameObjects));
        BindText(typeof(Texts));
        Bind<CanvasGroup>(typeof(CanvasGroups));
        
        SetPuzzleState(PuzzleState.Init);

        return true;
    }
    
    void Update()
    {
        switch (PuzzleState)
        {
            case PuzzleState.Wait:
                break;
            
            default:
                break;
        }
    }

    public void SetPuzzleState(PuzzleState puzzleState)
    {
        Debug.Log($"SetPuzzleState => {puzzleState}");
        
        PuzzleState = puzzleState;

        switch (puzzleState)
        {
            case PuzzleState.Init:
                _mapSize = new Vector2Int(5, 5);
                _turn = 1;
                _turnActionCount = 0;
                
                MakeMap();
                MakeCharacter();

                SetPuzzleState(PuzzleState.Play);
                break;
            
            case PuzzleState.Play:
                Get<CanvasGroup>((int) CanvasGroups.BlockItems).blocksRaycasts = true;
                break;
            
            case PuzzleState.Wait:
                Get<CanvasGroup>((int) CanvasGroups.BlockItems).blocksRaycasts = false;
                break;
            
            case PuzzleState.NextTurn:
                for (int row = 0; row < _uiTileItems.GetLength(0); row++)
                {
                    for (int col = 0; col < _uiTileItems.GetLength(1); col++)
                    {
                        UI_TileItem tileItem = _uiTileItems[row, col];
                        if (tileItem.BlockItem == null)
                        {
                            tileItem.Active(true);
                        }
                    }
                }
            
                RefreshTurnCount();
                SetPuzzleState(PuzzleState.Play);
                break;
            
            default:
                Debug.Log($"해당 상태 변화를 정의해주세요 => {puzzleState}");
                break;
        }
    }

    private IEnumerator SetPuzzleStateCoroutine(PuzzleState puzzleState, float delay)
    {
        yield return new WaitForSeconds(delay);
        SetPuzzleState(puzzleState);
    }

    private void CheckActionCount(UI_TileItem item)
    {
        _actionItems.Add(item);
        
        _turnActionCount++;
        if (_turnActionCount == TurnActionCountMax)
        {
            CheckBingoCount();
        }
        else
        {
            GameObject parent = Get<GameObject>((int) GameObjects.BlockItems);
            HorizontalLayoutGroup horizontal = parent.GetComponent<HorizontalLayoutGroup>();
            horizontal.enabled = false;
            
            int colDistance = 2 - item.Col;
            int rowDistance = 2 - item.Row;
            
            for (int row = 0; row < _uiTileItems.GetLength(0); row++)
            {
                for (int col = 0; col < _uiTileItems.GetLength(1); col++)
                {
                    UI_TileItem tileItem = _uiTileItems[row, col];
                    if (tileItem.BlockItem == null)
                    {
                        tileItem.Active(false);
                    }
                }
            }

            _uiTileItems[2 + rowDistance, 2 + colDistance].Active(true);
        }
    }

    void CheckBingoCount()
    {
        SetPuzzleState(PuzzleState.Wait);

        _bingoItems.Clear();
        _bingoItems.AddRange(_actionItems);
        
        #region Bingo Rule
        foreach (var actionItem in _actionItems)
        {
            List<UI_TileItem> bingoItems = new List<UI_TileItem>();
            
            int rowFlag = 1;
            int colFlag = 1;
            int crossRightFlag = 1;
            int crossLeftFlag = 1;

            for (int row = actionItem.Row; row > 0; row--)
            {
                UI_TileItem tile = _uiTileItems[row - 1, actionItem.Col];
                if (tile.BlockItem != null && tile.BlockItem.BlockType == actionItem.BlockItem.BlockType)
                {
                    rowFlag++;
                    bingoItems.Add(tile);
                }
                else
                    break;
            }

            for (int row = actionItem.Row; row < _mapSize.y - 1; row++)
            {
                UI_TileItem tile = _uiTileItems[row + 1, actionItem.Col];
                if (tile.BlockItem != null && tile.BlockItem.BlockType == actionItem.BlockItem.BlockType)
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
            
            for (int col = actionItem.Col; col > 0; col--)
            {
                UI_TileItem tile = _uiTileItems[actionItem.Row, col - 1];
                if (tile.BlockItem != null && tile.BlockItem.BlockType == actionItem.BlockItem.BlockType)
                {
                    colFlag++;
                    bingoItems.Add(tile);
                }
                else
                    break;
            }

            for (int col = actionItem.Col; col < _mapSize.x - 1; col++)
            {
                UI_TileItem tile = _uiTileItems[actionItem.Row, col + 1];
                if (tile.BlockItem != null && tile.BlockItem.BlockType == actionItem.BlockItem.BlockType)
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

            for (int row = actionItem.Row, col = actionItem.Col; row > 0 && col > 0; row--, col--)
            {
                UI_TileItem tile = _uiTileItems[row - 1, col - 1];
                if (tile.BlockItem != null && tile.BlockItem.BlockType == actionItem.BlockItem.BlockType)
                {
                    crossRightFlag++;
                    bingoItems.Add(tile);
                }
                else
                    break;
            }
            
            for (int row = actionItem.Row, col = actionItem.Col; row < _mapSize.y - 1 && col < _mapSize.x - 1; row++, col++)
            {
                UI_TileItem tile = _uiTileItems[row + 1, col + 1];
                if (tile.BlockItem != null && tile.BlockItem.BlockType == actionItem.BlockItem.BlockType)
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

            for (int row = actionItem.Row, col = actionItem.Col; row < _mapSize.y - 1 & col > 0; row++, col--)
            {
                UI_TileItem tile = _uiTileItems[row + 1, col - 1];
                if (tile.BlockItem != null && tile.BlockItem.BlockType == actionItem.BlockItem.BlockType)
                {
                    crossLeftFlag++;
                    bingoItems.Add(tile);
                }
                else
                    break;
            }

            for (int row = actionItem.Row, col = actionItem.Col; row > 0 && col < _mapSize.x - 1; row--, col++)
            {
                UI_TileItem tile = _uiTileItems[row - 1, col + 1];
                if (tile.BlockItem != null && tile.BlockItem.BlockType == actionItem.BlockItem.BlockType)
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
        }
        #endregion
        
        _actionItems.Clear();
        
        float refreshDelayTime = 0;
        foreach (UI_TileItem bingoItem in _bingoItems)
        {
            Color particleColor = PuzzleColor(bingoItem.BlockItem.BlockType);

            GameObject go = Managers.Resource.Instantiate($"Particle/UI/UIParticle_001", transform);
            go.transform.position = bingoItem.transform.position;
            
            ParticleSystem[] particleSystems = go.GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem particleSystem in particleSystems)
            {
                particleSystem.loop = true;
                particleSystem.startColor = particleColor;

                if (refreshDelayTime < particleSystem.duration)
                    refreshDelayTime = particleSystem.duration;
            }

            GameObject targetObject = _characterItems[
                bingoItem.BlockItem.BlockType == BlockType.Fire ? 0 :
                bingoItem.BlockItem.BlockType == BlockType.Water ? 1 :
                bingoItem.BlockItem.BlockType == BlockType.Earth ? 3 :
                bingoItem.BlockItem.BlockType == BlockType.Wind ? 2 :
                4
            ].gameObject;

            List<GameObject> targetObjects = new List<GameObject>();
            foreach (UI_CharacterItem uiCharacterItem in _characterItems)
            {
                if ((int) uiCharacterItem.CharacterData.ElementType == (int) bingoItem.BlockItem.BlockType)
                    targetObjects.Add(uiCharacterItem.gameObject);
            }
            
            Vector3[] paths = new Vector3[3];
            paths.SetValue(go.transform.position, 0);
            paths.SetValue(targetObject.transform.position, 2);
            paths.SetValue(new Vector3( paths[0].x + (paths[2].x - paths[0].x), paths[0].y + (paths[2].y - paths[0].y) * 0.5f, 0), 1);

            go.transform.DOPath(paths, 1.0f, PathType.CatmullRom).SetEase(Ease.OutCubic).SetDelay(refreshDelayTime * 0.5f).OnComplete(() =>
            {
                go.transform.DOScale(Vector3.one * 0.7f, 0.3f).OnComplete(() =>
                {
                    Managers.Resource.Destroy(go);
                    targetObject.transform.DOLocalMoveY(10, 0.3f).From(false).OnComplete(() =>
                    {
                        Vector3 targetPosition = targetObject.transform.localPosition;
                        targetPosition.y = 0.0f;
                        targetObject.transform.localPosition = targetPosition;
                    });

                    _monsterController.OnDamaged(30, (ElementType)((int) bingoItem.BlockItem.BlockType));
                    
                });
            });
        }

        int puzzleCount = 0;
        for (int row = 0; row < _uiTileItems.GetLength(0); row++)
        {
            for (int col = 0; col < _uiTileItems.GetLength(1); col++)
            {
                if (_uiTileItems[row, col].BlockItem != null)
                    puzzleCount++;
            }
        }

        if (puzzleCount == _mapSize.x * _mapSize.y)
        {
            Invoke("RefreshMap", refreshDelayTime);
        }
        
        StartCoroutine(SetPuzzleStateCoroutine(PuzzleState.NextTurn, refreshDelayTime));
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
                    UI_BlockItem blockItem = Managers.UI.MakeSubItem<UI_BlockItem>(uiTileItem.transform);
                    RectTransform rectTransform = blockItem.GetComponent<RectTransform>();
                    rectTransform.anchoredPosition = Vector2.zero;

                    uiTileItem.Init();
                    uiTileItem.SetPuzzleItem(blockItem);
                    uiTileItem.BlockItem.SetState(Define.BlockState.Impossible);
                }
            }
        }
        
        parent = Get<GameObject>((int) GameObjects.BlockItems);
        foreach (Transform t in parent.transform)
            Managers.Resource.Destroy(t.gameObject);

        for (int i = 0; i < 4; i++)
            Managers.UI.MakeSubItem<UI_BlockItem>(parent.transform);
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
                    uiTileItem.BlockItem.SetPuzzleType((Define.BlockType) Random.Range(1, 5));
                    uiTileItem.BlockItem.RefreshAnimation();
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
        // 턴 카운트 갱신
        _turn++;
        TextMeshProUGUI text = GetText((int) Texts.TurnCountText);
        text.text = $"{_turn} Turn";
        text.gameObject.transform.DOPunchScale(Vector3.one, 0.5f, 1);
        
        // 턴 행동 가능 횟수 초기화
        _turnActionCount = 0;
        
        // 블록 추가
        GameObject parent = Get<GameObject>((int) GameObjects.BlockItems);
        UI_BlockItem blockItem = Managers.UI.MakeSubItem<UI_BlockItem>(parent.transform);
        blockItem.RefreshAnimation();
        blockItem = Managers.UI.MakeSubItem<UI_BlockItem>(parent.transform);
        blockItem.RefreshAnimation();

        HorizontalLayoutGroup horizontal = parent.GetComponent<HorizontalLayoutGroup>();
        horizontal.enabled = true;
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform) horizontal.transform);
        horizontal.enabled = false;
    }

    void MakeCharacter()
    {
        GameObject parent = GetObject((int) GameObjects.CharacterPanel);
        foreach (Transform t in parent.transform)
            Managers.Resource.Destroy(t.gameObject);

        for (int i = 0; i < TeamCountMax; i++)
        {
            if (_characterItems.Count <= i)
                _characterItems.Add(Managers.UI.MakeSubItem<UI_CharacterItem>(parent.transform));

            Data.CharacterData characterData = null;
            Managers.Data.CharacterDict.TryGetValue(Managers.Main.TeamIds[i], out characterData);
            _characterItems[i].SetCharacterData(characterData);
        }

        parent = GetObject((int) GameObjects.MonsterPanel);
        foreach (Transform t in parent.transform)
            Managers.Resource.Destroy(t.gameObject);

        _monsterController = Managers.UI.MakeSubItem<MonsterController>(parent.transform);

        Data.CharacterData monsterData = null;
        Managers.Data.CharacterDict.TryGetValue(100, out monsterData);
        if (monsterData != null)
            _monsterController.SetData(monsterData);
    }
    
    void OnDropItem(UI_TileItem item)
    {
        if (item != null && item.BlockItem != null)
        {
            item.BlockItem.SetState(Define.BlockState.Impossible);
            CheckActionCount(item);
        }
    }
}

#region Not Used
/*
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
*/
#endregion