using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game.Model.ItemSystem
{
    public static class ItemMgr
    {
        private static Dictionary<int, IItemTriggerFactory> factoryDic = new Dictionary<int, IItemTriggerFactory>();

        /// <summary>
        /// 注册不同种类的触发器
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="factory"></param>
        public static void RegisterItemFactory(int itemId, IItemTriggerFactory factory)
        {
            if (!factoryDic.ContainsKey(itemId))
                factoryDic.Add(itemId, factory);
        }


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
        /// <summary>
        /// AbstractPerson通过技能名从这里获取技能索引
        /// </summary>
        public static Dictionary<int, AbstractItem> itemDic = new Dictionary<int, AbstractItem>();

        /// <summary>
        /// 文件读取Item
        /// </summary>
        /// <param name="path"></param>
        public static void LoadItemsFromFile(string path)
        {
            path = Application.streamingAssetsPath + "\\" + path;
            var sr = new StreamReader(path);
            AbstractItem item = null;
            do
            {
                string line = sr.ReadLine();
                if (line == null)
                    break;

                line = line.Trim();

                //注释写法   //注释
                if (line.StartsWith("//") || line == "")
                    continue;

                var strs = line.Split('|');
                item = CreateItem(line);
                itemDic.Add(Convert.ToInt32(strs[0].Trim()), item);


            } while (true);
        }
    }
}
