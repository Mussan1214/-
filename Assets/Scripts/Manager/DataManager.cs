using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;

public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}

public class DataManager
{
    public Dictionary<int, CharacterData> CharacterDict { get; private set; } = new Dictionary<int, CharacterData>();
    public Dictionary<int, SkillData> SkillDict { get; private set; } = new Dictionary<int, SkillData>();

    public void Init()
    {
        CharacterDict = LoadJson<CharacterDataLoader, int, CharacterData>("CharacterData").MakeDict();
        SkillDict = LoadJson<SkillDataLoader, int, SkillData>("SkillData").MakeDict();
    }

    Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
        TextAsset textAsset = Managers.Resource.Load<TextAsset>($"Data/{path}");
        Debug.Log(textAsset.text);
        return Newtonsoft.Json.JsonConvert.DeserializeObject<Loader>(textAsset.text);
    }
}
