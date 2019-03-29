using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Game.Control;
using UnityEngine;
using UnityEngine.UI;
using Game.Global;
using Game.Modal;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;
using Game.Const;
using UnityEditor;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;
using Game.Script;

namespace Game.View
{
    
    #region Panels from BasePanel
    
    /// <summary>
    /// 战斗界面Panel
    /// </summary>
    public class BattlePanel : BasePanel
    {
        private Player player;
        private Slider bloodBar;
        private Slider ExpBar;
        private Text bloodText;
        private Text expText;
        private Text rankText;
        protected override void Load()
        {
            
            Create(UIPath.Panel_Battle);
            
            this.player = AbstractPerson.GetInstance(CGameObjects.Player) as Player;
            Assert.IsTrue(this.player != null);
            
            var trans_bloodBar = m_TransFrom.Find("Image_PlayerStateBar/BloodBar");
            this.bloodBar = trans_bloodBar.GetComponent<Slider>();
            this.bloodText = trans_bloodBar.Find("Number").GetComponent<Text>();
            Assert.IsTrue(this.bloodBar!=null);
                
                
            var trans_expBar = m_TransFrom.Find("Image_PlayerStateBar/ExpBar");
            this.ExpBar = trans_expBar.GetComponent<Slider>();
            this.expText = trans_expBar.Find("Number").GetComponent<Text>();
            Assert.IsTrue(this.ExpBar != null);

            this.rankText = m_TransFrom.Find("Image_PlayerStateBar/Image_AvatorBG/Image_RankBG/Text_RankNum")
                .GetComponent<Text>();
            OnLevelUp(this.player.rank);

        }
        protected override void OnAddListener()
        {
            base.OnAddListener();
            //  LevelUp
            CEventCenter.AddListener<int>(Message.M_LevelUp,OnLevelUp);
            //  BloodChange
            CEventCenter.AddListener<int>(Message.M_BloodChange(player.obj),OnPlayerBloodChanged);
            //  ExpChange
            CEventCenter.AddListener<int>(Message.M_ExpChange,OnPlayerExpChanged);
        }

        protected override void OnRemoveListener()
        {
            base.OnRemoveListener();
            //  LevelUp
            CEventCenter.RemoveListener<int>(Message.M_LevelUp,OnLevelUp);
            //  BloodChange
            CEventCenter.RemoveListener<int>(Message.M_BloodChange(player.obj),OnPlayerBloodChanged);
            //  ExpChange
            CEventCenter.RemoveListener<int>(Message.M_ExpChange,OnPlayerExpChanged);
        }
        
        //////////////////////////    消息处理     /////////////////////////////

        void OnPlayerBloodChanged(int change)
        {
            this.bloodBar.value = player.Hp / (float) player.MaxHp;
            this.bloodText.text = player.Hp  + "/" + player.MaxHp;
        }

        void OnPlayerExpChanged(int change)
        {
            this.ExpBar.value = player.Exp / (float) player.MaxExp;
            this.expText.text = player.Exp + "/" + player.MaxExp;
        }

        void OnLevelUp(int newRank)
        {
            
//            Debug.Log("UI升级"+newRank+" 人物等级："+player.rank);
            this.bloodBar.value = player.Hp / (float) player.MaxHp;
            this.bloodText.text = player.Hp  + "/" + player.MaxHp;
            this.ExpBar.value = player.Exp / (float) player.MaxExp;
            this.expText.text = player.Exp + "/" + player.MaxExp;
            this.rankText.text =player.rank.ToString();
        }
    }

    
    /// <summary>
    /// 消息窗口类
    /// </summary>
	public class MessagePanel : BasePanel {
        public enum WindowType
        {
            Close,
            OK,
            OKConceal,
            Backpack
        }
        private readonly string text;
        private readonly string title;
        private readonly string OK = "";
        private readonly string conceal = "";
        private readonly WindowType windowType;

        public MessagePanel(string text,string title)
        {
            this.text = text;
            this.title = title;
            windowType = WindowType.Close;
            base.Create(UIPath.Panel_Dialog_Close);
        }
        public MessagePanel(string text, string ok,string title)
        {
            this.text = text;
            this.title = title;
            this.OK = ok;
            windowType = WindowType.OK;
            base.Create(UIPath.Panel_Dialog_OK);
        }
        public MessagePanel(string text, string ok,string conceal,string title)
        {
            this.text = text;
            this.title = title;
            this.OK = ok;
            this.conceal = conceal;
            windowType = WindowType.OKConceal;
            base.Create(UIPath.Panel_Dialog_OkConceal);
        }
        ////////////////   实现抽象     //////////////////

