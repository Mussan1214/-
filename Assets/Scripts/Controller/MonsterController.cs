using System;
using Data;
using DG.Tweening;
using Spine;
using Spine.Unity;
using UnityEngine;
using Random = UnityEngine.Random;
using static Define;

public class MonsterController : BaseController
{
    public CharacterData CharacterData { get; private set; }

    private AnimState _animState = AnimState.None;
    public AnimState AnimState
    {
        get { return _animState; }
        set
        {
            _animState = value;
            UpdateAnimation();
        }
    }

    private UI_HpBar _hpBar;
    private int _hp;
    public int Hp
    {
        get { return _hp; }
        set { _hp = value; RefreshHp(); }
    }
    

    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        // _hpBar = Utils.FindChild<UI_HpBar>(gameObject, "UI_HpBar", true);

        SekSkeletonAsset("Spine/Monster_100/monster_1_SkeletonData");
        PlayAnimation("idle");
        _anim.MatchRectTransformWithBounds();

        RectTransform rectTransform = (RectTransform) transform;
        rectTransform.anchoredPosition = new Vector2(0, -(rectTransform.rect.size.y * 0.5f));

        return true;
    }

    protected override void UpdateAnimation()
    {
        Init();

        switch (AnimState)
        {
            case AnimState.None:
                break;
            
            case AnimState.Idle:
                PlayAnimation("idle");
                ChangeSkin("default");
                break;
            
            case AnimState.Damage:
                float duration = PlayAnimationOnceWithDuration("default", "damage");
                _anim.DOColor(Define.HitColor, duration * 0.5f).OnComplete(() =>
                {
                    _anim.DOColor(Color.white, duration * 0.5f).OnComplete(() => AnimState = AnimState.Idle);
                });
                break;
        }
    }

    public virtual void OnDamaged(int damage, CharacterData characterData)
    {
        float elementDamageRate = 1.0f;
        if (CharacterData.ElementType != characterData.ElementType)
        {
            switch (CharacterData.ElementType)
            {
                case ElementType.None:
                    break;
                case ElementType.Fire:
                    if (characterData.ElementType == ElementType.Water)
                        elementDamageRate = 2.0f;
                    else if (characterData.ElementType == ElementType.Earth)
                        elementDamageRate = 0.5f;
                    break;
                
                case ElementType.Water:
                    if (characterData.ElementType == ElementType.Wind)
                        elementDamageRate = 2.0f;
                    else if (characterData.ElementType == ElementType.Fire)
                        elementDamageRate = 0.5f;
                    break;
                
                case ElementType.Earth:
                    if (characterData.ElementType == ElementType.Fire)
                        elementDamageRate = 2.0f;
                    else if (characterData.ElementType == ElementType.Wind)
                        elementDamageRate = 0.5f;
                    break;
                
                case ElementType.Wind:
                    if (characterData.ElementType == ElementType.Earth)
                        elementDamageRate = 2.0f;
                    else if (characterData.ElementType == ElementType.Water)
                        elementDamageRate = 0.5f;
                    break;
            }
        }

        damage = (int) ((float) damage * elementDamageRate);
        Debug.Log($"Damage => {damage}");
        
        Hp = Math.Max(Hp - damage, 0);
        // MakeHitEffect(damage);
        
        GameObject effectObject = Managers.Resource.Instantiate("Effect/AttackEffect", transform);

        int degree = Random.Range(0, 180);
        float radian = degree * Mathf.PI / 180;
        float distance = Random.Range(30f, 200f);

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
        uiOnDamage.SetDamage(damage, characterData.ElementType);

        if (AnimState != AnimState.Damage)
        {
            AnimState = AnimState.Damage;
        }
    }

    public void SetData(CharacterData characterData)
    {
        CharacterData = characterData;

        Init();

        Hp = characterData.MaxHp;

        Bone bone = _anim.Skeleton.FindSlot("FX_1").Bone;
        Debug.Log(bone.WorldX + " " + bone.WorldY);
    }

    public void SetHpBar(UI_HpBar uiHpBar)
    {
        if (_hpBar != null)
            _hpBar.gameObject.SetActive(false);

        _hpBar = uiHpBar;
    }

    private void RefreshHp()
    {
        if (_hpBar != null)
            _hpBar.SetValue((float) Hp / CharacterData.MaxHp);
    }
    
    void MakeHitEffect(int damage)
    {
        GameObject effectObject = Managers.Resource.Instantiate("Effect/AttackEffect", transform);

        int degree = Random.Range(0, 180);
        float radian = degree * Mathf.PI / 180;
        float distance = Random.Range(30f, 200f);

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
        uiOnDamage.SetDamage(damage, ElementType.Fire);
    }
}
