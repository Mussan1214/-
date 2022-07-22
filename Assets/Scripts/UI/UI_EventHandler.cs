using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EventHandler : MonoBehaviour,
    IPointerClickHandler, IPointerDownHandler, IPointerUpHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler,
    IPointerEnterHandler, IDropHandler, IPointerExitHandler
{
    public Action<PointerEventData> OnClickHandler = null;
    public Action<PointerEventData> OnPressedHandler = null;
    public Action<PointerEventData> OnPointerDownHandler = null;
    public Action<PointerEventData> OnPointerUpHandler = null;

    public Action<PointerEventData> OnBeginDragHandler = null;
    public Action<PointerEventData> OnDragHandler = null;
    public Action<PointerEventData> OnEndDragHandler = null;
    
    public Action<PointerEventData> OnPointerEnterHandler = null;
    public Action<PointerEventData> OnDropHandler = null;
    public Action<PointerEventData> OnPointerExitHandler = null;
    

    private bool _pressed = false;
    private const float _pressedTime = 0.25f;
    private float _pressedTimeValue = 0.0f;

    private void Update()
    {
        if (_pressed)
        {
            _pressedTimeValue += Time.deltaTime;
            if (_pressedTimeValue > _pressedTime)
            {
                _pressed = false;
                OnPressedHandler?.Invoke(null);
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClickHandler?.Invoke(eventData);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _pressed = true;
        _pressedTimeValue = 0.0f;
        
        OnPointerDownHandler?.Invoke(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _pressed = false;
        OnPointerUpHandler?.Invoke(eventData);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        OnBeginDragHandler?.Invoke(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        OnDragHandler?.Invoke(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        OnEndDragHandler?.Invoke(eventData);
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        OnPointerEnterHandler?.Invoke(eventData);
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop");
        OnDropHandler?.Invoke(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnPointerExitHandler?.Invoke(eventData);
    }
}
