using System;
using System.Collections.Generic;
using DialogSystem.Model;
using UnityEngine;
#if UNITY_EDITOR
    
using UnityEngine.Windows;
#endif

#if UNITY_EDITOR
using UnityEditor;
#endif
namespace DialogSystem.ScriptObject
{
    public class DialogProgressAsset : ScriptableObject
    {
        #region 单例

        private static DialogProgressAsset _instance;

        public static DialogProgressAsset Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = Resources.Load<DialogProgressAsset>("GlobalAssets/GlobalProgress");
                }
#if UNITY_EDITOR
                if (!_instance)
                {
                    var path = "Assets/Resources/GlobalAssets";
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    _instance = CreateInstance<DialogProgressAsset>();
                    AssetDatabase.CreateAsset(_instance, path+"/GlobalProgress.asset");
                }
#endif
                if (_instance == null)
                {
#if UNITY_EDITOR
                    throw new Exception("初始化失败");
#else
                    WindowsUtil.MessageBox("单例Scriptable,初始化失败");
#endif
                }

                return _instance;
            }
        }        

        #endregion
        
        public event Action<float> onProgressChanged; 
        
        public List<DialogProgressPoint> DialogProgressPoints=new List<DialogProgressPoint>();
        [SerializeField] private float currentProgress = 0f;

        public float CurrentProgress
        {
            get { return currentProgress; }
            internal set
            {
                currentProgress = value;
                onProgressChanged?.Invoke(value);
            }
        }
    }
}