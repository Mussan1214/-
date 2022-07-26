using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    #region Character
    [Serializable]
    public class CharacterData
    {
        public int ID;
        public string Name;

        public Define.ElementType ElementType;
        public int MaxHp;
        public int Atk;
        public int SkillID;

        public string ThumbnailPath;
        public string FullImagePath;
    }

    [Serializable]
    public class CharacterDataLoader : ILoader<int, CharacterData>
    {
        public List<CharacterData> characterDatas = new List<CharacterData>();

        public Dictionary<int, CharacterData> MakeDict()
        {
            Dictionary<int, CharacterData> dict = new Dictionary<int, CharacterData>();
            foreach (CharacterData characterData in characterDatas)    
            {
                dict.Add(characterData.ID, characterData);
                Debug.Log($"CharacterData\nID: {characterData.ID}, Name: {characterData.Name}, {characterData.ElementType}");
            }

            return dict;
        }
    }
    #endregion
    
    #region Skill

    [Serializable]
    public class SkillData
    {
        public int ID;
        public string Description;
    }

    [Serializable]
    public class SkillDataLoader : ILoader<int, SkillData>
    {
        public List<SkillData> skillDatas = new List<SkillData>();

        public Dictionary<int, SkillData> MakeDict()
        {
            Dictionary<int, SkillData> dict = new Dictionary<int, SkillData>();
            foreach (SkillData skillData in skillDatas)
            {
                dict.Add(skillData.ID, skillData);
                Debug.Log($"SkillData\nID: {skillData.ID}\nDescription: {skillData.Description}");
            }

            return dict;
        }
    }
    #endregion
}