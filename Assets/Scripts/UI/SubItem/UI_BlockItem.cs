using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UI_BlockItem : UI_Base
{
    enum Images
    {
        PuzzleImage,
    }
    
    public Define.BlockState BlockState { get; private set; }
    public Define.BlockType BlockType { get; private set; }

    private CanvasGroup _canvasGroup;
    private RectTransform _rectTransform;
    private Transform _previousParent;
    private Vector3 _previousPosition;

    private Button btn;

    private GameObject particleObject;

    private void Awake()
    {
        _canvasGroup = Utils.GetOrAddComponent<CanvasGroup>(gameObject);
        _rectTransform = Utils.GetOrAddComponent<RectTransform>(gameObject);
    }

    private void OnDestroy()
    {
        if (particleObject != null)
        {
            Managers.Resource.Destroy(particleObject);
            particleObject = null;
        }
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<Image>(typeof(Images));
        gameObject.BindEvent(OnBeginDrag, Define.UIEvent.BeginDrag);
        gameObject.BindEvent(OnDrag, Define.UIEvent.Drag);
        gameObject.BindEvent(OnEndDrag, Define.UIEvent.EndDrag);
        
        SetPuzzleType((Define.BlockType) Random.Range(1, 5));
        
        return true;
    }

    public void SetState(Define.BlockState state)
    {
        BlockState = state;
        if (BlockState == Define.BlockState.Impossible)
        {
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.alpha = 1.0f;
        }
    }

    public void SetPuzzleType(Define.BlockType blockType)
    {
        BlockType = blockType;
        Get<Image>((int)Images.PuzzleImage).sprite = 
        Managers.Resource.Load<Sprite>($"Sprites/Stone/{((int) blockType).ToString()}");
    }
    
    public void RefreshAnimation()
    {
        if (Init())
        {
            Color color = Get<Image>((int) Images.PuzzleImage).color;
            color.a = 0;
            Get<Image>((int) Images.PuzzleImage).color = color;
        }

        if (particleObject == null)
            particleObject = Managers.Resource.Instantiate($"Particle/UI/UIParticle_001", transform);
        
        Get<Image>((int)Images.PuzzleImage).DOFade(0.0f, 0.0f).OnComplete(() =>
        {
            Get<Image>((int)Images.PuzzleImage).DOFade(1.0f, 0.15f).SetDelay(0.3f).OnComplete(OnDestroy); 
        });
    }
    
    #region Handler Event
    void OnBeginDrag(PointerEventData eventData)
    {
        if (BlockState == Define.BlockState.Impossible)
            return;
        
        _previousParent = transform.parent;
        _previousPosition = transform.position;
        
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.alpha = 0.6f;
    }

    void OnDrag(PointerEventData eventData)
    {
        if (BlockState == Define.BlockState.Impossible)
            return;
        
        _rectTransform.position = eventData.position;
    }

    void OnEndDrag(PointerEventData eventData)
    {
        if (BlockState == Define.BlockState.Impossible)
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
