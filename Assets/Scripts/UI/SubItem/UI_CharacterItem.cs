using System;
using System.Collections;
using System.Collections.Generic;
using Data;
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

    public void OnDamage(CharacterData attacker)
    {
        float elementDamageRate = 1.0f;
        if (CharacterData.ElementType != attacker.ElementType)
        {
            switch (CharacterData.ElementType)
            {
                case ElementType.None:
                    break;
                case ElementType.Fire:
                    if (attacker.ElementType == ElementType.Water)
                        elementDamageRate = 2.0f;
                    else if (attacker.ElementType == ElementType.Earth)
                        elementDamageRate = 0.5f;
                    break;
                
                case ElementType.Water:
                    if (attacker.ElementType == ElementType.Wind)
                        elementDamageRate = 2.0f;
                    else if (attacker.ElementType == ElementType.Fire)
                        elementDamageRate = 0.5f;
                    break;
                
                case ElementType.Earth:
                    if (attacker.ElementType == ElementType.Fire)
                        elementDamageRate = 2.0f;
                    else if (attacker.ElementType == ElementType.Wind)
                        elementDamageRate = 0.5f;
                    break;
                
                case ElementType.Wind:
                    if (attacker.ElementType == ElementType.Earth)
                        elementDamageRate = 2.0f;
                    else if (attacker.ElementType == ElementType.Water)
                        elementDamageRate = 0.5f;
                    break;
            }
        }
        
        int damage = (int) ((float) attacker.Atk * elementDamageRate);
        Debug.Log($"{damage}");
        Hp = Math.Max(Hp - damage, 0);

        GameObject effectObject = Managers.Resource.Instantiate("Effect/AttackEffect", transform);

        int degree = UnityEngine.Random.Range(0, 180);
        float radian = degree * Mathf.PI / 180;
        float distance = UnityEngine.Random.Range(0.0f, 10.0f);

        Vector3 effectPosition = effectObject.transform.position;
        effectPosition.x += distance * Mathf.Cos(radian);
        effectPosition.y += distance * Mathf.Sin(radian);
        effectObject.transform.position = effectPosition;

        Animator animator = effectObject.GetComponent<Animator>();
        float clipLength = animator.runtimeAnimatorController.animationClips[0].length;
        Destroy(effectObject, clipLength);

        GameObject damageObject = Managers.Resource.Instantiate("UI/UI_OnDamage", transform);
        effectPosition.y += 50.0f;
        damageObject.transform.position = effectPosition;
        
        UI_OnDamage uiOnDamage = damageObject.GetComponent<UI_OnDamage>();
        uiOnDamage.SetDamage(damage, attacker.ElementType);
        
        if (Hp <= 0)
        {
            GetImage((int) Images.CharacterImage).DOColor(Color.gray, 1.0f);
            GetImage((int) Images.ElementImage).DOColor(Color.gray, 1.0f);
            GetImage((int) Images.FrameImage).DOColor(Color.gray, 1.0f);
        }
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
