using UnityEngine;
using UnityEngine.EventSystems;

public class UI_TitlePopup : UI_Popup
{
    enum Buttons
    {
        TouchToScreenButton
    }
    
    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        BindButton(typeof(Buttons));
        
        GetButton((int)Buttons.TouchToScreenButton).gameObject.BindEvent(OnClickTouchToScreen);
        
        return true;
    }

    private void OnClickTouchToScreen(PointerEventData eventData)
    {
        Debug.Log($"Next Lobby");

        Managers.UI.ShowPopupUI<UI_MainLobbyPopup>();
    }
}
