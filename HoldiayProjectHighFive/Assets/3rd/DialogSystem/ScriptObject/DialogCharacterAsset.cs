using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DialogSystem.ScriptObject
{
    public class DialogCharacterAsset:ScriptableObject
    {
        private static DialogCharacterAsset _instance;
        public static DialogCharacterAsset Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = Resources.Load<DialogCharacterAsset>("GlobalAssets/GlobalCharacter");
                }
#if UNITY_EDITOR
                if (!_instance)
                {
                    var path = "Assets/Resources/GlobalAssets";
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    _instance = CreateInstance<DialogCharacterAsset>();
                    AssetDatabase.CreateAsset(_instance, path+"/GlobalCharacter.asset");
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

        public List<string> characterNames=new List<string>();
    }
}