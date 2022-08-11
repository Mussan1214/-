using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class UI_OnDamage : UI_Base
{
    enum Texts
    {
        UI_OnDamage,
    }
    
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindText(typeof(Texts));
        
        return true;
    }

    public void SetDamage(int damage, Define.ElementType elementType)
    {
        Init();

        GetText((int) Texts.UI_OnDamage).text = damage.ToString();
        GetText((int) Texts.UI_OnDamage).colorGradient = new VertexGradient(Define.ElementColor(elementType, true), Define.ElementColor(elementType, true), Define.ElementColor(elementType), Define.ElementColor(elementType));
        
        GetText((int) Texts.UI_OnDamage).transform.DOPunchScale(Vector3.one, 0.15f)
        .OnComplete(() =>
        {
            GetText((int) Texts.UI_OnDamage).DOColor(Define.AlphaZero, 0.3f);
            GetText((int) Texts.UI_OnDamage).transform
            .DOLocalMoveY(GetText((int) Texts.UI_OnDamage).transform.localPosition.y + 50f, 0.3f)
            .OnComplete(() =>
            {
                Managers.Resource.Destroy(gameObject);
            });
        });
    }
}
