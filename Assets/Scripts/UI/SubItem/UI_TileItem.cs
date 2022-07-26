using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_TileItem : UI_Base
{
    enum Images
    {
        TileImage,
    }

    enum CanvasGroups
    {
        UI_TileItem,
    }

    public int Row { get; private set; } = -1;
    public int Col { get; private set; } = -1;

    public UI_PuzzleItem PuzzleItem { get; private set; } = null;

    private Action<UI_TileItem> _onDrop;
    
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<CanvasGroup>(typeof(CanvasGroups));
        Bind<Image>(typeof(Images));
        Bind<RectTransform>(typeof(Images));
        
        gameObject.BindEvent(OnDrop, Define.UIEvent.OnDrop);

        return true;
    }

    public void SetInfo(int row, int col, Action<UI_TileItem> onDrop)
    {
        Row = row;
        Col = col;
        _onDrop = onDrop;
    }

    public void SetPuzzleItem(UI_PuzzleItem uiPuzzleItem)
    {
        CanvasGroup canvasGroup = Get<CanvasGroup>((int) CanvasGroups.UI_TileItem);
        
        if (uiPuzzleItem != null)
        {
            PuzzleItem = uiPuzzleItem;
            canvasGroup.alpha = 1.0f;
        }
        else
        {
            if (PuzzleItem != null)
            {
                Managers.Resource.Destroy(PuzzleItem.gameObject);
                PuzzleItem = null;
            }
        }

        canvasGroup.blocksRaycasts = PuzzleItem == null;
    }

    public void Active(bool active)
    {
        CanvasGroup canvasGroup = Get<CanvasGroup>((int) CanvasGroups.UI_TileItem);
        if (canvasGroup != null)
        {
            canvasGroup.alpha = active ? 1.0f : 0.3f;
            if (active)
            {
                if (PuzzleItem == null)
                    canvasGroup.blocksRaycasts = true;
                else
                    canvasGroup.blocksRaycasts = false;
            }
            else
            {
                canvasGroup.blocksRaycasts = active;
            }
        }
    }
    
    void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            eventData.pointerDrag.transform.SetParent(transform);
            eventData.pointerDrag.transform.localPosition = Vector3.zero;

            UI_PuzzleItem uiPuzzleItem = eventData.pointerDrag.GetComponent<UI_PuzzleItem>();
            if (uiPuzzleItem != null)
                SetPuzzleItem(uiPuzzleItem);
            
            _onDrop?.Invoke(this);
        }
    }
}
