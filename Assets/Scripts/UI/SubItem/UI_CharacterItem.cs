using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
