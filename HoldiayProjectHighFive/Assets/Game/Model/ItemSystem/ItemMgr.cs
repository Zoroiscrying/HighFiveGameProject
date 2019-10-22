using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Game.Model.ItemSystem
{
    public class ItemMgr:ScriptableObject
    {
        #region 单例

        private static ItemMgr _instance;
        public static ItemMgr Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = Resources.Load<ItemMgr>("GlobalAssets/GlobalItemInfo");
                }
#if UNITY_EDITOR
                if (!_instance)
                {
                    _instance = CreateInstance<ItemMgr>();
                    var path = "Assets/Resources";
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    path += "/GlobalAssets";
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    path += "/GlobalItemInfo.asset";
                    UnityEditor.AssetDatabase.CreateAsset(_instance, path);
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
