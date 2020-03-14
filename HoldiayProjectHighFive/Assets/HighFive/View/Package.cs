using System;
using System.Collections.Generic;
using HighFive.Const;
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
		private RectTransform backPackGridObj;
		private Image moreInfoSpriteObj;
		private Text moreInfoTextObj;

		private Dictionary<string, Slot> G_idToItemUi=new Dictionary<string, Slot>();
		#endregion
		partial void OnLoad()
		{
			this.backPackGridObj = view["Image_BackGround/Image_BackPackGrid"].GetComponent<RectTransform>();
			this.moreInfoTextObj = view["Image_BackGround/Image_MoreInfo/Image_TextBackGround/Text"].GetComponent<Text>();
			this.moreInfoSpriteObj = view["Image_BackGround/Image_MoreInfo/Image_BigPicture"].GetComponent<Image>();
			Assert.IsTrue(this.moreInfoTextObj&&this.moreInfoSpriteObj&&this.backPackGridObj);
			
			//恢复背包信息

			foreach(var item in GlobalVar.G_Player.GetItems())
			{
				OnAddItem(item.ID, item.Count);
			}
			
			moreInfoSpriteObj.enabled = false;
		}
		
		public override void Destory()
		{
			base.Destory();
			G_idToItemUi.Clear();
		}

		private void OnRemoveItem(string itemId, int count)
        {
            if (!G_idToItemUi.ContainsKey(itemId))
                throw new Exception("Ui 上没有？" + itemId);
            G_idToItemUi[itemId].Remove(count);
        }

        private void OnAddItem(string itemId, int count)
        {
            if (!G_idToItemUi.ContainsKey(itemId))
            {
                var obj = ResourceMgr.InstantiateGameObject(PrefabName.Slot, backPackGridObj);
                var infoUi = obj.GetComponent<Slot>();
                if (!infoUi)
                    throw new Exception("获取到的ItemInfoUI为空");

                infoUi.Refresh(itemId, count, id => G_idToItemUi.Remove(id));

                G_idToItemUi.Add(itemId, infoUi);
                
                LayoutRebuilder.ForceRebuildLayoutImmediate(backPackGridObj.GetComponent<RectTransform>());
            }
            else
                G_idToItemUi[itemId].Add(count);
        }

        protected override void OnAddListener()
        {
            base.OnAddListener();
            CEventCenter.AddListener<string, int>(Message.M_AddItem, OnAddItem);
            CEventCenter.AddListener<string, int>(Message.M_RemoveItem, OnRemoveItem);
        }

        protected override void OnRemoveListener()
        {
            base.OnRemoveListener();
            CEventCenter.RemoveListener<string, int>(Message.M_AddItem, OnAddItem);
            CEventCenter.RemoveListener<string, int>(Message.M_RemoveItem, OnRemoveItem);
        }
        
        
        
        
        public override void Enable()
        {
	        base.Enable();
	        foreach (var kv in G_idToItemUi)
	        {
		        kv.Value.transform.SetParent(backPackGridObj);
		        kv.Value.onPointerEnter += OnPointerEnter;
		        kv.Value.onPointerExit += OnPointerExit;
	        }
	        LayoutRebuilder.ForceRebuildLayoutImmediate(backPackGridObj);
        }

        public override void Disable()
        {
	        base.Disable();
	        foreach (var kv in G_idToItemUi)
	        {
		        kv.Value.transform.SetParent(backPackGridObj);
		        kv.Value.onPointerEnter -= OnPointerEnter;
		        kv.Value.onPointerExit -= OnPointerExit;
	        }
//	        backPackGridObj.transform.DetachChildren();
        }
        
        
        private void OnPointerEnter(Slot ui)
        {
	        moreInfoSpriteObj.enabled = true;
	        moreInfoTextObj.text = ui.itemData.statement;
	        moreInfoSpriteObj.sprite = ResourceMgr.GetAsset<Sprite>(ui.itemData.ID);
	        //title.text = ui.itemData.uiText;
        }

        private void OnPointerExit(Slot ui)
        {
	        moreInfoTextObj.text = "";
	        moreInfoSpriteObj.enabled = false;
        }
	}
}
