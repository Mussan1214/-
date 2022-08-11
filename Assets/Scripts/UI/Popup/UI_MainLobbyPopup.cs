using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_MainLobbyPopup : UI_Popup
{
    enum Images
    {
        AdventureImage
    }
    
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindImage(typeof(Images));
        
        GetImage((int)Images.AdventureImage).gameObject.BindEvent(OnClickAdventure);
        
        return true;
    }

    private void OnClickAdventure(PointerEventData eventData)
    {
        // ClosePopupUI();
        Managers.UI.ShowPopupUI<UI_IngamePopup>();
    }
}
