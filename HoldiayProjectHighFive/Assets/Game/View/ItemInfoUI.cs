using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Const;
using UnityEngine;
using UnityEngine.UI;
using Game.Model.ItemSystem;
namespace Game.View
{
    public class ItemInfoUI:AbstractUI
    {
        private Text name;
        private Text function;
        private Text story;
        public ItemInfoUI()
        {
            Create(UIPath.Image_ItemInfo);
            this.name = m_TransFrom.Find("Text_Name").GetComponent<Text>();
            this.function = m_TransFrom.Find("Text_Function").GetComponent<Text>();
            this.story = m_TransFrom.Find("Text_Story").GetComponent<Text>();
        }
        
        public void Set(int itemID,Vector3 pos)
        {
            
             var item = ItemMgr.GetItem(itemID);
            this.name.text = item.Name;
            this.function.text = item.Capacity.ToString();
            this.story.text = item.Description;
            m_TransFrom.position = pos;
        }
    }
}
