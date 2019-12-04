using System;
using System.Collections.Generic;
using System.IO;
using HighFive.Control.SkillSystem;
using HighFive.Model.SpiritItemSystem;
using HighFive.View.AssetUIs.Panels;
using ReadyGamerOne.Data;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR

#endif

namespace HighFive.Control.PersonSystem.Persons
{

    [Serializable]
    public class CommonSkillInfo
    {
        public KeyCode key;
        public SkillInfoAsset skillAsset;
    }
    
    [Serializable]
    public class ComboSkillInfo
    {
        public SkillInfoAsset skillAsset;
        [Range(0, 1)] public float beginComboTest;
        public float canMoveTime;
        public bool ignoreInput;
        public float faultToleranceTime;

        public string SkillName => skillAsset.skillName.StringValue;
        public float StartTime => skillAsset.startTime;
        public float LastTime => skillAsset.LastTime;

        public void RunSkill(Player self, bool ignoreInput = false, float startTimer = 0f) =>
            skillAsset.RunSkill(self, ignoreInput, startTimer);
    }
    
    public class PlayerInfo:BaseCharacterInfo
    {
        #region Editor

 #if UNITY_EDITOR
         [MenuItem("ReadyGamerOne/Create/RPG/PlayerAsset")]
         public static void CreateAsset()
         {
             string[] strs = Selection.assetGUIDs;
         
             string path = AssetDatabase.GUIDToAssetPath(strs[0]);
         
             if (path.Contains("."))
             {
                 path=path.Substring(0, path.LastIndexOf('/'));
             }
         
             if (!Directory.Exists(path))
                 Directory.CreateDirectory(path);
             AssetDatabase.CreateAsset(CreateInstance<PlayerInfo>(), path + "/NewPlayerInfo.asset");
             AssetDatabase.Refresh();
         
             Selection.activeObject = AssetDatabase.LoadAssetAtPath<PlayerInfo>(path + "/NewPlayerInfo.asset");
         }   
 #endif       

            #endregion

        public KeyCode comboKey=KeyCode.J;
        public KeyCode superKey = KeyCode.Z;
        public KeyCode bagKey = KeyCode.Tab;
        
        public List<Player.ItemData> itemList = new List<Player.ItemData>();
        public int Maxdrag;   //最大药引上限
        public int money = 0;
        public float airXMove;
        public int MaxSpiritNum = 1;

        public PackagePanelAsset packagePanelAsset;
        
        public SerializableDictionary<string, AbstractSpiritItem> spiritDic =
            new SerializableDictionary<string, AbstractSpiritItem>();
        
        public List<CommonSkillInfo> commonSkillInfos=new List<CommonSkillInfo>();
        public List<ComboSkillInfo> comboSkillInfos=new List<ComboSkillInfo>();
    }
}