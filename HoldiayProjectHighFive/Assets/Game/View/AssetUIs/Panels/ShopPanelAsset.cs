using System;
using System.Collections.Generic;
using System.IO;
using Game.Const;
using Game.Model.ItemSystem;
using ReadyGamerOne.Common;
using ReadyGamerOne.EditorExtension;
using ReadyGamerOne.Global;
using ReadyGamerOne.MemorySystem;
using ReadyGamerOne.Script;
using ReadyGamerOne.View.AssetUi;
using TMPro;
#if UNITY_EDITOR
	
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Game.View.AssetUIs
{
    public class ShopPanelAsset:PanelUiAsset
    {
	    #region Editor

#if UNITY_EDITOR

	    [MenuItem("ReadyGamerOne/Create/UI/ShopPanelAsset")]
	    public static void CreateAsset()
	    {
		    string[] strs = Selection.assetGUIDs;

		    string path = AssetDatabase.GUIDToAssetPath(strs[0]);

		    if (path.Contains("."))
		    {
			    path=path.Substring(0, path.LastIndexOf('/'));
		    }

		    if (!Directory.Exists(path))
			    Directory.CreateDirectory(path);
		    var targetFullPath = path + "/NewShopPanelAsset";

		    if (File.Exists(targetFullPath + ".asset"))
			    targetFullPath += "(1)";
            
		    AssetDatabase.CreateAsset(CreateInstance<ShopPanelAsset>(), targetFullPath + ".asset");
		    AssetDatabase.Refresh();

		    Selection.activeObject = AssetDatabase.LoadAssetAtPath<ShopPanelAsset>(targetFullPath + ".asset");
	    }
        
#endif        

	    #endregion
    
    
        #region Private
        
        #region Fields

        
        private int currentItemIndex;
        private List<int> itemIdList;
        private List<GameObject> items =new List<GameObject>();

        private float scrollTime = .25f;
		
		
        private Transform itemList;
        public TransformPathChooser itemListPath;
        private Transform buyBtn;
        public TransformPathChooser buyBtnPath;
        private TextMeshProUGUI itemInfo;
        public TransformPathChooser itemInfoPath;

        private TextMeshProUGUI playerMoney;

        public TransformPathChooser playerMoneyPath;
        //private GameObject itemDataObj;
        private string itemDataPath;

        #endregion

        #region Properties
		private int CurrentItemIndex
		{
			set
			{
				
				currentItemIndex = value;
				this.itemInfo.text = ItemInfoAsset.Instance.GetItem(itemIdList[value]).Description;
			}
			get { return currentItemIndex; }
		}
		private int CurrentMoney
		{
			get { return Convert.ToInt32(this.playerMoney.text); }
			set { playerMoney.text = value.ToString(); }
		}
        

        #endregion
        
        #endregion
        
        
        		
		#region Override

		public override void InitializeObj(Transform parent)
		{
			base.InitializeObj(parent);
			this.itemIdList=new List<int>(DefaultData.shop);
			this.itemList = m_TransFrom.Find(itemListPath.Path);
			this.buyBtn = m_TransFrom.Find(buyBtnPath.Path);
			this.itemInfo = m_TransFrom.Find(itemInfoPath.Path)
				.GetComponent<TextMeshProUGUI>();
			this.itemDataPath = UiName.Image_ItemData;
			this.playerMoney = m_TransFrom.Find(playerMoneyPath.Path).GetComponent<TextMeshProUGUI>();
			//this.itemDataObj = MemoryMgr.GetSourceFromResources<GameObject>(UiName.Image_ItemData);
			Assert.IsTrue(itemList&&buyBtn&&itemInfo&&this.playerMoney);
			InitItemList();
			this.CurrentItemIndex = 0;



			var inputer=this.buyBtn.gameObject.AddComponent<UIInputer>();
			inputer.eventOnPointerClick += (data) => OnBuy();
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
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

		protected override void OnShow()
		{
			base.OnShow();
			CurrentMoney = GlobalVar.G_Player.Money;
		}
		
		#endregion


		private void OnMoneyChange(int change)
		{
			CurrentMoney += change;
		}
		private void InitItemList()
		{
			GameObject itemObj;
			foreach (var id in itemIdList)
			{
				var itemInfo = ItemInfoAsset.Instance.GetItem(id);
			    itemObj=MemoryMgr.InstantiateGameObject(DirPath.LittleUiDir+UiName.Image_ItemData, this.itemList);
				Assert.IsTrue(itemObj);
				
				
				var avator=itemObj.transform.Find("Image_ItemAvator");
				if(avator==null)
					throw new Exception("avator is null"); 
				var image=avator.GetComponent<Image>();
				if(image==null)
					throw new Exception("Image is null");
				image.sprite =
					MemoryMgr.GetSourceFromResources<Sprite>(itemInfo.SpritePath);
				itemObj.transform.Find("Tmp_Price").GetComponent<TextMeshProUGUI>().text = itemInfo.Price.ToString();
				
				this.items.Add(itemObj);
			}
		}
		private void OnBuy()
		{
			CEventCenter.BroadMessage(Message.M_OnTryBut,itemIdList[currentItemIndex]);
		}
		
    }
}