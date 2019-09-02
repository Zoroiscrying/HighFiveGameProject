using Game.Const;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using ReadyGamerOne.Common;
using ReadyGamerOne.Global;
using ReadyGamerOne.MemorySystem;

namespace Game.Model.ItemSystem
{
    public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {

        public bool IsEmpty
        {
            get
            {
                return this.itemUi == null;
            }
        }
        public ItemUI itemUi = null;
        public int index;

        public int ItemCount
        {
            get
            {
                if (IsEmpty)
                    return 0;
                return this.itemUi.count;
            }
            private set
            {
                Debug.Log(value);
                Assert.IsTrue(value >= 0 && value <= ItemMgr.GetItem(this.itemUi.itemId).Capacity);

                //数值上
                this.itemUi.count = value;


                //UI上
                if (value == 1)
                {
                    this.itemNum.text = "";
                }
                else
                {
                    this.itemNum.text = value.ToString();
                }

                foreach(var v in GlobalVar.G_Player.itemList)
                    Debug.Log("v.count:" + v.count + " v.id" + v.itemId);
            }
        }
        private Text itemNum;
        private Image itemImage;
        
        /// <summary>
        /// 实例化UI物体，加图片，为变量itemImage,itemNum赋值
        /// </summary>
        private void FillSlot()
        {
            if(this==null)
            {
                throw new Exception("????????????");
            }

            this.itemUi.obj = MemoryMgr.InstantiateGameObject(UiName.Image_ItemUI, transform);
            this.itemImage = this.itemUi.obj.GetComponent<Image>();
            Assert.IsTrue(this.itemImage);
            this.itemImage.sprite = MemoryMgr.GetSourceFromResources<Sprite>(this.itemUi.SpritePath);
            this.itemNum = this.itemUi.obj.GetComponentInChildren<Text>();
            Assert.IsTrue(this.itemNum);
        }
        /// <summary>
        /// 清空物品格
        /// </summary>
        public void ClearSlot()
        {
            Assert.IsTrue(!this.IsEmpty);
            this.itemNum = null;
            this.itemImage = null;
            if(this.itemUi.obj)
                Destroy(this.itemUi.obj);
            this.itemUi = null;
        }

        /// <summary>
        /// 添加物品
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="count"></param>
        public void AddItem(int itemId,int count)
        {
            if (count == 0)
                return;
            Assert.IsTrue(count > 0);
            //Assert.IsTrue(this.index >= Global.GlobalVar.Player.itemList.Count);
            if(this.IsEmpty)
            {
                Assert.IsTrue(this.itemUi == null);
                this.itemUi = new ItemUI(itemId);
                
                FillSlot();
                this.ItemCount = count;
            }
            else
            {
                this.ItemCount += count;
            }
        }

        public void AddItem(Control.PersonSystem.Player.ItemData itemData,int count)
        {

            AddItem(itemData.itemId, count);
        }

        public void AddItem(int count)
        {
            Assert.IsTrue(this.itemUi!=null);
            this.ItemCount += count;
        }

        /// <summary>
        /// 移除物品
        /// </summary>
        /// <param name="count"></param>
        public void RemoveItem(int count)
        {
            Assert.IsTrue(!IsEmpty && count > 0 && count <= this.ItemCount);
            if(count<ItemCount)
            {
                ItemCount -= count;
            }
            else
            {
                ClearSlot();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (IsEmpty)
                return;
            Debug.Log("???");
            CEventCenter.BroadMessage(Message.M_TouchItem, this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (IsEmpty)
                return;
            CEventCenter.BroadMessage(Message.M_ReleaseItem, this);
        }
    }
}
