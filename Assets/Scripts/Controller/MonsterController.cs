using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MonsterController : UI_Base
{
    enum Images
    {
        MonsterController,
    }

    public CharacterData CharacterData { get; private set; }

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

        Bind<Image>(typeof(Images));
        _hpBar = Utils.FindChild<UI_HpBar>(gameObject, "UI_HpBar", true);

        Image monsterImage = Get<Image>((int) Images.MonsterController);
        monsterImage.sprite = Managers.Resource.Load<Sprite>(CharacterData.FullImagePath);
        monsterImage.SetNativeSize();

        return true;
    }

    private bool isPlayHit = false;
    
    public virtual void OnDamaged(int damage)
    {
        Hp = Math.Max(Hp - damage, 0);

        GameObject effectObject = Managers.Resource.Instantiate("Effect/AttackEffect", transform);

        int degree = Random.Range(0, 360);
        float radian = degree * Mathf.PI / 180;
        float distance = Random.Range(10f, 150f);

        Vector3 effectPosition = effectObject.transform.position;
        effectPosition.x += distance * Mathf.Cos(radian);
        effectPosition.y += distance * Mathf.Sin(radian);
        effectObject.transform.position = effectPosition;

        Animator animator = effectObject.GetComponent<Animator>();
        float clipLength = animator.runtimeAnimatorController.animationClips[0].length;
        Destroy(effectObject, clipLength);
        
        if (isPlayHit) return;
        isPlayHit = true;
        
        Vector3 originPosition = transform.position;
        transform.DOShakePosition(0.5f, Vector3.right * 20f).SetEase(Ease.Flash).OnComplete(() =>
        {
            transform.DOMove(originPosition, 0.1f);
            isPlayHit = false;
        });

        Color color = new Color(1.0f, 0.5f, 0.5f, 1.0f);
        Get<Image>((int) Images.MonsterController).DOColor(color, 0.17f).OnComplete(() =>
        {
            Get<Image>((int) Images.MonsterController).DOColor(Color.white, 0.33f);
        });
    }

    public void SetData(CharacterData characterData)
    {
        CharacterData = characterData;

        Init();

        Hp = characterData.MaxHp;
    }

    private void RefreshHp()
    {
        if (_hpBar != null)
            _hpBar.SetValue((float) Hp / CharacterData.MaxHp);
    }
}
