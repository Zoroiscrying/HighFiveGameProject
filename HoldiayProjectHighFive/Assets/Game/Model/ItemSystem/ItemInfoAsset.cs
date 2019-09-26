using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Game.Model.ItemSystem
{
    public class ItemInfoAsset:ScriptableObject
    {
        #region 单例

        private static ItemInfoAsset _instance;
        public static ItemInfoAsset Instance
        {
            get
            {

                if (!_instance)
                {
                    if(File.Exists("Assets/GlobalItemInfo.asset"))
                        _instance = UnityEditor.AssetDatabase.LoadAssetAtPath<ItemInfoAsset>("Assets/GlobalItemInfo.asset");
                }
#if UNITY_EDITOR
                if (!_instance)
                {
                    _instance = CreateInstance<ItemInfoAsset>();
                    UnityEditor.AssetDatabase.CreateAsset(_instance, "Assets/GlobalItemInfo.asset");
                }
#endif
                if (_instance == null)
                    throw new System.Exception("初始化失败");

                return _instance;
            }
        }        

        #endregion

        public int GetID()
        {
            var index = 0;
            while (true)
            {
                var ok = true;
                foreach (var unit in items)
                {
                    if (index == unit.ID)
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
        
        public List<ItemUnitInfo> items=new List<ItemUnitInfo>();
        
        public ItemUnitInfo GetItem(int id)
        {
            foreach (var VARIABLE in items)
            {
                if (VARIABLE.ID == id)
                    return VARIABLE;
            }
            return null;
        }

        public T GetItem<T>(int id) where T : ItemUnitInfo
        {
            return GetItem(id) as T;
        }

    }
}
