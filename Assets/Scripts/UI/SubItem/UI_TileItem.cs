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

    public int Row { get; private set; } = -1;
    public int Col { get; private set; } = -1;

    public UI_PuzzleItem PuzzleItem { get; private set; } = null;

    private Action<UI_TileItem> _onDrop;
    
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

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
        if (uiPuzzleItem != null)
        {
            PuzzleItem = uiPuzzleItem;
        }
        else
        {
            if (PuzzleItem != null)
            {
                Managers.Resource.Destroy(PuzzleItem.gameObject);
                PuzzleItem = null;
            }
        }
    }

    public void Active(bool active)
    {
        Get<Image>((int) Images.TileImage).enabled = active;
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
