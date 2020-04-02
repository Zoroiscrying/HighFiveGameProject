using System;
using System.Collections.Generic;
using DG.Tweening;
using HighFive.Const;
using HighFive.Data;
using HighFive.Model.ItemSystem;
using ReadyGamerOne.Common;
using ReadyGamerOne.MemorySystem;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using HighFive.Global;
using HighFive.Script;
using ReadyGamerOne.Script;
using ReadyGamerOne.Utility;

namespace HighFive.View
{
	public partial class PackagePanel
	{       
		#region proprietary


		enum WorkMode
		{
			Normal,
			Info,
			Lock,
		}

		private WorkMode _workMode = WorkMode.Normal;

		#region 物品管理

		enum FilterType
		{
			All,
			Drag,
			Gem
		}
		private FilterType _filterType=FilterType.All;
		private AnimateGrid grid;
		private Transform filterMask;		

		#endregion
		
		#region 物品详情

		private Transform itemInfo;
		private RectTransform showPos;
		private RectTransform infoMask;
		private Text itemNameText;
		private Text itemPriceText;
		private Text itemStatements;
		private Text itemStory;
		private Text itemAccesses;
		private Slot curSlot;
		private Vector3 positionBefore;

		#endregion


		private Dictionary<string, Slot> G_idToItemUi=new Dictionary<string, Slot>();
		#endregion
		
		partial void OnLoad()
		{

			#region 物品管理

			this.filterMask = GetTransform("Image_BackGround/ItemBg/Mask");
			
			this.grid = GetComponent<AnimateGrid>("Image_BackGround/ItemBg/Image_ItemGrid");

			Assert.IsTrue(this.filterMask && this.grid);
			this.grid.CheckIfSort =
				childRect =>
				{
					var data = childRect.GetComponent<Slot>().itemData;

					switch (_filterType)
					{
						case FilterType.Drag:
							return data is DragData;
						case FilterType.Gem:
							return data is GemData;
						default:
							return true;
					}
				};

			var allBtn = GetComponent<Image>("Image_BackGround/ItemBg/BtnBar/AllBtn");
			allBtn.gameObject.AddComponent<UIInputer>().eventOnPointerClick +=
				data => OnClickBtn(FilterType.All, allBtn.transform);
			
			var dragBtn=GetComponent<Image>("Image_BackGround/ItemBg/BtnBar/DragBtn");
			dragBtn.gameObject.AddComponent<UIInputer>().eventOnPointerClick+=
				data => OnClickBtn(FilterType.Drag, dragBtn.transform);

			var gemBtn = GetComponent<Image>("Image_BackGround/ItemBg/BtnBar/GemBtn");
			gemBtn.gameObject.AddComponent<UIInputer>().eventOnPointerClick+=
				data => OnClickBtn(FilterType.Gem, gemBtn.transform);			

			#endregion

			#region 物品详情的展示

			this.itemInfo = GetTransform("Image_BackGround/ItemInfo");

			itemInfo.gameObject.AddComponent<UIInputer>().eventOnPointerClick += data => SwitchMode(WorkMode.Normal);
			
			
			this.showPos = this.itemInfo.GetComponent<RectTransform>("ShowPos");

			this.infoMask = this.itemInfo.GetComponent<RectTransform>("ItemInfo");


			this.itemNameText = this.itemInfo.GetComponent<Text>("ItemInfo/InfoBg/ItemName");
			this.itemPriceText = this.itemInfo.GetComponent<Text>("ItemInfo/InfoBg/Price");
			this.itemStatements = this.itemInfo.GetComponent<Text>("ItemInfo/InfoBg/Statements");
			this.itemStory = this.itemInfo.GetComponent<Text>("ItemInfo/InfoBg/Story");
			this.itemAccesses = this.itemInfo.GetComponent<Text>("ItemInfo/InfoBg/Accesses");
			
			
			#endregion
			
			//恢复背包信息
			foreach(var item in GlobalVar.G_Player.GetItems())
			{
				OnAddItem(item.ID, item.Count);
			}
		}

		#region 消息处理

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
                var obj = ResourceMgr.InstantiateGameObject(PrefabName.Slot, grid.transform);
                obj.transform.localPosition=Vector3.zero;
                var slotUi = obj.GetComponent<Slot>();
                if (!slotUi)
                    throw new Exception("获取到的ItemInfoUI为空");

