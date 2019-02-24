using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
using Game.Control;

namespace Game.Modal
{
    public class Backpack
    {
        private List<AbstractUIItem> itemList=new List<AbstractUIItem>();
        
        /// <summary>
        /// 获取物品数量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public int GetItemCount<T>()where T:AbstractUIItem
        {
            foreach (var item in itemList)
            {
                if (item is T)
                    return item.num;
            }
            return 0;
        }

        /// <summary>
        /// 添加物品
        /// </summary>
        /// <param name="uiItem"></param>
        public bool AddItem(AbstractUIItem uiItem)
        {
            return false;
        }

        /// <summary>
        /// 移除物品
        /// </summary>
        /// <param name="uiItem"></param>
        /// <returns></returns>
        public bool RemoveItem(AbstractItem uiItem)
        {
            return false;
        }
        
    }
}