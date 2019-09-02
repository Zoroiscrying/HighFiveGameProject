using UnityEngine;

namespace Game.Model.ItemSystem
{
    /// <summary>
    /// 这个类用来管理 用于显示的UI物品和GameObject
    /// </summary>
    public class ItemUI
    {
        public int itemId;
        public int count;
        //public int itemCount;

        public ItemUI(int itemid)
        {
            this.itemId = itemid;
        }

        public GameObject obj = null;
        public AbstractItem Item
        {
            get
            {
                return ItemMgr.GetItem(this.itemId);
            }
        }
        
        public string SpritePath
        {
            get
            {
                return ItemMgr.GetItem(this.itemId).SpritePath;
            }
        }
    }
}
