using System;
using System.IO;
using HighFive.Const;
using HighFive.Model.RankSystem;
using ReadyGamerOne.Common;
using ReadyGamerOne.EditorExtension;
using HighFive.Global;
using ReadyGamerOne.View.AssetUi;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
#if UNITY_EDITOR

#endif

namespace HighFive.View.AssetUIs.Panels
{
    public class BattlePanelAsset:PanelUiAsset
    {
        #region Editor

#if UNITY_EDITOR

        [MenuItem("ReadyGamerOne/Create/UI/BattlePanelAsset")]
        public new static void CreateAsset()
        {
            string[] strs = Selection.assetGUIDs;

            string path = AssetDatabase.GUIDToAssetPath(strs[0]);

            if (path.Contains("."))
            {
                path=path.Substring(0, path.LastIndexOf('/'));
            }

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var targetFullPath = path + "/NewBattlePanelAsset";

            if (File.Exists(targetFullPath + ".asset"))
                targetFullPath += "(1)";
            
            AssetDatabase.CreateAsset(CreateInstance<BattlePanelAsset>(), targetFullPath + ".asset");
            AssetDatabase.Refresh();

            Selection.activeObject = AssetDatabase.LoadAssetAtPath<BattlePanelAsset>(targetFullPath + ".asset");
        }
        
#endif        

        #endregion

        #region ComponentPath

        private Slider bloodBar;
        public TransformPathChooser bloodBarPath;
        private Slider ExpBar;
        public TransformPathChooser expBarPath;
        private Text bloodText;
        public TransformPathChooser bloodTextPath;
        private Text expText;
        public TransformPathChooser expTextPath;
        private Text smallRank;
        public TransformPathChooser smallRankPath;
        private Text largeRank;
        public TransformPathChooser largeRankPath;
        private TextMeshProUGUI moneyText;
        public TransformPathChooser moneyTextPath;        

        #endregion
        

        
        private int CurrentMoney
        {
            get { return Convert.ToInt32(moneyText.text); }
            set { moneyText.text = value.ToString(); }
        }
        
        #region Override
        protected override void InitializeObj(Transform parent)
        {
            base.InitializeObj(parent);
            Assert.IsTrue(GlobalVar.G_Player != null);

            var trans_bloodBar = m_TransFrom.Find(bloodBarPath.Path);
            this.bloodBar = trans_bloodBar.GetComponent<Slider>();
            this.bloodText = m_TransFrom.Find(bloodTextPath.Path).GetComponent<Text>();
            Assert.IsTrue(this.bloodBar != null);

            var trans_expBar = m_TransFrom.Find(expBarPath.Path);
            this.ExpBar = trans_expBar.GetComponent<Slider>();
            this.expText = m_TransFrom.Find(expTextPath.Path).GetComponent<Text>();
            Assert.IsTrue(this.ExpBar != null);

            this.moneyText = m_TransFrom.Find(moneyTextPath.Path).GetComponent<TextMeshProUGUI>();
            Assert.IsTrue(moneyText);
            

            this.largeRank = m_TransFrom.Find(largeRankPath.Path).GetComponent<Text>();
            Assert.IsTrue(this.largeRank);
            this.smallRank = m_TransFrom.Find(smallRankPath.Path).GetComponent<Text>();
            Assert.IsTrue(this.smallRank);
        }

        protected override void OnShow()
        {
            base.OnShow();
            CurrentMoney = GlobalVar.G_Player.Money;
        }
        
        protected override void OnAddListener()
        {
            base.OnAddListener();

            CEventCenter.AddListener<int>(Message.M_BloodChange(GlobalVar.G_Player.obj), OnPlayerBloodChanged);
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
            CEventCenter.RemoveListener<int>(Message.M_BloodChange(GlobalVar.G_Player.obj), OnPlayerBloodChanged);
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
            this.bloodBar.value = GlobalVar.G_Player.Hp / (float) GlobalVar.G_Player.MaxHp;
            this.bloodText.text = GlobalVar.G_Player.Hp + "/" + GlobalVar.G_Player.MaxHp;
        }

        void OnExpChange(int change)
        {
            this.ExpBar.value = GlobalVar.G_Player.rankMgr.Adder / (float) GlobalVar.G_Player.rankMgr.Max;
            this.expText.text = GlobalVar.G_Player.rankMgr.Adder + "/" + GlobalVar.G_Player.rankMgr.Max;
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