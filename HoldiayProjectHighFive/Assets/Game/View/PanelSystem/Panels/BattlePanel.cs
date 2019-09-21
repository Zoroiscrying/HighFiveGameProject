using Game.Const;
using Game.Control.PersonSystem;
using System;
using Game.Model.RankSystem;
using ReadyGamerOne.Common;
using ReadyGamerOne.Global;
using ReadyGamerOne.View.PanelSystem;
using TMPro;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Game.View.PanelSystem
{
    /// <summary>
    /// 战斗界面Panel
    /// </summary>
    public class BattlePanel : AbstractPanel
    {
        private Player player;
        private Slider bloodBar;
        private Slider ExpBar;
        private Text bloodText;
        private Text expText;
        private Text rankText;
        private Text smallRank;
        private Text largeRank;
        private TextMeshProUGUI moneyText;

        private int CurrentMoney
        {
            get { return Convert.ToInt32(moneyText.text); }
            set { moneyText.text = value.ToString(); }
        }

        #region Override

        protected override void Load()
        {
            Create(DirPath.PanelDir + Const.PanelName.battlePanel);

            this.player = GlobalVar.G_Player;
            Assert.IsTrue(this.player != null);

            var trans_bloodBar = m_TransFrom.Find("Image_PlayerStateBar/BloodBar");
            this.bloodBar = trans_bloodBar.GetComponent<Slider>();
            this.bloodText = trans_bloodBar.Find("Number").GetComponent<Text>();
            Assert.IsTrue(this.bloodBar != null);

            var trans_expBar = m_TransFrom.Find("Image_PlayerStateBar/ExpBar");
            this.ExpBar = trans_expBar.GetComponent<Slider>();
            this.expText = trans_expBar.Find("Number").GetComponent<Text>();
            Assert.IsTrue(this.ExpBar != null);

            this.moneyText = m_TransFrom.Find("Image_MoneyBk/Tmp_Money").GetComponent<TextMeshProUGUI>();
            Assert.IsTrue(moneyText);


            this.rankText = m_TransFrom.Find("Image_PlayerStateBar/Image_AvatorBG/Image_RankBG/Text_RankNum")
                .GetComponent<Text>();

            this.largeRank = m_TransFrom.Find("Image_PlayerStateBar/Text_LargeRank").GetComponent<Text>();
            Assert.IsTrue(this.largeRank);
            this.smallRank = m_TransFrom.Find("Image_PlayerStateBar/Text_SmallRank").GetComponent<Text>();
            Assert.IsTrue(this.smallRank);
        }

        public override void Enable()
        {
            base.Enable();

            CurrentMoney = GlobalVar.G_Player.Money;
        }

        protected override void OnAddListener()
        {
            base.OnAddListener();

            CEventCenter.AddListener<int>(Message.M_BloodChange(player.obj), OnPlayerBloodChanged);
            CEventCenter.AddListener<int>(Message.M_ChangeSmallLevel, OnExpChange);
            CEventCenter.AddListener<int>(Message.M_AchieveLargeLevel, OnLargeLevelUp);
            CEventCenter.AddListener<int>(Message.M_AchieveSmallLevel, OnSmallLevelUp);
            CEventCenter.AddListener<int, int>(Message.M_RankAwake, OnRankAwake);
            CEventCenter.AddListener<int>(Message.M_MoneyChange, OnMoneyChange);
        }

        protected override void OnRemoveListener()
        {
            base.OnRemoveListener();

            //  BloodChange
            CEventCenter.RemoveListener<int>(Message.M_BloodChange(player.obj), OnPlayerBloodChanged);
            CEventCenter.RemoveListener<int>(Message.M_ChangeSmallLevel, OnExpChange);
            CEventCenter.RemoveListener<int>(Message.M_AchieveLargeLevel, OnLargeLevelUp);
            CEventCenter.RemoveListener<int>(Message.M_AchieveSmallLevel, OnSmallLevelUp);
            CEventCenter.RemoveListener<int, int>(Message.M_RankAwake, OnRankAwake);
            CEventCenter.RemoveListener<int>(Message.M_MoneyChange, OnMoneyChange);
        }

        #endregion


        #region 消息处理

        //////////////////////////    消息处理     /////////////////////////////

        void OnMoneyChange(int change)
        {
//                    Debug.Log("UI金钱变化："+ change);
            CurrentMoney += change;
        }

        void OnPlayerBloodChanged(int change)
        {
            this.bloodBar.value = player.Hp / (float) player.MaxHp;
            this.bloodText.text = player.Hp + "/" + player.MaxHp;
        }

        void OnExpChange(int change)
        {
            this.ExpBar.value = player.rankMgr.Adder / (float) player.rankMgr.Max;
            this.expText.text = player.rankMgr.Adder + "/" + player.rankMgr.Max;
        }

        void OnSmallLevelUp(int newLevel)
        {
            this.smallRank.text = GlobalVar.G_Player.rankMgr.LargeRank.smallRanks[newLevel].name;
            this.ExpBar.value = 0;
            //Debug.Log("小升级："+this.smallRank.text);
        }

        void OnLargeLevelUp(int newLevel)
        {
            var rank = RankMgr.LargeRankList[newLevel];
            this.largeRank.text = rank.name;
            this.smallRank.text = rank.smallRanks[0].name;
            //Debug.Log("大升级: "+ this.largeRank.text);
            this.ExpBar.value = 0;
        }

        void OnRankAwake(int large, int small)
        {
            var rank = RankMgr.LargeRankList[large];
            //Debug.Log("LargeRank.CharacterName: "+rank.CharacterName);
            this.largeRank.text = rank.name;
            this.smallRank.text = rank.smallRanks[small].name;
            //Debug.Log("初始等级："+this.largeRank.text+"  "+this.smallRank.text);
            this.ExpBar.value = 0;
        }

        #endregion
    }
}