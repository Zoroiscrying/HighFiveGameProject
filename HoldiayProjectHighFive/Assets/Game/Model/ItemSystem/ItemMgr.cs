using System;
using System.Collections.Generic;

namespace Game.Model.ItemSystem
{
    public static class ItemMgr
    {
        public static AbstractItem GetItem(int id)
        {
            if(!itemDic.ContainsKey(id))
            {
                throw new Exception("没有这个物品信息" + id);
            }
            return itemDic[id];
        }

        public static T GetItem<T>(int id) where T : AbstractItem
        {
            return GetItem(id) as T;
        }
        /// <summary>
        /// AbstractPerson通过技能名从这里获取Item索引
        /// </summary>
        public static Dictionary<int, AbstractItem> itemDic = new Dictionary<int, AbstractItem>();

    }
}
