using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Directory = UnityEngine.Windows.Directory;

namespace ReadyGamerOne.Const
{
    public class ConstStringAsset:ScriptableObject
    {
        private static ConstStringAsset _instance;
        public static ConstStringAsset Instance
        {
            get
            {
                if (!_instance)
                {
                        _instance = Resources.Load<ConstStringAsset>("GlobalAssets/GlobalConstStrings");
                }
#if UNITY_EDITOR
                if (!_instance)
                {
                    _instance = CreateInstance<ConstStringAsset>();
                    var path = "Assets/Resources";
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    path += "/GlobalAssets";
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    path += "/GlobalConstStrings.asset";
                    UnityEditor.AssetDatabase.CreateAsset(_instance, path);
                }
#endif
                if (_instance == null)
                    throw new System.Exception("初始化失败");

                return _instance;
            }
        }

        public List<string> constStrings = new List<string>(); 
    }
}