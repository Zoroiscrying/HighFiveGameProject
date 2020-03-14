using System.Collections.Generic;
using System.IO;
using HighFive.Model.Person;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR

#endif

namespace HighFive.Control.PersonSystem.Persons
{
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
        
        public int Maxdrag;   //最大药引上限
        public int money = 0;
        public float airXMove;
        public int MaxSpiritNum = 1;

        public List<CommonSkillInfo> commonSkillInfos=new List<CommonSkillInfo>();
        public List<ComboSkillInfo> comboSkillInfos=new List<ComboSkillInfo>();
    }
}