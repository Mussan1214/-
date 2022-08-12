using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;

public class UI_BossHpBar : UI_HpBar
{
    enum Images
    {
        ElementTypeImage,
    }

    enum Texts
    {
        LevelText,
        NameText,
    }
    
    public CharacterData CharacterData { get; protected set; }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindImage(typeof(Images));
        BindText(typeof(Texts));
        
        return true;
    }

    public void SetInfo(CharacterData characterData)
    {
        CharacterData = characterData;

        Init();
        
        SetValue(1.0f);
        GetImage((int) Images.ElementTypeImage).sprite = Managers.Resource.Load<Sprite>($"Sprites/Ingame/{CharacterData.ElementType}");
        GetText((int) Texts.LevelText).text = "LV.1";
        GetText((int) Texts.NameText).text = characterData.Name;
    }
}
