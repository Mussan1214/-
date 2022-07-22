using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class UI_Base : MonoBehaviour
{
    protected Dictionary<Type, UnityEngine.Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>();

    protected bool _init = false;

    public virtual bool Init()
    {
        if (_init)
            return false;

        return _init = true;
    }

    private void Start()
    {
        Init();
    }

    protected void Bind<T>(Type type) where T : UnityEngine.Object
    {
        string[] names = Enum.GetNames(type);
        UnityEngine.Object[] objects = new UnityEngine.Object[names.Length];
        _objects.Add(typeof(T), objects);

        for (int i = 0; i < names.Length; i++)
        {
            if (typeof(T) == typeof(GameObject))
                objects[i] = Utils.FindChild(gameObject, names[i], true);
            else
                objects[i] = Utils.FindChild<T>(gameObject, names[i], true);
            
            if (objects[i] == null)
                Debug.Log($"Failed to bind({names[i]})");
        }
    }

    protected void BindObject(Type type) { Bind<GameObject>(type); }
    protected void BindText(Type type) { Bind<TextMeshProUGUI>(type); }
    protected void BindImage(Type type) { Bind<Image>(type); }
    protected void BindButton(Type type) { Bind<Button>(type); }

    protected T Get<T>(int idx) where T : UnityEngine.Object
    {
        UnityEngine.Object[] objects = null;
        if (_objects.TryGetValue(typeof(T), out objects) == false)
            return null;

        return objects[idx] as T;
    }
    
    protected GameObject GetObject(int idx) { return Get<GameObject>(idx); }
    protected TextMeshProUGUI GetText(int idx) { return Get<TextMeshProUGUI>(idx); }
    protected Image GetImage(int idx) { return Get<Image>(idx); }
    protected Button GetButton(int idx) { return Get<Button>(idx); }

    public static void BindEvent(GameObject go, Action<PointerEventData> action, Define.UIEvent type = Define.UIEvent.Click)
    {
        if (type == Define.UIEvent.OnDrop)
        {
            UI_DroppableHandler uiDroppableHandler = Utils.GetOrAddComponent<UI_DroppableHandler>(go);

            uiDroppableHandler.OnDropHandler -= action;
            uiDroppableHandler.OnDropHandler += action;
        }
        else
        {
            UI_EventHandler uiEventHandler = Utils.GetOrAddComponent<UI_EventHandler>(go);

            switch (type)
            {
                case Define.UIEvent.Click:
                    uiEventHandler.OnClickHandler -= action;
                    uiEventHandler.OnClickHandler += action;
                    break;
                case Define.UIEvent.Pressed:
                    uiEventHandler.OnPressedHandler -= action;
                    uiEventHandler.OnPressedHandler += action;
                    break;
                case Define.UIEvent.PointerDown:
                    uiEventHandler.OnPointerDownHandler -= action;
                    uiEventHandler.OnPointerDownHandler += action;
                    break;
                case Define.UIEvent.PointerUp:
                    uiEventHandler.OnPointerUpHandler -= action;
                    uiEventHandler.OnPointerUpHandler += action;
                    break;
                case Define.UIEvent.BeginDrag:
                    uiEventHandler.OnBeginDragHandler -= action;
                    uiEventHandler.OnBeginDragHandler += action;
                    break;
                case Define.UIEvent.Drag:
                    uiEventHandler.OnDragHandler -= action;
                    uiEventHandler.OnDragHandler += action;
                    break;
                case Define.UIEvent.EndDrag:
                    uiEventHandler.OnEndDragHandler -= action;
                    uiEventHandler.OnEndDragHandler += action;
                    break;
                case Define.UIEvent.PointerEnter:
                    break;
                case Define.UIEvent.PointerExit:
                    break;
            }
        }
    }
}
