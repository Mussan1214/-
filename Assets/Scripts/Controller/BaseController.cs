using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class BaseController : UI_Base
{
    protected SkeletonGraphic _anim = null;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        _anim = GetComponent<SkeletonGraphic>();
        return true;
    }
    
    protected virtual void UpdateAnimation() {}
    
    #region Spine Animation

    public void SekSkeletonAsset(string path)
    {
        Init();
        _anim.skeletonDataAsset = Managers.Resource.Load<SkeletonDataAsset>(path);
        _anim.Initialize(true);
    }

    public void PlayAnimation(string name, bool loop = true)
    {
        Init();
        _anim.startingAnimation = name;
        _anim.startingLoop = loop;
    }

    public void ChangeSkin(string name)
    {
        Init();
        _anim.initialSkinName = name;
        _anim.Initialize(true);
    }

    public void Refresh()
    {
        Init();
        _anim.Initialize(true);
    }

    public void PlayAnimationOnce(string name)
    {
        StartCoroutine(PlayAnimationOnceCoroutine(name));
    }

    private IEnumerator PlayAnimationOnceCoroutine(string name)
    {
        bool defaultLoop = _anim.startingLoop;
        string defaultName = _anim.startingAnimation;

        _anim.startingLoop = false;
        _anim.startingAnimation = name;

        float length = _anim.skeletonDataAsset.GetSkeletonData(true).FindAnimation(name).Duration;
        yield return new WaitForSeconds(length);
        
        PlayAnimation(defaultName, defaultLoop);
        
    }
    private IEnumerator PlayAnimationOnceCoroutine(string skin, string name)
    {
        bool defaultLoop = _anim.startingLoop;
        string defaultSkin = _anim.initialSkinName;
        string defaultName = _anim.startingAnimation;

        _anim.startingLoop = false;
        _anim.startingAnimation = name;
        ChangeSkin(skin);

        float length = _anim.skeletonDataAsset.GetSkeletonData(true).FindAnimation(name).Duration;
        yield return new WaitForSeconds(length);
        
        PlayAnimation(defaultName, defaultLoop);
        ChangeSkin(defaultSkin);
    }
    
    #endregion
}
