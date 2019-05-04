using Game.Const;
using Game.Control.PersonSystem;
using Game.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Common;
using Game.Model.RankSystem;
using UnityEngine;
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
        protected override void Load()
        {

            Create(UIPath.Panel_Battle);

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

            this.rankText = m_TransFrom.Find("Image_PlayerStateBar/Image_AvatorBG/Image_RankBG/Text_RankNum")
                .GetComponent<Text>();

            this.largeRank = m_TransFrom.Find("Image_PlayerStateBar/Text_LargeRank").GetComponent<Text>();
            Assert.IsTrue(this.largeRank);
            this.smallRank = m_TransFrom.Find("Image_PlayerStateBar/Text_SmallRank").GetComponent<Text>();
            Assert.IsTrue(this.smallRank);

        }
        protected override void OnAddListener()
        {
            base.OnAddListener();

            //  BloodChange
            CEventCenter.AddListener<int>(Message.M_BloodChange(player.obj), OnPlayerBloodChanged);
            CEventCenter.AddListener<int>(Message.M_ChangeSmallLevel, OnExpChange);
            CEventCenter.AddListener<int>(Message.M_AchieveLargeLevel, OnLargeLevelUp);
            CEventCenter.AddListener<int>(Message.M_AchieveSmallLevel, OnSmallLevelUp);
            CEventCenter.AddListener<int,int>(Message.M_RankAwake,OnRankAwake);

        }

        protected override void OnRemoveListener()
        {
            base.OnRemoveListener();

            //  BloodChange
            CEventCenter.RemoveListener<int>(Message.M_BloodChange(player.obj), OnPlayerBloodChanged);
            CEventCenter.RemoveListener<int>(Message.M_ChangeSmallLevel, OnExpChange);
            CEventCenter.RemoveListener<int>(Message.M_AchieveLargeLevel, OnLargeLevelUp);
            CEventCenter.RemoveListener<int>(Message.M_AchieveSmallLevel, OnSmallLevelUp);
            CEventCenter.RemoveListener<int,int>(Message.M_RankAwake,OnRankAwake);

        }

        //////////////////////////    消息处理     /////////////////////////////

        void OnPlayerBloodChanged(int change)
        {
            this.bloodBar.value = player.Hp / (float)player.MaxHp;
            this.bloodText.text = player.Hp + "/" + player.MaxHp;
        }

        void OnExpChange(int change)
        {
            this.ExpBar.value = player.rankMgr.Adder / (float)player.rankMgr.Max;
            this.expText.text = player.rankMgr.Adder + "/" + player.rankMgr.Max;
        }

        void OnSmallLevelUp(int newLevel)
        {
            this.smallRank.text = GlobalVar.G_Player.rankMgr.LargeRank.smallRanks[newLevel-1].name;
            this.ExpBar.value = 0;
        }

        void OnLargeLevelUp(int newLevel)
        {
            this.largeRank.text = RankMgr.LargeRankList[newLevel - 1].name;
            Debug.Log("LargeRank.name: "+ RankMgr.LargeRankList[newLevel - 1].name);
            this.ExpBar.value = 0;
        }

        void OnRankAwake(int large, int small)
        {
            var rank = RankMgr.LargeRankList[large];
            Debug.Log("LargeRank.name: "+rank.name);
            this.largeRank.text = rank.name;
            this.smallRank.text = rank.smallRanks[small].name;
            this.ExpBar.value = 0;
        }
        
        
    }
}
