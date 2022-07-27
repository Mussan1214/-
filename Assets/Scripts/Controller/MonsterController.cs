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

    private int _hp;

    public int Hp
    {
        get { return _hp; }
        set { _hp = value; RefreshHp(); }
    }

    private UI_HpBar _hpBar;

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
    
    public virtual void OnDamaged(int damage)
    {
        Hp = Math.Max(Hp - damage, 0);

        transform.DOPunchRotation(Vector3.one * (Random.Range(0, 2) == 0 ? 1f : -1f), 1.0f)
            .OnComplete(() =>
            {
                transform.DORotate(Vector3.zero, 0.1f);
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