                slotUi.Refresh(itemId, count, id => G_idToItemUi.Remove(id));

                G_idToItemUi.Add(itemId, slotUi);
                
                grid.ReBuild();
            }
            else
                G_idToItemUi[itemId].Add(count);
        }		

		#endregion


		#region Native

		private void OnClickBtn(FilterType filterType, Transform target)
		{
			_filterType = filterType;
			this.grid.ReBuild();
			DOTween.To(
					() => this.filterMask.position,
					value => filterMask.position = value,
					target.position,
					0.5f)
				.SetEase(Ease.InOutExpo);
		}

		#endregion
        

        #region Override

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
		        kv.Value.transform.SetParent(grid.transform);
		        kv.Value.onPointerClick += OnClickSlot;
	        }
	        
	        grid.ReBuild();
        }

        public override void Disable()
        {
	        base.Disable();
	        
	        _workMode = WorkMode.Normal;
	        itemInfo.gameObject.SetActive(false);
	        
	        foreach (var kv in G_idToItemUi)
	        {
		        kv.Value.onPointerEnter -= OnClickSlot;
	        }
        }
        
		public override void Destory()
		{
			base.Destory();
			G_idToItemUi.Clear();
		}        

        #endregion
        

        private void OnClickSlot(Slot slot)
        {
	        switch (_workMode)
	        {
		        case WorkMode.Info:
			        SwitchMode(WorkMode.Normal);
			        curSlot = null;
			        positionBefore=Vector3.zero;
			        break;
		        case WorkMode.Normal:
			        curSlot = slot;
			        positionBefore = slot.transform.position;
//			        Debug.Log("赋值："+slot.name);
			        SwitchMode(WorkMode.Info);
			        break;
		        default:
			        break;
	        }
        }

        private void SwitchMode(WorkMode mode)
        {
	        Assert.IsTrue(curSlot);
	        this._workMode = WorkMode.Lock;
	        switch (mode)
	        {
		        case WorkMode.Info:
			        
			        itemInfo.gameObject.SetActive(true);

			        curSlot.transform.SetParent(showPos, true);
			        
			        //数据
			        infoMask.sizeDelta = new Vector2(0, infoMask.sizeDelta.y);
			        
			        var itemData = curSlot.itemData;
			        itemNameText.text = itemData.uitext;
			        itemPriceText.text = $"买入\t{itemData.price}\n卖出：{itemData.outPrice}";
			        itemStatements.text = itemData.statement;

			        //动画
			        DOTween.To(
					        () => curSlot.transform.position,
					        value => curSlot.transform.position = value,
					        showPos.position,
					        0.5f)
				        .SetEase(Ease.InOutExpo);
			        DOTween.To(
					        () => curSlot.transform.localScale,
					        value => curSlot.transform.localScale = value,
					        Vector3.one * 2,
					        0.5f)
				        .SetEase(Ease.InOutExpo);
			        DOTween.To(
					        () => infoMask.sizeDelta.x,
					        value => infoMask.sizeDelta = new Vector2(value, infoMask.sizeDelta.y),
					        600,
					        0.5f)
				        .SetEase(Ease.InOutExpo)
				        .onComplete += () => this._workMode = WorkMode.Info;
			        break;
		        case WorkMode.Normal:

			        DOTween.To(
					        () => infoMask.sizeDelta.x,
					        value => infoMask.sizeDelta = new Vector2(value, infoMask.sizeDelta.y),
					        0,
					        0.5f)
				        .SetEase(Ease.InOutExpo);
			        DOTween.To(
					        () => curSlot.transform.localScale,
					        value => curSlot.transform.localScale = value,
					        Vector3.one,
					        0.5f)
				        .SetEase(Ease.InOutExpo);
			        DOTween.To(
					        () => curSlot.transform.position,
					        value => curSlot.transform.position = value,
					        positionBefore,
					        0.5f)
				        .SetEase(Ease.InOutExpo)
				        .onComplete += () =>
			        {
				        itemInfo.gameObject.SetActive(false);

				        curSlot.transform.SetParent(grid.transform, true);

				        _workMode = WorkMode.Normal;
			        };
			        break;
	        }
        }
	}
}
