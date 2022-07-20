using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScene : BaseScene
{
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        SceneType = Define.Scene.Main;
        Managers.UI.ShowPopupUI<UI_TitlePopup>();
        return true;
    }
}
