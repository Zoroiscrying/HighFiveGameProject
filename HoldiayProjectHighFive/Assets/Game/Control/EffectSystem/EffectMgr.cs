using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

#if UNITY_EDITOR
    
using UnityEditor;

#endif

namespace Game.Control.EffectSystem
{
    public class EffectMgr:ScriptableObject
    {
        #region Singleton_Editor

#if UNITY_EDITOR
        [MenuItem("ReadyGamerOne/Global/ShowEffectInfos")]
        public static void ShowEffects()
        {
            Selection.activeInstanceID = Instance.GetInstanceID();
        }
#endif
        
        private static EffectMgr _instance;
        
        public static EffectMgr Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = Resources.Load<EffectMgr>("GlobalAssets/EffectInfos");
                }
#if UNITY_EDITOR
                if (!_instance)
                {
                    _instance = CreateInstance<EffectMgr>();
                    var path = "Assets/Resources";
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    path += "/GlobalAssets";
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    path += "/EffectInfos.asset";
                    UnityEditor.AssetDatabase.CreateAsset(_instance, path);
                }
#endif
                if (_instance == null)
                    throw new System.Exception("初始化失败");

                return _instance;
            }        
        }        

        #endregion

        [Serializable]
        public class EffectAssetMsg
        {
            public int id;
            public string tag;
            public EffectInfoAsset EffectInfoAsset;
        }
        
        public List<EffectAssetMsg> effectInfoMsgs=new List<EffectAssetMsg>();
        public int GetID()
        {
            var index = 0;
            while (true)
            {
                var ok = true;
                foreach (var unit in effectInfoMsgs)
                {
                    if (index == unit.id)
                    {
                        ok = false;
                        break;
                    }
                }

                if (ok)
                    break;
                index++;
            }

            return index;
        }
    }
}