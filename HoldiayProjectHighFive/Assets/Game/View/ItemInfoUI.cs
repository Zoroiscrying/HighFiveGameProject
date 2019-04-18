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
        private Text content;
        public ItemInfoUI()
        {
            Create(UIPath.Image_ItemInfo);
            this.content = m_TransFrom.Find("Text").GetComponent<Text>();
        }
        
        public void Set(int itemID,Vector3 pos)
        {
            
            var item = ItemMgr.GetItem(itemID);
            this.content.text = item.Name+"\n"+ item.Capacity.ToString()+"\n"+ item.Description;
            m_TransFrom.position = pos;
        }
    }
}
