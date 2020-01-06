using System.Collections.Generic;
using HighFive.Const;
using HighFive.Control.PersonSystem.Persons;
using HighFive.Model.ItemSystem;
using ReadyGamerOne.Common;
using ReadyGamerOne.MemorySystem;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using HighFive.Global;

namespace HighFive.View
{
	public partial class Package
	{       
		#region proprietary
		public int SlotCount
		{
			get
			{
				return DefaultData.DefaultPackageSlotCount;
			}
		}
		private Transform backPackGridObj;
		private Transform moreInfoSpriteObj;
		private Transform moreInfoTextObj;

		private Slot[] slots;
		private List<Player.ItemData> itemList;
		private ItemInfoUI itemInfoUi;
		#endregion
		partial void OnLoad()
		{
			//do any thing you want
			this.itemList =GlobalVar.G_Player.playerInfo.itemList;

			this.backPackGridObj = view["Image_BackGround/Image_BackPackGrid"].transform;
			this.moreInfoTextObj = view["Image_BackGround/Image_MoreInfo/Image_TextBackGround/Text"].transform;
			this.moreInfoSpriteObj = view["Image_BackGround/Image_MoreInfo/Image_BigPicture"].transform;
			Assert.IsTrue(this.moreInfoTextObj&&this.moreInfoSpriteObj&&this.backPackGridObj);
			this.itemInfoUi = new ItemInfoUI();

			//恢复背包信息
			this.slots = backPackGridObj.GetComponentsInChildren<Slot>();
			for(int i=0;i<slots.Length;i++)
			{
				slots[i].index = i;
			}
			foreach(var item in this.itemList)
			{
				AddItem(item.itemId, item.count);
			}
			//LayoutRebuilder.ForceRebuildLayoutImmediate(挂载HorizontalLayoutGroup的物体的RectTransform);
			Debug.Log("Load:   slots.Length:" + slots.Length);
		}
		
		        public override void Disable()
        {
            base.Disable();
            if(this.itemInfoUi!=null)
                this.itemInfoUi.DestroyThis();
            foreach (var s in slots)
                if(!s.IsEmpty)
                    s.ClearSlot();
        }

        protected override void OnAddListener()
        {
            base.OnAddListener();
            CEventCenter.AddListener<Slot>(Message.M_TouchItem, this.OnTouchItem);
            CEventCenter.AddListener<Slot>(Message.M_ReleaseItem, this.OnReleaseItem);
        }

        protected override void OnRemoveListener()
        {
            base.OnRemoveListener();
            CEventCenter.RemoveListener<Slot>(Message.M_TouchItem,this.OnTouchItem);
            CEventCenter.RemoveListener<Slot>(Message.M_ReleaseItem,this.OnReleaseItem);
        }

        #region Private
        /// <summary>
        /// 从fromIndex寻找第一个id相同的物品
        /// </summary>
        /// <param CharacterName="itemId"></param>
        /// <param CharacterName="fromIndex"></param>
        /// <returns></returns>
        private Slot FindSlot(int itemId,int fromIndex=0)
        {
            for (int i = fromIndex; i < this.slots.Length; i++)
            {
                if (!slots[i].IsEmpty && slots[i].itemUi.itemId == itemId)
                {
                    return slots[i];
                }
            }
            return null;
        }
        /// <summary>
        /// 从fromIndex寻找第一个没有装满的id相同的物品
        /// </summary>
        /// <param CharacterName="itemId"></param>
        /// <param CharacterName="fromIndex"></param>
        /// <returns></returns>
        private Slot FindSlotNotFill(int itemId,int fromIndex=0)
        {
            for(int i= fromIndex; i<this.slots.Length;i++)
            {
                if(!slots[i].IsEmpty&&slots[i].itemUi.itemId==itemId)
                {
                    if (slots[i].ItemCount == ItemMgr.Instance.GetItem(slots[i].itemUi.itemId).Capacity)
                        continue;
                    else
                        return slots[i];
                }
            }
            return null;
        }
        /// <summary>
        /// 寻找第一个空格子
        /// </summary>
        /// <returns></returns>
        private Slot FindEmptySlot()
        {
            Debug.Log("寻找空格");
            Debug.Log("slots.Length:" + slots.Length);
            foreach(var s in this.slots)
            {
                Debug.Log("遍历中……");
                if (s.IsEmpty)
                {
                    Debug.Log("发现空格");
                    if (s == null)
                        Debug.Log("但是s is null");
                    return s;
                }
            }
            Debug.Log("woc，居然没有空格");
            return null;
        }
        private bool TryStoreManyItem(int itemId, int count)
        {
            int num = count;
            int capacity = ItemMgr.Instance.GetItem(itemId).Capacity;
            Assert.IsTrue(count > capacity);
            while (num>capacity)
            {
                var slot = FindEmptySlot();
                if(slot)
                {
                    slot.AddItem(itemId, capacity);
                    num -= capacity;
                }
                else
                {
                    return false;
                }
            }
            if(num==0)
            {
                return true;
            }
            else
            {
                var slot = FindEmptySlot();
                if(slot)
                {
                    slot.AddItem(itemId, num);
                    return true;
                }
                {
                    return false;
                }
            }
        }
        private bool TryRemoveManyItem(int itemId,int count)
        {
            Assert.IsTrue(count > 0);
            var num = count;
            while(num>0)
            {
                var slot = FindSlot(itemId);
                if (slot == null)
                    return false;
                var removeNum = Mathf.Min(num, slot.ItemCount);
                num -= removeNum;

            }
            return true;
        }

