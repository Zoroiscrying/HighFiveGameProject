using Game.Const;
using Game.Control.PersonSystem;
using Game.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Common;
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

        }
        protected override void OnAddListener()
        {
            base.OnAddListener();

            //  BloodChange
            CEventCenter.AddListener<int>(Message.M_BloodChange(player.obj), OnPlayerBloodChanged);

        }

        protected override void OnRemoveListener()
        {
            base.OnRemoveListener();

            //  BloodChange
            CEventCenter.RemoveListener<int>(Message.M_BloodChange(player.obj), OnPlayerBloodChanged);

        }

        //////////////////////////    消息处理     /////////////////////////////

        void OnPlayerBloodChanged(int change)
        {
            this.bloodBar.value = player.Hp / (float)player.MaxHp;
            this.bloodText.text = player.Hp + "/" + player.MaxHp;
        }


    }
}
