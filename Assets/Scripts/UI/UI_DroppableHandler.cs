using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class UI_DroppableHandler : MonoBehaviour, 
    IDropHandler
{
    public Action<PointerEventData> OnDropHandler = null;

    public void OnDrop(PointerEventData eventData)
    {
        OnDropHandler?.Invoke(eventData);
    }
}
