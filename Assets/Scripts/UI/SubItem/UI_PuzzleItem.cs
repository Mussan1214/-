using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_PuzzleItem : UI_Base
{
    enum Images
    {
        PuzzleImage,
    }
    
    public Define.PuzzleType PuzzleType { get; private set; }

    private CanvasGroup _canvasGroup;
    private RectTransform _rectTransform;
    private Transform _previousParent;
    private Vector3 _previousPosition;

    private Button btn;

    private void Awake()
    {
        _canvasGroup = Utils.GetOrAddComponent<CanvasGroup>(gameObject);
        _rectTransform = Utils.GetOrAddComponent<RectTransform>(gameObject);
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<Image>(typeof(Images));
        gameObject.BindEvent(OnBeginDrag, Define.UIEvent.BeginDrag);
        gameObject.BindEvent(OnDrag, Define.UIEvent.Drag);
        gameObject.BindEvent(OnEndDrag, Define.UIEvent.EndDrag);
        
        return true;
    }

    public void SetInfo(Define.PuzzleType type)
    {
        PuzzleType = type;
        if (PuzzleType == Define.PuzzleType.Impossible)
        {
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.alpha = 1.0f;
        }
    }
    
    #region Handler Event
    void OnBeginDrag(PointerEventData eventData)
    {
        if (PuzzleType == Define.PuzzleType.Impossible)
            return;
        
        _previousParent = transform.parent;
        _previousPosition = transform.position;
        
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.alpha = 0.6f;
    }

    void OnDrag(PointerEventData eventData)
    {
        if (PuzzleType == Define.PuzzleType.Impossible)
            return;
        
        _rectTransform.position = eventData.position;
    }

    void OnEndDrag(PointerEventData eventData)
    {
        if (PuzzleType == Define.PuzzleType.Impossible)
            return;
        
        if (transform.parent == _previousParent)
        {
            transform.position = _previousPosition;
        }
        
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.alpha = 1.0f;
    }
    #endregion
}
