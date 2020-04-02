using System;
using System.Collections;
using HighFive.Const;
using ReadyGamerOne.Common;
using HighFive.Global;
using ReadyGamerOne.Script;
using TMPro;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace HighFive.View
{
    public partial class BattlePanel        
    {
        private Slider bloodBar;
        private Slider ExpBar;
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
            this.bloodBar = trans_bloodBar.GetComponent<Slider>();
            this.bloodText = trans_bloodBar.Find("Number").GetComponent<Text>();
            Assert.IsTrue(this.bloodBar != null);

            var trans_expBar = GetTransform("Image_PlayerStateBar/ExpBar");
            this.ExpBar = trans_expBar.GetComponent<Slider>();
            this.expText = trans_expBar.Find("Number").GetComponent<Text>();
            Assert.IsTrue(this.ExpBar != null);

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
            CEventCenter.AddListener(Message.M_LevelUp,OnLevelUp);

            rankText.text = GlobalVar.G_Player.Rank;
            OnPlayerBloodChanged(0);
            OnExpChange(0);
            OnMoneyChange(0);
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
            CEventCenter.RemoveListener<int>(Message.M_PlayerBloodChange, OnPlayerBloodChanged);
            CEventCenter.RemoveListener<int>(Message.M_PlayerExpChange,OnExpChange);
            CEventCenter.RemoveListener<int>(Message.M_MoneyChange, OnMoneyChange);
            CEventCenter.RemoveListener(Message.M_LevelUp, OnLevelUp);
        }


        #region 消息处理


        /// <summary>
        /// 控制金钱
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
            this.bloodBar.value = GlobalVar.G_Player.Hp / (float) GlobalVar.G_Player.MaxHp;
            this.bloodText.text = GlobalVar.G_Player.Hp + "/" + GlobalVar.G_Player.MaxHp;
        }

        /// <summary>
        /// 药引变化
        /// </summary>
        /// <param name="change"></param>
        void OnExpChange(int change)
        {
            var value = GlobalVar.G_Player.ChangeDrag(change);
            this.ExpBar.value = value / (float) GlobalVar.G_Player.MaxExp;
            this.expText.text = value + "/" + GlobalVar.G_Player.MaxExp;
        }


        #endregion
    }
}