        #endregion

        /// <summary>
        /// 添加物品
        /// </summary>
        /// <param CharacterName="itemId"></param>
        /// <param CharacterName="count"></param>
        public void AddItem(int itemId,int count)
        {
            int capcity = ItemMgr.Instance.GetItem(itemId).Capacity;
            //查看是否存有同种物品
            var slot = FindSlotNotFill(itemId);
            if(slot)
            {
                //判断是否超过单格最大容量
                if(slot.ItemCount+count> capcity)
                {
                    Debug.Log("超量！");
                    var okCount = capcity - slot.ItemCount;
                    slot.AddItem(okCount);
                    //寻找新格子存放剩余物品
                    var newSlot = FindEmptySlot();
                    if(newSlot==null)
                    {
                        Debug.Log("无法存储更多物品");
                    }
                    else
                    {
                        var leftCount = count - okCount;
                        //剩余数量是否仍然超量
                        if(leftCount>=capcity)
                        {
                            if(!TryStoreManyItem(itemId, leftCount))
                            {
                                Debug.Log("无法存储更多物品");
                            }
                        }
                        else
                        {
                            newSlot.AddItem(itemId, leftCount);
                        }
                    }
                }
                else//不会超过单格最大容量
                {
                    Debug.Log("没有超量");
                    slot.AddItem(count);
                }
            }
            else//没有同种物品
            {
                var newSlot = FindEmptySlot();
                if (newSlot == null)
                {
                    Debug.Log("无法存储更多物品");
                }
                else
                {
                    //数量是否超量
                    if (count > capcity)
                    {
                        if (!TryStoreManyItem(itemId, count))
                        {
                            Debug.Log("无法存储更多物品");
                        }
                    }
                    else
                    {
                        newSlot.AddItem(itemId, count);
                    }
                }
            }
        }
        /// <summary>
        /// 移除物品
        /// </summary>
        /// <param CharacterName="itemId"></param>
        /// <param CharacterName="count"></param>
        public void RemoveItem(int itemId,int count)
        {
            var slot = FindSlot(itemId);
            if(null==slot)
            {
                Debug.Log("没有那么多的" + ItemMgr.Instance.GetItem(itemId).Name);
            }
            else
            {
                if(count<=slot.ItemCount)
                {
                    slot.RemoveItem(count);
                }
                else
                {
                    TryRemoveManyItem(itemId, count);
                }
            }

        }

        #region 消息处理

        private void OnTouchItem(Slot slot)
        {
            var item = ItemMgr.Instance.GetItem(slot.itemUi.itemId);
            this.itemInfoUi.Set(slot.itemUi.itemId, slot.transform.position);
            this.itemInfoUi.Show();
            this.moreInfoTextObj.GetComponent<Text>().text = item.ToString();
            var sprite = MemoryMgr.GetSourceFromResources<Sprite>(item.SpritePath);
            Assert.IsTrue(sprite);
            this.moreInfoSpriteObj.GetComponent<Image>().sprite = sprite;
        }
        private void OnReleaseItem(Slot slot)
        {
            this.itemInfoUi.Hide();
        }

        #endregion
	}
}