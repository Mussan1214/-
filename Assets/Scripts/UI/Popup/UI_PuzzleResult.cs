using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using static Define;

public class UI_PuzzleResult : UI_Popup
{
    enum Texts
    {
        ResultText,
    }

    enum Images
    {
        DimmedImage,
    }
    
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindText(typeof(Texts));
        BindImage(typeof(Images));
        
        gameObject.BindEvent(eventData =>
        {
            ClosePopupUI();
            UI_Popup uiPopup = Managers.UI.FindPopup<UI_IngamePopup>();
            uiPopup?.ClosePopupUI();
        });
        
        return true;
    }

    public void SetInfo(PuzzleResult puzzleResult)
    {
        Init();
        
        if (puzzleResult == PuzzleResult.Win)
        {
            GetText((int) Texts.ResultText).text = "CLEAR";
        }
        else
        {
            GetText((int) Texts.ResultText).text = "FAIL";
        }
        
        DOTween.To(x => GetText((int) Texts.ResultText).maxVisibleCharacters = (int) x, 0.0f,
            GetText((int) Texts.ResultText).text.Length, 1f);
    }
}
