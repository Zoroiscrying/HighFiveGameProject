using HighFive.Const;
using HighFive.Model.ItemSystem;
using ReadyGamerOne.View;
using UnityEngine;
using UnityEngine.UI;

namespace HighFive.View
{
    public class ItemInfoUI:AbstractUI
    {
        private Text name;
        public ItemInfoUI()
        {
            Create(UiPath.Image_ItemInfo);
            this.name = m_TransFrom.Find("Text").GetComponent<Text>();
        }
        
        public void Set(int itemID,Vector3 pos)
        {
            
             var item = ItemMgr.Instance.GetItem(itemID);
             this.name.text = item.ToString();
            m_TransFrom.position = pos;
        }
    }
}
