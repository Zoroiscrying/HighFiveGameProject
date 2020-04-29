using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
#if UNITY_EDITOR
using UnityEditor;
#endif
using ReadyGamerOne.Rougelike.Person;
using HighFive.Control.EffectSystem.Effects;

namespace HighFive.Control.EffectSystem
{
    /// <summary>
    /// 特效信息资源
    /// 一般而言，每个角色会有三个特效资源列表，攻击特效，击中特效，受击特效，
    /// 每个会产生特效的角色必须实现IEffector<T>接口
    /// 存储特效单元列表
    /// </summary>
    public class EffectInfoAsset:ScriptableObject
    {
        #region Editor

#if UNITY_EDITOR
        
        [MenuItem("ReadyGamerOne/RPG/Create/EffectInfoAsset")]
        public static void CreateInstance()
        {
            string[] strs = Selection.assetGUIDs;

            string path = AssetDatabase.GUIDToAssetPath(strs[0]);

            if (path.Contains("."))
            {
                path=path.Substring(0, path.LastIndexOf('/'));
            }

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            AssetDatabase.CreateAsset(CreateInstance<EffectInfoAsset>(), path + "/NewEffectInfos.asset");
            AssetDatabase.Refresh();

            Selection.activeObject = AssetDatabase.LoadAssetAtPath<EffectInfoAsset>(path + "/NewEffectInfos.asset");
        } 
#endif        

        #endregion
        [Serializable]
        public enum EffectorType
        {
            Ditascher,
            Receiver
        }

        public EffectorType effectorType = EffectorType.Ditascher;
        public List<EffectUnitInfo> EffectUnitInfos=new List<EffectUnitInfo>();

        public void Play(IEffector<AbstractPerson> ditacher, IEffector<AbstractPerson> receiver)
        {
            foreach (var VARIABLE in EffectUnitInfos)
            {
                if(VARIABLE!=null)
                    VARIABLE.Play(ditacher,receiver);
            }
        }
    }
}