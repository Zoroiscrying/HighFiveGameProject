using System;
using System.Collections;
using HighFive.Const;
using ReadyGamerOne.Common;
using HighFive.Global;
using ReadyGamerOne.Script;
using ReadyGamerOne.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace HighFive.View
{
    public partial class BattlePanel        
    {
        private SuperBloodBar bloodBar;
        private SuperBloodBar ExpBar;
        private SuperBloodBar dragBar;
        private Text dragText;
        private Text bloodText;
        private Text expText;
        private Text rankText;
        private TextMeshProUGUI moneyText;

        private int CurrentMoney
        {
            get { return Convert.ToInt32(moneyText.text); }
            set { moneyText.text = value.ToString(); }
        }

        partial void OnLoad()
        {
            //do any thing you want
            var trans_bloodBar = GetTransform("Image_PlayerStateBar/BloodBar");
            this.bloodBar = trans_bloodBar.GetComponent<SuperBloodBar>();
            this.bloodText = trans_bloodBar.Find("Number").GetComponent<Text>();
            Assert.IsTrue(this.bloodBar && bloodText);

            var trans_expBar = GetTransform("Image_PlayerStateBar/ExpBar");
            this.ExpBar = trans_expBar.GetComponent<SuperBloodBar>();
            this.expText = trans_expBar.Find("Number").GetComponent<Text>();
            Assert.IsTrue(this.ExpBar && expText);
            
            var trans_dragBar = GetTransform("Image_PlayerStateBar/DragBar");
            this.dragBar = trans_dragBar.GetComponent<SuperBloodBar>();
            this.dragText = trans_dragBar.Find("Number").GetComponent<Text>();
            Assert.IsTrue(this.dragBar && dragText);

            this.moneyText = GetComponent<TextMeshProUGUI>("Image_MoneyBk/Tmp_Money");
            Assert.IsTrue(moneyText);
            this.rankText = GetComponent<Text>("Image_PlayerStateBar/Text_Rank");
            Assert.IsTrue(this.rankText);
        }
        

        protected override void OnAddListener()
        {
            MainLoop.Instance.StartCoroutine(AddPlayerListener());
        }

        private IEnumerator AddPlayerListener()
        {
            while (GlobalVar.G_Player == null)
            {
                yield return null;
            }
            base.OnAddListener();
            CEventCenter.AddListener<int>(Message.M_MoneyChange, OnMoneyChange);
            CEventCenter.AddListener<int>(Message.M_PlayerExpChange,OnExpChange);
            CEventCenter.AddListener<int>(Message.M_PlayerBloodChange, OnPlayerBloodChanged);
            CEventCenter.AddListener<int>(Message.M_PlayerDragChange,OnPlayerDragChanged);
            CEventCenter.AddListener(Message.M_LevelUp,OnLevelUp);
            CEventCenter.AddListener(Message.M_ExitSuper,OnExitSuper);

            rankText.text = GlobalVar.G_Player.Rank;
            OnPlayerBloodChanged(0);
            OnExpChange(0);
            OnMoneyChange(0);
            OnPlayerDragChanged(0);
            
        }

        private void OnExitSuper()
        {
            var dragConsume =Mathf.RoundToInt(
                 GameSettings.Instance.superDragConsumeRate * GlobalVar.G_Player.MaxDrag);
            OnPlayerDragChanged(-dragConsume);

        }


        private void OnLevelUp()
        {
            rankText.text = GlobalVar.G_Player.Rank;
            OnPlayerBloodChanged(0);
            OnMoneyChange(0);
        }

        protected override void OnRemoveListener()
        {
            base.OnRemoveListener();

            //  BloodChange
            CEventCenter.RemoveListener(Message.M_ExitSuper,OnExitSuper);
            CEventCenter.RemoveListener<int>(Message.M_PlayerBloodChange, OnPlayerBloodChanged);
            CEventCenter.RemoveListener<int>(Message.M_PlayerExpChange,OnExpChange);
            CEventCenter.RemoveListener<int>(Message.M_MoneyChange, OnMoneyChange);
            CEventCenter.RemoveListener<int>(Message.M_PlayerDragChange, OnPlayerDragChanged);
            CEventCenter.RemoveListener(Message.M_LevelUp, OnLevelUp);
        }


        #region 消息处理
        
        /// <summary>
        /// 金钱变化
        /// </summary>
        /// <param name="change"></param>
        void OnMoneyChange(int change)
        {
            CurrentMoney = GlobalVar.G_Player.ChangeMoney(change);
        }

        /// <summary>
        /// 血量变化
        /// </summary>
        /// <param name="change"></param>
        void OnPlayerBloodChanged(int change)
        {
            if (GlobalVar.G_Player == null)
                throw new Exception($"GlobalVar.G_Player is null");
            this.bloodBar.Value = GlobalVar.G_Player.Hp / (float) GlobalVar.G_Player.MaxHp;
            this.bloodText.text = GlobalVar.G_Player.Hp + "/" + GlobalVar.G_Player.MaxHp;
        }
        
        /// <summary>
        /// 经验变化
        /// </summary>
        /// <param name="change"></param>
        /// <exception cref="Exception"></exception>
        void OnExpChange(int change)
        {
//            Debug.Log($"获取经验[{change}]");
            if (GlobalVar.G_Player == null)
                throw new Exception($"GlobalVar.G_Player is null");
            var value = GlobalVar.G_Player.ChangeExp(change);
            this.ExpBar.Value = value / (float) GlobalVar.G_Player.MaxExp;
            this.expText.text = value + "/" + GlobalVar.G_Player.MaxExp;
        }
        
        /// <summary>
        /// 药引变化
        /// </summary>
        /// <param name="change"></param>
        private void OnPlayerDragChanged(int change)
        {
            if (GlobalVar.G_Player == null)
                throw new Exception($"GlobalVar.G_Player is null");
            var value = GlobalVar.G_Player.ChangeDrag(change);
            this.dragBar.Value = value / (float) GlobalVar.G_Player.MaxDrag;
            this.dragText.text = value + "/" + GlobalVar.G_Player.MaxDrag;
        }
        
        #endregion
    }
}