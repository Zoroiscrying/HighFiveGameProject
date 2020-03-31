using System;
using System.Collections.Generic;
using System.Linq;
using HighFive.Const;
using HighFive.Data;
using ReadyGamerOne.Common;
using ReadyGamerOne.MemorySystem;
using ReadyGamerOne.Script;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using HighFive.Global;
using ReadyGamerOne.Data;


namespace HighFive.View
{
	public partial class ShopPanel
	{	
		private int currentItemIndex;

		private int CurrentItemIndex
		{
			set
			{
			
				currentItemIndex = value;
				this.itemInfo.text = CsvMgr.GetData<ShopItemData>(itemIdList[value]).statement;
			}
			get { return currentItemIndex; }
		}
		private int CurrentMoney
		{
			get { return Convert.ToInt32(this.playerMoney.text); }
			set { playerMoney.text = value.ToString(); }
		}

		private List<string> itemIdList=DefaultData.shop.ToList();
		private List<GameObject> items =new List<GameObject>();

		private float scrollTime = .25f;
		
		
		private Transform itemList;
		private Transform buyBtn;
		private TextMeshProUGUI itemInfo;

		private TextMeshProUGUI playerMoney;
		partial void OnLoad()
		{
			//do any thing you want
			this.itemList = view["Image_ShopBk/Image_Back/Empty_ItemList"].transform;
			this.buyBtn = view["Image_ShopBk/Image_ItemInfo/Tmp_BtnBuy"].transform;
			this.itemInfo = view["Image_ShopBk/Image_ItemInfo/Tmp_ItemInfo"]
				.GetComponent<TextMeshProUGUI>();
			this.playerMoney = view["Image_MoneyBk/Tmp_Num"].GetComponent<TextMeshProUGUI>();
			//this.itemDataObj = MemoryMgr.GetSourceFromResources<GameObject>(UiName.Image_ItemData);
			Assert.IsTrue(itemList&&buyBtn&&itemInfo&&this.playerMoney);
			InitItemList();
			this.CurrentItemIndex = 0;



			var inputer=this.buyBtn.gameObject.AddComponent<UIInputer>();
			inputer.eventOnPointerClick += (data) => OnBuy();
		}
		
		
		protected override void Update()
		{
			base.Update();
			var mouseScroll = Input.GetAxis("Mouse ScrollWheel");
			//Debug.Log(currentItemIndex);
			//Debug.Log(mouseScroll);
			if (mouseScroll < 0f)
			{
				if (currentItemIndex < itemIdList.Count - 1)
				
				{
					var speed = 134 / this.scrollTime;
					var pos = this.itemList.transform.position;
					MainLoop.Instance.UpdateForSeconds(
                          () => itemList.transform.Translate(Vector3.up * Time.deltaTime * speed),
                          this.scrollTime,
                          () => itemList.position = pos + new Vector3(0, 134, 0));
					CurrentItemIndex++;
				}
					

			}
			else if (mouseScroll > 0f)
			{
				if (currentItemIndex > 0)
				{
					//page down
					var speed = 134 / this.scrollTime;
					var pos = this.itemList.transform.position;
					MainLoop.Instance.UpdateForSeconds(
                		() => itemList.transform.Translate(Vector3.down * Time.deltaTime * speed),
                		this.scrollTime,
                		() => itemList.position = pos + new Vector3(0, -134, 0));
					CurrentItemIndex--;
				}
					
			}
		}

		protected override void OnAddListener()
		{
			base.OnAddListener();
			
			CEventCenter.AddListener<int>(Message.M_MoneyChange,OnMoneyChange);
		}

		protected override void OnRemoveListener()
		{
			base.OnRemoveListener();
			
			CEventCenter.RemoveListener<int>(Message.M_MoneyChange,OnMoneyChange);
		}

		public override void Enable()
		{
			base.Enable();
			CurrentMoney = GlobalVar.G_Player.Money;
		}		


		private void OnMoneyChange(int change)
		{
			CurrentMoney += change;
		}
		private void InitItemList()
		{
			GameObject itemObj;
			foreach (var id in itemIdList)
			{
				var itemInfo = CsvMgr.GetData<ShopItemData>(id);
			    itemObj=ResourceMgr.InstantiateGameObject(UiName.Image_ItemBk, this.itemList);
				Assert.IsTrue(itemObj);
				
				
				var avator=itemObj.transform.Find("Image_ItemAvator");
				if(avator==null)
					throw new Exception("avator is null"); 
				var image=avator.GetComponent<Image>();
				if(image==null)
					throw new Exception("Image is null");
				image.sprite =
					ResourceMgr.GetAsset<Sprite>(itemInfo.spriteName);
				itemObj.transform.Find("Tmp_Price").GetComponent<TextMeshProUGUI>().text = itemInfo.price.ToString();
				
				this.items.Add(itemObj);
			}
		}
		private void OnBuy()
		{
			CEventCenter.BroadMessage(Message.M_OnTryBut,itemIdList[currentItemIndex]);
		}
		
		
	}
}
