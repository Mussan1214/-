using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using static Define;

public class UI_CharacterItem : UI_Base
{
    enum Images
    {
        CharacterImage,
        FrameImage,
        ElementImage,
    }

    enum Sliders
    {
        HpBar
    }

    private AnimState _animState = AnimState.None;

    public AnimState AnimState
    {
        get { return _animState; }
        set
        {
            if (_animState == value) return;
            
            _animState = value;
            UpdateAnimation();
        }
    }

    private void UpdateAnimation()
    {
        switch (AnimState)
        {
            case AnimState.Idle:
                break;
            
            case AnimState.Damage:
                break;
            
            case AnimState.Attack:
                transform.DOLocalMoveY(10.0f, 0.3f).From(false)
                .OnComplete(() =>
                {
                    AnimState = AnimState.Idle;
                });
                break;
        }
    }

    public Data.CharacterData CharacterData { get; private set; }

    private int _hp;
    public int Hp
    {
        get { return _hp; }
        set { _hp = value; RefreshHpBar(); }
    }
    
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindImage(typeof(Images));
        Bind<Slider>(typeof(Sliders));

        if (CharacterData != null)
        {
            GetImage((int) Images.CharacterImage).sprite = Managers.Resource.Load<Sprite>(CharacterData.ThumbnailPath);
            GetImage((int) Images.FrameImage).sprite = Managers.Resource.Load<Sprite>($"Sprites/Ingame/{CharacterData.ElementType.ToString().ToLower()}_frame");
            GetImage((int) Images.ElementImage).sprite = Managers.Resource.Load<Sprite>($"Sprites/Ingame/{CharacterData.ElementType}");

            Hp = CharacterData.MaxHp;
        }
        else
        {
            GetImage((int) Images.CharacterImage).sprite = Managers.Resource.Load<Sprite>($"Sprites/Ingame/Characters_off");
            GetImage((int) Images.FrameImage).gameObject.SetActive(false);
            GetImage((int) Images.ElementImage).gameObject.SetActive(false);
            
            Get<Slider>((int) Sliders.HpBar).gameObject.SetActive(false);
        }
        
        gameObject.BindEvent(OnPressed, Define.UIEvent.Pressed);
        
        return true;
    }

    public void SetCharacterData(Data.CharacterData characterData)
    {
        CharacterData = characterData;
    }

    void RefreshHpBar()
    {
        Get<Slider>((int) Sliders.HpBar).value = (float) Hp / CharacterData.MaxHp;
    }
    
    

    void OnPressed(PointerEventData eventData)
    {
        if (CharacterData == null)
        {
            Debug.Log("CharacterData is null");
            return;
        }

        UI_CharacterInfoPopup uiCharacterInfoPopup = Managers.UI.FindPopup<UI_CharacterInfoPopup>();
        if (uiCharacterInfoPopup == null)
            Managers.UI.ShowPopupUI<UI_CharacterInfoPopup>().SetCharacterData(CharacterData);
        else
        {
            uiCharacterInfoPopup.SetCharacterData(CharacterData);
            uiCharacterInfoPopup.RefreshUI();
        }
    }
}
