using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_CharacterInfoPopup : UI_Popup
{
    enum Texts
    {
        CharacterInfoText,
    }

    private Data.CharacterData _characterData;
    
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindText(typeof(Texts));
        
        gameObject.BindEvent((eventData) =>
        {
            ClosePopupUI();
        }, Define.UIEvent.Click);
        
        if (_characterData == null)
        {
            Debug.Log("CharacterData is null");
        }
        
        Data.SkillData skillData = null;
        Managers.Data.SkillDict.TryGetValue(_characterData.SkillID, out skillData);
        
        Debug.Log($"!캐릭터 정보 팝업!\n캐릭터 이름 : {_characterData.Name}\n스킬설명: {(skillData != null ? skillData.Description : string.Empty)}");
        GetText((int) Texts.CharacterInfoText).text =
            $"캐릭터 이름 : {_characterData.Name}\n스킬설명: {(skillData != null ? skillData.Description : string.Empty)}";
        
        
        return true;
    }

    public void SetCharacterData(Data.CharacterData characterData)
    {
        _characterData = characterData;
    }
}
