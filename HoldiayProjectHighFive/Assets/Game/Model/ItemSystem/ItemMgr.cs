using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Game.Data;
using UnityEngine;

namespace Game.Model.ItemSystem
{
    public static class ItemMgr
    {
        private static Dictionary<int, IItemTriggerFactory> factoryDic = new Dictionary<int, IItemTriggerFactory>();

        private static AbstractItem CreateItem(string args)//skillType,int id,float startTime, float lastTime,string args)
        {
            var strs = args.Split('|');
            var type =Convert.ToInt32( strs[0].Trim());
            if (!factoryDic.ContainsKey(type))
                throw new Exception("工厂中没有这个触发器类型" + args);
            return factoryDic[type].CreateItem(args);//skillType,id,startTime,lastTime,args);
        }
        

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
