using System;
using System.Collections.Generic;
using DialogSystem.Model;
using UnityEngine;
#if UNITY_EDITOR
using UnityEngine.Windows;
using UnityEditor;
#endif

namespace DialogSystem.ScriptObject
{
    public class DialogVarAsset:ScriptableObject
    {
        private static DialogVarAsset _instance;
        public static DialogVarAsset Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = Resources.Load<DialogVarAsset>("GlobalAssets/GlobalVar");
                }
#if UNITY_EDITOR
                if (!_instance)
                {
                    var path = "Assets/Resources/GlobalAssets";
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    _instance = CreateInstance<DialogVarAsset>();
                    AssetDatabase.CreateAsset(_instance, path+"/GlobalVar.asset");
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

        public List<VarUnitInfo> varInfos = new List<VarUnitInfo>();

        public VarUnitInfo GetVarWithName(string name)
        {
            foreach (var VARIABLE in varInfos)
            {
                if (VARIABLE.VarName == name)
                    return VARIABLE;
            }

            return null;
        }
    }
}