        protected override void OnAddListener()
        {
            base.OnAddListener();
            m_TransFrom.Find("Image_Title").GetComponentInChildren<Text>().text = title;
            m_TransFrom.Find("Text").GetComponent<Text>().text = this.text;
            m_TransFrom.Find("Image_Close").gameObject.AddComponent<UIInputer>().FunOnPointerClick += DestroyThis;
            switch(windowType)
            {
                case WindowType.Close:
                    break;
                case WindowType.OK:
                    m_TransFrom.Find("Image_Mid").GetComponentInChildren<Text>().text = this.OK;
                    m_TransFrom.Find("Image_Mid").gameObject.AddComponent<UIInputer>().FunOnPointerClick += OnClickOK;
                    break;
                case WindowType.OKConceal:
                    m_TransFrom.Find("Image_Left").GetComponentInChildren<Text>().text = this.OK;
                    m_TransFrom.Find("Image_Left").gameObject.AddComponent<UIInputer>().FunOnPointerClick += OnClickOK;
                    m_TransFrom.Find("Image_Right").GetComponentInChildren<Text>().text = this.conceal;
                    m_TransFrom.Find("Image_Right").gameObject.AddComponent<UIInputer>().FunOnPointerClick += OnClickConceal;
                    break;
            }
		}

        protected override void Update()
        {
            base.Update();
            Debug.Log("每帧调用了");
        }

        protected override void OnRemoveListener()
        {
            base.OnRemoveListener();
            Object.Destroy(m_TransFrom.Find("Image_Close").GetComponent<UIInputer>());
            switch(windowType)
            {
                case WindowType.Close:
                    break;
                case WindowType.OK:
                    Object.Destroy(m_TransFrom.Find("Image_Mid").GetComponent<UIInputer>());
                    break;
                case WindowType.OKConceal:
                    Object.Destroy(m_TransFrom.Find("Image_Left").GetComponent<UIInputer>());
                    Object.Destroy(m_TransFrom.Find("Image_Right").gameObject.AddComponent<UIInputer>());
                    break;
            }
        }

        ////////////////     消息响应    ///////////////////

        protected virtual void OnClickLeft()
        {
            
        }

        protected virtual void OnClickRight()
        {
            
        }
        
        private void OnClickOK(PointerEventData eventData)
        {
            OnClickLeft();
            DestroyThis();
        }
        private void OnClickConceal(PointerEventData eventData)
        {
            OnClickRight();
            DestroyThis();
        }

        protected override void Load()
        {
            
        }
    }

    public class WelcomePanel : MessagePanel
           {
               public WelcomePanel(string text, string title) : base(text, title)
               {
               }
       
               public WelcomePanel(string text, string ok, string title) : base(text, ok, title)
               {
               }
       
               public WelcomePanel(string text, string ok, string conceal, string title) : base(text, ok, conceal, title)
               {
               }
       
               protected override void OnClickLeft()
               {
                   base.OnClickLeft();
                   SceneMgr.Instance.LoadScene(Const.SceneName.testScene);
               }
       
               protected override void OnClickRight()
               {
                   base.OnClickRight();
                   SceneMgr.Instance.LoadScene(Const.SceneName.jbScene);
               }
           }
    
    #endregion
    


    #region OtherUi form AbstractWindow
    /// <summary>
    /// 伤害数值
    /// </summary>
    public class NumberTip : AbstractWindow
    {


        public GameObject go;
        private Text Text;
        private Transform target;
        private float yOffect;
        private float targetX;
        private float dur;
        private int dir;
  
        public NumberTip(int number,float yOffect,int size,Color color,Transform targetPerson,int dir,float dur=0.5f,float time=1f)
        {
            Create(UIPath.text_number);
            this.go=m_TransFrom.gameObject;
            this.target = targetPerson;
            
            this.yOffect = yOffect;
            this.dur = dur;
            this.dir = dir;

            if (go == null)
                Debug.Log("go实例化失败");
            
            this.Text = this.m_TransFrom.GetComponent<Text>();
            Text.color = color;
            Text.fontSize = size;
            Text.text = number.ToString();
            
            m_TransFrom.position=Camera.main.WorldToScreenPoint(target.position+new Vector3(0,this.yOffect*1.5f,0));
            this.targetX = m_TransFrom.position.x+this.dir*6f;
            
            var rig=this.go.AddComponent<Rigidbody2D>();
            rig.gravityScale = 100;
            Show();
            
            MainLoop.Instance.ExecuteLater<PointerEventData>(DestroyThis, time,null);
        }

        protected override void Update()
        {
            base.Update();
            if (target == null)
                return;
            var r = Random.Range(1.5f, 4.5f);
            m_TransFrom.position += new Vector3(this.dir * r, 0);
        }   
    }


    /// <summary>
    /// 血条
    /// </summary>
    public class BloodBar : AbstractWindow
    {
        public Slider slider;
        private AbstractPerson ap;
        public BloodBar(AbstractPerson ap)
        {
            this.ap =ap;
            base.Create(UIPath.Slider_BloodBar);
            m_TransFrom.position = Camera.main.WorldToScreenPoint(ap.Pos + new Vector3(0, ap.Scanler * 0.3f, 0));
            this.slider = m_TransFrom.GetComponent<Slider>();
            this.slider.value = (float)this.ap.Hp / this.ap.MaxHp;
            Show();
        }

        protected override void OnAddListener()
        {
            base.OnAddListener();
            CEventCenter.AddListener<int>(Message.M_BloodChange(ap.obj),OnBloodChanged);
            CEventCenter.AddListener<PointerEventData>(Message.M_Destory(ap.obj),DestroyThis);
        }

