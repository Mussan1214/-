using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UI_HpBar : UI_Base
{
    enum Sliders
    {
        UI_HpBar,
        InHpBar,
    }
    
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<Slider>(typeof(Sliders));

        Get<Slider>((int) Sliders.UI_HpBar).value = 1.0f;
        Get<Slider>((int) Sliders.InHpBar).value = 1.0f;
        
        return true;
    }

    public void SetValue(float value)
    {
        Init();

        Get<Slider>((int) Sliders.UI_HpBar).value = value;

        Slider slider = Get<Slider>((int) Sliders.InHpBar);
        DOTween.To(()=>slider.value, x => slider.value = x, value, 2.5f);
    }
}
