using System;
using System.Collections.Generic;
using DG.Tweening;
using Game.Scripts;
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

		#region 矿石槽

		private HighFiveItem[] gemItems;
		private Transform effectiveGemBar;

		#endregion

		
		private Dictionary<string, HiveFiveItemSlot> G_idToItemUi=new Dictionary<string, HiveFiveItemSlot>();

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
		private HiveFiveItemSlot _curHiveFiveItemSlot;
		private Vector3 positionBefore;

		#endregion


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
					var data = childRect.GetComponent<HiveFiveItemSlot>().ItemData;

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

			#region 宝石槽初始化

			effectiveGemBar = GetTransform("Image_BackGround/Image_Player/EffectiveGemBar");
			Assert.IsTrue(effectiveGemBar);
			
			var gemCount = effectiveGemBar.childCount;
			if(gemItems==null)
				gemItems=new HighFiveItem[gemCount];

			#endregion
			
			//恢复背包信息
            foreach(var kv in GlobalVar.G_Player.GetItems())
            {
	            var item = kv.Value;
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
	            //实例化
                var obj = ResourceMgr.InstantiateGameObject(UiName.Slot);

                //初始化Slot信息
                var slotUi = obj.GetComponent<HiveFiveItemSlot>();
                if (!slotUi)
                    throw new Exception("获取到的ItemInfoUI为空");
                slotUi.Init(itemId, count, id => G_idToItemUi.Remove(id));
				slotUi.InitDragger(GemboxIsTarget,SlotIsTarget,grid.ReBuild);
				
				//初始化拖拽状态
                var item = GlobalVar.G_Player.GetItems()[itemId];
                if (item.gemBoxIndex == -1)
                {
	                var parent = this.grid.transform;
	                slotUi.SwitchDragType(DragType.Package, parent.gameObject);
                }
                else
                {
	                var parent = this.effectiveGemBar.GetChild(item.gemBoxIndex);
	                slotUi.SwitchDragType(DragType.Gembox, parent.gameObject);
                }

                //Ui重建
                grid.ReBuild();
                //本地保存
                G_idToItemUi.Add(itemId,slotUi);
            }
            else
				G_idToItemUi[itemId].Add(count);
        }		

		#endregion


		#region Native

		/// <summary>
		/// 宝石槽里的宝石移动目标判定
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		private bool GemboxIsTarget(GameObject obj)
		{
			return obj == this.grid.gameObject;
		}

		/// <summary>
		/// 物品槽里的宝石判定移动目标
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		private bool SlotIsTarget(GameObject obj)
		{
			var ans = obj.transform.parent == effectiveGemBar;
//			if (!ans)
//			{
//				Debug.LogWarning($"obj:{obj.name},parent:{obj.transform.parent.name}");
//			}
			return ans;
		}



		/// <summary>
		/// 切换物品过滤模式
		/// </summary>
		/// <param name="filterType"></param>
		/// <param name="target"></param>
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
		
		/// <summary>
		/// 点击物品槽的逻辑
		/// </summary>
		/// <param name="hiveFiveItemSlot"></param>
        private void OnClickSlot(HiveFiveItemSlot hiveFiveItemSlot)
        {
	        switch (_workMode)
	        {
		        case WorkMode.Info:
			        SwitchMode(WorkMode.Normal);
			        _curHiveFiveItemSlot = null;
			        positionBefore=Vector3.zero;
			        break;
		        case WorkMode.Normal:
			        _curHiveFiveItemSlot = hiveFiveItemSlot;
			        positionBefore = hiveFiveItemSlot.transform.position;
//			        Debug.Log("赋值："+hiveFiveItemSlot.name);
			        SwitchMode(WorkMode.Info);
			        break;
		        default:
			        break;
	        }
        }

        /// <summary>
        /// 切换显示模式（详情、普通，锁定）
        /// </summary>
        /// <param name="mode"></param>
        private void SwitchMode(WorkMode mode)
        {
	        Assert.IsTrue(_curHiveFiveItemSlot);
	        this._workMode = WorkMode.Lock;
	        switch (mode)
	        {
		        case WorkMode.Info:
			        
			        itemInfo.gameObject.SetActive(true);

			        _curHiveFiveItemSlot.transform.SetParent(showPos, true);
			        
			        //数据
			        infoMask.sizeDelta = new Vector2(0, infoMask.sizeDelta.y);
			        
			        var itemData = _curHiveFiveItemSlot.ItemData;
			        itemNameText.text = itemData.uitext;
			        itemPriceText.text = $"买入\t{itemData.price}\n卖出：{itemData.outPrice}";
			        itemStatements.text = itemData.statement;

			        //动画
			        DOTween.To(
					        () => _curHiveFiveItemSlot.transform.position,
					        value => _curHiveFiveItemSlot.transform.position = value,
					        showPos.position,
					        0.5f)
				        .SetEase(Ease.InOutExpo);
			        DOTween.To(
					        () => _curHiveFiveItemSlot.transform.localScale,
					        value => _curHiveFiveItemSlot.transform.localScale = value,
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
					        () => _curHiveFiveItemSlot.transform.localScale,
					        value => _curHiveFiveItemSlot.transform.localScale = value,
					        Vector3.one,
					        0.5f)
				        .SetEase(Ease.InOutExpo);
			        DOTween.To(
					        () => _curHiveFiveItemSlot.transform.position,
					        value => _curHiveFiveItemSlot.transform.position = value,
					        positionBefore,
					        0.5f)
				        .SetEase(Ease.InOutExpo)
				        .onComplete += () =>
			        {
				        itemInfo.gameObject.SetActive(false);

				        _curHiveFiveItemSlot.transform.SetParent(grid.transform, true);

				        _workMode = WorkMode.Normal;
			        };
			        break;
	        }
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
		        kv.Value.onPointerClick -= OnClickSlot;
	        }
        }
        
		public override void Destory()
		{
			base.Destory();
			G_idToItemUi.Clear();
		}        

        #endregion
        


	}
}