        protected override void OnRemoveListener()
        {
            base.OnRemoveListener();
            CEventCenter.RemoveListener<int>(Message.M_BloodChange(ap.obj),OnBloodChanged);
            CEventCenter.RemoveListener<PointerEventData>(Message.M_Destory(ap.obj),DestroyThis);
        }

        protected override void Update()
        {
            base.Update();
            if (ap.obj == null)
                return;
            m_TransFrom.position = Camera.main.WorldToScreenPoint(ap.Pos + new Vector3(0, ap.Scanler*0.7f, 0));
        }

        void OnBloodChanged(int change)
        {
            this.slider.value = (float)this.ap.Hp / this.ap.MaxHp;
        }
    }
    
    #endregion
 
//    /// <summary>
//    /// 存储库类
//    /// </summary>
//    public abstract class Inventory : AbstractWindow
//    {
//        
//    }
//    
//    
//    /// <summary>
//    /// 物品UI类
//    /// </summary>
//    [RequireComponent(typeof(Image))]
//    public class ItemUI : AbstractWindow
//    {
//        private Item Item { get; set; }
//
//        public int ItemID
//        {
//            get { return Item.ID; }
//        }
//        private int Num { get; set; }
//        private Image Image { get; set; }
//        private Text Text { get; set; }
//        private float TargetScale { get;set; }
//        private float Smothing { get; set; }
//
//        public ItemUI(Item item, int num,float targetScale=1.0f,float smothing=4f)
//        {
//            Item = item;
//            Num = num;
//            Create(UIPath.window_ItemUI);
//            Image = m_TransFrom.GetComponent<Image>();
//            Text = m_TransFrom.GetComponentInChildren<Text>();
//            this.Image.sprite = Resources.Load<Sprite>(Item.Sprite);
//            Text.text = num > 1 ? num.ToString() : "";
//            TargetScale = targetScale;
//            Smothing = smothing;
//        }
//        
//        protected override void Update()
//        {
//            base.Update();
//            if (Math.Abs(m_TransFrom.localScale.x - TargetScale) <0.1f)  return;
//            var scaleTemp = Mathf.Lerp(m_TransFrom.localScale.x, TargetScale, Smothing*Time.deltaTime);
//            m_TransFrom.localScale = new Vector3(scaleTemp, scaleTemp, scaleTemp);
//            if (Mathf.Abs(m_TransFrom.localScale.x-TargetScale) < 0.02f)//插值运算达不到临界值，比较耗性能，加上临界值判断能更好的控制
//            {
//                m_TransFrom.localScale = new Vector3(TargetScale, TargetScale, TargetScale);
//            }
//        }
//
//        private void SetItem(Item item, int num)
//        {
//            this.Item = Item;
//            this.Num = num;
//            this.Image.sprite = Resources.Load<Sprite>(Item.Sprite);
//            Text.text = Num > 1 ? Num.ToString() : "";
//        }
//
//        public bool AddItemNum(int num)
//        {
//            if (Num >= Item.Capacity)
//                return false;
//            Num += num;
//            Text.text = Num > 1 ? Num.ToString() : "";
//            return true;
//        }
//
//        public bool RemoveItemNum(int num)
//        {
//            if (Num - num < 0) return false;
//            Num -= num;
//            Text.text = Num > 1 ? Num.ToString() : "";
//            return true;
//        }
//
//        public void SetLocalPosition(Vector3 pos)
//        {
//            m_TransFrom.localPosition = pos;
//        }
//
//        public void Exchange(ItemUI it)
//        {
//            var v = it.Item;
//            var n = it.Num;
//            it.SetItem(this.Item, this.Num);
//            this.SetItem(v, n);
//        }
//    }
//    
//    
//    /// <summary>
//    /// 格子类
//    /// </summary>
//    [Serializable]
//    public class Slot : AbstractWindow
//    {
//        private ItemUI ItemUi { get; set; }
//
//        public Slot(Transform parent, Vector3 localPos, ItemUI itemUi=null)
//        {
//            ItemUi = itemUi;
//            Create(UIPath.window_Slot);
//            m_TransFrom.SetParent(parent);
//            m_TransFrom.localPosition = localPos;
//        }
//
//        public bool StoreItem(ItemUI itemUi)
//        {
//            if (ItemUi == null)
//            {
//                ItemUi = itemUi;
//                ItemUi.m_TransFrom.SetParent(m_TransFrom);
//                ItemUi.SetLocalPosition(Vector3.zero);
//                ItemUi.AddItemNum(1);
//                return true;
//            }
//
//            if (ItemUi.ItemID == itemUi.ItemID)
//            {
//                return ItemUi.AddItemNum(1);
//            }
//
//            return false;
//        }
//
//        public int GetItemId()
//        {
//            if (ItemUi == null)
//                return -1;
//            return ItemUi.ItemID;
//        }
//        //////////////////////////   这里可以实现右键菜单，显示提示，拖拽，点击放置   /////////////////////////
//    }
}
