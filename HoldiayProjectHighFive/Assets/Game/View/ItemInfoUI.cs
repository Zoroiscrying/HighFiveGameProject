using Game.Const;
using UnityEngine;
using UnityEngine.UI;
using Game.Model.ItemSystem;
using ReadyGamerOne.View;

namespace Game.View
{
    public class ItemInfoUI:AbstractUI
    {
        private Text name;
        public ItemInfoUI()
        {
            Create(DirPath.LittleUiDir+ UiName.Image_ItemInfo);
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
